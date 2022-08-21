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
    private StateMachine<ChampionState> championStateMachine;
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

    private void OnGUI() {
        if(GUILayout.Button("cast ability")) {
            Debug.Log(currentChampionStats.attackDamage);
            traits.Add(new TestTrait(ActivateTestAdditionalEffect));
            traits[0].ActivateTrait(5,this);
            Debug.Log(currentChampionStats.attackDamage);
        }
    }
    private void OnMouseDrag() {
        // Debug.Log(Input.mousePosition);
        // Debug.Log(Camera.main.ScreenToWorldPoint(Input.mousePosition));
        Vector3 screenPos = Camera.main.WorldToScreenPoint(transform.position);
        Debug.Log(screenPos);
        transform.position = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x,Input.mousePosition.y,screenPos.z));
    }
    public void InitFSM() {
        championStateMachine.AddState(ChampionState.PREPARE,new ChampionPrepare(false));
        championStateMachine.AddState(ChampionState.IDLE,new ChampionIdle(Animator,false));//把所有要添加的state加入进来,把需要的参数传进去
        championStateMachine.AddState(ChampionState.WALK,new ChampionWalk(false));
        championStateMachine.AddState(ChampionState.ATTACK,new ChampionAttack(false));
        championStateMachine.AddState(ChampionState.DEAD,new ChampionDead(false));

        championStateMachine.AddTriggerTransition("BattleStart",ChampionState.IDLE,ChampionState.WALK);//战斗开始,开始行动

        championStateMachine.AddTriggerTransitionFromAny("Dead",ChampionState.DEAD);//随时可以会死,prepare虽然不会,但是不触发就好了
    }
    public void OnBattleStart() {//需要找到一种合适的方式触发这件事情
        championStateMachine.Trigger("BattleStart");
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
        
    }
    public override void OnLogic() {
        
    }
    public override void OnExit() {
        
    }
}

public class ChampionPrepare: StateBase<ChampionState> {
    public ChampionPrepare(bool needsExitTime) : base(needsExitTime) {

    }
    public override void OnEnter() {
        
    }
    public override void OnLogic() {
        
    }
    public override void OnExit() {
        
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