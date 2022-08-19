using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ChampionState {
    PREPARE,//备战席
    IDLE,//在场上挂机
    WALK,//走到目标
    ATTACK,//和目标战斗
    DEAD,//棋子死亡
}

[RequireComponent(typeof(Animator))]
public class Champion : MonoBehaviour {//棋子类,
//棋子的逻辑是这样的:游戏开始的时候首先寻路到最近的敌人,然后一直平A,蓝够了就放技能,敌人死了就找下一个
//一个英雄所包含的所有数据,应该是配置好的
    protected Animator animator;
    protected Animator Animator {
        get{
            if(animator == null) {
                animator = GetComponent<Animator>();
            }
            return animator;
        }
    }
    private ChampionStats defaultChampionStats;//默认的英雄数值,但是怎么合理配置呢?
    public ChampionStats currentChampionStats;//当前英雄数值
    private List<TraitBase> traits = new List<TraitBase>();//一个英雄拥有的所有羁绊
    public ChampionAbility championAbility;

    protected ChampionState currentChampionState;

    private void OnGUI() {
        // if(GUILayout.Button("cast ability")) {
        //     Debug.Log(currentChampionStats.attackDamage);
        //     traits.Add(new TestTrait());
        //     traits[0].ActivateTrait(5,this);
        //     Debug.Log(currentChampionStats.attackDamage);
        // }
    }
}
public class ChampionAbility:ScriptableObject {//每个英雄的大招不一样
    public virtual void Execute() {
        
    }
}
