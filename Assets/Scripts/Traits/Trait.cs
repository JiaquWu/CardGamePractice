using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class TraitBase : ScriptableObject {//羁绊的基类
    public virtual string TriatName {
        get {
            return "default";
        }protected set{}} //羁绊名字
    public int[] amountToActivate;
    public virtual int CalculateNewIndex(int count) {
        Debug.Log("amountToActivate" + amountToActivate.Length);
        return count.GetIndexInArray(amountToActivate);
    }    
    // protected virtual int ActivatedTraitsIndex {get; set;} = -1;//0就是激活了第一级羁绊
    // protected Action<int> ActivateAdditionalEffect;//int是说第几级,英雄知道每级效果会给自己什么不同的额外效果.0是1级
    // //protected int[] amountToActivate;//激活羁绊所需要的英雄数量
    // public TraitBase() {
        
    // }
    // public virtual Champion ActivateTrait(int championAmount, Champion chsampion) {//激活羁绊会发生的事
    //     return champion;
    // }
    // public virtual void AdditionalEffect() {//一些特殊能力

    // }
    //需要一个方法,外部传进来一个英雄的数量,方法返回一个index,champion拿着这个index去触发对应的事情?
    
    // protected virtual int CalculateNewIndex(int[] amountToActivate,int amount) {//计算英雄数量所指向的新的index是几
    //     int result = -1;
    //     for (int i = 0; i < amountToActivate.Length; i++) {
    //         if(amountToActivate[i] <= amount) {
    //             result = i;//只要进来一次,result就不是-1
    //             continue;
    //         }
    //     }
    //     return result;
    // }
}

