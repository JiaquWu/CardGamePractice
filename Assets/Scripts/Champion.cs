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
    Quad lastQuad = null;
    Quad currentQuad = null;

    private void Update() {
        if(championStateMachine == null) //for testing
        championStateMachine.OnLogic();
    }
    private void OnGUI() {
        if(GUILayout.Button("cast ability")) {
            Debug.Log(currentChampionStats.attackDamage);
            traits.Add(new TestTrait(ActivateTestAdditionalEffect));
            traits[0].ActivateTrait(5,this);
            Debug.Log(currentChampionStats.attackDamage);
        }
    }
    public void OnDeploy() {
        //被部署到备战席的时候应该调用一些方法
        InitFSM();
        RegisterThisChampion();
        GameEventsManager.StartListening(GameEventTypeVoid.ENTER_DEPLOY_STATE,OnEnterDeployState);
        GameEventsManager.StartListening(GameEventTypeVoid.ENTER_COMBAT_STATE,OnEnterCombatState);
        GameEventsManager.StartListening(GameEventTypeVoid.ENETR_BONUS_STATE,OnEnterBonusState);
    }
    public void OnDisappear() {//卖掉或者升级会触发的函数,主要是取消事件监听
        GameEventsManager.StopListening(GameEventTypeVoid.ENTER_DEPLOY_STATE,OnEnterDeployState);
        GameEventsManager.StopListening(GameEventTypeVoid.ENTER_COMBAT_STATE,OnEnterCombatState);
        GameEventsManager.StopListening(GameEventTypeVoid.ENETR_BONUS_STATE,OnEnterBonusState);
    }
    private void OnMouseDrag() {
        // Debug.Log(Input.mousePosition);
        // Debug.Log(Camera.main.ScreenToWorldPoint(Input.mousePosition));
        Vector3 screenPos = Camera.main.WorldToScreenPoint(transform.position);
        Vector3 temp = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x,Input.mousePosition.y,screenPos.z));
        temp.y = QuadsManager.Instance.CurrentMap.OriginPoint.y;
        transform.position = temp;
        //还要通过quadmanager知道当前对应的quad,调用高亮和退出的方法
        currentQuad = QuadsManager.Instance.GetQuadByPosition(transform.position);
        if(currentQuad == null) {
            //应该让英雄停留在上个格子
            transform.position = lastQuad.node.worldPosition;
            return;
        }
        if(lastQuad != null && lastQuad != currentQuad) {//说明进入了新的quad
            lastQuad.OnChampionExit();
        }
        currentQuad.OnChampionEnter();
        lastQuad = currentQuad;
    }
    private void OnMouseUp() {
        if(lastQuad != null) {
            transform.position = lastQuad.node.worldPosition;
            lastQuad.OnChampionStay();
            if(lastQuad is DeployQuad) {
                championStateMachine.Trigger("OnDeployQuad");
            }else if(lastQuad is PreparationQuad){     
                championStateMachine.Trigger("OnPreparationQuad");
            }
        }
        
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