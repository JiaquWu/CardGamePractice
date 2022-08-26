using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public enum GameEventTypeGameObject {

}
public enum GameEventTypeVoid {
    ENTER_INIT_STATE,
    ENTER_PLAY_STATE,
    ENTER_IDLE_STATE,
    ENTER_DEPLOY_STATE,
    ENTER_COMBAT_STATE,
    ENETR_BONUS_STATE
}
public enum GameEventTypeInt {

}
public enum GameEventTypeFloat {

}
public enum GameEventTypeString {

}
public class GameEventsManager : SingletonManager<GameEventsManager> {
    public class VoidUnityEvent : UnityEvent<GameEventTypeVoid> {

    }
    public class GameObjectUnityEvent : UnityEvent<GameEventTypeGameObject,GameObject> {

    }
    public class IntUnityEvent : UnityEvent<GameEventTypeInt,int> {

    }
    public class FloatUnityEvent : UnityEvent<GameEventTypeFloat,float> {

    }
    public class StringUnityEvent : UnityEvent<GameEventTypeString,string> {

    }

    private static Dictionary<GameEventTypeGameObject,GameObjectUnityEvent> gameobjectEventDict;
    private static Dictionary<GameEventTypeVoid,VoidUnityEvent> voidEventDict;
    private static Dictionary<GameEventTypeInt,IntUnityEvent> intEventDict;
    private static Dictionary<GameEventTypeFloat,FloatUnityEvent> floatEventDict;
    private static Dictionary<GameEventTypeString,StringUnityEvent> stringEventDict;

    protected override void Init() {
        if(gameobjectEventDict == null) {
            gameobjectEventDict = new Dictionary<GameEventTypeGameObject, GameObjectUnityEvent>();
        }
        if(voidEventDict == null) {
            voidEventDict = new Dictionary<GameEventTypeVoid, VoidUnityEvent>();
        }
        if(intEventDict == null) {
            intEventDict = new Dictionary<GameEventTypeInt, IntUnityEvent>();
        }
        if(floatEventDict == null) {
            floatEventDict = new Dictionary<GameEventTypeFloat, FloatUnityEvent>();
        }
        if(stringEventDict == null) {
            stringEventDict = new Dictionary<GameEventTypeString, StringUnityEvent>();
        }
    }
    public static void StartListening(GameEventTypeVoid eventTypeVoid,UnityAction<GameEventTypeVoid> listener) {
        VoidUnityEvent unityEvent = null;
        if(voidEventDict.TryGetValue(eventTypeVoid,out unityEvent) == false) {//如果字典里面没有就加一个新的,如果有的话就被拿出来赋给unityEvent了
            unityEvent = new VoidUnityEvent();
            voidEventDict.Add(eventTypeVoid,unityEvent);
        }
        unityEvent.AddListener(listener);
    }
    public static void StartListening(GameEventTypeGameObject eventTypeGameObject,UnityAction<GameEventTypeGameObject,GameObject> listener) {
        GameObjectUnityEvent unityEvent = null;
        if(gameobjectEventDict.TryGetValue(eventTypeGameObject,out unityEvent) == false) {
            unityEvent = new GameObjectUnityEvent();
            gameobjectEventDict.Add(eventTypeGameObject,unityEvent);
        }
        unityEvent.AddListener(listener);
    }
    public static void StartListening(GameEventTypeInt eventTypeInt, UnityAction<GameEventTypeInt,int> listener) {
        IntUnityEvent unityEvent = null;
        if(intEventDict.TryGetValue(eventTypeInt,out unityEvent) == false) {
            unityEvent = new IntUnityEvent();
            intEventDict.Add(eventTypeInt,unityEvent);
        }
        unityEvent.AddListener(listener);
    }
    public static void StartListening(GameEventTypeFloat eventTypeFloat,UnityAction<GameEventTypeFloat,float> listener) {
        FloatUnityEvent unityEvent = null;
        if(floatEventDict.TryGetValue(eventTypeFloat,out unityEvent) == false) {
            unityEvent = new FloatUnityEvent();
            floatEventDict.Add(eventTypeFloat,unityEvent);
        }
        unityEvent.AddListener(listener);
    }
    public static void StartListening(GameEventTypeString eventTypeString,UnityAction<GameEventTypeString,string> listener) {
        StringUnityEvent unityEvent = null;
        if(stringEventDict.TryGetValue(eventTypeString,out unityEvent) == false) {
            unityEvent = new StringUnityEvent();
            stringEventDict.Add(eventTypeString,unityEvent);
        }
        unityEvent.AddListener(listener);
    }
    public static void StopListening(GameEventTypeVoid eventTypeVoid,UnityAction<GameEventTypeVoid> listener) {
        if(voidEventDict != null && voidEventDict.TryGetValue(eventTypeVoid,out VoidUnityEvent unityEvent)) {//只有字典不是空的并且能查找到的时候才能拿出来
            unityEvent.RemoveListener(listener);
        }
    }
    public static void StopListening(GameEventTypeGameObject eventTypeGameObject,UnityAction<GameEventTypeGameObject,GameObject> listener) {
        if(gameobjectEventDict != null && gameobjectEventDict.TryGetValue(eventTypeGameObject,out GameObjectUnityEvent unityEvent)) {
            unityEvent.RemoveListener(listener);
        }
    }
    public static void StopListening(GameEventTypeInt eventTypeInt,UnityAction<GameEventTypeInt,int> listener) {
        if(intEventDict != null && intEventDict.TryGetValue(eventTypeInt,out IntUnityEvent unityEvent)) {
            unityEvent.RemoveListener(listener);
        }
    }
    public static void StopListening(GameEventTypeFloat eventTypeFloat,UnityAction<GameEventTypeFloat,float> listener) {
        if(floatEventDict != null && floatEventDict.TryGetValue(eventTypeFloat, out FloatUnityEvent unityEvent)) {
            unityEvent.RemoveListener(listener);
        }
    }
    public static void StopListening(GameEventTypeString eventTypeString, UnityAction<GameEventTypeString,string> listener) {
        if(stringEventDict != null && stringEventDict.TryGetValue(eventTypeString,out StringUnityEvent unityEvent)) {
            unityEvent.RemoveListener(listener);
        }
    }
    public static void TriggerEvent(GameEventTypeVoid eventTypeVoid) {
        if(voidEventDict != null && voidEventDict.TryGetValue(eventTypeVoid,out VoidUnityEvent unityEvent)) {
            unityEvent.Invoke(eventTypeVoid);
        }
    }
    public static void TriggerEvent(GameEventTypeGameObject eventTypeGameObject,GameObject p1) {
        if(gameobjectEventDict != null && gameobjectEventDict.TryGetValue(eventTypeGameObject,out GameObjectUnityEvent unityEvent)) {
            unityEvent.Invoke(eventTypeGameObject,p1);
        }
    }
    public static void TriggerEvent(GameEventTypeInt eventTypeInt,int p1) {
        if(intEventDict != null && intEventDict.TryGetValue(eventTypeInt,out IntUnityEvent unityEvent)) {
            unityEvent.Invoke(eventTypeInt,p1);
        }
    }
    public static void TriggerEvent(GameEventTypeFloat eventTypeFloat,float p1) {
        if(floatEventDict != null && floatEventDict.TryGetValue(eventTypeFloat, out FloatUnityEvent unityEvent)) {
            unityEvent.Invoke(eventTypeFloat,p1);
        }
    }
    public static void TriggerEvent(GameEventTypeString eventTypeString,string p1) {
        if(stringEventDict != null && stringEventDict.TryGetValue(eventTypeString,out StringUnityEvent unityEvent)) {
            unityEvent.Invoke(eventTypeString,p1);
        }
    }
}
