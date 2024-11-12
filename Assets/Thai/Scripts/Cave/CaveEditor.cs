using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(CaveManager), true)]
public class CaveEditor : Editor
{
    CaveManager caveManager;

    private void Awake()
    {
        caveManager = (CaveManager)target;
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        if(GUILayout.Button("Generate"))
        {
            caveManager.GenerateMap();
        }
    }
}
