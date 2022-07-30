using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndlessTerrain : MonoBehaviour {
    public const float maxViewDist = 300;
    public Transform viewer;

    public static Vector2 viewerPosition;
    int chunkSize;
    int chunksVisibleInViewDist;

    private void Start() {
        chunkSize = LandGenerator.mapChunkSize -1;//一个chunk有多少格
        chunksVisibleInViewDist = Mathf.RoundToInt(maxViewDist/chunkSize);//最大可视格数除以每chunk格数等于最大可视chunk
    }

    void UpdateVisibleChunks() {
        int currentChunkCoordX = Mathf.RoundToInt(viewerPosition.x / chunkSize);
        int currentChunkCoordY = Mathf.RoundToInt(viewerPosition.x / chunkSize);
    }
    
}
