using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
public class ChampionGenerator : SingletonManager<ChampionGenerator> {
    public void GenerateChampion(GameObject championPrefab) {
        //考虑要不要加限制,目前调试阶段不用,之后写好逻辑应该也还好
        Dictionary<Vector2,Quad> dict = QuadsManager.Instance.preparationQuadsDict;
        for (int i = 0; i < dict.Count; i++) {
            if(dict.ElementAt(i).Value.ChampionOnThisQuad == null) {
                //说明可以在这里生成
                Vector3 pos = QuadsManager.Instance.GetQuadPositionByCoordinate(dict.ElementAt(i).Key);
                GameObject go = Instantiate(championPrefab,pos,Quaternion.identity);
                if(go.TryGetComponent<Champion>(out Champion champion)) {
                    dict.ElementAt(i).Value.OnChampionStay(champion);
                }
                return;
            }
        }
        //如果没有位置可以生成
        Debug.LogWarning("no place for instantiating a new champion");
    }
}
