using System.Linq;

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardManager : SingletonManager<CardManager> {
    [SerializeField]
    private List<CardSO> level1CardSOs;
    [SerializeField]
    private List<CardSO> level2CardSOs;
    [SerializeField]
    private List<CardSO> level3CardSOs;
    [SerializeField]
    private List<CardSO> level4CardSOs;
    [SerializeField]
    private List<CardSO> level5CardSOs;
    private static List<CardSO> level1Cards = new List<CardSO>();//卡池里面的所有一费卡
    private static List<CardSO> level2Cards = new List<CardSO>();
    private static List<CardSO> level3Cards = new List<CardSO>();
    private static List<CardSO> level4Cards = new List<CardSO>();
    private static List<CardSO> level5Cards = new List<CardSO>();
    public static Dictionary<int,List<CardSO>> currentAvailableCards = new Dictionary<int, List<CardSO>>() {//int = tier
        {1,level1Cards},
        {2,level2Cards},
        {3,level3Cards},
        {4,level4Cards},
        {5,level5Cards}
    };
    public static Dictionary<string,CardSO> championCardDict = new Dictionary<string, CardSO>();
    protected override void Init() {
        GameEventsManager.StartListening(GameEventTypeVoid.ENTER_PLAY_STATE,InitCardLists);
        GameEventsManager.StartListening(GameEventTypeChampion.BUY_A_CHAMPION,OnBuyAChampion);
        GameEventsManager.StartListening(GameEventTypeChampion.SELL_A_CHAMPION,OnSellAChampion);
        GameEventsManager.StartListening(GameEventTypeChampion.CHAMPION_UPGRADE_LEVEL_2,OnChampionUpgradeLevel2);
    }
    private void OnDisable() {
        GameEventsManager.StopListening(GameEventTypeVoid.ENTER_PLAY_STATE,InitCardLists);
        GameEventsManager.StopListening(GameEventTypeChampion.BUY_A_CHAMPION,OnBuyAChampion);
        GameEventsManager.StopListening(GameEventTypeChampion.SELL_A_CHAMPION,OnSellAChampion);
        GameEventsManager.StopListening(GameEventTypeChampion.CHAMPION_UPGRADE_LEVEL_2,OnChampionUpgradeLevel2);
    }
    private void InitCardLists(GameEventTypeVoid ev) {//感觉很笨
        for (int i = 0; i < Instance.level1CardSOs.Count; i++) {
            for (int j = 0; j < GameRulesManager.championPoolSizeByLevel[i+1]; j++) {
                level1Cards.Add(Instance.level1CardSOs[i]);
            }
            Champion champion = Instance.level1CardSOs[i].ChampionPrefab.GetComponent<Champion>();
            if(champion != null) {
                championCardDict.Add(champion.ChampionName,Instance.level1CardSOs[i]);
            } 
        }
        for (int i = 0; i < Instance.level2CardSOs.Count; i++) {
            for (int j = 0; j < GameRulesManager.championPoolSizeByLevel[i+1]; j++) {
                level2Cards.Add(Instance.level2CardSOs[i]);
            }
            Champion champion = Instance.level2CardSOs[i].ChampionPrefab.GetComponent<Champion>();
            if(champion != null) {
                championCardDict.Add(champion.ChampionName,Instance.level2CardSOs[i]);
            } 
        }
        for (int i = 0; i < Instance.level3CardSOs.Count; i++) {
            for (int j = 0; j < GameRulesManager.championPoolSizeByLevel[i+1]; j++) {
                level3Cards.Add(Instance.level3CardSOs[i]);
            }
            Champion champion = Instance.level3CardSOs[i].ChampionPrefab.GetComponent<Champion>();
            if(champion != null) {
                championCardDict.Add(champion.ChampionName,Instance.level3CardSOs[i]);
            } 
        }
        for (int i = 0; i < Instance.level4CardSOs.Count; i++) {
            for (int j = 0; j < GameRulesManager.championPoolSizeByLevel[i+1]; j++) {
                level4Cards.Add(Instance.level4CardSOs[i]);
            }
            Champion champion = Instance.level4CardSOs[i].ChampionPrefab.GetComponent<Champion>();
            if(champion != null) {
                championCardDict.Add(champion.ChampionName,Instance.level4CardSOs[i]);
            } 
        }
        for (int i = 0; i < Instance.level5CardSOs.Count; i++) {
            for (int j = 0; j < GameRulesManager.championPoolSizeByLevel[i+1]; j++) {
                level5Cards.Add(Instance.level5CardSOs[i]);
            }
            Champion champion = Instance.level5CardSOs[i].ChampionPrefab.GetComponent<Champion>();
            if(champion != null) {
                championCardDict.Add(champion.ChampionName,Instance.level5CardSOs[i]);
            } 
        }
    }
    private void OnBuyAChampion(GameEventTypeChampion ev, Champion _champion) {
        if(currentAvailableCards.ContainsKey(_champion.Tier)) {//先检测英雄费用是否能找到
            //一个比较笨的方法就是用linq来找到list里面含有这个champion的cardSO,或者有一个办法来保存对应的英雄
            if(championCardDict.ContainsKey(_champion.ChampionName) && championCardDict[_champion.ChampionName] != null) {//再检测能否通过这个英雄找到对应的cardSO
                if(currentAvailableCards[_champion.Tier].Contains(championCardDict[_champion.ChampionName])) {//再检测现有卡池里面能能找到这张卡
                    //再移除这张卡
                    currentAvailableCards[_champion.Tier].Remove(championCardDict[_champion.ChampionName]);
                    Debug.Log(currentAvailableCards[_champion.Tier].Count);
                }
            }
        }
    }
    private void OnSellAChampion(GameEventTypeChampion ev, Champion _champion) {
        if(currentAvailableCards.ContainsKey(_champion.Tier)) {//先检测英雄费用是否能找到
            if(championCardDict.ContainsKey(_champion.ChampionName) && championCardDict[_champion.ChampionName] != null) {//再检测能否通过这个英雄找到对应的cardSO
                if(currentAvailableCards[_champion.Tier].Any(x => x.ChampionName == _champion.ChampionName) || _champion.Level == 2) {//如果list里面没有那就不会加卡,因为已经三星了,
                    int amount = _champion.Level == 0 ? 1 : _champion.Level == 1 ? 3 : 9;//一星卡卖掉进1张,2星进3张,3星进9张
                    for (int i = 0; i < amount; i++) {
                        currentAvailableCards[_champion.Tier].Add(championCardDict[_champion.ChampionName]);
                    }             
                    Debug.Log(currentAvailableCards[_champion.Tier].Count);
                }  
            }
        }
    }
    private void OnChampionUpgradeLevel2(GameEventTypeChampion ev,Champion champion) {
        RemoveAllChampionsInDict(champion);//除了要删卡没想到别的
    }
    private void RemoveAllChampionsInDict(Champion champion) {
        if(currentAvailableCards.ContainsKey(champion.Tier)) {
            currentAvailableCards[champion.Tier].RemoveAll(x=>x.ChampionName == champion.ChampionName);
        }
    }
    public static CardSO GenerateRandomCard() {
        CardSO card = null;
        List<float> dropRate = Player.Instance.CurrentChampionDropRate;
        if(dropRate == null) return card;
        float target = Random.Range(0f,1f);//目前这么写的问题是,如果某一费的卡池里面一张卡都没有,就会有问题
        //但是实际游戏中卡只要稍微多一点就不会出现这个问题
        float temp = 0;
        int index = 0;//在几费的字典里面找
        for (int i = 0; i < dropRate.Count; i++) {
            temp += dropRate[i];
            if(temp >= target) {
                index = i;
                break;
            }
        }
        if(index <= currentAvailableCards.Count && currentAvailableCards[index+1].Count > 0) {
            int cardIndex = Random.Range(0,currentAvailableCards[index+1].Count);//+1是因为index默认是0,而字典里面是12345
            card = currentAvailableCards[index+1][cardIndex];
        }
        return card;
    }
}
