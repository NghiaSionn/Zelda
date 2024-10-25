using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(DungeonManager), true)]
public class DungeonEditor : Editor
{
    private DungeonManager dungeonManager;

    void OnEnable()
    {
        dungeonManager = (DungeonManager)target;
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        if(GUILayout.Button("Generate Dungeon"))
        {
            dungeonManager.GenerateDungeon();
        }
        if(GUILayout.Button("Clear Dungeon"))
        {
            dungeonManager.ClearDungeon();
        }
    }
}
