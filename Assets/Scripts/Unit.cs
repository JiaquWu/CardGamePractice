﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour {
    public Transform target;
    float speed = 1;
    Vector3[] path;
    int targetIndex;

    private void Start() {
        PathRequestManager.RequestPath(transform.position,target.position,OnPathFound);
    }
    public void OnPathFound(Vector3[] newPath, bool pathSuccessful) {
        if(pathSuccessful) {
            path = newPath;
            StopCoroutine(FollowPath());
            StartCoroutine(FollowPath());
        }
    }
    IEnumerator FollowPath() {
        if(path.Length > 0) {
            Vector3 currentWayPoint = path[0];
            while(true) {
                if(transform.position == currentWayPoint) {
                    targetIndex ++;
                    if(targetIndex >= path.Length) {//大于等于说明超出范围了
                        yield break;//中止协程
                    }
                    currentWayPoint = path[targetIndex];

                }
                //那么这里就要移动这个unit,
                transform.position = Vector3.MoveTowards(transform.position,currentWayPoint,speed * Time.deltaTime);
                yield return null;
            }
        }

    }
}