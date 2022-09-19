using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndlessTerrain : MonoBehaviour {
    
    public LODInfo[] detailLevels;
    public static float maxViewDist;
    public Transform viewer;
    public Material mapMaterial;
    public static Vector2 viewerPosition;
    int chunkSize;
    int chunksVisibleInViewDist;
    static LandGenerator landGenerator;
    Dictionary<Vector2,TerrianChunk> terrainChunkDictionary = new Dictionary<Vector2, TerrianChunk>();
    List<TerrianChunk> terrainChunksVisbleLastUpdate = new List<TerrianChunk>();
    private void Start() {
        landGenerator = FindObjectOfType<LandGenerator>();
        maxViewDist = detailLevels[detailLevels.Length-1].visibleDistThreshold;//最大的可视距离就是所有lod里面最大的阈限
        chunkSize = LandGenerator.mapChunkSize -1;//一个chunk有多少格
        chunksVisibleInViewDist = Mathf.RoundToInt(maxViewDist/chunkSize);//最大可视格数除以每chunk格数等于最大可视chunk
    }
    private void Update() {
        viewerPosition = new Vector2(viewer.position.x,viewer.position.z);
        UpdateVisibleChunks();
    }
    void UpdateVisibleChunks() {
        for (int i = 0; i < terrainChunksVisbleLastUpdate.Count; i++) {
            terrainChunksVisbleLastUpdate[i].SetVisible(false);//先把上一帧加进来的所有都关掉,下面的方法再重新开一次
        }
        terrainChunksVisbleLastUpdate.Clear();

        int currentChunkCoordX = Mathf.RoundToInt(viewerPosition.x / chunkSize);//chunk的坐标,(0,1)这种
        int currentChunkCoordY = Mathf.RoundToInt(viewerPosition.y / chunkSize);//比如chunksize是240,position是260,那么坐标就是1,因为坐标从(0,0)开始
        
        for (int yOffset = -chunksVisibleInViewDist; yOffset <= chunksVisibleInViewDist; yOffset++) {//从负数开始是因为角色前后左右
            for (int xOffset = -chunksVisibleInViewDist; xOffset <= chunksVisibleInViewDist; xOffset++) {//offset就是说人不管在哪里,我前后左右若干格都应该能看到
                Vector2 viewedChunkCoord = new Vector2(currentChunkCoordX + xOffset, currentChunkCoordY + yOffset);
                if(terrainChunkDictionary.ContainsKey(viewedChunkCoord)) {//说明这个坐标来过
                    terrainChunkDictionary[viewedChunkCoord].UpdateTerrainChunk();//更新一下,一方面setvisible,一方面更新LOD数据
                    if(terrainChunkDictionary[viewedChunkCoord].IsVisible()) {
                       terrainChunksVisbleLastUpdate.Add(terrainChunkDictionary[viewedChunkCoord]);
                    }
                }else {
                    terrainChunkDictionary.Add(viewedChunkCoord,new TerrianChunk(viewedChunkCoord,chunkSize,detailLevels,transform,mapMaterial));//因为在update调用,
                    //所以这一帧add进去了,下一帧会updateterrainchunk
                }
            }
        }
    }
    public class TerrianChunk {
        GameObject meshObject;
        Vector2 position;
        Bounds bounds;
        MapData mapData;
        bool mapDataReceived;
        int previousLODIndex = -1;
        MeshRenderer meshRenderer;
        MeshFilter meshFilter;
        LODInfo[] detailLevels;
        LODMesh[] lODMeshes;
        
        
        public TerrianChunk(Vector2 coord, int size,LODInfo[] _detailLevels, Transform parent,Material material) {
            detailLevels = _detailLevels;
            position = coord * size;
            bounds = new Bounds(position,Vector2.one * size);
            Vector3 positionV3 = new Vector3(position.x,0,position.y);
            meshObject = new GameObject("Terrain Chunk");
            meshRenderer = meshObject.AddComponent<MeshRenderer>();
            meshRenderer.material = material;
            meshFilter = meshObject.AddComponent<MeshFilter>();
            meshObject.transform.position = positionV3;
            meshObject.transform.parent = parent;
            SetVisible(false);

            lODMeshes = new LODMesh[detailLevels.Length];//有多少度LOD,就有多少种mesh
            for (int i = 0; i < detailLevels.Length; i++) {
                lODMeshes[i] = new LODMesh(_detailLevels[i].lod);//在创建这个chunk的时候,就把所有lod的mesh放在这个数组里面
            }
            landGenerator.RequestMapData(OnMapDataReceived);
        }
        void OnMapDataReceived(MapData mapData) {
            this.mapData = mapData;
            mapDataReceived = true;
            //print("Map data received");
            //landGenerator.RequestMeshData(mapData,OnMeshDataReiceived);
        }
        public void UpdateTerrainChunk() {
            if(!mapDataReceived) return;//如果没有拿到mapdata,那么就不执行
            float viewerDistFromNearestEdge = Mathf.Sqrt(bounds.SqrDistance(viewerPosition));//一个点离本bound最小距离
            bool visible = viewerDistFromNearestEdge <= maxViewDist;//离本chunk最近的距离如果小于我的可视距离,那么就看得见
            //实际上如果viewerDistFromNearestEdge > maxViewDist(即detailLevels[detailLevels.Length-1].visibleDistThreshold),那么就不是visible的,不会进入下面的代码
            if(visible) {
                int lodIndex = 0;
                for (int i = 0; i < detailLevels.Length-1; i++) {//不看最后一个的原因是最后一个是看不见的,进入这里的条件是visible,如果viewerDist 大于最后一个,那么进不来,矛盾了
                    if(viewerDistFromNearestEdge > detailLevels[i].visibleDistThreshold) {//如果距离超过阈限,就继续找
                        lodIndex = i+1;
                    }else {//找到正确的区间了
                        break;
                    }
                }
                if(lodIndex != previousLODIndex) {//找到的不是上一个index,避免重复
                    LODMesh lODMesh = lODMeshes[lodIndex];//那么在index确认的情况下,就在这个数组里面去把它拿出来
                    if(lODMesh.hasMesh) {
                        previousLODIndex = lodIndex;//标记一下
                        meshFilter.mesh = lODMesh.mesh;//切换本chunk的mesh
                    }else if(!lODMesh.hasRequestedMesh) {
                        lODMesh.RequestMesh(mapData);
                    }
                }
            }
            SetVisible(visible);
        }
        public void SetVisible(bool visible) {
            meshObject.SetActive(visible);
        }
        public bool IsVisible() {
            return meshObject.activeSelf;
        }

    }
    class LODMesh {//通过这个类来记录一个chunk里面不同lod对应的mesh,至于是哪个程度,是外面创建对象的时候决定的
        public Mesh mesh;
        public bool hasRequestedMesh;
        public bool hasMesh;
        int lod;//lod指数
        public LODMesh(int _lod) {
            lod = _lod;
        }
        void OnMeshDataReceived(MeshData meshData) {
            mesh = meshData.CreateMesh();
            hasMesh = true;
        }
        public void RequestMesh(MapData mapdata) {
            hasRequestedMesh = true;
            landGenerator.RequestMeshData(mapdata,lod,OnMeshDataReceived);//这个类属于EndlessTerrain,所以这个landGenerator能直接用
            //这个类的一个对象去请求一个mesh过来,如果拿到了,就hasMesh了
        }
    }
    [System.Serializable]
    public struct LODInfo {
        public int lod;
        public float visibleDistThreshold;//超过这个阈限就切换lod
    }
}

