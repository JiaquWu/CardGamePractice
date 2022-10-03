using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathRequestManager : SingletonManager<PathRequestManager> {
    Queue<PathRequest> pathRequestQueue = new Queue<PathRequest>();
    PathRequest currentPathRequest;
    bool isProcessingPath;
    public static void RequestPath(Vector3 pathStart, Vector3 pathEnd, Action<Vector3[], bool> callback) {
        PathRequest newRequest = new PathRequest(pathStart,pathEnd,callback);
        Instance.pathRequestQueue.Enqueue(newRequest);
        Instance.TryProcessNext();
    }
    
    void TryProcessNext() {
        if(!isProcessingPath && pathRequestQueue.Count > 0) {
            currentPathRequest = pathRequestQueue.Dequeue();
            isProcessingPath = true;
            QuadsManager.Instance.StartFindPath(currentPathRequest.pathStart,currentPathRequest.pathEnd);
        }
    }
    public void FinishedProcessingPath(Vector3[] path, bool success) {
        currentPathRequest.callback(path,success);
        isProcessingPath = false;
        TryProcessNext();
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
