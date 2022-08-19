using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="ChampionStats",menuName ="ChampionStats")]
public class ChampionStats:ScriptableObject {//英雄数据,暂时还是用scriptableObject来保存吧,读表应该更好
    public string championName;
    public int healthPoints;
    public int maxHealthPoints;
    public int manaPoints;
    public int maxManaPoints;
    public int attackDamage;//攻击力
    [Range(0,1)]
    public float criticalChance;//暴击率
    [Range(1,3)]
    public float criticalDamage;//暴击伤害
    [Range(1,8)]
    public int attackRange;//攻击距离
    public int armor;//护甲
    public int magicResistance;//魔抗
    public ChampionStats(int healthPoints, int manaPoints, int attackDamage, float criticalChance, float criticalDamage, int attackRange) {
        this.healthPoints = healthPoints;
        this.manaPoints = manaPoints;
        this.attackDamage = attackDamage;
        this.criticalChance = criticalChance;
        this.criticalDamage = criticalDamage;
        this.attackRange = attackRange;
    }
    public void Init() {
        healthPoints = maxHealthPoints;
        manaPoints = maxManaPoints;
    }
}
