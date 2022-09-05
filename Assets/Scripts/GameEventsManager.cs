﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public enum GameEventTypeChampion {
    BUY_A_CHAMPION,
    SELL_A_CHAMPION
}
public enum GameEventTypeGameObject {
    
}
public enum GameEventTypeVoid {
    ENTER_INIT_STATE,
    EXIT_INIT_STATE,
    ENTER_PLAY_STATE,
    EXIT_PLAY_STATE,
    ENTER_IDLE_STATE,
    EXIT_IDLE_STATE,
    ENTER_DEPLOY_STATE,
    EXIT_DEPLOY_STATE,
    ENTER_COMBAT_STATE,
    EXIT_COMBAT_STATE,
    ENETR_BONUS_STATE,
    EXIT_BONUS_STATE,
    ON_SELL_BUTTON_DOWN,
    ON_REFRESH_BUTTON_DOWN
}
public enum GameEventTypeInt {
    CHAMPION_UPGRADE,
    LEVEL_UP//买经验升级
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
    public class ChampionUnityEvent : UnityEvent<GameEventTypeChampion,Champion> {

    }

    private static Dictionary<GameEventTypeGameObject,GameObjectUnityEvent> gameobjectEventDict;
    private static Dictionary<GameEventTypeVoid,VoidUnityEvent> voidEventDict;
    private static Dictionary<GameEventTypeInt,IntUnityEvent> intEventDict;
    private static Dictionary<GameEventTypeFloat,FloatUnityEvent> floatEventDict;
    private static Dictionary<GameEventTypeString,StringUnityEvent> stringEventDict;
    private static Dictionary<GameEventTypeChampion,ChampionUnityEvent> championEventDict;

    static void InitDict() {
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
        if(championEventDict == null) {
            championEventDict = new Dictionary<GameEventTypeChampion, ChampionUnityEvent>();
        }
    }
    public static void StartListening(GameEventTypeVoid eventTypeVoid,UnityAction<GameEventTypeVoid> listener) {
        InitDict();
        VoidUnityEvent unityEvent = null;
        if(voidEventDict.TryGetValue(eventTypeVoid,out unityEvent) == false) {//如果字典里面没有就加一个新的,如果有的话就被拿出来赋给unityEvent了
            unityEvent = new VoidUnityEvent();
            voidEventDict.Add(eventTypeVoid,unityEvent);
        }
        unityEvent.AddListener(listener);
    }
    public static void StartListening(GameEventTypeGameObject eventTypeGameObject,UnityAction<GameEventTypeGameObject,GameObject> listener) {
        InitDict();
        GameObjectUnityEvent unityEvent = null;
        if(gameobjectEventDict.TryGetValue(eventTypeGameObject,out unityEvent) == false) {
            unityEvent = new GameObjectUnityEvent();
            gameobjectEventDict.Add(eventTypeGameObject,unityEvent);
        }
        unityEvent.AddListener(listener);
    }
    public static void StartListening(GameEventTypeInt eventTypeInt, UnityAction<GameEventTypeInt,int> listener) {
        InitDict();
        IntUnityEvent unityEvent = null;
        if(intEventDict.TryGetValue(eventTypeInt,out unityEvent) == false) {
            unityEvent = new IntUnityEvent();
            intEventDict.Add(eventTypeInt,unityEvent);
        }
        unityEvent.AddListener(listener);
    }
    public static void StartListening(GameEventTypeFloat eventTypeFloat,UnityAction<GameEventTypeFloat,float> listener) {
        InitDict();
        FloatUnityEvent unityEvent = null;
        if(floatEventDict.TryGetValue(eventTypeFloat,out unityEvent) == false) {
            unityEvent = new FloatUnityEvent();
            floatEventDict.Add(eventTypeFloat,unityEvent);
        }
        unityEvent.AddListener(listener);
    }
    public static void StartListening(GameEventTypeString eventTypeString,UnityAction<GameEventTypeString,string> listener) {
        InitDict();
        StringUnityEvent unityEvent = null;
        if(stringEventDict.TryGetValue(eventTypeString,out unityEvent) == false) {
            unityEvent = new StringUnityEvent();
            stringEventDict.Add(eventTypeString,unityEvent);
        }
        unityEvent.AddListener(listener);
    }
    public static void StartListening(GameEventTypeChampion eventTypeChampion,UnityAction<GameEventTypeChampion,Champion> listener) {
        InitDict();
        ChampionUnityEvent unityEvent = null;
        if(championEventDict.TryGetValue(eventTypeChampion,out unityEvent) == false) {
            unityEvent = new ChampionUnityEvent();
            championEventDict.Add(eventTypeChampion,unityEvent);
        }
        unityEvent.AddListener(listener);
;    }
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
    public static void StopListening(GameEventTypeChampion eventTypeChampion, UnityAction<GameEventTypeChampion,Champion> listener) {
        if(championEventDict != null && championEventDict.TryGetValue(eventTypeChampion, out ChampionUnityEvent unityEvent)) {
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
    public static void TriggerEvent(GameEventTypeChampion eventTypeChampion,Champion p1) {
        if(championEventDict != null && championEventDict.TryGetValue(eventTypeChampion,out ChampionUnityEvent unityEvent)) {
            unityEvent.Invoke(eventTypeChampion,p1);
        }
    }
}
