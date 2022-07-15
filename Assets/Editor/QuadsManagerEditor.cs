using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
[CustomEditor(typeof(QuadsManager))]
public class QuadsManagerEditor : Editor {
    public override void OnInspectorGUI() {
        base.OnInspectorGUI();
        QuadsManager quadsManager = (QuadsManager)target;
        if(GUILayout.Button("DeleteAllQuads")) {
            quadsManager.DeleteAllQuads();
        }

    }
}
