using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class MeshGenerator {
    public static MeshData GenerateTerrainMesh(float[,] heightMap) {
        int width = heightMap.GetLength(0);
        int height = heightMap.GetLength(1);
        float topLeftX = (width-1)/-2f;//比如横向总共三个点，那么第二点为中心点，第一点的横坐标就是-1
        float topLeftZ = (height-1)/2f;//从左上角开始
        MeshData meshData = new MeshData(width,height);
        int vertexIndex = 0;
        for (int y = 0; y < height; y++) {
            for (int x = 0; x < width; x++) {
                meshData.vertices[vertexIndex] = new Vector3(topLeftX + x,heightMap[x,y],topLeftZ - y);
                meshData.uvs[vertexIndex] = new Vector2(x/(float)width,y/(float)height);//让uvs里面每个值是这个点在整张图中的相对位置
                if(x < width -1 && y < height - 1) {//意味着不是右边和下面的边界
                    meshData.AddTriangle(vertexIndex,vertexIndex + width + 1,vertexIndex + width);//顺时针三个点
                    meshData.AddTriangle(vertexIndex,vertexIndex + 1,vertexIndex + width + 1);
                }  
                vertexIndex ++;
            }
        }
        return meshData;
    }
}

public class MeshData {
    public Vector3[] vertices;
    public int[] triangles;
    public Vector2[] uvs;
    int triangleIndex;
    public MeshData(int meshWidth, int meshHeight) {
        vertices = new Vector3[meshHeight * meshWidth];
        uvs = new Vector2[meshHeight * meshWidth];
        triangles = new int[(meshHeight-1)*(meshWidth-1)*6];//(meshHeight-1)*(meshWidth-1)是长为width,宽为height的点阵能画多少个正方形，
        //一个正方形包含两个三角形，一个三角形需要三个点
    }
    public void AddTriangle(int a,int b,int c) {
        triangles[triangleIndex] = a;
        triangles[triangleIndex + 1] = b;
        triangles[triangleIndex + 2] = c;
        triangleIndex += 3;
    }
    public Mesh CreateMesh() {
        Mesh mesh = new Mesh();
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.uv = uvs;
        mesh.RecalculateNormals();
        return mesh;
    }
}
