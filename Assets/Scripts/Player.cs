using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : SingletonManager<Player> {
    private int money;
    public int Money => money;

    private List<float> currentChampionDropRate;
    public List<float> CurrentChampionDropRate => currentChampionDropRate;
    int currentLevel;

    protected override void Init() {
        GameEventsManager.StartListening(GameEventTypeVoid.ENTER_PLAY_STATE,OnEnterPlayState);
        GameEventsManager.StartListening(GameEventTypeChampion.BUY_A_CHAMPION,OnBuyAChampion);
        GameEventsManager.StartListening(GameEventTypeChampion.SELL_A_CHAMPION,OnSellAChampion);
        GameEventsManager.StartListening(GameEventTypeInt.LEVEL_UP,OnLevelUp);
        
    }
    private void OnEnterPlayState(GameEventTypeVoid ev) {
        money = 100;//游戏初始化,那么金币清零
        currentLevel = 1;
        currentChampionDropRate = GameRulesManager.championDropRatesByLevel[1];
    }
    private void OnBuyAChampion(GameEventTypeChampion ev,Champion _champion) {
        money -= _champion.Cost;
        Debug.Log("还有这么多钱: " + money);
    }
    private void OnSellAChampion(GameEventTypeChampion ev,Champion _champion) {
        money += _champion.Cost;
        Debug.Log("还有这么多钱: " + money);
    }
    private void OnLevelUp(GameEventTypeInt ev,int targetLevel) {
        if(GameRulesManager.championDropRatesByLevel.ContainsKey(targetLevel)) {
            currentChampionDropRate = GameRulesManager.championDropRatesByLevel[targetLevel];
        }
    }
    private void OnDisable() {
        GameEventsManager.StopListening(GameEventTypeVoid.ENTER_PLAY_STATE,OnEnterPlayState);
        GameEventsManager.StopListening(GameEventTypeChampion.BUY_A_CHAMPION,OnBuyAChampion);
        GameEventsManager.StopListening(GameEventTypeChampion.SELL_A_CHAMPION,OnSellAChampion);
        GameEventsManager.StopListening(GameEventTypeInt.LEVEL_UP,OnLevelUp);
    }   
}
