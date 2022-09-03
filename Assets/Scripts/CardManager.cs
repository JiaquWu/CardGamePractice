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
    public static Dictionary<int,List<CardSO>> currentAvailableCards = new Dictionary<int, List<CardSO>>() {
        {1,level1Cards},
        {2,level2Cards},
        {3,level3Cards},
        {4,level4Cards},
        {5,level5Cards}
    };
    protected override void Init() {
        GameEventsManager.StartListening(GameEventTypeVoid.ENTER_PLAY_STATE,InitCardLists);
    }
    private void OnDisable() {
        GameEventsManager.StopListening(GameEventTypeVoid.ENTER_PLAY_STATE,InitCardLists);
    }
    private void InitCardLists(GameEventTypeVoid ev) {//感觉很笨
        for (int i = 0; i < Instance.level1CardSOs.Count; i++) {
            for (int j = 0; j < GameRulesManager.championPoolSizeByLevel[i+1]; j++) {
                level1Cards.Add(Instance.level1CardSOs[i]);
            }
        }
        for (int i = 0; i < Instance.level2CardSOs.Count; i++) {
            for (int j = 0; j < GameRulesManager.championPoolSizeByLevel[i+1]; j++) {
                level2CardSOs.Add(Instance.level2CardSOs[i]);
            }
        }
        for (int i = 0; i < Instance.level3CardSOs.Count; i++) {
            for (int j = 0; j < GameRulesManager.championPoolSizeByLevel[i+1]; j++) {
                level3CardSOs.Add(Instance.level3CardSOs[i]);
            }
        }
        for (int i = 0; i < Instance.level4CardSOs.Count; i++) {
            for (int j = 0; j < GameRulesManager.championPoolSizeByLevel[i+1]; j++) {
                level4CardSOs.Add(Instance.level4CardSOs[i]);
            }
        }
        for (int i = 0; i < Instance.level5CardSOs.Count; i++) {
            for (int j = 0; j < GameRulesManager.championPoolSizeByLevel[i+1]; j++) {
                level5CardSOs.Add(Instance.level5CardSOs[i]);
            }
        }
    }
    public static CardSO GenerateRandomCard() {
        CardSO card = null;
        List<float> dropRate = Player.Instance.CurrentChampionDropRate;
        float target = Random.Range(0,1);
        float temp = 0;
        int index = 0;
        for (int i = 0; i < dropRate.Count; i++) {
            temp += dropRate[i];
            if(temp >= target) {
                index = i;
                break;
            }
        }
        if(index <= currentAvailableCards.Count) {
            int cardIndex = Random.Range(0,currentAvailableCards[index+1].Count);//+1是因为index默认是0,而字典里面是12345
            card = currentAvailableCards[index+1][cardIndex];
        }
        return card;
    }
}
