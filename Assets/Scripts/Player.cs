using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : SingletonManager<Player> {
    private int money;
    public int Money => money;




    protected override void Init() {
        GameEventsManager.StartListening(GameEventTypeVoid.ENTER_INIT_STATE,OnEnterInitState);
        GameEventsManager.StartListening(GameEventTypeGameObject.BUY_A_CHAMPION,OnBuyAChampion);
        GameEventsManager.StartListening(GameEventTypeGameObject.SELL_A_CHAMPION,OnSellAChampion);
        
    }
    private void OnEnterInitState(GameEventTypeVoid ev) {
        money = 100;//游戏初始化,那么金币清零
    }
    private void OnBuyAChampion(GameEventTypeGameObject ev,GameObject go) {
        Debug.Log("OnBuyAChampion");
        if(go.TryGetComponent<Champion>(out Champion _champion)) {
           money -= _champion.Cost;
        }
    }
    private void OnSellAChampion(GameEventTypeGameObject ev,GameObject go) {
        if(go.TryGetComponent<Champion>(out Champion _champion)) {
           money += _champion.Cost;
        }
    }
    private void OnDisable() {
        GameEventsManager.StopListening(GameEventTypeVoid.ENTER_INIT_STATE,OnEnterInitState);
        GameEventsManager.StopListening(GameEventTypeGameObject.BUY_A_CHAMPION,OnBuyAChampion);
        GameEventsManager.StopListening(GameEventTypeGameObject.SELL_A_CHAMPION,OnSellAChampion);
    }   
}
