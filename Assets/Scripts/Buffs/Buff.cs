using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Buff {
    public abstract void Apply(int level);
    public abstract void Remove();
    public abstract void UpdateBuff(int level);//羁绊人数变化产生的buff变化要更新
    public bool isApplied;
}

public abstract class Buff<DataType> : Buff {
    public DataType data;
    public Champion target;//目前这个游戏里面所有的buff都是应用于champion的
    
}
