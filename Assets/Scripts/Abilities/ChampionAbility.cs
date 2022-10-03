using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChampionAbility:ScriptableObject {
    public virtual void Execute(Champion champion) {
        
    }
}
public class ChampionBuffAbility : ChampionAbility {
    public BuffFactory buffFactory;
    public Buff buff;
    public override void Execute(Champion champion) {
        Apply(champion);
    }
    public virtual void Apply(Champion champion) {
        if(buffFactory != null) {
            if(buff == null) {
                buff = buffFactory.GetBuff(champion);
            }
            buff.Apply(champion.Level);
        }
    }
}
public class ChampionAbilityWithSphereTrigger : ChampionAbility {
    public CapsuleTriggerData data;
    public Action<Champion> action;
    public override void Execute(Champion champion) {
        GameObject triggerObj = PoolManager.Instance.ReuseObject
        (PoolObjectType.ABILITY_SPHERE_TRIGGER,champion.transform.position + data.initiatePositionOffset,champion.transform.rotation);
        if(triggerObj != null && triggerObj.TryGetComponent<AbilityCustomTrigger>(out AbilityCustomTrigger trigger)) {
            trigger.UpdateTrigger(action,data);
        }
    }
}