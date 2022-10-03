using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IPoolObject {
    void OnObjectReuse();
    void Destroy();//make it invisible?
}
