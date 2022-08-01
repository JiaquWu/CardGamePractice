using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
[CustomEditor(typeof(LandGenerator))]
public class LandGeneratorEditor : Editor {
    public override void OnInspectorGUI() {
        LandGenerator generator = (LandGenerator)target;
        if(DrawDefaultInspector()) {
            if(generator.autoUpdate) {
                generator.DrawMapInEditor();
            }
        }
        if(GUILayout.Button("Generate")) {
            generator.DrawMapInEditor();
        }

    }
}
