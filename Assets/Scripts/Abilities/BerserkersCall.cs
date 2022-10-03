using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName ="BerserkersCall",menuName ="Abilities/BerserkersCall")]
public class BerserkersCall : ChampionAbilityWithSphereTrigger {
    public override void Execute(Champion champion) {
        action += (targetChampion)=> {
            if(!targetChampion.IsAllyChampion) {
                targetChampion.ForceAttackTarget(champion);
            } };
        base.Execute(champion);
    }
}
