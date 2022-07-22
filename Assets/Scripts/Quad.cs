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
    private void Awake() {
        EmissionShader.SetActive(false);
    }
    public void OnMouseEnter() {
        EnableEmissionShader(true);
    }
    public void OnMouseExit() {
        EnableEmissionShader(false);
    }
    public void EnableEmissionShader(bool enable) {
        if(enable) {
            emissionShader.GetComponent<Renderer>().material.SetFloat(shaderTime,Time.time);
            EmissionShader.SetActive(true);
        }else {
            EmissionShader.SetActive(false);
        }
    }
    public virtual void InitializeNode(bool _walkable, Vector3 _worldPos, int _gridX, int _gridY) {
        
    }
}

public class Node {//a星算法相关
    public bool walkable;
    public Vector3 worldPosition;
    public int gridX;
    public int gridY;//这里和生成坐标里面的不一样,因为preparation里面的node和算法不相关
    public int gCost;
    public int hCost;
    public Node parent;
    public Node(bool _walkable, Vector3 _worldPos, int _gridX, int _gridY) {
        walkable = _walkable;
        worldPosition = _worldPos;
        gridX = _gridX;
        gridY = _gridY;
    }

    public int fCost => gCost +hCost;
}
