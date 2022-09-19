using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="TestAbility",menuName ="Abilities/TestAbility",order = 0)]
public class TestAbility : ChampionAbility {
    public override void Execute() {
        Debug.Log("用技能啦");
    }
}
