using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Quad : MonoBehaviour {
    private bool isChampionEnteredOnMouse;//鼠标带着英雄进来了
    private Champion championOnThisQuad;
    public Champion ChampionOnThisQuad => championOnThisQuad;
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
    public virtual void OnMouseEnter() {
        if(InputManager.Instance.IsLeftMouseButtonPressed) return;//只有鼠标左键没按的时候才会有用
        EnableEmissionShader(true);
    }
    public virtual void OnMouseDown() {
        EnableEmissionShader(false);
    }
    public virtual void OnMouseExit() {
        if(InputManager.Instance.IsLeftMouseButtonPressed) return;
        EnableEmissionShader(false);
    }
    public void OnChampionEnterOnMouse(Champion champion) {//玩家拖着英雄到格子上面的时候
        if(isChampionEnteredOnMouse) return;//如果已经在上面了就不用执行后面的了
        isChampionEnteredOnMouse = true;
        EnableEmissionShader(true);
    }

    public void OnChampionExitOnMouse(Champion champion) {
        isChampionEnteredOnMouse = false;
        EnableEmissionShader(false);
    }
    public void OnChampionStay(Champion champion,bool isSwaping = false) {
        //鼠标松开,让英雄站在上面,这里的逻辑应该是,如果自己的上面已经有一个英雄了,那应该让这个英雄的位置到来的这个英雄之前的位置上去     
        if(championOnThisQuad != null && !isSwaping) {
            champion.OnChampionSwap(championOnThisQuad);
        }
        championOnThisQuad = champion;
        EnableEmissionShader(false);
    }
    public void OnChampionLeave(Champion champion) {
        Debug.Log("OnChampionLeave");
        championOnThisQuad = null;
    }
    public void EnableEmissionShader(bool enable) {
        if(enable) {
            EmissionShader.GetComponent<Renderer>().material.SetFloat(shaderTime,Time.time);
            EmissionShader.SetActive(true);
        }else {
            EmissionShader.SetActive(false);
        }
    }
    public virtual void InitializeNode(bool _walkable, Vector3 _worldPos, int _gridX, int _gridY,Quad attachedQuad) {
        
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
    public Quad attachedQuad;//这样node就知道自己在哪个quad上面
    public Node(bool _walkable, Vector3 _worldPos, int _gridX, int _gridY,Quad _attachedQuad) {
        walkable = _walkable;
        worldPosition = _worldPos;
        gridX = _gridX;
        gridY = _gridY;
        attachedQuad = _attachedQuad;
    }
    public Node(Vector3 _worldPos,Quad _attachedQuad) {//不参与寻路的node
        worldPosition = _worldPos;
        attachedQuad = _attachedQuad;
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
