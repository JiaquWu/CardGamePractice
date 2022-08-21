using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//管理基类,友方和敌方不太一样,但是大部分方法应该是通用的
public class ChampionManagerBase : SingletonManager<ChampionManagerBase> {
    protected Dictionary<string,List<Champion>> championsDict;
    protected override void Init() {
        championsDict = new Dictionary<string, List<Champion>>();
    }

    public static void RegisterChampion(Champion champion) {
        if(Instance.championsDict.ContainsKey(champion.ChampionName)) {
            Instance.championsDict[champion.ChampionName].Add(champion);
        }else {
            Instance.championsDict.Add(champion.ChampionName,new List<Champion>());
        }
    }

    public static void UnregisterChampion(Champion champion) {
        if(!Instance.championsDict.ContainsKey(champion.ChampionName)) {
            Debug.LogError("no champion instance in this dictionary");
        }else if(Instance.championsDict[champion.ChampionName].Count != 0){
            Instance.championsDict[champion.ChampionName].Remove(champion);
        }
    }
}
