using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
public class ChampionGenerator : SingletonManager<ChampionGenerator> {
    protected override void Init() {
        GameEventsManager.StartListening(GameEventTypeChampion.BUY_A_CHAMPION,GenerateAllyChampion);
    }
    public void GenerateAllyChampion(GameEventTypeChampion ev,Champion _champion) {//传进来之前已经判断好了,一定有位置生成英雄
        //考虑要不要加限制,目前调试阶段不用,之后写好逻辑应该也还好
        Dictionary<Vector2,Quad> preparationDict = QuadsManager.Instance.preparationQuadsDict;
        for (int i = 0; i < preparationDict.Count; i++) {
            if(preparationDict.ElementAt(i).Value.ChampionOnThisQuad == null) {
                //说明可以在这里生成
                Vector3 pos = preparationDict.ElementAt(i).Value.node.worldPosition;
                GameObject go = Instantiate(_champion.gameObject,pos,Quaternion.identity);
                if(go.TryGetComponent<Champion>(out Champion champion)) {
                    champion.OnDeploy(preparationDict.ElementAt(i).Value,true);
                }
                return;
            }
        }
        //如果场下没有位置了,那就要看看场上
        Dictionary<Vector2,Quad> deployDict = QuadsManager.Instance.deployQuadsDict;
        for (int i = 0; i < deployDict.Count; i++) {
            if(deployDict.ElementAt(i).Value.ChampionOnThisQuad == null) {
                //说明可以在这里生成
                Vector3 pos = deployDict.ElementAt(i).Value.node.worldPosition;
                GameObject go = Instantiate(_champion.gameObject,pos,Quaternion.identity);
                if(go.TryGetComponent<Champion>(out Champion champion)) {
                    champion.OnDeploy(deployDict.ElementAt(i).Value,true);
                }
                return;
            }
        }
        //如果没有位置可以生成
        Debug.LogWarning("no place for instantiating a new champion");
    }
    public void GenerateEnemyChampion(EnemyUnit enemy) {
        if(QuadsManager.Instance.enemyQuadsDict.ContainsKey(enemy.quadToStayCoordinate)) {
            Quad quad = QuadsManager.Instance.enemyQuadsDict[enemy.quadToStayCoordinate];
            Vector3 pos = quad.node.worldPosition;
            GameObject go = Instantiate(enemy.enemyGameObject,pos,Quaternion.identity);
            go.transform.rotation = Quaternion.Euler(0,180,0);
            if(go.TryGetComponent<Champion>(out Champion champion)) {
                champion.OnDeploy(quad,false,enemy.level);
            }
        } 
        
    }
    public void GenerateEnemyChampionsInCurrentRound() {
        EnemyBuildSO build = GameManager.Instance.GetCurrentEnemyBuild();
        foreach (var item in build.enmiesInOneTurn) {
            GenerateEnemyChampion(item);
        }
    }
    private void OnDisable() {
        GameEventsManager.StopListening(GameEventTypeChampion.BUY_A_CHAMPION,GenerateAllyChampion);
    }
}
