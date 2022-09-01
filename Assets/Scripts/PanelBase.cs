using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PanelBase : MonoBehaviour {
    // protected virtual void Awake() {
        
    // }
    // protected virtual void OnInit() {

    // }
    // protected virtual void OnExit() {

    // }
    // protected virtual void OnDisable() {
        
    // }
    public CanvasGroup canvasGroup;
    public void BaseOnLoad() {
        canvasGroup = GetComponent<CanvasGroup>();
        if(canvasGroup == null) {
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
        }
        canvasGroup.blocksRaycasts = true;
        canvasGroup.interactable = true;
        canvasGroup.alpha = 1;
    }
}
