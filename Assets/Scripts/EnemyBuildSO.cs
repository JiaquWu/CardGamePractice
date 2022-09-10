using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(menuName = "EnemyBuilds/EnemyBuild",fileName = "EnemyBuild")]
public class EnemyBuildSO : ScriptableObject {
    public List<EnemyUnit> enmiesInOneTurn;
}
[Serializable]
public struct EnemyUnit {
    public GameObject enemyGameObject;
    public Vector2 quadToStayCoordinate;//从字典中找
    public int level;//enemy等级,0为1级,2为3级
}