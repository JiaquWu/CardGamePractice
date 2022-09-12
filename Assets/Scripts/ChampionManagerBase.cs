using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//管理基类,友方和敌方不太一样,但是大部分方法应该是通用的
public class ChampionManagerBase<T> : SingletonManager<T>
where T:ChampionManagerBase<T> {
    protected Dictionary<string,List<Champion>> championsDict;
    
    //在战斗阶段买的所有英雄都会放到这里面,战斗阶段开始清零,deploy阶段开始的之后会挨个检测是否能合成
    protected override void Init() {
        GameEventsManager.StartListening(GameEventTypeVoid.ENTER_DEPLOY_STATE,OnEnterDeployState);
        GameEventsManager.StartListening(GameEventTypeVoid.ENTER_COMBAT_STATE,OnEnterCombatState);
    }
    private void OnDisable() {
        GameEventsManager.StopListening(GameEventTypeVoid.ENTER_DEPLOY_STATE,OnEnterDeployState);
        GameEventsManager.StopListening(GameEventTypeVoid.ENTER_COMBAT_STATE,OnEnterCombatState);
    }
    protected void InitDict() {
        if(championsDict == null) {
            championsDict = new Dictionary<string, List<Champion>>();
        }
    }
    public virtual void RegisterChampion(Champion champion) {
        InitDict();
        if(!championsDict.ContainsKey(champion.ChampionName)) {
            championsDict.Add(champion.ChampionName,new List<Champion>());
        }
        championsDict[champion.ChampionName].Add(champion);
    }
    public virtual void UnregisterChampion(Champion champion) {
        InitDict();
        if(!championsDict.ContainsKey(champion.ChampionName)) {
            Debug.LogError("no champion instance in this dictionary");
        }else if(championsDict[champion.ChampionName].Count != 0){
            championsDict[champion.ChampionName].Remove(champion);
        }
    }
    public Vector3 GetNearestOpponentChampion(Champion champion,out Champion targetChampion) {
        //将字典里面每个英雄位置和目标英雄计算得出最近的位置
        //下面两种方法不知道哪个快,下面一个方法的好处是可以out
        targetChampion = null;
        // var champions = from list in championsDict.Values.ToList()
        //                 from cham in list
        //                 select cham;
        // var positions = from champ in champions
        //                 select champ.transform.position; 
        // Vector3 min = positions.ToList().Aggregate((pos1,pos2) => 
        // Vector3.Distance(pos1,champion.transform.position)
        // < Vector3.Distance(pos2,champion.transform.position)
        // ? pos1 : pos2);
        // return min;
        Vector3 temp = Vector3.positiveInfinity;
        for (int i = 0; i < championsDict.Count; i++) {
            for (int j = 0; j < championsDict.ElementAt(i).Value.Count; j++) {
                if(Vector3.Distance(championsDict.ElementAt(i).Value[j].transform.position,champion.transform.position) 
                < Vector3.Distance(temp,champion.transform.position)) {
                    temp = championsDict.ElementAt(i).Value[j].transform.position;
                    targetChampion = championsDict.ElementAt(i).Value[j];
                }

            }
        }
        return temp;
    }
    protected virtual void OnEnterCombatState(GameEventTypeVoid ev) {
        
    }
    protected virtual void OnEnterDeployState(GameEventTypeVoid ev) {//开始买东西之前把每个champion都检查一遍
        
    }
}
