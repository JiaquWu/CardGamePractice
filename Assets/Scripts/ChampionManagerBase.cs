using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//管理基类,友方和敌方不太一样,但是大部分方法应该是通用的
public class ChampionManagerBase : SingletonManager<ChampionManagerBase> {
    protected static Dictionary<string,List<Champion>> championsDict;
    private static int spaceTakenByChampions;//已经占用的人口数量
    public static int SpaceTakenByChampions => spaceTakenByChampions;//在考虑要不要把这个字段换到其他位置?不确定 再看看
    protected override void Init() {
        spaceTakenByChampions = 0;//初始状态下没英雄在上面
    }
    static void InitDict() {
        if(championsDict == null) {
            championsDict = new Dictionary<string, List<Champion>>();
        }
    }
    public static void RegisterChampion(Champion champion) {
        InitDict();
        if(!championsDict.ContainsKey(champion.ChampionName)) {
            championsDict.Add(champion.ChampionName,new List<Champion>());
        }
        championsDict[champion.ChampionName].Add(champion);
        //应该需要遍历一遍这个list,如果有三个0级的,就让第一个0级的callback(1),其他两个callback(0)
        //如果有三个0级的并且有两个1级的,就让第一个1级的callback(2),其余的全部callback(0)
        if(championsDict[champion.ChampionName].Count <= 2) return;
        IEnumerable<Champion> championLevel0IE =  
        from item in championsDict[champion.ChampionName]
        where item.Level == 0
        select item;
        List<Champion> championLevel0 = championLevel0IE.ToList();
        IEnumerable<Champion> championLevel1IE = 
        from item in championsDict[champion.ChampionName]
        where item.Level == 1
        select item;
        List<Champion> championLevel1 = championLevel1IE.ToList();
        if(championLevel0.Count >= 3) {
            championLevel0[0].OnChampionUpgrade(1);
            championLevel0[1].OnChampionUpgrade(0);
            championLevel0[2].OnChampionUpgrade(0);
            championLevel1.Add(championLevel0[0]);
            GameEventsManager.TriggerEvent(GameEventTypeInt.CHAMPION_UPGRADE,0);
        }
        if(championLevel1.Count >= 3) {
            championLevel1[0].OnChampionUpgrade(2);
            championLevel1[1].OnChampionUpgrade(0);
            championLevel1[2].OnChampionUpgrade(0);
            GameEventsManager.TriggerEvent(GameEventTypeInt.CHAMPION_UPGRADE,2);
        }
    }
    public static void UnregisterChampion(Champion champion) {
        InitDict();
        if(!championsDict.ContainsKey(champion.ChampionName)) {
            Debug.LogError("no champion instance in this dictionary");
        }else if(championsDict[champion.ChampionName].Count != 0){
            championsDict[champion.ChampionName].Remove(champion);
        }
    }
    public static void OnSpaceChange(int modifier) {
        //如果新英雄进来,那么人口增加,如果有英雄somehow出去,那就减少
        spaceTakenByChampions += modifier;
        Debug.Log("现在的人口是: " + spaceTakenByChampions);
    }
}
