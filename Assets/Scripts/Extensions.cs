using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Extensions {
    
    public static Dictionary<TKey,TValue> MergeTwoDictionary<TKey,TValue>(this Dictionary<TKey,TValue> myDict,Dictionary<TKey,TValue> secondDict) {
        //myDict不用传进来,默认是this
        Dictionary<TKey,TValue> res = new Dictionary<TKey, TValue>();
        foreach (var item in myDict) {
            if(!res.ContainsKey(item.Key)) {
                res.Add(item.Key,item.Value);
            }
        }
        foreach (var item in secondDict) {
            if(!res.ContainsKey(item.Key)) {
                res.Add(item.Key,item.Value);
            }
        }
        return res;
    }
}
