using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolManager : SingletonManager<PoolManager> {
    public Dictionary<int,Queue<ObjectInstance>> poolDictionary = new Dictionary<int, Queue<ObjectInstance>>();//储存所有对象池的字典
    public void CreatePool(GameObject prefab, int poolSize) {//先创建一个对象池,注册到字典里面
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
    public void ReuseObject(GameObject prefab,Vector3 position, Quaternion rotation) {//如果有这个对象池在字典里面,那么就拿一个出来用,并且调用reuse函数
        int poolKey = prefab.GetInstanceID();
        if(poolDictionary.ContainsKey(poolKey)) {
            ObjectInstance objectToReuse = poolDictionary[poolKey].Dequeue();
            poolDictionary[poolKey].Enqueue(objectToReuse);
            objectToReuse.Reuse(position,rotation);//拿出来之后执行初始化函数       
        }
    }
    public GameObject GetInstance(GameObject prefab) {//只是想得到一个实例,而不调用reuse函数,也许会用上
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
public class ObjectInstance {//储存对象池中的对象数据
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
