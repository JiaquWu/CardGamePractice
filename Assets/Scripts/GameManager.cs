using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FSM;
[Serializable]
public enum GameState {
    INIT,//开始游戏之前的初始化
    PLAY,//整个游戏流程
    IDLE//游戏的默认界面,待定
}
public enum OnPlayState {
    DEPLOY,
    COMBAT,
    BONUS//选秀 待定
}
public class GameManager : SingletonManager<GameManager> {
    
    private StateMachine<GameState,string> gameManagerStateMachine;
    private StateMachine<GameState> initState;
    private StateMachine<GameState,OnPlayState,string> playState;
    private StateMachine<GameState> idleState;
    private void Start() {
        InitFSM();
    }
    private void Update() {
        gameManagerStateMachine.OnLogic();
    }
    void InitFSM() {
        gameManagerStateMachine = new StateMachine<GameState, string>();

        initState = new StateMachine<GameState>();
        initState.AddState(GameState.INIT,new InitState(false,false));

        playState = new StateMachine<GameState, OnPlayState, string>();
        playState.AddState(OnPlayState.DEPLOY,new OnPlayStateDeploy(false));
        playState.AddState(OnPlayState.COMBAT,new OnPlayStateCombat(false));
        playState.AddState(OnPlayState.BONUS,new OnPlayStateBonus(false));
        playState.AddTriggerTransitionFromAny("triggerDeploy",OnPlayState.DEPLOY);
        playState.AddTriggerTransitionFromAny("triggerCombat",OnPlayState.COMBAT);
        playState.AddTriggerTransitionFromAny("triggerBonus",OnPlayState.BONUS);
        
        idleState = new StateMachine<GameState>();
        idleState.AddState(GameState.IDLE,new IdleState(false,false));

        gameManagerStateMachine.AddState(GameState.INIT,initState);
        gameManagerStateMachine.AddState(GameState.PLAY,playState);
        gameManagerStateMachine.AddState(GameState.IDLE,idleState);
        gameManagerStateMachine.AddTriggerTransitionFromAny("triggerInit",GameState.INIT);
        gameManagerStateMachine.AddTriggerTransitionFromAny("triggerPlay",GameState.PLAY);
        gameManagerStateMachine.AddTriggerTransitionFromAny("triggerIdle",GameState.IDLE);

        gameManagerStateMachine.Init();
    }
    public void OnGameStateChange(GameStateSelector selector) {
        switch (selector.state) {
            case GameState.INIT:
            gameManagerStateMachine.Trigger("triggerInit");
            GameEventsManager.TriggerEvent(GameEventTypeVoid.ENTER_INIT_STATE);
            break;
            case GameState.PLAY:
            gameManagerStateMachine.Trigger("triggerPlay");
            GameEventsManager.TriggerEvent(GameEventTypeVoid.ENTER_PLAY_STATE);
            break;
            case GameState.IDLE:
            gameManagerStateMachine.Trigger("triggerIdle");
            GameEventsManager.TriggerEvent(GameEventTypeVoid.ENTER_IDLE_STATE);
            break;
        }
    }
    public void OnOnPlayStateChange(OnPlayStateSelector selector) {
        //如果不在这个阶段在游戏里面是不可能进入这个阶段的
        //if(playState.ActiveState == null) return;//这个方法要改statemachine里面activestate的代码,不去触发报错
        switch (selector.state) {
            case OnPlayState.DEPLOY:
            playState.Trigger("triggerDeploy");
            GameEventsManager.TriggerEvent(GameEventTypeVoid.ENTER_DEPLOY_STATE);
            break;
            case OnPlayState.COMBAT:
            playState.Trigger("triggerCombat");
            GameEventsManager.TriggerEvent(GameEventTypeVoid.ENTER_COMBAT_STATE);
            break;
            case OnPlayState.BONUS:
            playState.Trigger("triggerBonus");
            GameEventsManager.TriggerEvent(GameEventTypeVoid.ENETR_BONUS_STATE);
            break;
        }
    }
}

public class OnPlayStateDeploy : StateBase<OnPlayState> {
    public OnPlayStateDeploy(bool needsExitTime) : base(needsExitTime) {
    
    }
    public override void OnLogic() {
        Debug.Log("OnPlayStateDeploy");
    }
}

public class OnPlayStateCombat : StateBase<OnPlayState> {
    public OnPlayStateCombat(bool needsExitTime) : base(needsExitTime) {
    
    }
    public override void OnLogic() {
        Debug.Log("OnPlayStateCombat");
    }
}

public class OnPlayStateBonus : StateBase<OnPlayState> {
    public OnPlayStateBonus(bool needsExitTime) : base(needsExitTime) {

    }
    public override void OnLogic() {
        Debug.Log("OnPlayStateBonus");
    }
}

public class InitState : StateBase<GameState> {
    public InitState(bool needsExitTime, bool isGhostState = false) : base(needsExitTime, isGhostState) {
        //可能会是ghost state,只是初始化要做一些事情放在这个地方
    }
    public override void OnLogic() {
        Debug.Log("InitState");
    }
}

public class IdleState : StateBase<GameState> {
    public IdleState(bool needsExitTime, bool isGhostState = false) : base(needsExitTime, isGhostState) {

    }
    public override void OnLogic() {
        Debug.Log("IdleState");
    }
}