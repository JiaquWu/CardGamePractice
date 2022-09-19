using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapDisplay : MonoBehaviour {
    public Renderer textureRender;
    public MeshFilter meshFilter;
    public MeshRenderer meshRenderer;
    public void DrawTexture(Texture2D texture) {
        textureRender.sharedMaterial.mainTexture = texture;
        textureRender.transform.localScale = new Vector3(texture.width,1,texture.height);
    }
    public void DrawMesh(MeshData meshData,Texture2D texture) {
        meshFilter.sharedMesh = meshData.CreateMesh();//用传来的meshData数据创建一个新的mesh,shared是因为可以在editor修改
        meshRenderer.sharedMaterial.mainTexture = texture;//把材质替换成传来的计算好的材质， 
    }
}
