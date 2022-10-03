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
                    instance = go.AddComponent<T>();
                    Debug.LogWarning(string.Format("There was no {0} object in any of the currently loaded scenes",instance));
                }else {
                    instance.Init();
                }
                
            }
            return instance;
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
