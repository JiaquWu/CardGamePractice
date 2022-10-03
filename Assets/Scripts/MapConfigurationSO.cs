using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Configurations/MapConfiguration",fileName ="MapConfiguration")]
public class MapConfigurationSO : ScriptableObject {
    [SerializeField]
    private float scaleRatio;
    public float ScaleRatio => scaleRatio;
    [SerializeField]
    private int quadSizeX;//path find related
    public  int QuadSizeX => quadSizeX;
    [SerializeField]
    private int quadSizeY;//path find related
    public int QuadSizeY => quadSizeY;
    [SerializeField]
    private Vector3 originPoint;//the very left down vertice of the map
    public Vector3 OriginPoint => originPoint;
    [SerializeField]
    private List<Vector2> preparationQuadCoordinates;
    public List<Vector2> PreparationQuadCoordinates => preparationQuadCoordinates;
    [SerializeField]
    private List<Vector2> deployQuadCoordinates;
    public List<Vector2> DeployQuadCoordinates => deployQuadCoordinates;
    [SerializeField]
    private List<Vector2> enemyQuadCoordinates;
    public List<Vector2> EnemyQuadCoordinates => enemyQuadCoordinates;

    public Vector3 CalculatePosition(Vector2 coordinate) {
        Vector3 result;
        result.x = originPoint.x + (coordinate.x + 0.5f) * scaleRatio;//+0.5 because the origin point != position of the quad(center point)
        result.y = originPoint.y;
        result.z = originPoint.z + (coordinate.y + 0.5f) * scaleRatio;
        return result;
    }

    public Vector2 AStarCoordinateToCoordinate(Vector2 co) {
        //only works for this map
        return new Vector2(co.x,co.y+1);
    }

    public Vector2 GetOffset() {
        Vector2 res = new Vector2();
        if(DeployQuadCoordinates.Count != 0) {
            res = new Vector2(DeployQuadCoordinates[0].x * ScaleRatio,DeployQuadCoordinates[0].y * ScaleRatio);
            //offset for actual distance
        }
        return res;
    }
}
