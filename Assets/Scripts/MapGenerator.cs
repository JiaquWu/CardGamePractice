using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : MonoBehaviour{
    [SerializeField]
    private MapConfigurationSO mapToGenerate;
    [SerializeField]
    private GameObject preparationQuad_Prefab;
    [SerializeField]
    private GameObject deployQuad_Prefab;
    [SerializeField]
    private GameObject enemyQuad_Prefab;
    public void GenerateNewMap() {
        Debug.Log(mapToGenerate.OriginPoint);
        GenerateQuads(mapToGenerate,mapToGenerate.PreparationQuadCoordinates,preparationQuad_Prefab,QuadsManager.Instance.PreparationQuads);
        GenerateQuads(mapToGenerate,mapToGenerate.EnemyQuadCoordinates,enemyQuad_Prefab,QuadsManager.Instance.EnemyQuads);
        GenerateQuads(mapToGenerate,mapToGenerate.DeployQuadCoordinates,deployQuad_Prefab,QuadsManager.Instance.DeployQuads);
        
    }
    private void GenerateQuads(MapConfigurationSO map,List<Vector2> quadCoordinates,GameObject prefab,Transform parent) {
        foreach (var item in quadCoordinates) {
            GameObject go = Instantiate(prefab,parent);
            go.transform.position = map.CalculatePosition(item);
            go.transform.rotation = prefab.transform.rotation;
        }
        
    }
}
