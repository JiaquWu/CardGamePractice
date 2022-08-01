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
            terrainChunksVisbleLastUpdate[i].SetVisible(false);
        }
        terrainChunksVisbleLastUpdate.Clear();

        int currentChunkCoordX = Mathf.RoundToInt(viewerPosition.x / chunkSize);//chunk的坐标,(0,1)这种
        int currentChunkCoordY = Mathf.RoundToInt(viewerPosition.y / chunkSize);
        
        for (int yOffset = -chunksVisibleInViewDist; yOffset <= chunksVisibleInViewDist; yOffset++) {
            for (int xOffset = -chunksVisibleInViewDist; xOffset <= chunksVisibleInViewDist; xOffset++) {
                Vector2 viewedChunkCoord = new Vector2(currentChunkCoordX + xOffset, currentChunkCoordY + yOffset);
                if(terrainChunkDictionary.ContainsKey(viewedChunkCoord)) {
                    terrainChunkDictionary[viewedChunkCoord].UpdateTerrainChunk();
                    if(terrainChunkDictionary[viewedChunkCoord].IsVisible()) {
                       terrainChunksVisbleLastUpdate.Add(terrainChunkDictionary[viewedChunkCoord]);
                    }
                }else {
                    terrainChunkDictionary.Add(viewedChunkCoord,new TerrianChunk(viewedChunkCoord,chunkSize,transform,mapMaterial));
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

