using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum DamageType {
    PHYSICS,
    MAGIC,
    PURE
}
public static class DamageHandler {
    public static float CalculateDamage(this Champion champion,DamageType type,float damage) {
        float result = damage;
        ChampionStats stats = champion.CurrentChampionStats;
        switch (type) {
            case DamageType.PHYSICS:
            //一个简单的伤害计算办法: 护甲/护甲+100 = 物理减伤率
            //(1-物理减伤率) * 伤害 = 最终伤害
            //法伤同理
            result = Mathf.RoundToInt((1-(stats.Armor / (stats.Armor + 100))) * damage);
            Debug.Log("怎么他妈计算的 " + stats.Armor + "??" + damage + "????" + result);
            break;
            case DamageType.MAGIC:
            result = Mathf.RoundToInt((1-(stats.MagicResistance / (stats.MagicResistance + 100))) * damage);
            break;
            case DamageType.PURE:
            //不用管,因为上面初始化了
            break;
        }
        return result;
    }
}
