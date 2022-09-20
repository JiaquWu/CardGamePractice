using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AllyChampionManager : ChampionManagerBase<AllyChampionManager> {
    public Dictionary<TraitBase,List<Champion>> traitsDict;
    private List<Champion> championsBoughtLastCombatRound;
    private static int spaceTakenByChampions;//已经占用的人口数量
    public static int SpaceTakenByChampions => spaceTakenByChampions;//在考虑要不要把这个字段换到其他位置?不确定 再看看
    protected override void Init() {
        spaceTakenByChampions = 0;//初始状态下没英雄在上面
        championsBoughtLastCombatRound = new List<Champion>();
        traitsDict = new Dictionary<TraitBase, List<Champion>>();
        GameEventsManager.StartListening(GameEventTypeVoid.ENTER_DEPLOY_STATE,OnEnterDeployState);
        GameEventsManager.StartListening(GameEventTypeVoid.ENTER_COMBAT_STATE,OnEnterCombatState);
    }
    private void OnDisable() {
        GameEventsManager.StopListening(GameEventTypeVoid.ENTER_DEPLOY_STATE,OnEnterDeployState);
        GameEventsManager.StopListening(GameEventTypeVoid.ENTER_COMBAT_STATE,OnEnterCombatState);
    }
    public override void RegisterChampion(Champion champion) {
        base.RegisterChampion(champion);
        if(GameManager.Instance.PlayState.ActiveState.name == OnPlayState.COMBAT) {
            championsBoughtLastCombatRound.Add(champion);//如果在战斗中买的,要加进来
        }
        CheckChampionUpgrade(champion);
    }
    public void UpdateCurrentTraits(Champion champion,bool isAdding) {
        if(isAdding) {
            foreach (TraitBase trait in champion.traits) {
                if(!traitsDict.ContainsKey(trait)) {
                    traitsDict.Add(trait,new List<Champion>());
                }
                if(!traitsDict[trait].Any(x=>x.ChampionName == champion.ChampionName)) {//如果list中已经有一个这个英雄了,那就不加,否则要加
                    
                }
                traitsDict[trait].Add(champion);//无论有没有都要加
                Debug.Log(traitsDict[trait].Count + "有tama几个呢");
            }
        }else {
            //那就是减
            foreach (TraitBase trait in champion.traits) {
                Debug.Assert(traitsDict.ContainsKey(trait));
                traitsDict[trait].Remove(champion);
                Debug.Log(traitsDict[trait].Count + "有tama几个呢");
            }
            
        }
        GameEventsManager.TriggerEvent(GameEventTypeVoid.UPDATE_CURRENT_TRAITS);
    }
    public bool CanThisChampionUpgrade(Champion champion) {
        //给一个英雄进来,看看它能不能买了就升级,这里要判断在不在战斗,不在战斗的话场下场上都可以合成,在的话只能场下合成
        if(championsDict.ContainsKey(champion.ChampionName)) {
            //如果字典里面有这个英雄,那还要看看有没有两个一级的
            int amount = 0;
            if(GameManager.Instance.PlayState.ActiveState.name == OnPlayState.DEPLOY) {
                foreach (var item in championsDict[champion.ChampionName]){
                    if(item.Level == 0) amount ++;
                }
            }else if(GameManager.Instance.PlayState.ActiveState.name == OnPlayState.COMBAT) {
                foreach (var item in championsDict[champion.ChampionName]){
                    if(item.Level == 0 && item.LastQuadThisChampionStand is PreparationQuad)  amount ++;
                }
            }
            return amount >= 2;//一星卡最多场上有两张
        }
        return false;
    }
    private void CheckChampionUpgrade(Champion champion) {
        if(championsDict[champion.ChampionName].Count <= 2) return;
        //要分combat还是deploy来判断怎么执行
        IEnumerable<Champion> championLevel0IE =  GameManager.Instance.PlayState.ActiveState.name == OnPlayState.DEPLOY?//不同的状态,不同的判定条件
        from item in championsDict[champion.ChampionName]
        where item.Level == 0
        select item :
        GameManager.Instance.PlayState.ActiveState.name == OnPlayState.COMBAT?
        from item in championsDict[champion.ChampionName]
        where item.Level == 0 && (item.LastQuadThisChampionStand is PreparationQuad || championsBoughtLastCombatRound.Contains(item)) 
        //在战斗状态下,如果是1级并且是刚刚买的champion或者在preparation上面的就可以
        select item : null;
        List<Champion> championLevel0 = championLevel0IE.ToList();
        IEnumerable<Champion> championLevel1IE = GameManager.Instance.PlayState.ActiveState.name == OnPlayState.DEPLOY? //同上
        from item in championsDict[champion.ChampionName]
        where item.Level == 1
        select item : 
        GameManager.Instance.PlayState.ActiveState.name == OnPlayState.COMBAT?
        from item in championsDict[champion.ChampionName]
        where item.Level == 1 && (item.LastQuadThisChampionStand is PreparationQuad || championsBoughtLastCombatRound.Contains(item))
        select item : null;
        List<Champion> championLevel1 = championLevel1IE.ToList();
        if(championLevel0 != null && championLevel0.Count >= 3) {//意识到这里的3是写死的,如果改变玩法下面的代码都要改,不够通用!
            championLevel0[0].OnChampionUpgrade(1);
            championLevel0[1].OnChampionUpgrade(0);
            championLevel0[2].OnChampionUpgrade(0);
            championLevel1.Add(championLevel0[0]);
            GameEventsManager.TriggerEvent(GameEventTypeChampion.CHAMPION_UPGRADE_LEVEL_1,championLevel0[0]);
        }
        if(championLevel1 != null && championLevel1.Count >= 3) {
            championLevel1[0].OnChampionUpgrade(2);
            championLevel1[1].OnChampionUpgrade(0);
            championLevel1[2].OnChampionUpgrade(0);
            GameEventsManager.TriggerEvent(GameEventTypeChampion.CHAMPION_UPGRADE_LEVEL_2,championLevel1[0]);
        }
    }
    public void OnSpaceChange(int modifier) {//不能写成静态方法,要不然不需要实例
        //如果新英雄进来,那么人口增加,如果有英雄somehow出去,那就减少
        spaceTakenByChampions += modifier;
    }
    protected override void OnEnterCombatState(GameEventTypeVoid ev) {
        if(championsBoughtLastCombatRound.Count > 0) {//战斗开始的时候清空
            championsBoughtLastCombatRound.Clear();
        }
    }
    protected override void OnEnterDeployState(GameEventTypeVoid ev) {//开始买东西之前把每个champion都检查一遍
        if(championsBoughtLastCombatRound == null && championsBoughtLastCombatRound.Count == 0) return;
        championsBoughtLastCombatRound.Distinct().ToList();
        foreach (var item in championsBoughtLastCombatRound) {
            CheckChampionUpgrade(item);
        }
    }
}
