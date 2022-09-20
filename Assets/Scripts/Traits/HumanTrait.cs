using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(menuName = "Trait/HumanTrait",fileName = "HumanTrait")]
public class HumanTrait : TraitBase {
    public override string TriatName { 
        get {
            return "Human";
    }protected set{ } }
}
