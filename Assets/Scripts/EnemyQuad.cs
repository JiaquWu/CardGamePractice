using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyQuad : Quad {
    public override void OnMouseEnter() {
        
    }
    public override void OnMouseExit() {

    }
    public override void OnMouseDown() {

    }
    public override void InitializeNode(bool _walkable, Vector3 _worldPos, int _gridX, int _gridY,Quad _attachedQuad) {
        node = new Node(_walkable,_worldPos,_gridX,_gridY-1,_attachedQuad);//-1是因为生成map的坐标和算法相关不一样
    }
}
