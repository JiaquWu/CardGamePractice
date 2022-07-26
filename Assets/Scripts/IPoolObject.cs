using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IPoolObject {
    void OnObjectReuse();
    void Destroy();//让物体隐形
}
