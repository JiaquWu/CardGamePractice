using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Extensions {
    
    public static Dictionary<TKey,TValue> MergeTwoDictionary<TKey,TValue>(this Dictionary<TKey,TValue> myDict,Dictionary<TKey,TValue> targetDict) {
        foreach (var item in targetDict) {
            if(!myDict.ContainsKey(item.Key)) {
                myDict.Add(item.Key,item.Value);
            }
        }
        return myDict;
    }
}
