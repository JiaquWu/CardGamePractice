using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class TraitBase : ScriptableObject {
    public virtual string TriatName {
        get {
            return "default";
        }protected set{}}
    public int[] amountToActivate;
    public virtual int CalculateNewIndex(int count) {
        Debug.Log("amountToActivate" + amountToActivate.Length);
        return count.GetIndexInArray(amountToActivate);
    }
    public BuffFactory buffFactory;
    public Buff buff;
    public void Apply(Champion champion,int traitLevel) {
        if(buffFactory != null) {
            if(buff != null && buff.isApplied) {
                buff.UpdateBuff(traitLevel);
            }else {
                buff = buffFactory.GetBuff(champion);
                buff.Apply(traitLevel);
                buff.isApplied = true;
            }
        }
    }
    public void Remove(Champion champion) {
        if(buffFactory != null && buff != null && buff.isApplied) {
            buff.Remove();
        }
    }
}

