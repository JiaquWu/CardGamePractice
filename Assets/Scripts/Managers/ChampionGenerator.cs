using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
public class ChampionGenerator : SingletonManager<ChampionGenerator> {
    protected override void Init() {
        GameEventsManager.StartListening(GameEventTypeChampion.BUY_A_CHAMPION,GenerateAllyChampion);
    }
    public void GenerateAllyChampion(GameEventTypeChampion ev,Champion _champion) {//will check if there's an availble place before using it
        Dictionary<Vector2,Quad> preparationDict = QuadsManager.Instance.preparationQuadsDict;
        for (int i = 0; i < preparationDict.Count; i++) {
            if(preparationDict.ElementAt(i).Value.ChampionOnThisQuad == null) {
                Vector3 pos = preparationDict.ElementAt(i).Value.node.worldPosition;
                GameObject go = Instantiate(_champion.gameObject,pos,Quaternion.identity);
                if(go.TryGetComponent<Champion>(out Champion champion)) {
                    champion.OnDeploy(preparationDict.ElementAt(i).Value,true);
                }
                return;
            }
        }
        //if preparation area is full, then check the deploy area!
        Dictionary<Vector2,Quad> deployDict = QuadsManager.Instance.deployQuadsDict;
        for (int i = 0; i < deployDict.Count; i++) {
            if(deployDict.ElementAt(i).Value.ChampionOnThisQuad == null) {
                Vector3 pos = deployDict.ElementAt(i).Value.node.worldPosition;
                GameObject go = Instantiate(_champion.gameObject,pos,Quaternion.identity);
                if(go.TryGetComponent<Champion>(out Champion champion)) {
                    champion.OnDeploy(deployDict.ElementAt(i).Value,true);
                }
                return;
            }
        }
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
