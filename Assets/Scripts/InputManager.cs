using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : SingletonManager<InputManager> {
    public bool IsLeftMouseButtonPressed => Input.GetMouseButton(0); //鼠标左键有没有按下
    private void Update() {
        if(Input.GetButtonDown("Sell")) {
            Debug.Log("sell");
            GameEventsManager.TriggerEvent(GameEventTypeVoid.ON_SELL_BUTTON_DOWN);
        }
    }
}
