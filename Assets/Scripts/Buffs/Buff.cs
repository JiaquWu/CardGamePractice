using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Buff {
    public virtual void Apply() {

    }
    public virtual void Apply(int level) {

    }
    public virtual void Remove() {

    }
    public virtual void UpdateBuff(int level){
        
    }
    public bool isApplied;
}

public abstract class Buff<DataType> : Buff {
    public DataType data;
    public Champion target;
    
}
