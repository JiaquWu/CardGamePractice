using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum BuffTypeEnum {
    INSTANT,
    BATTLE_START

}
public abstract class BuffFactory : ScriptableObject {
    public abstract Buff GetBuff(Champion target);
    public BuffTypeEnum buffType;
}

public class BuffFactory<DataType,BuffType> : BuffFactory
where BuffType : Buff<DataType>, new() {
    public DataType data;
    public override Buff GetBuff(Champion _target) {
       return new BuffType {data = this.data, target = _target};
    }
}



