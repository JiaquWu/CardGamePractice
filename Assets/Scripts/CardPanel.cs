using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardPanel : PanelBase {
    List<CardButton> cardButtons = new List<CardButton>();
    void OnDisable() {
        GameEventsManager.StopListening(GameEventTypeVoid.ON_REFRESH_BUTTON_DOWN,OnRefreshButtonDown);
    }

    private void OnEnable() {
        cardButtons = GetComponentsInChildren<CardButton>(true).ToList<CardButton>();
        Debug.Log(cardButtons.Count);
        GameEventsManager.StartListening(GameEventTypeVoid.ON_REFRESH_BUTTON_DOWN,OnRefreshButtonDown);
        BaseOnLoad();
        RefreshCardPanel();
        
    }

    void OnRefreshButtonDown(GameEventTypeVoid ev) {
        RefreshCardPanel();
        Debug.Log("refresh!!!!");
    }
    void RefreshCardPanel() {
        if(cardButtons.Count == 0) return;
        foreach (var item in cardButtons) {
            item.OnRefresh(CardManager.GenerateRandomCard());
        }
    }
}
