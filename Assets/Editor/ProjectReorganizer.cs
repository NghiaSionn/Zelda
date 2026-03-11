using UnityEngine;
using UnityEditor;
using System.IO;

public class ProjectReorganizer : EditorWindow
{
    [MenuItem("Tools/Reorganize Project")]
    public static void Reorganize()
    {
        // Define paths
        string projectPath = "Assets/_Project";
        string settingsPath = "Assets/Settings";
        string thirdPartyPath = "Assets/ThirdParty";
        string editorPath = "Assets/Editor";

        // Create root folders
        CreateFolderIfNotExistsRecursively(projectPath);
        CreateFolderIfNotExistsRecursively(settingsPath);
        CreateFolderIfNotExistsRecursively(thirdPartyPath);
        CreateFolderIfNotExistsRecursively(projectPath + "/Sprites");
        CreateFolderIfNotExistsRecursively(settingsPath + "/URP");

        // Move to _Project
        MoveAsset("Assets/Animations", projectPath + "/Animations");
        MoveAsset("Assets/Audio", projectPath + "/Audio");
        MoveAsset("Assets/Fonts", projectPath + "/Fonts");
        MoveAsset("Assets/Materials", projectPath + "/Materials");
        MoveAsset("Assets/Prefabs", projectPath + "/Prefabs");
        MoveAsset("Assets/Scenes", projectPath + "/Scenes");
        MoveAsset("Assets/ScriptableObject", projectPath + "/ScriptableObjects");
        MoveAsset("Assets/Scripts", projectPath + "/Scripts");
        MoveAsset("Assets/Shader", projectPath + "/Shaders");
        MoveAsset("Assets/Time Line", projectPath + "/Timelines");
        
        // Move to _Project/Sprites
        MoveAsset("Assets/Resource", projectPath + "/Sprites/Resource");
        MoveAsset("Assets/Resources", projectPath + "/Sprites/Resources");
        MoveAsset("Assets/TiledMap", projectPath + "/Sprites/TiledMap");

        // Move to Plugins (already exists probably, but just in case)
        CreateFolderIfNotExistsRecursively("Assets/Plugins");
        MoveAsset("Assets/Dialogue System", "Assets/Plugins/Dialogue System");
        MoveAsset("Assets/NavMeshPlus", "Assets/Plugins/NavMeshPlus");

        // Move to ThirdParty
        MoveAsset("Assets/TextMesh Pro", thirdPartyPath + "/TextMesh Pro");
        MoveAsset("Assets/TileRules", thirdPartyPath + "/TileRules");

        // Move to Settings/URP
        MoveAsset("Assets/URP", settingsPath + "/URP/URP_Folder");
        MoveAsset("Assets/URP.asset", settingsPath + "/URP/URP.asset");
        MoveAsset("Assets/URP_Renderer.asset", settingsPath + "/URP/URP_Renderer.asset");
        MoveAsset("Assets/UniversalRenderPipelineGlobalSettings.asset", settingsPath + "/URP/UniversalRenderPipelineGlobalSettings.asset");
        MoveAsset("Assets/DefaultVolumeProfile.asset", settingsPath + "/URP/DefaultVolumeProfile.asset");

        // Move Editor Default Resources
        MoveAsset("Assets/Editor Default Resources", editorPath + "/Editor Default Resources");

        // Move loose scripts if they exist
        CreateFolderIfNotExistsRecursively(projectPath + "/Scripts/Audio");
        MoveAsset("Assets/PlaySoundEnter.cs", projectPath + "/Scripts/Audio/PlaySoundEnter.cs");
        MoveAsset("Assets/PlaySoundExit.cs", projectPath + "/Scripts/Audio/PlaySoundExit.cs");

        AssetDatabase.Refresh();
        Debug.Log("✅ [DONE] Project Reorganization Completed Successfully! You can now safely delete the `_Recovery` folder if you do not need it.");
    }

    private static void MoveAsset(string oldPath, string newPath)
    {
        bool isAsset = !string.IsNullOrEmpty(AssetDatabase.AssetPathToGUID(oldPath));
        if (isAsset)
        {
            // If the destination directory or asset already exists, skip to prevent errors.
            if (AssetDatabase.IsValidFolder(newPath) || !string.IsNullOrEmpty(AssetDatabase.AssetPathToGUID(newPath)))
            {
                Debug.Log($"⚠️ Skipped: {newPath} already exists.");
                return;
            }

            string result = AssetDatabase.MoveAsset(oldPath, newPath);
            if (!string.IsNullOrEmpty(result))
            {
                Debug.LogWarning($"❌ Failed to move {oldPath} to {newPath}: {result}");
            }
            else
            {
                Debug.Log($"✅ Moved: {oldPath} -> {newPath}");
            }
        }
    }

    private static void CreateFolderIfNotExistsRecursively(string path)
    {
        if (AssetDatabase.IsValidFolder(path)) return;
        
        string parent = Path.GetDirectoryName(path).Replace("\\", "/");
        if (!AssetDatabase.IsValidFolder(parent))
        {
            CreateFolderIfNotExistsRecursively(parent);
        }
        
        string newFolderName = Path.GetFileName(path);
        AssetDatabase.CreateFolder(parent, newFolderName);
    }
}
