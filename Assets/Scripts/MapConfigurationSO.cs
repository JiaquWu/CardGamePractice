using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Configurations/MapConfiguration",fileName ="MapConfiguration")]
public class MapConfigurationSO : ScriptableObject {
    private float scaleRatio = 1f;//坐标和实际坐标系的比率
    [SerializeField]
    private Vector3 originPoint;//坐标轴原点,这里默认坐标轴的x轴和unity坐标x轴对应,y轴和unity坐标z轴对应
    public Vector3 OriginPoint => originPoint;
    [SerializeField]
    private List<Vector2> preparationQuadCoordinates;//备战区坐标点
    public List<Vector2> PreparationQuadCoordinates => preparationQuadCoordinates;
    [SerializeField]
    private List<Vector2> deployQuadCoordinates;//友方战斗区域
    public List<Vector2> DeployQuadCoordinates => deployQuadCoordinates;
    [SerializeField]
    private List<Vector2> enemyQuadCoordinates;//敌方战斗区域
    public List<Vector2> EnemyQuadCoordinates => enemyQuadCoordinates;

    public Vector3 CalculatePosition(Vector2 coordinate) {
        Vector3 result;
        result.x = originPoint.x + coordinate.x * scaleRatio;
        result.y = originPoint.y;
        result.z = originPoint.z + coordinate.y * scaleRatio;
        return result;
    }

}
