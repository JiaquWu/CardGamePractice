using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : SingletonManager<InputManager> {
    public bool IsLeftMouseButtonPressed => Input.GetMouseButton(0);
    private void Update() {
        if(Input.GetButtonDown("Sell")) {
            OnSellButtonDown();
        }
        if(Input.GetButtonDown("Refresh")) {
            OnRefreshButtonDown();
        }
        if(Input.GetButtonDown("BuyExp")) {
            OnBuyLevelButtonDown();
        }
    }
    public void OnSellButtonDown() {
        Debug.Log("sell");
        GameEventsManager.TriggerEvent(GameEventTypeVoid.ON_SELL_BUTTON_DOWN);
    }
    public void OnRefreshButtonDown() {
        Debug.Log("refresh");
        GameEventsManager.TriggerEvent(GameEventTypeVoid.ON_REFRESH_BUTTON_DOWN);
    }
    public void OnBuyLevelButtonDown() {
        Debug.Log("buy exp");
        GameEventsManager.TriggerEvent(GameEventTypeVoid.ON_BUY_EXPERIENCE_BUTTON_DOWN);
    }
}
