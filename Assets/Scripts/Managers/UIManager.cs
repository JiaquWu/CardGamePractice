using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum UIElement {
    GAME_INIT_UI,
    GAME_IDLE_UI,
    GAME_PLAY_UI,
    CARD_PANEL,
    TRAIT_PANEL
}
public class UIManager : SingletonManager<UIManager> {
    //这个类主要是要控制在不同的游戏阶段该出现哪些UI
    [SerializeField]
    private GameObject cardPanel;
    [SerializeField]
    private GameObject traitPanel;
    public static Dictionary<UIElement,GameObject> uiElements;
    protected override void Init() {
        uiElements = new Dictionary<UIElement, GameObject>() {
            {UIElement.CARD_PANEL,cardPanel},
            {UIElement.TRAIT_PANEL,traitPanel}
        };
        foreach (var element in uiElements.Values) {
            element.SetActive(false);
        }
        GameEventsManager.StartListening(GameEventTypeVoid.ENTER_DEPLOY_STATE,OnEnterDeployState);//第一次进来就会打开了
        GameEventsManager.StartListening(GameEventTypeVoid.EXIT_PLAY_STATE,OnExitPlayState);//结束游戏才会消失
    }

    void OnEnterDeployState(GameEventTypeVoid ev) {
        if(uiElements[UIElement.CARD_PANEL] != null) {
            uiElements[UIElement.CARD_PANEL].SetActive(true);
        }
        if(uiElements[UIElement.TRAIT_PANEL] != null) {
            uiElements[UIElement.TRAIT_PANEL].SetActive(true);
        }
    }
    void OnExitPlayState(GameEventTypeVoid ev) {
        if(uiElements[UIElement.CARD_PANEL] != null) {
            uiElements[UIElement.CARD_PANEL].SetActive(false);
        }
        if(uiElements[UIElement.TRAIT_PANEL] != null) {
            uiElements[UIElement.TRAIT_PANEL].SetActive(false);
        }
    }
    private void OnDisable() {
        GameEventsManager.StopListening(GameEventTypeVoid.ENTER_DEPLOY_STATE,OnEnterDeployState);
        GameEventsManager.StopListening(GameEventTypeVoid.EXIT_PLAY_STATE,OnExitPlayState);
    }
}
