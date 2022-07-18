using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Quad : MonoBehaviour {
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
        emissionShader.GetComponent<Renderer>().material.SetFloat(shaderTime,Time.time);
        EmissionShader.SetActive(true);
    }
    public void OnMouseExit() {
        EmissionShader.SetActive(false);
    }
}
