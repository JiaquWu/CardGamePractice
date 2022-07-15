using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SingletonManager<T> : MonoBehaviour
where T:SingletonManager<T> {
    private static T instance;
    public static T Instance {
        get {
            if(instance == null) { 
                instance = FindObjectOfType<T>();
                if(instance == null) {
                    GameObject go = new GameObject();
                    instance = go.AddComponent<T>();//这里会立即执行awake
                    Debug.LogWarning(string.Format("There was no {0} object in any of the currently loaded scenes",instance));
                }else {
                    instance.Init();//是空的,并且是被找到的而不是新建的,所以才需要init初始化
                }
                
            }
            return instance;//之后拿到它都不是第一次了
        }
    }
    protected void Awake() {
        if(instance == null) {
            instance = this as T;
            Init();
        }
    }
    protected virtual void Init() {

    }
}
