using System;
using System.Threading;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LandGenerator : MonoBehaviour {
    public enum DrawMode {NoiseMap, colorMap,Mesh}
    public DrawMode drawMode;
    public const int mapChunkSize = 241;
    [Range(0,6)]
    public int levelOfDetail;//2,4,6,8,10,12,这些都能被241-1整除
    public float noiseScale;

    public int octaves;
    [Range(0,1)]
    public float persistance;
    public float lacunarity;

    public int seed;
    public Vector2 offset;
    public float meshHeightMultiplier;
    public AnimationCurve meishHeightCurve;
    public bool autoUpdate;
    public TerrianType[] regions;
    Queue<MapThreadInfo<MapData>> mapDataThreadInfoQueue = new Queue<MapThreadInfo<MapData>>();
    Queue<MapThreadInfo<MeshData>> meshDataThreadInfoQueue = new Queue<MapThreadInfo<MeshData>>();
    public void DrawMapInEditor() {
        MapData mapData = GenerateMapData();
        MapDisplay display = FindObjectOfType<MapDisplay>();
        if(drawMode == DrawMode.NoiseMap) {//如果只是得到噪点图，那就是黑白的，
            display.DrawTexture(TextureGenerator.TextureFromHeightMap(mapData.heightMap));
        }else if(drawMode == DrawMode.colorMap) {//如果要加颜色，那么就是这个脚本开放变量中设置的颜色+
            display.DrawTexture(TextureGenerator.TextureFromColorMap(mapData.colorMap,mapChunkSize,mapChunkSize));
        }else if (drawMode == DrawMode.Mesh) {//如果是要生成mesh
            display.DrawMesh(MeshGenerator.GenerateTerrainMesh(mapData.heightMap,meshHeightMultiplier,meishHeightCurve,levelOfDetail),TextureGenerator.TextureFromColorMap(mapData.colorMap,mapChunkSize,mapChunkSize));
        }
    }

    public void RequestMapData(Action<MapData> callback) {
        ThreadStart threadStart = delegate {//ThreadStart is a Type which define what the Thread is gonna do. It is required when you declare a new Thread.
            MapDataThread(callback);
        };
        new Thread(threadStart).Start();
    }
    void MapDataThread(Action<MapData> callback) {
        MapData mapData = GenerateMapData();
        lock(mapDataThreadInfoQueue) {//一个进程访问到这里的时候,不让其他进程访问
            mapDataThreadInfoQueue.Enqueue(new MapThreadInfo<MapData>(callback,mapData));
        }
    }

    public void RequestMeshData(MapData mapData,Action<MeshData> callback) {//从mapdata得到meshdata
        ThreadStart threadStart = delegate {
            MeshDataThread(mapData,callback);
        };
        new Thread(threadStart).Start();
    }
    void MeshDataThread(MapData mapData,Action<MeshData> callback) {
        MeshData meshData = MeshGenerator.GenerateTerrainMesh(mapData.heightMap,meshHeightMultiplier,meishHeightCurve,levelOfDetail);
        lock(meshDataThreadInfoQueue) {
            meshDataThreadInfoQueue.Enqueue(new MapThreadInfo<MeshData>(callback,meshData));
        }
    }
    private void Update() {
        if(mapDataThreadInfoQueue.Count>0) {
            for (int i = 0; i < mapDataThreadInfoQueue.Count; i++) {
                MapThreadInfo<MapData> threadInfo = mapDataThreadInfoQueue.Dequeue();
                threadInfo.callback(threadInfo.parameter);
            }
        }
        if(meshDataThreadInfoQueue.Count>0) {
            for (int i = 0; i < meshDataThreadInfoQueue.Count; i++) {
                MapThreadInfo<MeshData> threadInfo = meshDataThreadInfoQueue.Dequeue();
                threadInfo.callback(threadInfo.parameter);
            }
        }
    }
    MapData GenerateMapData() {//生成顺序：先由noise类的方法得到一张float[,]类型的噪点图，也就是heightMap，然后噪点图经过色彩处理要么变成黑白的texture，要么变成彩色的texture
        float[,] noiseMap = Noise.GenerateNoiseMap(mapChunkSize,mapChunkSize,seed,noiseScale,octaves,persistance,lacunarity,offset);
        Color[] colorMap = new Color[mapChunkSize * mapChunkSize];
        for (int y = 0; y < mapChunkSize; y++) {
            for (int x = 0; x < mapChunkSize; x++) {
                float currentHeight = noiseMap[x,y];
                for (int i = 0; i < regions.Length; i++) {
                    if(currentHeight <= regions[i].height) {
                        colorMap[y*mapChunkSize + x] = regions[i].color;
                        break;
                    }
                }
            }
        }
        return new MapData(noiseMap,colorMap);

    }
    private void OnValidate() {
        if(lacunarity < 1) {
            lacunarity = 1;
        }
        if(octaves < 0) {
            octaves = 0;
        }
    }
    struct MapThreadInfo<T> {//用于储存回调函数和函数需要的参数的结构
        public readonly Action<T> callback;
        public readonly T parameter;//函数参数

        public MapThreadInfo(Action<T> _callback,T _parameter) {
            callback = _callback;
            parameter = _parameter;
        }
    }
}
[System.Serializable]
public struct TerrianType {
    public string name;
    public float height;
    public Color color;
}

public struct MapData {
    public readonly float[,] heightMap;
    public readonly Color[] colorMap;
    public MapData(float[,] _heightMap, Color[] _colorMap) {
        heightMap = _heightMap;
        colorMap = _colorMap;
    }

}