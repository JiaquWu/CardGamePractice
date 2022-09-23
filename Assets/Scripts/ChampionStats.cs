using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ChampionStatsType {
    MAX_HEALTH_POINT,
    MAX_MANA_POINT,
    HEALTH_POINT,
    MANA_POINT,
    ATTACK_DAMAGE,
    ATTACK_SPEED,
    ATTACK_RANGE,
    CRITICAL_CHANCE,
    CRITICAL_DAMAGE,
    ARMOR,
    MAGIC_RESISTANCE
}
[Serializable]
public class Ref<T> where T : struct {
    public T Value {get;set;}
    public Ref(T t) {
        Value = t;
    }
}
[CreateAssetMenu(fileName ="ChampionStats",menuName ="ChampionStats")]
public class ChampionStats:ScriptableObject {//英雄数据,暂时还是用scriptableObject来保存吧,读表应该更好
    public Dictionary<ChampionStatsType,Ref<float>> statsDict;
    public string championName;
    [HideInInspector]
    public Ref<float> healthPoints;
    [HideInInspector]
    public Ref<float> maxHealthPoints{get;private set;}
    public List<float> maxHealthPointsList;
    [HideInInspector]
    public Ref<float> manaPoints;
    [HideInInspector]
    public Ref<float> maxManaPoints{get;private set;}
    public List<float> maxManaPointsList;
    [HideInInspector]
    public Ref<float> attackDamage{get;private set;}
    public List<float> attackDamageList;//攻击力
    [HideInInspector]
    public Ref<float> attackSpeed{get;private set;}
    public List<float> attackSpeedList;//攻速,默认是1,越高攻速越快
    [HideInInspector]
    public Ref<float> criticalChance;//暴击率
    public List<float> criticalChanceList;//[Range(0,1)]
    [HideInInspector]
    public Ref<float> armor{get;private set;}//护甲
    public List<float> armorList;
    [HideInInspector]
    public Ref<float> magicResistance{get;private set;}//魔抗
    public List<float> magicResistanceList;
    [Range(1,3)]
    public float defaultCriticalDamage;
    public Ref<float> criticalDamage;//暴击伤害
    [Range(1,8)]
    public float defaultAttackRange;
    public Ref<float> attackRange;//攻击距离
    public float defautInitialManaPoints;
    public Ref<float> initialManaPoints;
    public float defaultManaGainedPerAttack;
    public Ref<float> manaGainedPerAttack;
    public float defaultManaGainedByTakingDamage;//掉多少血会加1滴蓝
    public Ref<float> manaGainedByTakingDamage;
    // public ChampionStats(int healthPoints, int manaPoints, int attackDamage, float criticalChance, float criticalDamage, int attackRange) {
    //     this.healthPoints = healthPoints;
    //     this.manaPoints = manaPoints;
    //     this.attackDamage = attackDamage;
    //     this.criticalChance = criticalChance;
    //     this.criticalDamage = criticalDamage;
    //     this.attackRange = attackRange;
    // }
    public void CopyChampionStats(ChampionStats targetStats,int level) {//什么时候会new一个呢,在初始化数值的时候应该需要,初始的数值就是根据几星来从list中拿数据
        this.maxHealthPoints = new Ref<float>(targetStats.maxHealthPointsList[level]); 
        this.maxManaPoints = new Ref<float>(targetStats.maxManaPointsList[level]); 
        this.initialManaPoints = new Ref<float>(targetStats.defautInitialManaPoints);
        this.attackDamage = new Ref<float>(targetStats.attackDamageList[level]); 
        this.attackSpeed = new Ref<float>(targetStats.attackSpeedList[level]); 
        this.criticalChance = new Ref<float>(targetStats.criticalChanceList[level]); 
        this.criticalDamage = new Ref<float>(targetStats.defaultCriticalDamage);
        this.attackRange = new Ref<float>(targetStats.defaultAttackRange);
        this.armor = new Ref<float>(targetStats.armorList[level]); 
        this.magicResistance = new Ref<float>(targetStats.magicResistanceList[level]); 
        this.manaGainedPerAttack = new Ref<float>(targetStats.defaultManaGainedPerAttack);
        this.manaGainedByTakingDamage = new Ref<float>(targetStats.defaultManaGainedByTakingDamage);
        this.healthPoints = new Ref<float>(this.maxHealthPoints.Value);
        this.manaPoints = new Ref<float>(this.initialManaPoints.Value);

        // this.MaxHealthPoints = targetStats.maxHealthPointsList[level];     
        // this.MaxManaPoints = targetStats.maxManaPointsList[level];
        // this.DefautManaPoints = targetStats.DefautManaPoints;
        // this.AttackDamage = targetStats.attackDamageList[level];
        // this.AttackSpeed = targetStats.attackSpeedList[level];
        // this.criticalChance = targetStats.criticalChanceList[level];
        // this.criticalDamage = targetStats.criticalDamage;
        // this.attackRange = targetStats.attackRange;
        // this.Armor = targetStats.ArmorList[level];
        // this.MagicResistance = targetStats.MagicResistanceList[level];
        // this.ManaGainedPerAttack = targetStats.ManaGainedPerAttack;
        // this.ManaGainedByTakingDamage = targetStats.ManaGainedByTakingDamage;
        // this.healthPoints = this.MaxHealthPoints;
        // this.manaPoints = this.DefautManaPoints;
        statsDict = new Dictionary<ChampionStatsType, Ref<float>>() {
            {ChampionStatsType.MAX_HEALTH_POINT,maxHealthPoints},
            {ChampionStatsType.MAX_MANA_POINT,maxManaPoints},
            {ChampionStatsType.HEALTH_POINT,healthPoints},
            {ChampionStatsType.MANA_POINT,manaPoints},
            {ChampionStatsType.ATTACK_DAMAGE,attackDamage},
            {ChampionStatsType.ATTACK_SPEED,attackSpeed},
            {ChampionStatsType.ATTACK_RANGE,attackRange},
            {ChampionStatsType.CRITICAL_CHANCE,criticalChance},
            {ChampionStatsType.CRITICAL_DAMAGE,criticalDamage},
            {ChampionStatsType.ARMOR,armor},
            {ChampionStatsType.MAGIC_RESISTANCE,magicResistance},
        };
    }
    public void UpdateStats(ChampionStats defaultStats,int championLevel) {
        this.maxHealthPoints.Value = defaultStats.maxHealthPointsList[championLevel];     
        this.maxManaPoints.Value = defaultStats.maxManaPointsList[championLevel];
        this.attackDamage.Value = defaultStats.attackDamageList[championLevel];
        this.attackSpeed.Value = defaultStats.attackSpeedList[championLevel];
        this.criticalChance.Value = defaultStats.criticalChanceList[championLevel];
        this.armor.Value = defaultStats.armorList[championLevel];
        this.magicResistance.Value = defaultStats.magicResistanceList[championLevel];
        this.healthPoints.Value = this.maxHealthPoints.Value;
        this.manaPoints.Value = this.initialManaPoints.Value;
    }
}

//在scriptableobject中好像用不了
public class DataForThreeLevels<T> {
    public List<T> datas;
    public DataForThreeLevels(T t1,T t2,T t3) {
        datas = new List<T>(3);
        datas.Add(t1);
        datas.Add(t2);
        datas.Add(t3);
    }
    public T this[int index] {
        get {
            T data;
            if(index >= 0 && index <= datas.Count-1) {
                data = datas[index];
            }else {
                data = default(T);
            }
            return data;
        }
    }
}