using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : SingletonManager<Player> {
    private int money;
    public int Money => money;

    private List<float> currentChampionDropRate;
    public List<float> CurrentChampionDropRate => currentChampionDropRate;
    private int currentLevel;
    public int CurrentLevel => currentLevel;
    private int currentExp;
    public int CurrentExp => currentExp;
    private int currentRefreshCost;
    private int currentBuyExpCost;
    private int currentExpIncrementEachBuy;
    protected override void Init() {
        GameEventsManager.StartListening(GameEventTypeVoid.ENTER_PLAY_STATE,OnEnterPlayState);
        GameEventsManager.StartListening(GameEventTypeVoid.ENTER_DEPLOY_STATE,OnEnterDeployState);
        GameEventsManager.StartListening(GameEventTypeVoid.ON_REFRESH_BUTTON_DOWN,OnRefreshButtonDown);
        GameEventsManager.StartListening(GameEventTypeVoid.ON_BUY_EXPERIENCE_BUTTON_DOWN,OnBuyExpButtonDown);
        GameEventsManager.StartListening(GameEventTypeInt.GAIN_EXPERIENCE,OnExpGain);
        GameEventsManager.StartListening(GameEventTypeChampion.BUY_A_CHAMPION,OnBuyAChampion);
        GameEventsManager.StartListening(GameEventTypeChampion.SELL_A_CHAMPION,OnSellAChampion);
        GameEventsManager.StartListening(GameEventTypeInt.LEVEL_UP,OnLevelUp);
        
    }
    private void OnEnterPlayState(GameEventTypeVoid ev) {
        money = 100;//游戏初始化,那么金币清零
        currentLevel = 1;
        currentExp = 0;
        currentRefreshCost = GameRulesManager.defaultRefreshCost;
        currentBuyExpCost = GameRulesManager.defaultBuyExpCost;
        currentExpIncrementEachBuy = GameRulesManager.defaultExpIncrementEachBuy;
        currentChampionDropRate = GameRulesManager.championDropRatesByLevel[1];
    }
    private void OnBuyAChampion(GameEventTypeChampion ev,Champion _champion) {
        money -= _champion.Cost;
        GameEventsManager.TriggerEvent(GameEventTypeInt.UPDATE_MONEY,money);
    }
    private void OnSellAChampion(GameEventTypeChampion ev,Champion _champion) {
        money += _champion.Cost;
        GameEventsManager.TriggerEvent(GameEventTypeInt.UPDATE_MONEY,money);
    }
    private void OnRefreshButtonDown(GameEventTypeVoid ev) {
        if(money >= currentRefreshCost) {
            money -= currentRefreshCost;
            GameEventsManager.TriggerEvent(GameEventTypeVoid.EXECUTE_REFRESH);
            GameEventsManager.TriggerEvent(GameEventTypeInt.UPDATE_MONEY,money);
        }
    }
    private void OnBuyExpButtonDown(GameEventTypeVoid ev) {
        if(money >= currentBuyExpCost) {
            money -= currentBuyExpCost;
            GameEventsManager.TriggerEvent(GameEventTypeInt.UPDATE_MONEY,money);
            GameEventsManager.TriggerEvent(GameEventTypeInt.GAIN_EXPERIENCE,currentExpIncrementEachBuy);
        }
    }
    private void OnExpGain(GameEventTypeInt ev,int expGaining) {
        //这里要加经验，然后计算有没有升级
        currentExp += expGaining;
        if(GameRulesManager.experienceRequirementByLevel.ContainsKey(currentLevel)) {
            int currentExpRequirement = GameRulesManager.experienceRequirementByLevel[currentLevel];//知道当前升级所需经验
            if(currentExp >= currentExpRequirement) {
                //说明要升级了
                currentExp -= currentExpRequirement;
                currentLevel += 1;
                GameEventsManager.TriggerEvent(GameEventTypeInt.LEVEL_UP,currentLevel);//触发升级
                GameEventsManager.TriggerEvent(GameEventTypeInt.UPDATE_LEVEL,currentLevel);
            }
            //升不升级都要更新一下UI
            GameEventsManager.TriggerEvent(GameEventTypeInt.UPDATE_EXP,currentExp);
        }else {
            Debug.LogError("current level is not existing in the dictionary");
        }
    }
    private void OnLevelUp(GameEventTypeInt ev,int targetLevel) {
        if(GameRulesManager.championDropRatesByLevel.ContainsKey(targetLevel)) {
            currentChampionDropRate = GameRulesManager.championDropRatesByLevel[targetLevel];
        }
    }
    private void OnEnterDeployState(GameEventTypeVoid ev) {
        if(GameManager.isPlayStateStart) {//说明不是第一次进入
            GameEventsManager.TriggerEvent(GameEventTypeInt.GAIN_EXPERIENCE,GameRulesManager.defaultExpIncrementEachTurn);
        }
    }
    private void OnDisable() {
        GameEventsManager.StopListening(GameEventTypeVoid.ENTER_PLAY_STATE,OnEnterPlayState);
        GameEventsManager.StopListening(GameEventTypeVoid.ENTER_DEPLOY_STATE,OnEnterDeployState);
        GameEventsManager.StopListening(GameEventTypeVoid.ON_REFRESH_BUTTON_DOWN,OnRefreshButtonDown);
        GameEventsManager.StopListening(GameEventTypeVoid.ON_BUY_EXPERIENCE_BUTTON_DOWN,OnBuyExpButtonDown);
        GameEventsManager.StopListening(GameEventTypeInt.GAIN_EXPERIENCE,OnExpGain);
        GameEventsManager.StopListening(GameEventTypeChampion.BUY_A_CHAMPION,OnBuyAChampion);
        GameEventsManager.StopListening(GameEventTypeChampion.SELL_A_CHAMPION,OnSellAChampion);
        GameEventsManager.StopListening(GameEventTypeInt.LEVEL_UP,OnLevelUp);
    }   
}
