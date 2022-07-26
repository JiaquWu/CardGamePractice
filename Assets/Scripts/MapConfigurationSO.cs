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

    public Vector2 AStarCoordinateToCoordinate(Vector2 co) {
        //这个方法本质上要把和寻路相关的坐标转为deploy和enemy区域的坐标,寻路是从(0,0)开始,寻路的第一格是deploy的第一格,因此这里(x,y+1)
        //这里的写法只能应用于目前的地图,需要一些更高级的做法.
        return new Vector2(co.x,co.y+1);
    }

    public Vector2 GetOffset() {
        Vector2 res = new Vector2();
        if(DeployQuadCoordinates.Count != 0) {
            res = new Vector2(DeployQuadCoordinates[0].x * ScaleRatio,DeployQuadCoordinates[0].y * ScaleRatio);
            //友方第一个坐标和(0,0)在世界坐标中的差值就是寻路坐标原点和所有点坐标原点的偏移量
            //这里是计算实际距离的偏移量,和上一个方法中计算坐标的偏移量是不一样的
        }
        return res;
    }
}
