using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PreparationQuad : Quad {
    public override void InitializeNode(bool _walkable, Vector3 _worldPos, int _gridX, int _gridY, Quad _attachedQuad) {
        node = new Node(_worldPos,_attachedQuad);
    }
}
