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
    private Dictionary<Vector2,Quad> findPathDict = new Dictionary<Vector2, Quad>();//preparation和enemy区域的棋子和这个有关系
    public bool IsPreparationQuadsFull {
        get {
            return !preparationQuadsDict.Values.Any(x => x.ChampionOnThisQuad == null);//如果有一个是null就说明不是满的还有位置
        }
    }
    private int quadSizeX;//这个是x轴总共能放多少个quad
    private int quadSizeY;
    public int MaxSize => quadSizeX * quadSizeY;
    Vector2 gridWorldSize;//和寻路有关的区域面积大小
    Bounds bounds;
    float nodeRadius;
    Node[,] grid;//所有跟寻路有关的node,(0,0)是地图里面的(1,0),因为不包括preparation,这个转换是地图自己做
    PathRequestManager pathRequestManager => PathRequestManager.Instance;
    public MapConfigurationSO CurrentMap => MapManager.Instance.CurrentMapConfiguration;
    public float unitScaleRatio => CurrentMap.ScaleRatio;//每格长度
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
        //bounds = new Bounds((map.OriginPoint + new Vector3(gridWorldSize.x,0,gridWorldSize.y)/2),new Vector3(gridWorldSize.x,2,gridWorldSize.y));//原点加上这片的中心点,2随便给的
        for (int x = 0; x < quadSizeX; x++) {
            for (int y = 0; y < quadSizeY; y++) {
                Vector2 coordinate = map.AStarCoordinateToCoordinate(new Vector2(x,y));
                if(findPathDict.ContainsKey(coordinate)) {
                    grid[x,y] = findPathDict[coordinate].node;
                }else {
                    UnityEngine.Debug.LogWarning("key is missing!");//目前编辑器中生成地图也会执行到这里,目前不用管
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
        //两件事情,首先要把quad里面的node给初始化,然后把quad注册到字典中
        int count = quadsParent.childCount;
        for (int i = 0; i < count; i++) {
           if(quadsParent.GetChild(i).TryGetComponent<Quad>(out Quad quad)) {
               quad.InitializeNode(true,quadsParent.GetChild(i).position,(int)quadCoordinates[i].x,(int)quadCoordinates[i].y,quad);
               //quad.node = new Node(true,quadsParent.GetChild(i).position,(int)quadCoordinates[i].x,(int)quadCoordinates[i].y);
               //因为生成的时候是按quadCoordinates的顺序生成,因此这里也是这个顺序
               dict.Add(quadCoordinates[i],quad);//所以字典里面的坐标并不是寻路的坐标,而是地图的坐标!
           }else {
               UnityEngine.Debug.Log("没有quad");
           }
        } 
    }
    public Quad GetQuadByPosition(Vector3 worldPosition) {
        //应该有一种方法来判断点在哪片区域
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
    public Vector3 GetQuadPositionByCoordinate(Vector2 coordinate) {
       if(CurrentMap != null) {
          return CurrentMap.CalculatePosition(coordinate);
       }
       throw new NullReferenceException();
    }
    public Node NodeFromWorldPoint(Vector3 worldPosition) {
        //if(!bounds.Contains(worldPosition)) return null;
        MapConfigurationSO map = MapManager.Instance.CurrentMapConfiguration;
        Vector2 offset = map.GetOffset();//寻路的原点和所有点坐标原点的偏移量
        float percentX = (worldPosition.x - (MapManager.Instance.CurrentMapConfiguration.OriginPoint.x + offset.x)) / gridWorldSize.x;
        float percentY = (worldPosition.z - (MapManager.Instance.CurrentMapConfiguration.OriginPoint.z + offset.y)) / gridWorldSize.y; 
        percentY = Mathf.Clamp01(percentY);
        int x = Mathf.CeilToInt(quadSizeX * percentX) - 1;//这里是推算出来的要ceil,先算出百分比,ceil是因为最大7.5,7.0到7.5都是第八格,然后因为index要-1
        int y = Mathf.CeilToInt(quadSizeY * percentY) - 1;//同理,因为这里quadSizeY是8,所以一样的
        //UnityEngine.Debug.Log("坐标是: " + new Vector2(x,y));
        return grid[x,y];
    }
    public Quad QuadFromNode(Node node) {
        Vector2 coordinate = CurrentMap.AStarCoordinateToCoordinate(new Vector2(node.gridX,node.gridY));
        if(findPathDict.ContainsKey(coordinate)) {//这样不对,因为字典里面是地图的坐标而不是寻路的坐标,所以需要转化
            return findPathDict[coordinate];
        }else {
            UnityEngine.Debug.LogError("cannot find this quad");
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
    private IEnumerator FindPath(Vector3 startPos, Vector3 targetPos) { //A star algorhithm
        
        Stopwatch sw = new Stopwatch();
        sw.Start();
        Vector3[] waypoints = new Vector3[0];
        bool pathSuccess = false;
        Node startNode = NodeFromWorldPoint(startPos);
        Node targetNode = NodeFromWorldPoint(targetPos);
        if(!startNode.walkable || !targetNode.walkable) yield return null;
        Heap<Node> openSet = new Heap<Node>(MaxSize);
        //List<Node> openSet = new List<Node>();
        HashSet<Node> closedSet = new HashSet<Node>();
        openSet.Add(startNode);
        
        while(openSet.Count > 0) {
            Node currentNode = openSet.RemoveFirst();//heap!
            // for (int i = 1; i < openSet.Count; i++) {
            //     if(openSet[i].fCost < currentNode.fCost || openSet[i].fCost == currentNode.fCost && openSet[i].hCost < currentNode.hCost) {
            //         currentNode = openSet[i];
            //     }
            // }
            // openSet.Remove(currentNode);
            closedSet.Add(currentNode);
            if(currentNode == targetNode) {
                sw.Stop();
                UnityEngine.Debug.Log("Path found: " + sw.ElapsedMilliseconds);
                pathSuccess = true;
                
                break;//对于ienumerator来说,yield break = return, break只是跳出这个循环
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
        yield return null;//这里循环结束了,那么要么找到了路,pathSuccess是true,要么找失败了,pathSuccess是false
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
        while (currentNode != startNode) {//如果起点和终点一样,那么path就是空的
            path.Add(currentNode);
            currentNode = currentNode.parent;
        }
        Vector3[] waypoints = SimplifyPath(path);
        Array.Reverse(waypoints);
        //目前下面执行的是显示找到的道路,之后会被更改
        foreach (var item in path) {
            UnityEngine.Debug.Log(new Vector2(item.gridX,item.gridY));
            if(QuadFromNode(item).TryGetComponent<Quad>(out Quad quad)) {
               quad.EnableEmissionShader(true);
            } 
        }
        return waypoints;
    }
    Vector3[] SimplifyPath(List<Node> path) {//让node转化为道路的具体坐标点,其中相同方向的点会被忽略
        List<Vector3> waypoints = new List<Vector3>();
        Vector2 directionOld = Vector2.zero;

        for (int i = 1; i < path.Count; i++) {//如果起点终点一样,path为空,根本不会进循环,path.count为1就是只有两个点,也不会进循环.
            Vector2 directionNew = new Vector2(path[i-1].gridX - path[i].gridX,path[i-1].gridY - path[i].gridY);//之前node的寻路系统坐标和之后node
            if(directionNew != directionOld) {//第一个点会被记下来,然后到下一次改方向的时候会记下来
                //waypoints.Add(path[i].worldPosition);//改方向之后加新的点,第一格add的点一定是终点旁边的点而不是终点,并且行径路线会出现斜线的情况
                waypoints.Add(path[i-1].worldPosition);//如果是i-1,那也就是终点也会记下来,因为RetracePath方法中一定会add endNode,所以unit会到达终点而不是终点旁边,并且走的路线更直,而不是斜线
            }
            directionOld = directionNew;
        }
        return waypoints.ToArray();//如果path有两个点以上,才会进循环,waypoints才不是空的.其实没有问题,因为一个点说明目标就在旁边,说明已经找到路了
        //使用的时候需要注意判断返回的这个Vector3[]里面有没有东西就好了!
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
