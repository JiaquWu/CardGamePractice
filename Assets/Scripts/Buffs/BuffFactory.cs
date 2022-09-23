using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum BuffType {//何时触发这个Buff,首先羁绊会触发buff,有的是放到场上就能触发,有的是战斗开始触发,别的以后再添加吧
    INSTANT,
    BATTLE_START

}
public abstract class BuffFactory : ScriptableObject {
    public abstract Buff GetBuff(Champion target);
    public BuffType buffType;
}

public class BuffFactory<DataType,BuffType> : BuffFactory
where BuffType : Buff<DataType>, new() {
    public DataType data;
    public override Buff GetBuff(Champion _target) {
       return new BuffType {data = this.data, target = _target};
    }
}



