using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FSM;

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
    private StateMachine<GameState,OnPlayState,string> initState;
    private StateMachine<GameState,OnPlayState,string> playState;
    private StateMachine<GameState,OnPlayState,string> idleState;

    void InitFSM() {
        initState = new StateMachine<GameState, OnPlayState, string>();
        
        playState = new StateMachine<GameState, OnPlayState, string>();
        playState.AddState(OnPlayState.DEPLOY,new OnPlayStateDeploy(false));
        
        idleState = new StateMachine<GameState, OnPlayState, string>();


        gameManagerStateMachine.AddState(GameState.INIT,initState);
        gameManagerStateMachine.AddState(GameState.PLAY,playState);
        gameManagerStateMachine.AddState(GameState.IDLE,idleState);


        gameManagerStateMachine.Init();
    }
}

public class OnPlayStateDeploy : StateBase<OnPlayState> {
    public OnPlayStateDeploy(bool needsExitTime) : base(needsExitTime) {
    
    }
}

public class OnPlayStateCombat : StateBase<OnPlayState> {
    public OnPlayStateCombat(bool needsExitTime) : base(needsExitTime) {
    
    }
}

public class OnPlayStateBonus : StateBase<OnPlayState> {
    public OnPlayStateBonus(bool needsExitTime) : base(needsExitTime) {
    }
}