﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FSM;

public enum ChampionState {
    PREPARE,//备战席
    IDLE,//在场上挂机
    WALK,//走到目标
    ATTACK,//和目标战斗
    DEAD,//棋子死亡
}

[RequireComponent(typeof(Animator))]
public class Champion : MonoBehaviour {//棋子类,
//棋子的逻辑是这样的:游戏开始的时候首先寻路到最近的敌人,然后一直平A,蓝够了就放技能,敌人死了就找下一个
//一个英雄所包含的所有数据,应该是配置好的
    [SerializeField]
    private string championName;
    public string ChampionName => championName;
    [SerializeField]
    private int tier;//几费卡?
    public int Tier => tier;
    [SerializeField]
    private int defaultCost;//应该某种办法让card的和它保持一致,如果升级了应该发生变化,先手动配吧/卖几块钱?
    private int currentCost;
    public int Cost {
        get {
            return currentCost == 0? defaultCost : currentCost;//如果currentCost还没来得及初始化,那就返回默认的
        }
    }
    // [SerializeField]//enemy不是1级默认
    // private int defaultLevel = 0;//0是一星,1是2星,2是3星卡!
    private int currentLevel;
    public int Level => currentLevel;
    [SerializeField]
    private int space;//所占格子,默认是1,比如龙神是2
    public int Space {
        get {
            if(space == 0) {
                space = 1;
            }
            return space;
        }
    }
    private bool isMouseHoveringOnThisChampion;
    [SerializeField]
    private bool isAllyChampion;//true就是友方
    private StateMachine<ChampionState> championStateMachine = new StateMachine<ChampionState>();
    ChampionPrepare championPrepareState;
    ChampionIdle championIdleState;
    ChampionWalk championWalkState;
    ChampionAttack championAttackState;
    ChampionDead championDeadState;
    protected Animator animator;
    protected Animator Animator {
        get{
            if(animator == null) {
                animator = GetComponent<Animator>();
            }
            return animator;
        }
    }
    [SerializeField]//这个要自己配好
    private ChampionStats defaultChampionStats;//默认的英雄数值,但是怎么合理配置呢?
    [HideInInspector]
    public ChampionStats currentChampionStats;//当前英雄数值
    private List<TraitBase> traits = new List<TraitBase>();//一个英雄拥有的所有羁绊
    public ChampionAbility championAbility;

    protected ChampionState currentChampionState;
    Quad lastQuadThisChampionStand = null;//英雄的位置发生变化,包括进来离开,都应该改变
    public Quad LastQuadThisChampionStand => lastQuadThisChampionStand;
    Quad lastMouseHoveringQuad = null;//这个变量和currentMouseHoveringQuad在鼠标拖拽过程中会变化
    Quad currentMouseHoveringQuad = null;

    private void Update() {
        if(championStateMachine != null) {//for testing
            championStateMachine.OnLogic();
        } 
    }
    private void OnGUI() {
        // if(GUILayout.Button("cast ability")) {
        //     Debug.Log(currentChampionStats.attackDamage);
        //     traits.Add(new TestTrait(ActivateTestAdditionalEffect));
        //     traits[0].ActivateTrait(5,this);
        //     Debug.Log(currentChampionStats.attackDamage);
        // }
    }
    public void OnDeploy(Quad quadToStay,bool isAlly,int level = 0) {
        //被部署到备战席的时候应该调用一些方法
        isAllyChampion = isAlly;
        if(isAlly) {          
            currentLevel = 0;
            currentCost = defaultCost;
        }else {
            currentLevel = level;
        }        
        if(defaultChampionStats != null) {
            //currentChampionStats = new ChampionStats(defaultChampionStats,currentLevel);
            currentChampionStats = (ChampionStats)ScriptableObject.CreateInstance("ChampionStats");
            currentChampionStats.CopyChampionStats(defaultChampionStats,currentLevel);
        }
        InitFSM();
        OnEnterQuad(quadToStay);
        GameEventsManager.StartListening(GameEventTypeVoid.ENTER_DEPLOY_STATE,OnEnterDeployState);
        GameEventsManager.StartListening(GameEventTypeVoid.ENTER_COMBAT_STATE,OnEnterCombatState);
        GameEventsManager.StartListening(GameEventTypeVoid.ENETR_BONUS_STATE,OnEnterBonusState);
        GameEventsManager.StartListening(GameEventTypeChampion.SELL_A_CHAMPION,OnSell);
        GameEventsManager.StartListening(GameEventTypeVoid.ON_SELL_BUTTON_DOWN,OnSellButtonDown);
        RegisterThisChampion();//所有工作都做完再注册
    }
    public void OnDisappear() {//卖掉或者升级会触发的函数,主要是取消事件监听
        GameEventsManager.StopListening(GameEventTypeVoid.ENTER_DEPLOY_STATE,OnEnterDeployState);
        GameEventsManager.StopListening(GameEventTypeVoid.ENTER_COMBAT_STATE,OnEnterCombatState);
        GameEventsManager.StopListening(GameEventTypeVoid.ENETR_BONUS_STATE,OnEnterBonusState);
        GameEventsManager.StopListening(GameEventTypeChampion.SELL_A_CHAMPION,OnSell);
        GameEventsManager.StopListening(GameEventTypeVoid.ON_SELL_BUTTON_DOWN,OnSellButtonDown);
    }
    public void OnChampionSwap(Champion championOnTheTargetQuad) {
        //championOnTheTargetQuad.gameObject.transform.position = lastMouseHoveringQuadThisChampionStand.node.worldPosition;
        //除了改位置,还应该改一些变量啥的
        championOnTheTargetQuad.OnEnterQuad(lastQuadThisChampionStand,true);
    }
    public void OnEnterQuad(Quad quad,bool isSwaping = false) {//某种方式让champion进入到一个quad,要做很多事
        if(quad is DeployQuad) {
            championStateMachine.Trigger("OnDeployQuad");
            if(lastQuadThisChampionStand == null || lastQuadThisChampionStand is PreparationQuad) {//如果是null,说明直接买进去,或者从下面上去
                if(isAllyChampion) {
                    AllyChampionManager.Instance.OnSpaceChange(Space);
                }
                Debug.Log("说明成功从备战到了场上,那么英雄数量会+1");
            }
        }else if(quad is PreparationQuad){     
            championStateMachine.Trigger("OnPreparationQuad");
            if(lastQuadThisChampionStand != null && lastQuadThisChampionStand is DeployQuad) {
                if(isAllyChampion) {
                    AllyChampionManager.Instance.OnSpaceChange(Space * -1);
                }
                Debug.Log("说明是从场上撤下来,英雄数量-1");
            }
        }else if(quad is EnemyQuad) {
            championStateMachine.Trigger("EnterEnemyQuad");
        }
        transform.position = quad.node.worldPosition;
        if(lastQuadThisChampionStand != null && quad.ChampionOnThisQuad == null && lastQuadThisChampionStand != quad) {
            //说明是从一个quad到另一个空quad,且不是从一个quad拿起来又放下
            lastQuadThisChampionStand.OnChampionLeave(this);
        }
        quad.OnChampionStay(this,isSwaping);//先告诉quad,champion要stay
        lastQuadThisChampionStand = quad;//再改自己的这个
        lastMouseHoveringQuad = quad;
        currentMouseHoveringQuad = quad;
    }
    public void MoveToNewQuad(Vector3 target,float speed,Champion targetChampion,ref bool isWalking,Action findNewPathFunc,Action triggerAttackFunc) {       
        if(Vector3.Distance(transform.position,target) <= float.Epsilon) {
            //说明champion的位置到了新的一格,先判断攻击有没有被触发,没有的话就再重新再寻一次路
            if(Vector3.Distance(targetChampion.transform.position,transform.position) 
            <= currentChampionStats.attackRange * MapManager.Instance.CurrentMapConfiguration.ScaleRatio) {
                transform.LookAt(target);
                triggerAttackFunc?.Invoke();
                isWalking = false;
            }else {
                findNewPathFunc?.Invoke();
                //可能出现距离还不够,但是找不到路了?
            }
        }else {
            transform.position = Vector3.MoveTowards(transform.position,target,speed * Time.deltaTime);
            transform.LookAt(target); 
        }
    }
    public void QuadStateChange(Vector3 target) {
        Debug.Log("quad的状态改变了一次");
        Quad quad = QuadsManager.Instance.GetCombatQuadByPostion(target);
        if(quad != null) {
            lastQuadThisChampionStand.OnChampionLeave(this);
            quad.OnChampionStay(this);
            lastQuadThisChampionStand = quad;
        }
    }
    private void OnMouseDrag() {
        // Debug.Log(Input.mousePosition);
        // Debug.Log(Camera.main.ScreenToWorldPoint(Input.mousePosition));
        if(!(GameManager.Instance.PlayState.ActiveState.name == OnPlayState.DEPLOY
        || (GameManager.Instance.PlayState.ActiveState.name == OnPlayState.COMBAT && lastQuadThisChampionStand is PreparationQuad))) {
            return;//只有deploy阶段和combat在场下的棋子可以动
        }
        
        Vector3 screenPos = Camera.main.WorldToScreenPoint(transform.position);
        Vector3 temp = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x,Input.mousePosition.y,screenPos.z));
        temp.y = QuadsManager.Instance.CurrentMap.OriginPoint.y;
        transform.position = temp;
        //还要通过quadmanager知道当前对应的quad,调用高亮和退出的方法
        currentMouseHoveringQuad = QuadsManager.Instance.GetAllyQuadByPosition(transform.position);
        if(currentMouseHoveringQuad == null && lastMouseHoveringQuad != null) {
            //应该让英雄停留在上个格子
            transform.position = lastMouseHoveringQuad.node.worldPosition;
            return;
        }
        if(lastMouseHoveringQuad != null && lastMouseHoveringQuad != currentMouseHoveringQuad) {//说明进入了新的quad
            lastMouseHoveringQuad.OnChampionExitOnMouse(this);
        }
        currentMouseHoveringQuad.OnChampionEnterOnMouse(this);
        lastMouseHoveringQuad = currentMouseHoveringQuad;
    }
    private void OnMouseUp() {//这里本质上要判断能不能进入新的地方?
        if(lastMouseHoveringQuad != null) {
            if(isAllyChampion) {
                if(lastQuadThisChampionStand is PreparationQuad && lastMouseHoveringQuad is DeployQuad) {
                    //如果是要从下面上去,
                    if(GameManager.Instance.PlayState.ActiveState.name == OnPlayState.DEPLOY//只有deploy才能放上去吖
                    && ((lastMouseHoveringQuad.ChampionOnThisQuad == null && Player.Instance.TotalAvailabeSpace 
                        >= AllyChampionManager.SpaceTakenByChampions + Space)
                    || (lastMouseHoveringQuad.ChampionOnThisQuad != null && Player.Instance.TotalAvailabeSpace 
                        >= AllyChampionManager.SpaceTakenByChampions + lastMouseHoveringQuad.ChampionOnThisQuad.Space - Space))) {
                        // 如果加上我这个英雄,空间还够,那就可以进去,或者我去的地方有英雄,那么就是交换,也可以进去,但是交换要看交换的英雄space
                        OnEnterQuad(lastMouseHoveringQuad);
                    }else {//既然上不去,那就回去
                        OnEnterQuad(lastQuadThisChampionStand);
                    }
                }else if(lastQuadThisChampionStand is DeployQuad && lastMouseHoveringQuad is PreparationQuad) {
                    if(lastMouseHoveringQuad.ChampionOnThisQuad == null 
                    ||(lastMouseHoveringQuad.ChampionOnThisQuad != null && Player.Instance.TotalAvailabeSpace 
                        >= AllyChampionManager.SpaceTakenByChampions + lastMouseHoveringQuad.ChampionOnThisQuad.Space - Space)) {
                            //如果是放下来,形成交换,那也要判定space
                            OnEnterQuad(lastMouseHoveringQuad);
                    }else {
                        OnEnterQuad(lastQuadThisChampionStand);
                    }
                    
                }else {//其余情况随便动
                    OnEnterQuad(lastMouseHoveringQuad);
                }
            } 
        }
        
    }
    private void OnMouseDown() {
        
    }
    private void OnMouseOver() {
        isMouseHoveringOnThisChampion = true;
    }
    private void OnMouseExit() {
        isMouseHoveringOnThisChampion = false;
    }
    public void InitFSM() {
        championPrepareState = new ChampionPrepare(Animator,false);
        championIdleState = new ChampionIdle(Animator,false);
        championWalkState = new ChampionWalk(this,Animator,(target)=>
        {championAttackState.UpdateTargetChampion(target);championStateMachine.Trigger("Attack");}, false);
        championAttackState = new ChampionAttack(Animator,false);
        championDeadState = new ChampionDead(Animator,false);

        championStateMachine.AddState(ChampionState.PREPARE,championPrepareState);
        championStateMachine.AddState(ChampionState.IDLE,championIdleState);//把所有要添加的state加入进来,把需要的参数传进去
        championStateMachine.AddState(ChampionState.WALK,championWalkState);
        championStateMachine.AddState(ChampionState.ATTACK,championAttackState);
        championStateMachine.AddState(ChampionState.DEAD,championDeadState);

        championStateMachine.AddTriggerTransition("BattleStart",ChampionState.IDLE,ChampionState.WALK);//战斗开始,开始行动
        championStateMachine.AddTriggerTransition("Attack",ChampionState.WALK,ChampionState.ATTACK);
        championStateMachine.AddTriggerTransition("OnPreparationQuad",ChampionState.IDLE,ChampionState.PREPARE);
        championStateMachine.AddTriggerTransition("OnDeployQuad",ChampionState.PREPARE,ChampionState.IDLE);

        championStateMachine.AddTriggerTransitionFromAny("Dead",ChampionState.DEAD);//随时可以会死,prepare虽然不会,但是不触发就好了
        //championStateMachine.AddTriggerTransitionFromAny("EnterDeployStatePrepare",ChampionState.PREPARE);感觉不需要
        championStateMachine.AddTriggerTransitionFromAny("EnterDeployStateIdle",ChampionState.IDLE);
        championStateMachine.AddTriggerTransitionFromAny("EnterEnemyQuad",ChampionState.IDLE);//enemy用的
        championStateMachine.Init();
    }
    public void HitTarget() {
        Debug.Log("哥们儿攻击呢");
        championAttackState.HitTarget();
    }
    public void OnHit(Champion champion) {
        //被传进来的champion打了,要掉血啥的
    }
    public void OnEnterDeployState(GameEventTypeVoid ev) {
        if(lastQuadThisChampionStand is DeployQuad) {
            championStateMachine.Trigger("EnterDeployStateIdle");
        }
    }
    public void OnEnterCombatState(GameEventTypeVoid ev) {
        championStateMachine.Trigger("BattleStart");
    }
    public void OnEnterBonusState(GameEventTypeVoid ev) {
        
    }
    //目前先把空留着,不具体实现
    //应该有不同的羁绊效果的方法,比如
    public void ActivateTestAdditionalEffect(int level) {
        Debug.Log("激活这一级的: "+ level);
    }
    public void RegisterThisChampion() {
        if(isAllyChampion) {
            AllyChampionManager.Instance.RegisterChampion(this);
        }else {
            EnemyChampionManager.Instance.RegisterChampion(this);
        }
    }
    public void UnRegisterThisChampion() {
        if(isAllyChampion) {
            AllyChampionManager.Instance.UnregisterChampion(this);
        }else {
            EnemyChampionManager.Instance.UnregisterChampion(this);
        }
    }
    public Vector3 GetNearestOpponentChampionPos(out Champion targetChampion) {
        targetChampion = null;
        Vector3 result = Vector3.zero;
        if(isAllyChampion) {
           result = EnemyChampionManager.Instance.GetNearestOpponentChampion(this,out targetChampion);
        }else {
           result = AllyChampionManager.Instance.GetNearestOpponentChampion(this,out targetChampion);
        }
        return result;
    }
    public void OnChampionUpgrade(int level) {
        //level = 0 : remove, level = 1 : upgrade to level 1, level = 2 : upgrade to level 2
        if(level == 0) {
            Debug.Log("要卖掉这个英雄");
            //OnSell(GameEventTypeChampion.SELL_A_CHAMPION,this);//应该有自己的函数,不应该用这个
            lastQuadThisChampionStand.OnChampionLeave(this);
            if(lastQuadThisChampionStand is DeployQuad) {
                AllyChampionManager.Instance.OnSpaceChange(Space * -1);
            }
            OnDisappear();
            UnRegisterThisChampion();
            Destroy(gameObject);//不需要检测null
        }else if(level == 1) {
            currentLevel = 1;
            currentCost = tier == 1? 3 : tier * 3 - 1;//一级卡两星卖三块,其余 -1
            Debug.Log("要升两星了");
            transform.localScale *= 1.2f;
        }else if(level == 2) {
            currentLevel = 2;
            currentCost = tier == 1? 9 : tier * 9 - 5;//暂时-5试试
            Debug.Log("要升三星了");
            transform.localScale *= 1.2f;
            //升星之后就应该刷不出这个卡了.
        }
    }
    public void OnSell(GameEventTypeChampion ev,Champion _champion) {
        if(_champion != this) return;//其他英雄不应该卖自己
        //要判断英雄如果是在deploy区域,那么space会减少
        _champion.lastQuadThisChampionStand.OnChampionLeave(_champion);
        if(_champion.lastQuadThisChampionStand is DeployQuad) {
            if(_champion.isAllyChampion) {//目前enemy先不管
                AllyChampionManager.Instance.OnSpaceChange(_champion.Space * -1);
            }
        }
        _champion.OnDisappear();
        _champion.UnRegisterThisChampion();
        Destroy(_champion.gameObject);//不需要检测null
    }
    
    public void OnSellButtonDown(GameEventTypeVoid ev) {
        //可以售卖的逻辑:首先必须鼠标放在上面,并且是正在游玩
        //如果是deploy则是allychampion都可以卖
        //如果是combat则只能卖在preparationquad上面的
        if(isMouseHoveringOnThisChampion && GameManager.Instance.GameManagerStateMachine.ActiveState.name == GameState.PLAY && isAllyChampion) {
            if((GameManager.Instance.PlayState.ActiveState.name == OnPlayState.DEPLOY ) 
            || (GameManager.Instance.PlayState.ActiveState.name == OnPlayState.COMBAT && lastQuadThisChampionStand is PreparationQuad)) { 
                GameEventsManager.TriggerEvent(GameEventTypeChampion.SELL_A_CHAMPION,this);
            }
            
        }
    }
}
public class ChampionAbility:ScriptableObject {//每个英雄的大招不一样
    public virtual void Execute() {
        
    }
}

public class ChampionIdle : StateBase<ChampionState> {//idle是已经在场上了,prepare是还在下面,播放动画都是idle,但是性质不一样
    Animator animator;
    public ChampionIdle(Animator animator, bool needsExitTime) : base(needsExitTime) {
        this.animator = animator;
    }
    public override void OnEnter() {
        Debug.Log("DeployQuad");
        animator.SetTrigger("Idle");
    }
    public override void OnLogic() {
        Debug.Log("DeployQuadOnLogic");
    }
    public override void OnExit() {
        Debug.Log("DeployQuadExit");
    }
}

public class ChampionPrepare: StateBase<ChampionState> {
    Animator animator;
    public ChampionPrepare(Animator animator, bool needsExitTime) : base(needsExitTime) {
        this.animator = animator;
    }
    public override void OnEnter() {
        Debug.Log("Prepare");
        animator.SetTrigger("Idle");
    }
    public override void OnLogic() {
        Debug.Log("PrepareOnLogic");
    }
    public override void OnExit() {
        Debug.Log("PrepareExit");
    }
}
public class ChampionWalk: StateBase<ChampionState> {
    private Animator animator;
    private Champion champion;
    Vector3[] path;
    int targetIndex;
    float speed = 1;
    MonoBehaviour mono;
    Champion targetChampion;
    Action<Champion> attackTrigger;
    bool isWalking;
    public ChampionWalk(Champion champion, Animator animator,Action<Champion> trigger, bool needsExitTime) : base(needsExitTime) {
        mono = champion;
        this.champion = champion;
        this.animator = animator;
        attackTrigger = trigger;
    }
    public override void OnEnter() {
        //战斗开始的时候会进来,这里首先应该判断是否有怪可以攻击
        //没有检测是否存在,为了省事先这样,手动配的时候需要确保有walk
        //需要请求一个寻路,需要知道终点是啥
        //对于ally来说,终点应该是离自己最近的enemy,反之亦然
        CheckNearestOpponent();
    }
    public override void OnLogic() {
        

    }
    public override void OnExit() {
        animator.ResetTrigger("Walk");
    }
    public void OnPathFound(Vector3[] newPath, bool pathSuccessful) {
        //寻路寻到了就开始走
        Debug.Log("路寻到了吗" + pathSuccessful);
        if(pathSuccessful) {
            path = newPath;
            mono.StopCoroutine(FollowPath());
            mono.StartCoroutine(FollowPath());
        }
    }
    public void CheckNearestOpponent() {
        Vector3 target = champion.GetNearestOpponentChampionPos(out targetChampion);
        if(Vector3.Distance(target,champion.transform.position) 
        <= champion.currentChampionStats.attackRange * MapManager.Instance.CurrentMapConfiguration.ScaleRatio) {
            champion.transform.LookAt(targetChampion.transform);
            TriggerAttackBehavior();
        }else {
            FindPathTowardsNearestOpponent();
            //可能出现距离还不够,但是找不到路了?
        }
    }
    public void FindPathTowardsNearestOpponent() {
        Vector3 target = champion.GetNearestOpponentChampionPos(out targetChampion);
        Debug.Log("起点是: " +champion.transform.position + "终点是 " + target);
        PathRequestManager.RequestPath(champion.transform.position,target,OnPathFound);
    }
    public void TriggerAttackBehavior() {
        Debug.Log("那么就说明我找到我要打的敌人了");
        animator.SetTrigger("Attack");
        attackTrigger?.Invoke(targetChampion);
    }
    IEnumerator FollowPath() {
        //不应该这么写
        //逻辑应该是这样的:
        //找到路之后,我先标记我要走的格子walkable = false
        //然后标记我脚下的格子walkable = true
        //然后走到下一格之后,重新寻路,再来一次
        if(path.Length > 0) {
            animator.SetTrigger("Walk");
            Vector3 currentWayPoint = path[0];
            Debug.Log("现在要去哪里" + currentWayPoint);
            champion.QuadStateChange(currentWayPoint);//执行一次,改quad的状态
            while(true) {
                if(champion.transform.position == currentWayPoint) {
                    CheckNearestOpponent();
                    yield break;
                }else {
                    champion.transform.position = Vector3.MoveTowards(champion.transform.position,currentWayPoint,speed * Time.deltaTime);
                    champion.transform.LookAt(currentWayPoint);
                    yield return null;
                }
            }
        }else {
            CheckNearestOpponent();
        }
    }
}
public class ChampionAttack: StateBase<ChampionState> {
    private Animator animator;
    private Champion targetChampion;
    public ChampionAttack(Animator animator, bool needsExitTime) : base(needsExitTime) {
        this.animator = animator;
    }
    public void UpdateTargetChampion(Champion target) {
        targetChampion = target;
    }
    public void HitTarget() {
        //champion的动画判断出champion打到人了,具体逻辑在这里执行
    }
    public override void OnEnter() {
        Debug.Log("这里英雄开始和它战斗了" + targetChampion.ChampionName);
    }
    public override void OnLogic() {
        
    }
    public override void OnExit() {
        
    }
}
public class ChampionDead: StateBase<ChampionState> {
    private Animator animator;
    public ChampionDead(Animator animator,bool needsExitTime) : base(needsExitTime) {
        this.animator = animator;
    }
    public override void OnEnter() {
        
    }
    public override void OnLogic() {
        
    }
    public override void OnExit() {
        
    }
}