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
    public Vector2 quadToStayCoordinate;
    public int level;//0 = 1 star
}