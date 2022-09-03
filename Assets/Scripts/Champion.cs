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
    private int defaultCost;//应该某种办法让card的和它保持一致,如果升级了应该发生变化,先手动配吧
    private int currentCost;
    public int Cost => currentCost;
    private int defaultLevel = 0;
    private int currentLevel;
    public int Level => currentLevel;
    private bool isMouseHoveringOnThisChampion;
    [SerializeField]
    private bool isAllyChampion;//true就是友方
    private StateMachine<ChampionState> championStateMachine = new StateMachine<ChampionState>();
    protected Animator animator;
    protected Animator Animator {
        get{
            if(animator == null) {
                animator = GetComponent<Animator>();
            }
            return animator;
        }
    }
    private ChampionStats defaultChampionStats;//默认的英雄数值,但是怎么合理配置呢?
    public ChampionStats currentChampionStats;//当前英雄数值
    private List<TraitBase> traits = new List<TraitBase>();//一个英雄拥有的所有羁绊
    public ChampionAbility championAbility;

    protected ChampionState currentChampionState;
    Quad lastQuadThisChampionStand = null;//这个变量只有在鼠标松开之后变化,和换位置有关
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
    public void OnDeploy(Quad quadToStay) {
        //被部署到备战席的时候应该调用一些方法
        isAllyChampion = true;
        currentLevel = defaultLevel;
        InitFSM();
        RegisterThisChampion();
        OnEnterQuad(quadToStay);
        GameEventsManager.StartListening(GameEventTypeVoid.ENTER_DEPLOY_STATE,OnEnterDeployState);
        GameEventsManager.StartListening(GameEventTypeVoid.ENTER_COMBAT_STATE,OnEnterCombatState);
        GameEventsManager.StartListening(GameEventTypeVoid.ENETR_BONUS_STATE,OnEnterBonusState);
        GameEventsManager.StartListening(GameEventTypeGameObject.SELL_A_CHAMPION,OnSell);
        GameEventsManager.StartListening(GameEventTypeVoid.ON_SELL_BUTTON_DOWN,OnSellButtonDown);
    }
    public void OnDisappear() {//卖掉或者升级会触发的函数,主要是取消事件监听
        GameEventsManager.StopListening(GameEventTypeVoid.ENTER_DEPLOY_STATE,OnEnterDeployState);
        GameEventsManager.StopListening(GameEventTypeVoid.ENTER_COMBAT_STATE,OnEnterCombatState);
        GameEventsManager.StopListening(GameEventTypeVoid.ENETR_BONUS_STATE,OnEnterBonusState);
        GameEventsManager.StopListening(GameEventTypeGameObject.SELL_A_CHAMPION,OnSell);
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
        }else if(quad is PreparationQuad){     
            championStateMachine.Trigger("OnPreparationQuad");
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
    private void OnMouseDrag() {
        // Debug.Log(Input.mousePosition);
        // Debug.Log(Camera.main.ScreenToWorldPoint(Input.mousePosition));
        Vector3 screenPos = Camera.main.WorldToScreenPoint(transform.position);
        Vector3 temp = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x,Input.mousePosition.y,screenPos.z));
        temp.y = QuadsManager.Instance.CurrentMap.OriginPoint.y;
        transform.position = temp;
        //还要通过quadmanager知道当前对应的quad,调用高亮和退出的方法
        currentMouseHoveringQuad = QuadsManager.Instance.GetQuadByPosition(transform.position);
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
    private void OnMouseUp() {
        if(lastMouseHoveringQuad != null) {
            // transform.position = lastMouseHoveringQuad.node.worldPosition;
            // lastMouseHoveringQuad.OnChampionStay(this);
            OnEnterQuad(lastMouseHoveringQuad);
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
        championStateMachine.AddState(ChampionState.PREPARE,new ChampionPrepare(false));
        championStateMachine.AddState(ChampionState.IDLE,new ChampionIdle(Animator,false));//把所有要添加的state加入进来,把需要的参数传进去
        championStateMachine.AddState(ChampionState.WALK,new ChampionWalk(false));
        championStateMachine.AddState(ChampionState.ATTACK,new ChampionAttack(false));
        championStateMachine.AddState(ChampionState.DEAD,new ChampionDead(false));

        championStateMachine.AddTriggerTransition("BattleStart",ChampionState.IDLE,ChampionState.WALK);//战斗开始,开始行动
        championStateMachine.AddTriggerTransition("OnPreparationQuad",ChampionState.IDLE,ChampionState.PREPARE);
        championStateMachine.AddTriggerTransition("OnDeployQuad",ChampionState.PREPARE,ChampionState.IDLE);

        championStateMachine.AddTriggerTransitionFromAny("Dead",ChampionState.DEAD);//随时可以会死,prepare虽然不会,但是不触发就好了

        championStateMachine.Init();
    }
    public void OnEnterDeployState(GameEventTypeVoid ev) {
        
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
            AllyChampionManager.RegisterChampion(this);
        }else {
            EnemyChampionManager.RegisterChampion(this);
        }
    }
    public void UnRegisterThisChampion() {
        if(isAllyChampion) {
            AllyChampionManager.UnregisterChampion(this);
        }else {
            EnemyChampionManager.UnregisterChampion(this);
        }
    }
    public void OnChampionUpgrade(int level) {
        //level = 0 : remove, level = 1 : upgrade to level 1, level = 2 : upgrade to level 2
        if(level == 0) {
            Debug.Log("要卖掉这个英雄");
            OnSell(GameEventTypeGameObject.SELL_A_CHAMPION,gameObject);//应该有自己的函数,不应该用这个
        }else if(level == 1) {
            currentLevel = 1;
            Debug.Log("要升两星了");
            transform.localScale *= 1.2f;
        }else if(level == 2) {
            currentLevel = 2;
            Debug.Log("要升三星了");
            transform.localScale *= 1.2f;
        }
    }
    public void OnSell(GameEventTypeGameObject ev,GameObject go) {
        if(go.TryGetComponent<Champion>(out Champion _champion)) {
            _champion.OnDisappear();
            _champion.UnRegisterThisChampion();
            if(go != null) {
                Destroy(go);
            }
            
        }
    }
    public void OnSellButtonDown(GameEventTypeVoid ev) {
        //可以售卖的逻辑:首先必须鼠标放在上面,并且是正在游玩
        //如果是deploy则是allychampion都可以卖
        //如果是combat则只能卖在preparationquad上面的
        if(isMouseHoveringOnThisChampion && GameManager.Instance.GameManagerStateMachine.ActiveState.name == GameState.PLAY && isAllyChampion) {
            if((GameManager.Instance.PlayState.ActiveState.name == OnPlayState.DEPLOY ) 
            || (GameManager.Instance.PlayState.ActiveState.name == OnPlayState.COMBAT && lastQuadThisChampionStand is PreparationQuad)) { 
                GameEventsManager.TriggerEvent(GameEventTypeGameObject.SELL_A_CHAMPION,gameObject);
            }
            
        }
    }
}
public class ChampionAbility:ScriptableObject {//每个英雄的大招不一样
    public virtual void Execute() {
        
    }
}

public class ChampionIdle : StateBase<ChampionState> {
    Animator animator;
    public ChampionIdle(Animator animator, bool needsExitTime) : base(needsExitTime) {

    }
    public override void OnEnter() {
        Debug.Log("DeployQuad");
    }
    public override void OnLogic() {
        Debug.Log("DeployQuadOnLogic");
    }
    public override void OnExit() {
        Debug.Log("DeployQuadExit");
    }
}

public class ChampionPrepare: StateBase<ChampionState> {
    public ChampionPrepare(bool needsExitTime) : base(needsExitTime) {

    }
    public override void OnEnter() {
        Debug.Log("Prepare");
    }
    public override void OnLogic() {
        Debug.Log("PrepareOnLogic");
    }
    public override void OnExit() {
        Debug.Log("PrepareExit");
    }
}
public class ChampionWalk: StateBase<ChampionState> {
    public ChampionWalk(bool needsExitTime) : base(needsExitTime) {

    }
    public override void OnEnter() {
        
    }
    public override void OnLogic() {
        
    }
    public override void OnExit() {
        
    }
}
public class ChampionAttack: StateBase<ChampionState> {
    public ChampionAttack(bool needsExitTime) : base(needsExitTime) {

    }
    public override void OnEnter() {
        
    }
    public override void OnLogic() {
        
    }
    public override void OnExit() {
        
    }
}
public class ChampionDead: StateBase<ChampionState> {
    public ChampionDead(bool needsExitTime) : base(needsExitTime) {

    }
    public override void OnEnter() {
        
    }
    public override void OnLogic() {
        
    }
    public override void OnExit() {
        
    }
}