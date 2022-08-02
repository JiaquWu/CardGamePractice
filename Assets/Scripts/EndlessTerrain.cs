using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndlessTerrain : MonoBehaviour {
    public const float maxViewDist = 450;
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
                    terrainChunkDictionary[viewedChunkCoord].UpdateTerrainChunk();//
                    if(terrainChunkDictionary[viewedChunkCoord].IsVisible()) {
                       terrainChunksVisbleLastUpdate.Add(terrainChunkDictionary[viewedChunkCoord]);
                    }
                }else {
                    terrainChunkDictionary.Add(viewedChunkCoord,new TerrianChunk(viewedChunkCoord,chunkSize,transform,mapMaterial));//因为在update调用,
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
        MeshRenderer meshRenderer;
        MeshFilter meshFilter;
        public TerrianChunk(Vector2 coord, int size,Transform parent,Material material) {
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
            landGenerator.RequestMapData(OnMapDataReceived);
        }
        void OnMapDataReceived(MapData mapData) {
            print("Map data received");
            landGenerator.RequestMeshData(mapData,OnMeshDataReiceived);
        }
        void OnMeshDataReiceived(MeshData meshData) {
            meshFilter.mesh = meshData.CreateMesh();
        }
        public void UpdateTerrainChunk() {
            float viewerDistFromNearestEdge = Mathf.Sqrt(bounds.SqrDistance(viewerPosition));//一个点离这个bounds最小距离的开方
            bool visible = viewerDistFromNearestEdge <= maxViewDist;//离本chunk最近的距离如果小于我的可视距离,那么就看得见
            SetVisible(visible);
        }
        public void SetVisible(bool visible) {
            meshObject.SetActive(visible);
        }
        public bool IsVisible() {
            return meshObject.activeSelf;
        }

    }

}

