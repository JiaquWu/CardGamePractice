using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuadsManager : SingletonManager<QuadsManager> {
    private Transform deployQuads;
    public Transform DeployQuads {
        get {
            if(deployQuads == null) {
                deployQuads = transform.Find("DeployQuads");
                if(deployQuads == null) {
                    Debug.LogWarning("no deployQuads exists");
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
                    Debug.LogWarning("no enemyQuads exists");
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
                    Debug.LogWarning("no preparationQuads exists");
                    GameObject go = new GameObject("PreparationQuads");
                    go.transform.SetParent(transform);
                }
            }
            return preparationQuads;
        }
    }
    
    private Dictionary<Vector2,Quad> deployQuadsDict = new Dictionary<Vector2, Quad>();
    private Dictionary<Vector2,Quad> preparationQuadsDict = new Dictionary<Vector2, Quad>();
    private Dictionary<Vector2,Quad> enemyQuadsDict = new Dictionary<Vector2, Quad>();
    private Dictionary<Vector2,Quad> findPathDict = new Dictionary<Vector2, Quad>();//preparation和enemy区域的棋子和这个有关系
    private int quadSizeX;//这个是x轴总共能放多少个quad
    private int quadSizeY;
    Vector2 gridWorldSize;//这个是说坐标系的X轴有多宽
    float nodeRadius;
    Node[,] grid;//所有跟寻路有关的node,(0,0)是地图里面的(1,0),因为不包括preparation
    public Transform start;
    public Transform end;
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
        MapConfigurationSO map = MapManager.Instance.CurrentMapConfiguration;
        quadSizeX = map.QuadSizeX;
        quadSizeY = map.QuadSizeY-1;//-1是因为quadSize是9
        grid = new Node[quadSizeX,quadSizeY];
        gridWorldSize = new Vector2(quadSizeX * map.ScaleRatio,quadSizeY * map.ScaleRatio);
        for (int x = 0; x < quadSizeX; x++) {
            for (int y = 0; y < quadSizeY; y++) {
                if(findPathDict.ContainsKey(new Vector2(x,y+1))) {//y+1是因为不包括preparation区域
                    grid[x,y] = findPathDict[new Vector2(x,y+1)].node;
                }else {
                    Debug.LogWarning("key is missing!");//目前编辑器中生成地图也会执行到这里,目前不用管
                }
            }            
        }
        
    }
    private void Update() {
        FindPath(start.position,end.position);
    }
    private void InitializeAllQuads() {    
        RegisterQuads(DeployQuads,deployQuadsDict,MapManager.Instance.CurrentMapConfiguration.DeployQuadCoordinates);
        RegisterQuads(EnemyQuads,enemyQuadsDict,MapManager.Instance.CurrentMapConfiguration.EnemyQuadCoordinates);
        RegisterQuads(PreparationQuads,preparationQuadsDict,MapManager.Instance.CurrentMapConfiguration.PreparationQuadCoordinates);
        findPathDict = deployQuadsDict.MergeTwoDictionary<Vector2,Quad>(enemyQuadsDict);    
    }
    private void RegisterQuads(Transform quadsParent,Dictionary<Vector2,Quad> dict,List<Vector2> quadCoordinates) {
        //两件事情,首先要把quad里面的node给初始化,然后把quad注册到字典中
        int count = quadsParent.childCount;
        for (int i = 0; i < count; i++) {
           if(quadsParent.GetChild(i).TryGetComponent<Quad>(out Quad quad)) {
               quad.InitializeNode(true,quadsParent.GetChild(i).position,(int)quadCoordinates[i].x,(int)quadCoordinates[i].y);
               //quad.node = new Node(true,quadsParent.GetChild(i).position,(int)quadCoordinates[i].x,(int)quadCoordinates[i].y);
               //因为生成的时候是按quadCoordinates的顺序生成,因此这里也是这个顺序
               dict.Add(quadCoordinates[i],quad);
           }else {
               Debug.Log("没有quad");
           }
        } 
    }
    public Node NodeFromWorldPoint(Vector3 worldPosition) {
        float percentX = (worldPosition.x - MapManager.Instance.CurrentMapConfiguration.OriginPoint.x) / gridWorldSize.x;
        float percentY = (worldPosition.z - (MapManager.Instance.CurrentMapConfiguration.OriginPoint.z+1)) / gridWorldSize.y; //+1因为不算preparation
        percentX = Mathf.Clamp01(percentX);
        percentY = Mathf.Clamp01(percentY);
        int x = Mathf.CeilToInt(quadSizeX * percentX) - 1;//这里是推算出来的要ceil,先算出百分比,ceil是因为最大7.5,7.0到7.5都是第八格,然后因为index要-1
        int y = Mathf.CeilToInt(quadSizeY * percentY) - 1;//同理,因为这里quadSizeY是8,所以一样的
        return grid[x,y];
    }
    public Quad QuadFromNode(Node node) {
        Vector2 coordinate = new Vector2(node.gridX,node.gridY);
        if(findPathDict.ContainsKey(coordinate)) {
            return findPathDict[coordinate];
        }else {
            Debug.LogError("cannot find this quad");
            return null;
        }
    }
    public List<Node> GetNeighbors(Node node) {
        List<Node> neighbors = new List<Node>();
        for (int x = -1; x <= 1; x++) {
            for (int y = -1; y <= 1; y++) { //九宫格个
                if(x == 0 && y == 0) continue; //它自己
                int checkX = node.gridX + x;
                int checkY = node.gridY + y;

                if(checkX >= 0 && checkX < quadSizeX && checkY >= 0 && checkY < quadSizeY) {
                    neighbors.Add(grid[checkX,checkY]);
                }
            }
        }
        return neighbors;
    }
    private void FindPath(Vector3 startPos, Vector3 targetPos) { //A star algorhithm
        Node startNode = NodeFromWorldPoint(startPos);
        Node targetNode = NodeFromWorldPoint(targetPos);

        List<Node> openSet = new List<Node>();
        HashSet<Node> closedSet = new HashSet<Node>();
        openSet.Add(startNode);

        while(openSet.Count > 0) {
            Node currentNode = openSet[0];
            for (int i = 1; i < openSet.Count; i++) {
                if(openSet[i].fCost < currentNode.fCost || openSet[i].fCost == currentNode.fCost && openSet[i].hCost < currentNode.hCost) {
                    currentNode = openSet[i];
                }
            }
            openSet.Remove(currentNode);
            closedSet.Add(currentNode);
            if(currentNode == targetNode) {
                RetracePath(startNode,targetNode);
                return;
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
                    }
                }
            }
        }
    }
    void RetracePath(Node startNode, Node endNode) {
        List<Node> path = new List<Node>();
        Node currentNode = endNode;
        while (currentNode != startNode) {
            path.Add(currentNode);
            currentNode = currentNode.parent;
        }
        path.Reverse();
        foreach (var item in path) {
            Debug.Log(new Vector2(item.gridX,item.gridY));
            if(QuadFromNode(item).TryGetComponent<Quad>(out Quad quad)) {
               quad.EnableEmissionShader(true);
            } 
        }
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
