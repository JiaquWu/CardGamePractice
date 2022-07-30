using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Noise {
    public static float[,] GenerateNoiseMap(int mapWidth, int mapHeight,int seed, float scale,int octaves,float persistance,float lacunarity,Vector2 offset) {
        float[,] noiseMap = new float[mapWidth,mapHeight];
        System.Random prng = new System.Random(seed);//prng = pseudo-random number generator 伪随机数字生成 
        Vector2[] octaveOffsets = new Vector2[octaves];//让每张图在不同地方生成
        for (int i = 0; i < octaves; i++) {
            float offsetX = prng.Next(-100000,100000) + offset.x;//加随机的offset确保每个seed对应的不一样,加了自己设置的offset方便滚动观看附近的
            float offsetY = prng.Next(-100000,100000) + offset.y;
            octaveOffsets[i] = new Vector2(offsetX,offsetY);
        }
        if(scale <= 0) {
            scale = 0.0001f;
        }
        float maxNoiseHeight = float.MinValue;
        float minNoiseHeight = float.MaxValue;
        float halfWidth = mapWidth/2;
        float halfHeight = mapHeight/2;
        for (int y = 0; y < mapHeight; y++) {
            for (int x = 0; x < mapWidth; x++) {
                float amplitude = 1;
                float frequency = 1;
                float noiseHeight = 0;
                for (int i = 0; i < octaves; i++) {
                    float sampleX = (x-halfWidth) / scale * frequency + octaveOffsets[i].x;//乘以频率,这样两个值之间的差距会更大,更不连贯,(x-halfWidth)是因为全体向左下角偏移一半,这样就能缩放到中心
                    float sampleY = (y-halfHeight) / scale * frequency + octaveOffsets[i].y;//这里没搞懂为什么x-halfwidth就行了

                    float perlinValue = Mathf.PerlinNoise(sampleX,sampleY) * 2 -1;//原本返回随机0到1,这样就会随机-1到1
                    noiseHeight += perlinValue * amplitude;//由于amplitude会越来越小,noiseHeight的变化会越来越小
                    
                    amplitude *= persistance;// persistance是0到1,所以幅度越来越小
                    frequency *= lacunarity;// lacunarity大于1,所以频率越来越大
                }
                if(noiseHeight > maxNoiseHeight) {
                    maxNoiseHeight = noiseHeight;//更新max
                }
                if(noiseHeight < minNoiseHeight) {
                    minNoiseHeight = noiseHeight;//更新min
                }
                noiseMap[x,y] = noiseHeight;
            }
        }
        for (int y = 0; y < mapHeight; y++) {
            for (int x = 0; x < mapWidth; x++) {
                noiseMap[x,y] = Mathf.InverseLerp(minNoiseHeight,maxNoiseHeight,noiseMap[x,y]);//一个方法知道目标值在最小值和最大值的位置(比例),这样也能确保值在(0,1)之间
            }
        }
        return noiseMap;
    }
}
