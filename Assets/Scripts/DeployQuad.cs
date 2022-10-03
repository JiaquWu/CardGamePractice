﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeployQuad : Quad {
    public override void InitializeNode(bool _walkable, Vector3 _worldPos, int _gridX, int _gridY, Quad _attachedQuad) {
        node = new Node(_walkable,_worldPos,_gridX,_gridY-1,_attachedQuad);//-1 because of this map
    }
}
