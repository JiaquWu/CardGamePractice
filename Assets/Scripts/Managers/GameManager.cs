﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FSM;
[Serializable]
public enum GameState {
    INIT,//before playing it 
    PLAY,//core state
    IDLE//tbd
}
public enum OnPlayState {
    INIT,
    DEPLOY,
    COMBAT,
    BONUS,
    EXIT
}
public class GameManager : SingletonManager<GameManager> {
    
    private StateMachine<GameState,string> gameManagerStateMachine;
    private StateMachine<GameState> initState;
    private StateMachine<GameState,OnPlayState,string> playState;
    private StateMachine<GameState> idleState;

    public StateMachine<GameState, string> GameManagerStateMachine => gameManagerStateMachine;
    public StateMachine<GameState, OnPlayState, string> PlayState  => playState;
    public static bool isPlayStateStart;
    public bool isInCombat => playState.ActiveState.name == OnPlayState.COMBAT;
    private int roundCount;
    public int RoundCount => roundCount;
    [SerializeField]
    private EnemyBuildSequence enemyBuildSequence;
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
        playState.AddState(OnPlayState.INIT,new OnPlayStateInit(false));
        playState.AddState(OnPlayState.DEPLOY,new OnPlayStateDeploy(false));
        playState.AddState(OnPlayState.COMBAT,new OnPlayStateCombat(false));
        playState.AddState(OnPlayState.BONUS,new OnPlayStateBonus(false));
        playState.AddState(OnPlayState.EXIT,new OnPlayStateExit(false));
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
            break;
            case GameState.PLAY:
            gameManagerStateMachine.Trigger("triggerPlay"); 
            break;
            case GameState.IDLE:
            gameManagerStateMachine.Trigger("triggerIdle");
            break;
        }
    }
    public void OnOnPlayStateChange(OnPlayStateSelector selector) {

        switch (selector.state) {
            case OnPlayState.DEPLOY:
            playState.Trigger("triggerDeploy");
            break;
            case OnPlayState.COMBAT:
            playState.Trigger("triggerCombat");
            break;
            case OnPlayState.BONUS:
            playState.Trigger("triggerBonus");
            break;
        }
    }
    public void AddRoundCount() {
        roundCount ++;
    }
    public EnemyBuildSO GetCurrentEnemyBuild() {
        if(roundCount >= 1) {
            return enemyBuildSequence.GetCurrentEnemyBuild(roundCount-1);
        }
        return null;
        
    }
}
public class OnPlayStateInit : StateBase<OnPlayState> {
    public OnPlayStateInit(bool needsExitTime, bool isGhostState = false) : base(needsExitTime, isGhostState) {
    }
    public override void OnEnter() {
        GameEventsManager.TriggerEvent(GameEventTypeVoid.ENTER_PLAY_STATE);
    }
    public override void OnExit() {
        
    }
}
public class OnPlayStateExit : StateBase<OnPlayState> {
    public OnPlayStateExit(bool needsExitTime, bool isGhostState = false) : base(needsExitTime, isGhostState) {
    }
    public override void OnExit() {
        GameEventsManager.TriggerEvent(GameEventTypeVoid.EXIT_PLAY_STATE);
    }
}
public class OnPlayStateDeploy : StateBase<OnPlayState> {
    public OnPlayStateDeploy(bool needsExitTime) : base(needsExitTime) {
    
    }
    public override void OnEnter() {
        GameManager.Instance.AddRoundCount();
        GameEventsManager.TriggerEvent(GameEventTypeVoid.ENTER_DEPLOY_STATE);
        if(GameManager.isPlayStateStart == false) GameManager.isPlayStateStart = true;
    }
    public override void OnExit() {
        GameEventsManager.TriggerEvent(GameEventTypeVoid.EXIT_DEPLOY_STATE);
    }
    public override void OnLogic() {

    }
    
}

public class OnPlayStateCombat : StateBase<OnPlayState> {
    public OnPlayStateCombat(bool needsExitTime) : base(needsExitTime) {
    
    }
    public override void OnEnter() {
        GameEventsManager.TriggerEvent(GameEventTypeVoid.ENTER_COMBAT_STATE);
    }
    public override void OnExit() {
        GameEventsManager.TriggerEvent(GameEventTypeVoid.EXIT_COMBAT_STATE);
    }
    public override void OnLogic() {

    }
}

public class OnPlayStateBonus : StateBase<OnPlayState> {
    public OnPlayStateBonus(bool needsExitTime) : base(needsExitTime) {

    }
    public override void OnEnter() {
        GameEventsManager.TriggerEvent(GameEventTypeVoid.ENETR_BONUS_STATE);
    }
    public override void OnExit() {
        GameEventsManager.TriggerEvent(GameEventTypeVoid.EXIT_BONUS_STATE);
    }
    public override void OnLogic() {

    }
}

public class InitState : StateBase<GameState> {
    public InitState(bool needsExitTime, bool isGhostState = false) : base(needsExitTime, isGhostState) {
        //might be a ghost state, do some init stuff?
    }
    public override void OnEnter() {
        GameEventsManager.TriggerEvent(GameEventTypeVoid.ENTER_INIT_STATE);
    }
    public override void OnExit() {
        GameEventsManager.TriggerEvent(GameEventTypeVoid.EXIT_INIT_STATE);
    }
    public override void OnLogic() {

    }
}

public class IdleState : StateBase<GameState> {
    public IdleState(bool needsExitTime, bool isGhostState = false) : base(needsExitTime, isGhostState) {

    }
    public override void OnEnter() {
        GameEventsManager.TriggerEvent(GameEventTypeVoid.ENTER_IDLE_STATE);
    }
    public override void OnExit() {
        GameEventsManager.TriggerEvent(GameEventTypeVoid.EXIT_IDLE_STATE);
    }
    public override void OnLogic() {

    }
}