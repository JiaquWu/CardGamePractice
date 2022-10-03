using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum PoolObjectType {
    ABILITY_SPHERE_TRIGGER,
}
public class PoolManager : SingletonManager<PoolManager> {
    [SerializeField]
    private GameObject abilitySphereTrigger;
    private Dictionary<PoolObjectType,GameObject> poolObjectDict;
    protected override void Init() {
        poolObjectDict = new Dictionary<PoolObjectType, GameObject>() {
            {PoolObjectType.ABILITY_SPHERE_TRIGGER,abilitySphereTrigger}
        };
        foreach (PoolObjectType type in poolObjectDict.Keys) {
            if(poolObjectDict[type] != null) {
                CreatePool(poolObjectDict[type], 10);//10个够了
            }
        }
    }
    public Dictionary<int,Queue<ObjectInstance>> poolDictionary = new Dictionary<int, Queue<ObjectInstance>>();
    public void CreatePool(GameObject prefab, int poolSize) {
        int poolKey = prefab.GetInstanceID();
        GameObject poolHolder = new GameObject(prefab.name + " pool");
        poolHolder.transform.parent = transform;
        if(!poolDictionary.ContainsKey(poolKey)) {
            poolDictionary.Add(poolKey,new Queue<ObjectInstance>());
            for (int i = 0; i < poolSize; i++) {
                ObjectInstance newObject = new ObjectInstance(Instantiate(prefab));
                poolDictionary[poolKey].Enqueue(newObject);
                newObject.SetParent(poolHolder.transform);
            }
        }
    }
    public void ReuseObject(GameObject prefab,Vector3 position, Quaternion rotation) {
        int poolKey = prefab.GetInstanceID();
        if(poolDictionary.ContainsKey(poolKey)) {
            ObjectInstance objectToReuse = poolDictionary[poolKey].Dequeue();
            poolDictionary[poolKey].Enqueue(objectToReuse);
            objectToReuse.Reuse(position,rotation);     
        }
    }
    public GameObject ReuseObject(PoolObjectType type,Vector3 position, Quaternion rotation) {
        if(poolObjectDict == null || poolObjectDict[type] == null) return null;
        int poolKey = poolObjectDict[type].GetInstanceID();
        if(poolDictionary.ContainsKey(poolKey)) {
            ObjectInstance objectToReuse = poolDictionary[poolKey].Dequeue();
            poolDictionary[poolKey].Enqueue(objectToReuse);
            objectToReuse.Reuse(position,rotation);   
            return objectToReuse.GameObject;
        }else {
            Debug.LogWarning("gameobject is missing");
            return null;
        }
    }
    public GameObject GetInstance(GameObject prefab) {//might be useful?
        int poolKey = prefab.GetInstanceID();
        if(poolDictionary.ContainsKey(poolKey)) {
            ObjectInstance objectToReuse = poolDictionary[poolKey].Dequeue();
            poolDictionary[poolKey].Enqueue(objectToReuse);
            return objectToReuse.GameObject;
        }else {
            Debug.LogWarning("gameobject is missing");
            return null;
        }      
    }
}
public class ObjectInstance {
    GameObject gameObject;
    public GameObject GameObject => gameObject; //weird name
    Transform transform;
    bool hasPoolObjectComponent;
    IPoolObject poolObjectScript;
    public ObjectInstance(GameObject objectInstance) {
        gameObject = objectInstance;
        transform = gameObject.transform;
        gameObject.SetActive(false);
        if(gameObject.TryGetComponent<IPoolObject>(out IPoolObject _poolObject)) {
            hasPoolObjectComponent = true;
            poolObjectScript = _poolObject;
        }
    }
    public void Reuse(Vector3 position, Quaternion rotation) {
        if(hasPoolObjectComponent) {
            poolObjectScript.OnObjectReuse();
        }
        gameObject.SetActive(true);
        transform.position = position;
        transform.rotation = rotation;
    }

    public void SetParent(Transform parent) {
        transform.parent = parent;
    }
}
