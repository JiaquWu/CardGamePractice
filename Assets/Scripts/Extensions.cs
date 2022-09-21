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
    public static bool IsInRange(this int myInt,int min,int max) {
        return myInt >= min && myInt < max;//取下不取上,在trait的使用场景中,如果达到max就说明到了一个新level
    }
    public static int GetIndexInArray(this int count,int[] arr) {
        Debug.Log("arr.Length" + arr.Length );
        if(arr.Length == 0 || (arr.Length != 0 && count < arr[0])) return -1;
        
        for (int i = 0; i < arr.Length; i++) {
            if(i<arr.Length -1 ) {
                if(count.IsInRange(arr[i],arr[i+1])) return i;
            }else {
                if(count >= arr[i]) return i;
            }
        }
        return -1;
    }
}
