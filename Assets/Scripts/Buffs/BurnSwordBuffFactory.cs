using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(menuName = "Buff/BurnSwordBuff",fileName = "BurnSwordBuff")]
public class BurnSwordBuffFactory : BuffFactory<BurnSwordBuffData,BurnSwordBuff> {
    
}
[Serializable]
public class BurnSwordBuffData {
    public float[] damagePerTick;
    public float tickTime;
    public float duration;
    public int numberOfAttacks;
}

public class BurnSwordBuff : Buff<BurnSwordBuffData>
{
    int count = 0;
    public override void Apply(int level) {
        target.additionalAttackEffect += BurnSword;
    }

    public override void Remove() {
       target.additionalAttackEffect -= BurnSword;
    }
    private void BurnSword(Champion castChampion,Champion targetChampion) {
        if(targetChampion.gameObject.activeSelf) {
            targetChampion.StartCoroutine(TakeBurnDamage(castChampion,targetChampion));
        }
        count ++;
        if(count >= data.numberOfAttacks) {
            Remove();
        }
    }
    private IEnumerator TakeBurnDamage(Champion castChampion, Champion targetChampion) {
        float timer = Time.time;
        while(Time.time - timer < data.duration) {
            yield return new WaitForSeconds(data.tickTime);
            targetChampion.TakeDamage(castChampion,DamageType.MAGIC, data.damagePerTick[castChampion.Level]);
        }
    }
    public override void UpdateBuff(int level)
    {
        throw new NotImplementedException();
    }
}