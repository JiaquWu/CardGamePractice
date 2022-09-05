using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//需要手动在unity中关闭
public class CardPanel : PanelBase {
    List<CardButton> cardButtons = new List<CardButton>();
    [SerializeField]
    private Text goldText;
    [SerializeField]
    private Text levelText;
    [SerializeField]
    private Text expText;
    private void OnEnable() {
        cardButtons = GetComponentsInChildren<CardButton>(true).ToList<CardButton>();
        GameEventsManager.StartListening(GameEventTypeVoid.EXECUTE_REFRESH,OnRefreshExecute);
        GameEventsManager.StartListening(GameEventTypeInt.UPDATE_MONEY,UpdateGoldText);
        GameEventsManager.StartListening(GameEventTypeInt.UPDATE_LEVEL,UpdateLevelText);
        GameEventsManager.StartListening(GameEventTypeInt.UPDATE_EXP,UpdateExpText);
        BaseOnLoad();
        RefreshCardPanel();
        GameEventsManager.TriggerEvent(GameEventTypeInt.UPDATE_MONEY,Player.Instance.Money);//这三个需要初始化刷新
        GameEventsManager.TriggerEvent(GameEventTypeInt.UPDATE_EXP,Player.Instance.CurrentExp);
        GameEventsManager.TriggerEvent(GameEventTypeInt.UPDATE_LEVEL,Player.Instance.CurrentLevel);
    }
    void OnDisable() {
        GameEventsManager.StopListening(GameEventTypeVoid.EXECUTE_REFRESH,OnRefreshExecute);
        GameEventsManager.StopListening(GameEventTypeInt.UPDATE_MONEY,UpdateGoldText);
        GameEventsManager.StopListening(GameEventTypeInt.UPDATE_LEVEL,UpdateLevelText);
        GameEventsManager.StopListening(GameEventTypeInt.UPDATE_EXP,UpdateExpText);
    }
    void OnRefreshExecute(GameEventTypeVoid ev) {
        RefreshCardPanel();
        Debug.Log("refresh!!!!");
    }
    void RefreshCardPanel() {
        if(cardButtons.Count == 0) return;
        foreach (var item in cardButtons) {
            item.OnRefresh(CardManager.GenerateRandomCard());
        }
    }
    void UpdateGoldText(GameEventTypeInt ev,int goldAmount) {
        if(goldText != null) {
            goldText.text = "Gold : " + goldAmount;
        }
    }
    void UpdateLevelText(GameEventTypeInt ev,int currentLevel) {
        if(levelText != null) {
            levelText.text = "Level : " + currentLevel;
        }
    }
    void UpdateExpText(GameEventTypeInt ev,int currentExp) {
        if(expText != null) {
            expText.text = "Exp : " + currentExp + " / " + GameRulesManager.experienceRequirementByLevel[Player.Instance.CurrentLevel];
            //这里没有检测后面字典是否能找到是因为触发这个函数的代码有检测，但是这样还是不是很好
        }
    }
}
