using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuadsManager : SingletonManager<QuadsManager> {
    private Transform deployQuads;
    public Transform DeployQuads {
        get {
            if(deployQuads == null) {
                deployQuads = transform.Find("DeployQuads");
                if(deployQuads == null) {
                    Debug.LogWarning("no deployQuads exists");
                    GameObject go = new GameObject("DeployQuads");
                    go.transform.SetParent(transform);
                }
            }
            return deployQuads;
        }
    }
    private Transform enemyQuads;
    public Transform EnemyQuads {
        get {
            if(enemyQuads == null) {
                enemyQuads = transform.Find("EnemyQuads");
                if(enemyQuads == null) {
                    Debug.LogWarning("no enemyQuads exists");
                    GameObject go = new GameObject("EnemyQuads");
                    go.transform.SetParent(transform);
                }
            }
            return enemyQuads;
        }
    }
    private Transform preparationQuads;
    public Transform PreparationQuads {
        get {
            if(preparationQuads == null) {
                preparationQuads = transform.Find("PreparationQuads");
                if(preparationQuads == null) {
                    Debug.LogWarning("no preparationQuads exists");
                    GameObject go = new GameObject("PreparationQuads");
                    go.transform.SetParent(transform);
                }
            }
            return preparationQuads;
        }
    }
    public void DeleteAllQuads() {
        DeleteQuads(DeployQuads);
        DeleteQuads(EnemyQuads);
        DeleteQuads(PreparationQuads);
    }
    public void DeleteQuads(Transform quadsParent) {
        int count = quadsParent.childCount;
        for (int i = 0; i < count; i++) {
            DestroyImmediate(quadsParent.GetChild(0).gameObject);
        }
    }
    protected override void Init() {
        
    }
}
