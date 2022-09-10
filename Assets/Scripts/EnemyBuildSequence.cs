using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(menuName = "EnemyBuilds/EnemyBuildSequence",fileName = "EnemyBuildSequence",order = 0)]
public class EnemyBuildSequence : ScriptableObject {
    public List<EnemyBuildSO> enemyBuilds;
    public EnemyBuildSO GetCurrentEnemyBuild(int index) {
        if(index <= enemyBuilds.Count - 1) {
            return enemyBuilds[index];
        }
        return null;
    }
}
