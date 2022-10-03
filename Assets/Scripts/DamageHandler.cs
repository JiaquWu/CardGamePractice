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
            //a simple formula
            damage = Probability.Chance(stats.criticalChance.Value)? damage * stats.criticalDamage.Value : damage;
            result = Mathf.RoundToInt((1-(stats.armor.Value / (stats.armor.Value + 100))) * damage);
            break;
            case DamageType.MAGIC:
            result = Mathf.RoundToInt((1-(stats.magicResistance.Value / (stats.magicResistance.Value + 100))) * damage);
            break;
            case DamageType.PURE:
            break;
        }
        return result;
    }
}

public static class Probability {
    public static bool Chance(float breakpoint) {
        return breakpoint == 1.0f || UnityEngine.Random.Range(0.0f, 1.0f) < breakpoint;
    }
}