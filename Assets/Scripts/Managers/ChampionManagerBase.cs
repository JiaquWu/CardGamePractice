using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChampionManagerBase<T> : SingletonManager<T>
where T:ChampionManagerBase<T> {
    protected Dictionary<string,List<Champion>> championsDict;
    public Dictionary<TraitBase,List<Champion>> traitsDict;
    
    protected override void Init() {
        traitsDict = new Dictionary<TraitBase, List<Champion>>();
    }
    private void OnDisable() {
        
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
    public Vector3 GetNearestOpponentChampion(Champion champion,out Champion targetChampion, out bool isAChampionAvailable) {
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
        isAChampionAvailable = false;
        Vector3 temp = Vector3.positiveInfinity;
        for (int i = 0; i < championsDict.Count; i++) {
            for (int j = 0; j < championsDict.ElementAt(i).Value.Count; j++) {
                if(championsDict.ElementAt(i).Value[j].IsActive && Vector3.Distance(championsDict.ElementAt(i).Value[j].LastQuadThisChampionStand.node.worldPosition,champion.LastQuadThisChampionStand.node.worldPosition) 
                < Vector3.Distance(temp,champion.LastQuadThisChampionStand.node.worldPosition)) {
                    //temp = championsDict.ElementAt(i).Value[j].transform.position;
                    if(!isAChampionAvailable) {
                        isAChampionAvailable = true;
                    }
                    temp = championsDict.ElementAt(i).Value[j].LastQuadThisChampionStand.node.worldPosition;
                    //这里还要做一件事,由于champion会移动,它的位置可能和quad位置不一样,但算法需要的是quad的位置,因此需要得出离这个点最近的quad
                    //但其实不用计算最近的quad,只用看lastQuadStand就好了
                    targetChampion = championsDict.ElementAt(i).Value[j];
                }

            }
        }
        return temp;
    }
    protected virtual void OnEnterCombatState(GameEventTypeVoid ev) {
        
    }
    protected virtual void OnEnterDeployState(GameEventTypeVoid ev) {
        
    }
    public void UpdateCurrentTraits(Champion champion,bool isAdding) {
        if(isAdding) {
            foreach (TraitBase trait in champion.traits) {
                if(!traitsDict.ContainsKey(trait)) {
                    traitsDict.Add(trait,new List<Champion>());
                }
                traitsDict[trait].Add(champion);
                int index = trait.CalculateNewIndex(GetTraitChampionCount(trait));
                ActivateChampionsForATrait(trait,index);
            }
        }else {
            foreach (TraitBase trait in champion.traits) {
                Debug.Assert(traitsDict.ContainsKey(trait));
                traitsDict[trait].Remove(champion);
                int index = trait.CalculateNewIndex(GetTraitChampionCount(trait));
                ActivateChampionsForATrait(trait,index);
            }
            
        }
        GameEventsManager.TriggerEvent(GameEventTypeVoid.UPDATE_CURRENT_TRAITS);
    }
    private void ActivateChampionsForATrait(TraitBase trait,int traitLevel) {
        if(traitsDict.ContainsKey(trait)) {
            foreach (Champion champion in traitsDict[trait]) {
                champion.UpdateTraitLevelToChampion(trait,traitLevel);
            }
        }
    }
    public int GetTraitChampionCount(TraitBase targetTrait) {
        if(!traitsDict.ContainsKey(targetTrait)) return 0;
        int count = traitsDict[targetTrait].GroupBy(x=>x.ChampionName).Select(x=>x.FirstOrDefault()).ToList().Count;
        return count;
    }
}
