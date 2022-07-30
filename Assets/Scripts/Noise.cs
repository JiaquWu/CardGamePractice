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
                    float sampleX = (x-halfWidth) / scale * frequency + octaveOffsets[i].x;//乘以频率,这样两个值之间的差距会更大,更不连贯,
                    float sampleY = (y-halfHeight) / scale * frequency + octaveOffsets[i].y;//y-halfHeight能让一半的值是负数，
                    //对于正数来说，scale越大，x/scale值越小，但对于负数来说，scale越大，x/scale值越大，
                    //因此这样以中心为原点，建立坐标轴，四个象限的点会以不同比例(x正比，y正比；x反比，y反比；x正比，y反比；x反比，y正比)缩放
                    //从而使视觉效果上实现中心缩放
                    //另外，从结果上来看这个图的坐标轴和一般的是相反的，如果旋转180度坐标轴就是正的，很奇怪，先不管了

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
