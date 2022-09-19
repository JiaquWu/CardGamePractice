using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathRequestManager : SingletonManager<PathRequestManager> {
    Queue<PathRequest> pathRequestQueue = new Queue<PathRequest>();
    PathRequest currentPathRequest;
    bool isProcessingPath;
    public static void RequestPath(Vector3 pathStart, Vector3 pathEnd, Action<Vector3[], bool> callback) {
        PathRequest newRequest = new PathRequest(pathStart,pathEnd,callback);//谁想请求一条道路,就新建一个pathrequest,其中callback是找到之后执行的方法
        Instance.pathRequestQueue.Enqueue(newRequest);
        Instance.TryProcessNext();
    }
    
    void TryProcessNext() {
        if(!isProcessingPath && pathRequestQueue.Count > 0) {//如果没有其他正在找的路,并且有路可找
            currentPathRequest = pathRequestQueue.Dequeue();//那么就拿一个出来找
            isProcessingPath = true;//现在正在找路!
            QuadsManager.Instance.StartFindPath(currentPathRequest.pathStart,currentPathRequest.pathEnd);
        }
    }
    public void FinishedProcessingPath(Vector3[] path, bool success) {//找完了
        currentPathRequest.callback(path,success);//找完了,就告诉寻路的人找到的路线和结果
        isProcessingPath = false;//上一条找完了,目前没有找的
        TryProcessNext();//再看看有没有更多的路去寻找
    }
    struct PathRequest {
        public Vector3 pathStart;
        public Vector3 pathEnd;
        public Action<Vector3[], bool> callback;
        public PathRequest(Vector3 _pathStart, Vector3 _pathEnd, Action<Vector3[], bool> _callback) {
            pathStart = _pathStart;
            pathEnd = _pathEnd;
            callback = _callback;
        }
    }
}
