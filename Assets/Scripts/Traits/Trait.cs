using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class TraitBase : ScriptableObject {//羁绊的基类
    public virtual string TriatName{get;protected set;} //羁绊名字
    protected virtual int ActivatedTraitsIndex {get; set;} = -1;//0就是激活了第一级羁绊
    protected Action<int> ActivateAdditionalEffect;//int是说第几级,英雄知道每级效果会给自己什么不同的额外效果.0是1级
    protected int[] amountToActivate;//激活羁绊所需要的英雄数量
    public TraitBase() {
        
    }
    public TraitBase(Action<int> ActivateAdditionalEffect) {//创建对象的时候,把
        this.ActivateAdditionalEffect = ActivateAdditionalEffect;
        Init();
    }
    public virtual Champion ActivateTrait(int championAmount, Champion champion) {//激活羁绊会发生的事
        return champion;
    }
    public virtual void AdditionalEffect() {//一些特殊能力

    }
    public virtual void Init() {//构造对象时需要初始化的东西
        amountToActivate = new int[]{};
    }
    protected virtual int CalculateNewIndex(int amount) {//计算英雄数量所指向的新的index是几
        int result = -1;
        for (int i = 0; i < amountToActivate.Length; i++) {
            if(amountToActivate[i] <= amount) {
                result = i;//只要进来一次,result就不是-1
                continue;
            }
        }
        return result;
    }
}

public class TestTrait: TraitBase {
    public override string TriatName { get; protected set; } = "shimmerscale";
    public TestTrait(Action<int> action):base(action) {
        
    } 
    public override void Init() {
        amountToActivate = new int[]{3,5,7,9};
    }
    public override void AdditionalEffect() {
        //比如说金鳞龙会给不同的物品,再比如说玉龙的雕像,攻击提供攻速等,这些怎么写呢?

    }
    public override Champion ActivateTrait(int championAmount, Champion champion) {
        //首先要判断是否需要激活,如果当前的index和championAmount所指向的是同一个index,那么就直接return;
        if(ActivatedTraitsIndex == CalculateNewIndex(championAmount)) {
            return champion; 
        }else {
            ActivatedTraitsIndex = CalculateNewIndex(championAmount);
        } 
        //champion.currentChampionStats.attackDamage += 1;
        ActivateAdditionalEffect?.Invoke(ActivatedTraitsIndex);
        return champion;
    }
}