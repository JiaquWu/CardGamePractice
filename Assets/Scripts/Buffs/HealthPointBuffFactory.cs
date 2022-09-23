using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(menuName = "Buff/HealthPointBuff",fileName = "HealthPointBuff")]
public class HealthPointBuffFactory : BuffFactory<HealthPointBuffData,HealthPointBuff> {
    
}
[Serializable]
public class HealthPointBuffData {
    public float[] additionalHealthPoints;
    public bool isTemporary;//如果是true就有倒计时,没有就整场
    public float duration;
    [HideInInspector]
    public int level;
}

public class HealthPointBuff : Buff<HealthPointBuffData> {

    public override void Apply(int level) {
        if(level <= data.additionalHealthPoints.Length -1 ) {
            data.level = level;
            target.ModifyStats(ChampionStatsType.MAX_HEALTH_POINT, data.additionalHealthPoints[level]);
        }
        
    }
    public override void UpdateBuff(int level) {
        //更新level,去掉之前的,增加新的
        Remove();
        Apply(level);
    }

    public override void Remove() {//一定是apply之后才会remove
        if(data.level <= data.additionalHealthPoints.Length -1) {
            target.ModifyStats(ChampionStatsType.MAX_HEALTH_POINT, data.additionalHealthPoints[data.level] * (-1));
        }
    }
}