using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Configurations/MapConfiguration",fileName ="MapConfiguration")]
public class MapConfigurationSO : ScriptableObject {
    [SerializeField]
    private float scaleRatio;//坐标和实际坐标系的比率
    public float ScaleRatio => scaleRatio;
    [SerializeField]
    private int quadSizeX;//横向地图格数
    public  int QuadSizeX => quadSizeX;
    [SerializeField]
    private int quadSizeY;//竖向地图格数
    public int QuadSizeY => quadSizeY;
    [SerializeField]
    private Vector3 originPoint;//坐标轴原点,这里默认坐标轴的x轴和unity坐标x轴对应,y轴和unity坐标z轴对应,左下角顶点
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
        result.x = originPoint.x + (coordinate.x + 0.5f) * scaleRatio;//+0.5是因为原点是第一格的左下角坐标,不应该是第一格中心坐标
        result.y = originPoint.y;
        result.z = originPoint.z + (coordinate.y + 0.5f) * scaleRatio;
        return result;
    }

}
