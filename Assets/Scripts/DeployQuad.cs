﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeployQuad : Quad {
    public override void InitializeNode(bool _walkable, Vector3 _worldPos, int _gridX, int _gridY) {
        node = new Node(_walkable,_worldPos,_gridX,_gridY-1);//-1是因为生成map的坐标和算法相关不一样,这里也是只符合这张地图
    }
}
