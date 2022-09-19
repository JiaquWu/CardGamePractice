using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class TextureGenerator {
    public static Texture2D TextureFromColorMap(Color[] colorMap, int width, int height) {//这个方法是让经过色彩加工变成了colorMap的heightMap转化为texture
        Texture2D texture = new Texture2D(width,height);
        texture.filterMode = FilterMode.Point;
        texture.wrapMode = TextureWrapMode.Clamp;
        texture.SetPixels(colorMap);
        texture.Apply();
        return texture;
    }

    public static Texture2D TextureFromHeightMap(float[,] heightMap) {//这个方法能把noiseMap生成的高度图转化成黑白的texture
        int width = heightMap.GetLength(0);//0就是第一个维度,就是宽
        int height = heightMap.GetLength(1);

        Color[] colorMap = new Color[width*height];
        for (int y = 0; y < height; y++) {
            for (int x = 0; x < width; x++) {
                colorMap[y*width+x] = Color.Lerp(Color.black,Color.white,heightMap[x,y]);//y*width+x y是高,所以先确定在第几行
            }
        }
        return TextureFromColorMap(colorMap,width,height);//这里传入的colorMap是黑白的，所以得到的texture是黑白的
    }
}
