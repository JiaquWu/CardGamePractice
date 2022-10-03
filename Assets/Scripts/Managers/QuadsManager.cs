using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class QuadsManager : SingletonManager<QuadsManager> {
    private Transform deployQuads;
    public Transform DeployQuads {
        get {
            if(deployQuads == null) {
                deployQuads = transform.Find("DeployQuads");
                if(deployQuads == null) {
                    UnityEngine.Debug.LogWarning("no deployQuads exists");
                    GameObject go = new GameObject("DeployQuads");
                    go.transform.SetParent(transform);
                }
            }
            return deployQuads;
        }
    }
    private Transform enemyQuads;
    public Transform EnemyQuads {
        get {
            if(enemyQuads == null) {
                enemyQuads = transform.Find("EnemyQuads");
                if(enemyQuads == null) {
                    UnityEngine.Debug.LogWarning("no enemyQuads exists");
                    GameObject go = new GameObject("EnemyQuads");
                    go.transform.SetParent(transform);
                }
            }
            return enemyQuads;
        }
    }
    private Transform preparationQuads;
    public Transform PreparationQuads {
        get {
            if(preparationQuads == null) {
                preparationQuads = transform.Find("PreparationQuads");
                if(preparationQuads == null) {
                    UnityEngine.Debug.LogWarning("no preparationQuads exists");
                    GameObject go = new GameObject("PreparationQuads");
                    go.transform.SetParent(transform);
                }
            }
            return preparationQuads;
        }
    }
    
    public Dictionary<Vector2,Quad> deployQuadsDict = new Dictionary<Vector2, Quad>();
    public Dictionary<Vector2,Quad> preparationQuadsDict = new Dictionary<Vector2, Quad>();
    public Dictionary<Vector2,Quad> enemyQuadsDict = new Dictionary<Vector2, Quad>();
    private Dictionary<Vector2,Quad> findPathDict = new Dictionary<Vector2, Quad>();//preparation and enemy area
    public bool IsPreparationQuadsFull {
        get {
            return !preparationQuadsDict.Values.Any(x => x.ChampionOnThisQuad == null);
        }
    }
    private int quadSizeX;//how many quads in X axis
    private int quadSizeY;
    public int MaxSize => quadSizeX * quadSizeY;
    Vector2 gridWorldSize;//path find related
    Bounds bounds;
    float nodeRadius;
    Node[,] grid;//path find related node,(0,0) = (1,0) in map,because it doesn't contain preparation area
    PathRequestManager pathRequestManager => PathRequestManager.Instance;
    public MapConfigurationSO CurrentMap => MapManager.Instance.CurrentMapConfiguration;
    public float unitScaleRatio => CurrentMap.ScaleRatio;//length for each unit
    private void OnDrawGizmos() {
    }
    public void DeleteAllQuads() {
        DeleteQuads(DeployQuads);
        DeleteQuads(EnemyQuads);
        DeleteQuads(PreparationQuads);
    }
    public void DeleteQuads(Transform quadsParent) {
        int count = quadsParent.childCount;
        for (int i = 0; i < count; i++) {
            DestroyImmediate(quadsParent.GetChild(0).gameObject);
        }
    }
    protected override void Init() {
        InitializeAllQuads();
        InitializeGrid();
    }

    private void InitializeGrid() {
        MapConfigurationSO map = MapManager.Instance.CurrentMapConfiguration;
        quadSizeX = map.QuadSizeX;
        quadSizeY = map.QuadSizeY;
        grid = new Node[quadSizeX,quadSizeY];
        gridWorldSize = new Vector2(quadSizeX * map.ScaleRatio,quadSizeY * map.ScaleRatio);
        for (int x = 0; x < quadSizeX; x++) {
            for (int y = 0; y < quadSizeY; y++) {
                Vector2 coordinate = map.AStarCoordinateToCoordinate(new Vector2(x,y));
                if(findPathDict.ContainsKey(coordinate)) {
                    grid[x,y] = findPathDict[coordinate].node;
                }else {
                    UnityEngine.Debug.LogWarning("key is missing!");
                }
            }            
        }
    }
    private void InitializeAllQuads() {    
        RegisterQuads(DeployQuads,deployQuadsDict,MapManager.Instance.CurrentMapConfiguration.DeployQuadCoordinates);
        RegisterQuads(EnemyQuads,enemyQuadsDict,MapManager.Instance.CurrentMapConfiguration.EnemyQuadCoordinates);
        RegisterQuads(PreparationQuads,preparationQuadsDict,MapManager.Instance.CurrentMapConfiguration.PreparationQuadCoordinates);
        findPathDict = deployQuadsDict.MergeTwoDictionary<Vector2,Quad>(enemyQuadsDict);    
    }
    private void RegisterQuads(Transform quadsParent,Dictionary<Vector2,Quad> dict,List<Vector2> quadCoordinates) {
        int count = quadsParent.childCount;
        for (int i = 0; i < count; i++) {
           if(quadsParent.GetChild(i).TryGetComponent<Quad>(out Quad quad)) {
               quad.InitializeNode(true,quadsParent.GetChild(i).position,(int)quadCoordinates[i].x,(int)quadCoordinates[i].y,quad);
               dict.Add(quadCoordinates[i],quad);//map coordinate, not path find coordinate
           }else {
               
           }
        } 
    }
    public Quad GetAllyQuadByPosition(Vector3 worldPosition) {
        //except enemy area
        for (int i = 0; i < preparationQuadsDict.Count; i++) {
            Vector3 nodePos = preparationQuadsDict.ElementAt(i).Value.node.worldPosition;
            if((nodePos.x - unitScaleRatio / 2 <= worldPosition.x && worldPosition.x <= nodePos.x + unitScaleRatio / 2)
            && (nodePos.z - unitScaleRatio / 2 <= worldPosition.z && worldPosition.z <= nodePos.z + unitScaleRatio / 2)) {
                return preparationQuadsDict.ElementAt(i).Value.node.attachedQuad;
            }
        }
        for (int i = 0; i < deployQuadsDict.Count; i++) {
            Vector3 nodePos = deployQuadsDict.ElementAt(i).Value.node.worldPosition;
            if((nodePos.x - unitScaleRatio / 2 <= worldPosition.x && worldPosition.x <= nodePos.x + unitScaleRatio / 2)
            && (nodePos.z - unitScaleRatio / 2 <= worldPosition.z && worldPosition.z <= nodePos.z + unitScaleRatio / 2)) {
                return deployQuadsDict.ElementAt(i).Value.node.attachedQuad;
            }
        }
        return null;
    }
    public Quad GetCombatQuadByPostion(Vector3 worldPosition) {
        //battle area
        for (int i = 0; i < deployQuadsDict.Count; i++) {
            Vector3 nodePos = deployQuadsDict.ElementAt(i).Value.node.worldPosition;
            if((nodePos.x - unitScaleRatio / 2 <= worldPosition.x && worldPosition.x <= nodePos.x + unitScaleRatio / 2)
            && (nodePos.z - unitScaleRatio / 2 <= worldPosition.z && worldPosition.z <= nodePos.z + unitScaleRatio / 2)) {
                return deployQuadsDict.ElementAt(i).Value.node.attachedQuad;
            }
        }
        for (int i = 0; i < enemyQuadsDict.Count; i++) {
            Vector3 nodePos = enemyQuadsDict.ElementAt(i).Value.node.worldPosition;
            if((nodePos.x - unitScaleRatio / 2 <= worldPosition.x && worldPosition.x <= nodePos.x + unitScaleRatio / 2)
            && (nodePos.z - unitScaleRatio / 2 <= worldPosition.z && worldPosition.z <= nodePos.z + unitScaleRatio / 2)) {
                return enemyQuadsDict.ElementAt(i).Value.node.attachedQuad;
            }
        }
        return null;
    }
    public Vector3 GetQuadPositionByCoordinate(Vector2 coordinate) {
       if(CurrentMap != null) {
          return CurrentMap.CalculatePosition(coordinate);
       }
       throw new NullReferenceException();
    }
    public Node NodeFromWorldPoint(Vector3 worldPosition) {
        MapConfigurationSO map = MapManager.Instance.CurrentMapConfiguration;
        Vector2 offset = map.GetOffset();
        float percentX = (worldPosition.x - (MapManager.Instance.CurrentMapConfiguration.OriginPoint.x + offset.x)) / gridWorldSize.x;
        float percentY = (worldPosition.z - (MapManager.Instance.CurrentMapConfiguration.OriginPoint.z + offset.y)) / gridWorldSize.y; 
        percentY = Mathf.Clamp01(percentY);
        int x = Mathf.CeilToInt(quadSizeX * percentX) - 1;
        int y = Mathf.CeilToInt(quadSizeY * percentY) - 1;
        if(x < grid.GetLength(0) && y < grid.GetLength(1)) {
            return grid[x,y];
        }else {
            return null;
        }
        
    }
    public Quad QuadFromNode(Node node) {
        Vector2 coordinate = CurrentMap.AStarCoordinateToCoordinate(new Vector2(node.gridX,node.gridY));
        if(findPathDict.ContainsKey(coordinate)) {
            return findPathDict[coordinate];
        }else {
            UnityEngine.Debug.LogError("cannot find this quad");
            return null;
        }
    }
    public List<Node> GetNeighbors(Node node) {
        List<Node> neighbors = new List<Node>();
        for (int x = -1; x <= 1; x++) {
            for (int y = -1; y <= 1; y++) { 
                if(x == 0 && y == 0) continue; //itself
                int checkX = node.gridX + x;
                int checkY = node.gridY + y;

                if(checkX >= 0 && checkX < quadSizeX && checkY >= 0 && checkY < quadSizeY) {
                    neighbors.Add(grid[checkX,checkY]);
                }
            }
        }
        return neighbors;
    }
    private IEnumerator FindPath(Vector3 startPos, Vector3 targetPos) { //A star algorhithm
        Vector3[] waypoints = new Vector3[0];
        bool pathSuccess = false;
        Node startNode = NodeFromWorldPoint(startPos);
        Node targetNode = NodeFromWorldPoint(targetPos);
        if(startNode == null || targetNode == null) yield break;
        //if(!startNode.walkable || !targetNode.walkable) yield return null;
        Heap<Node> openSet = new Heap<Node>(MaxSize);
        //List<Node> openSet = new List<Node>();
        HashSet<Node> closedSet = new HashSet<Node>();
        openSet.Add(startNode);
        while(openSet.Count > 0) {
            Node currentNode = openSet.RemoveFirst();//heap!
            closedSet.Add(currentNode);

            if(GetNeighbors(currentNode).Contains(targetNode)) {//a champion in the target node, it's not walkable, just to find it's neighbors
                targetNode = currentNode;
                pathSuccess = true;
                break;//,yield break = return for ienumerator, break is only for breaking the loop
            }
            foreach (var item in GetNeighbors(currentNode)) {
                if(!item.walkable || closedSet.Contains(item)) {
                    continue;
                }

                int newMovementCostToNieghbor = currentNode.gCost + GetDistance(currentNode,item);
                if(newMovementCostToNieghbor < item.gCost || !openSet.Contains(item)) {
                    item.gCost = newMovementCostToNieghbor;
                    item.hCost = GetDistance(item,targetNode);
                    item.parent = currentNode;
                    if(!openSet.Contains(item)) {
                        openSet.Add(item);
                    }else {
                        openSet.UpdateItem(item);
                    }
                }
            }
        }
        yield return null;
        if(pathSuccess) {
            waypoints = RetracePath(startNode,targetNode);
        }
        pathRequestManager.FinishedProcessingPath(waypoints, pathSuccess);
    }
    public void StartFindPath(Vector3 startPos,Vector3 targetPos) {
        StartCoroutine(FindPath(startPos,targetPos));
    }
    Vector3[] RetracePath(Node startNode, Node endNode) {
        List<Node> path = new List<Node>();
        Node currentNode = endNode;
        while (currentNode != startNode) {//which means it's null
            path.Add(currentNode);
            currentNode = currentNode.parent;
        }
        Vector3[] waypoints = SimplifyPath(path);
        Array.Reverse(waypoints);
        return waypoints;
    }
    Vector3[] SimplifyPath(List<Node> path) {
        List<Vector3> waypoints = new List<Vector3>();
        for (int i = 0; i < path.Count; i++) {
            waypoints.Add(path[i].worldPosition);
        }
        return waypoints.ToArray();
        //need to check if it actually has a node
    }
    int GetDistance(Node nodeA,Node nodeB) {
        int disX = Mathf.Abs(nodeA.gridX - nodeB.gridX);
        int disY = Mathf.Abs(nodeA.gridY - nodeB.gridY);
        if(disX > disY) {
            return disY * 14 + (disX - disY) * 10;
        }else {
            return disX * 14 + (disY - disX) * 10;
        }
    }
}
