using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardMenu : MenuBase {

    void Awake() {
        GameEventsManager.StartListening(GameEventTypeVoid.ENTER_DEPLOY_STATE,OnInit);
        GameEventsManager.StartListening(GameEventTypeVoid.EXIT_DEPLOY_STATE,OnExit);
    }

    void OnDisable() {
        GameEventsManager.StopListening(GameEventTypeVoid.ENTER_DEPLOY_STATE,OnInit);
        GameEventsManager.StopListening(GameEventTypeVoid.EXIT_DEPLOY_STATE,OnExit);
    }

    void OnExit(GameEventTypeVoid ev) {

    }

    void OnInit(GameEventTypeVoid ev) {
        
    }
}
