﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LandGenerator : MonoBehaviour {
    public enum DrawMode {NoiseMap, colorMap,Mesh}
    public DrawMode drawMode;
    public int mapWidth;
    public int mapHeight;
    public float noiseScale;

    public int octaves;
    [Range(0,1)]
    public float persistance;
    public float lacunarity;
    
    public int seed;
    public Vector2 offset;
    public bool autoUpdate;
    public TerrianType[] regions;
    //生成顺序：先由noise类的方法得到一张float[,]类型的噪点图，也就是heightMap，然后噪点图经过色彩处理要么变成黑白的texture，要么变成彩色的texture
    public void GenerateMap() {
        float[,] noiseMap = Noise.GenerateNoiseMap(mapWidth,mapHeight,seed,noiseScale,octaves,persistance,lacunarity,offset);
        Color[] colorMap = new Color[mapHeight * mapWidth];
        for (int y = 0; y < mapHeight; y++) {
            for (int x = 0; x < mapWidth; x++) {
                float currentHeight = noiseMap[x,y];
                for (int i = 0; i < regions.Length; i++) {
                    if(currentHeight <= regions[i].height) {
                        colorMap[y*mapWidth + x] = regions[i].color;
                        break;
                    }
                }
            }
        }
        MapDisplay display = FindObjectOfType<MapDisplay>();
        if(drawMode == DrawMode.NoiseMap) {//如果只是得到噪点图，那就是黑白的，
            display.DrawTexture(TextureGenerator.TextureFromHeightMap(noiseMap));
        }else if(drawMode == DrawMode.colorMap) {//如果要加颜色，那么就是这个脚本开放变量中设置的颜色+
            display.DrawTexture(TextureGenerator.TextureFromColorMap(colorMap,mapWidth,mapHeight));
        }else if (drawMode == DrawMode.Mesh) {//如果是要生成mesh
            display.DrawMesh(MeshGenerator.GenerateTerrainMesh(noiseMap),TextureGenerator.TextureFromColorMap(colorMap,mapWidth,mapHeight));
        }
        
    }
    private void OnValidate() {
        if(mapWidth < 1) {
            mapWidth = 1;
        }
        if(mapHeight < 1) {
            mapHeight = 1;
        }
        if(lacunarity < 1) {
            lacunarity = 1;
        }
        if(octaves < 0) {
            octaves = 0;
        }
    }
}
[System.Serializable]
public struct TerrianType {
    public string name;
    public float height;
    public Color color;
}