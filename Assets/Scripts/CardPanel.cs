using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardPanel : PanelBase {
    void OnDisable() {
        GameEventsManager.StopListening(GameEventTypeVoid.ON_REFRESH_BUTTON_DOWN,OnRefreshButtonDown);
    }

    private void OnEnable() {
        BaseOnLoad();
        GameEventsManager.StartListening(GameEventTypeVoid.ON_REFRESH_BUTTON_DOWN,OnRefreshButtonDown);
    }

    void OnRefreshButtonDown(GameEventTypeVoid ev) {
        Debug.Log("refresh!!!!");
    }
}
