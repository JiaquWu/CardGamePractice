using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Quad : MonoBehaviour {
    public Node node;
    private string shaderTime = "Time_Current";
    private GameObject emissionShader;
    public GameObject EmissionShader {
        get {
            if(emissionShader == null) {
                emissionShader = transform.Find("EmissionShader").gameObject;
                if(emissionShader == null) {
                    Debug.LogError("emission shader is missing");
                }
            }
            return emissionShader;
        }
    }
    protected void Start() {
        EnableEmissionShader(false);
    }
    public void OnMouseEnter() {
        EnableEmissionShader(true);
    }
    public void OnMouseExit() {
        EnableEmissionShader(false);
    }
    public void EnableEmissionShader(bool enable) {
        if(enable) {
            EmissionShader.GetComponent<Renderer>().material.SetFloat(shaderTime,Time.time);
            EmissionShader.SetActive(true);
        }else {
            EmissionShader.SetActive(false);
        }
    }
    public virtual void InitializeNode(bool _walkable, Vector3 _worldPos, int _gridX, int _gridY) {
        
    }
}

public class Node : IHeapItem<Node> {//a星算法相关
    public bool walkable;
    public Vector3 worldPosition;
    public int gridX;
    public int gridY;//这里和生成坐标里面的不一样,因为preparation里面的node和算法不相关
    public int gCost;
    public int hCost;
    int heapIndex;
    public Node parent;
    public Node(bool _walkable, Vector3 _worldPos, int _gridX, int _gridY) {
        walkable = _walkable;
        worldPosition = _worldPos;
        gridX = _gridX;
        gridY = _gridY;
    }

    public int fCost => gCost +hCost;

    public int HeapIndex {
        get {
            return heapIndex;
        }
        set {
            heapIndex = value;
        }
    }

    public int CompareTo(Node nodeToCompare) {
        int compare = fCost.CompareTo(nodeToCompare.fCost);
        if(compare == 0) {
            compare = hCost.CompareTo(nodeToCompare.hCost);
        }
        return -compare;//更高的compare意味着消耗更大,所以在比较来看更不好
    }
}
