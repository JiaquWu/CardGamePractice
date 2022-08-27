using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : SingletonManager<Player> {
    private int money;
    public int Money => money;




    protected override void Init() {
        GameEventsManager.StartListening(GameEventTypeVoid.ENTER_INIT_STATE,OnEnterInitState);
    }
    private void OnEnterInitState(GameEventTypeVoid ev) {
        money = 100;//游戏初始化,那么金币清零
    }
    private void OnDisable() {
        GameEventsManager.StopListening(GameEventTypeVoid.ENTER_INIT_STATE,OnEnterInitState);
    }   
}
