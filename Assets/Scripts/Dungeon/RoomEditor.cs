using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(RoomManager), true)]
public class RoomEditor : Editor
{
    private RoomManager dungeonManager;

    void OnEnable()
    {
        dungeonManager = (RoomManager)target;
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
