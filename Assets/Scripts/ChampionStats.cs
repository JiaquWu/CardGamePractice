using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="ChampionStats",menuName ="ChampionStats")]
public class ChampionStats:ScriptableObject {//英雄数据,暂时还是用scriptableObject来保存吧,读表应该更好
    public string championName;
    [HideInInspector]
    public int healthPoints;
    [HideInInspector]
    public int MaxHealthPoints{get;private set;}
    public List<int> maxHealthPointsList;
    [HideInInspector]
    public int manaPoints;
    public int DefautManaPoints{get;private set;}
    [HideInInspector]
    public int MaxManaPoints{get;private set;}
    public List<int> maxManaPointsList;
    [HideInInspector]
    public int AttackDamage{get;private set;}
    public List<int> attackDamageList;//攻击力
    [HideInInspector]
    public float criticalChance;//暴击率
    public List<float> criticalChanceList;//[Range(0,1)]
    [Range(1,3)]
    public float criticalDamage;//暴击伤害
    [Range(1,8)]
    public float attackRange;//攻击距离
    [HideInInspector]
    public int Armor{get;private set;}//护甲
    public List<int> ArmorList;
    [HideInInspector]
    public int MagicResistance{get;private set;}//魔抗
    public List<int> MagicResistanceList;

    // public ChampionStats(int healthPoints, int manaPoints, int attackDamage, float criticalChance, float criticalDamage, int attackRange) {
    //     this.healthPoints = healthPoints;
    //     this.manaPoints = manaPoints;
    //     this.attackDamage = attackDamage;
    //     this.criticalChance = criticalChance;
    //     this.criticalDamage = criticalDamage;
    //     this.attackRange = attackRange;
    // }
    public void CopyChampionStats(ChampionStats targetStats,int level) {//什么时候会new一个呢,在初始化数值的时候应该需要,初始的数值就是根据几星来从list中拿数据
        this.MaxHealthPoints = targetStats.maxHealthPointsList[level];     
        this.MaxManaPoints = targetStats.maxManaPointsList[level];
        this.DefautManaPoints = targetStats.DefautManaPoints;
        this.AttackDamage = targetStats.attackDamageList[level];
        this.criticalChance = targetStats.criticalChanceList[level];
        this.criticalDamage = targetStats.criticalDamage;
        this.attackRange = targetStats.attackRange;
        this.Armor = targetStats.ArmorList[level];
        this.MagicResistance = targetStats.MagicResistanceList[level];
        this.healthPoints = this.MaxHealthPoints;
    }
    public void UpdateStats(ChampionStats defaultStats,int championLevel) {
        this.MaxHealthPoints = defaultStats.maxHealthPointsList[championLevel];     
        this.MaxManaPoints = defaultStats.maxManaPointsList[championLevel];
        this.AttackDamage = defaultStats.attackDamageList[championLevel];
        this.criticalChance = defaultStats.criticalChanceList[championLevel];
        this.Armor = defaultStats.ArmorList[championLevel];
        this.MagicResistance = defaultStats.MagicResistanceList[championLevel];
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