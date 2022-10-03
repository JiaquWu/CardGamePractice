using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Quad : MonoBehaviour {
    private bool isChampionEnteredOnMouse;
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
        if(InputManager.Instance.IsLeftMouseButtonPressed) return;
        EnableEmissionShader(true);
    }
    public virtual void OnMouseDown() {
        EnableEmissionShader(false);
    }
    public virtual void OnMouseExit() {
        if(InputManager.Instance.IsLeftMouseButtonPressed) return;
        EnableEmissionShader(false);
    }
    public void OnChampionEnterOnMouse(Champion champion) {
        if(isChampionEnteredOnMouse || GameManager.Instance.PlayState.ActiveState.name != OnPlayState.DEPLOY) return;
        isChampionEnteredOnMouse = true;
        EnableEmissionShader(true);
    }

    public void OnChampionExitOnMouse(Champion champion) {
        isChampionEnteredOnMouse = false;
        EnableEmissionShader(false);
    }
    public void OnChampionStay(Champion champion,bool isSwaping = false) {
        //check swap
        if(championOnThisQuad != null && !isSwaping && !GameManager.Instance.isInCombat) {
            champion.OnChampionSwap(championOnThisQuad);
        }
        championOnThisQuad = champion;
        node.walkable = false;
        EnableEmissionShader(false);
    }
    public void OnChampionLeave(Champion champion) {
        championOnThisQuad = null;
        node.walkable = true;
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

public class Node : IHeapItem<Node> {//a star algorithm related
    public bool walkable;
    public Vector3 worldPosition;
    public int gridX;
    public int gridY;//!= coordinate
    public int gCost;
    public int hCost;
    int heapIndex;
    public Node parent;
    public Quad attachedQuad;
    public Node(bool _walkable, Vector3 _worldPos, int _gridX, int _gridY,Quad _attachedQuad) {
        walkable = _walkable;
        worldPosition = _worldPos;
        gridX = _gridX;
        gridY = _gridY;
        attachedQuad = _attachedQuad;
    }
    public Node(Vector3 _worldPos,Quad _attachedQuad) {
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
        return -compare;//more cost is not good
    }
}
