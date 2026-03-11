using System.IO;
using UnityEngine;

public static class SaveLoadUtility
{
    public static void SaveToJSON<T>(T data, string fileName)
    {
        string json = JsonUtility.ToJson(data, true);
        string filePath = Path.Combine(Application.persistentDataPath, fileName);
        File.WriteAllText(filePath, json);
        Debug.Log($"Data saved to {filePath}");
    }

    public static T LoadFromJSON<T>(string fileName) where T : new()
    {
        string filePath = Path.Combine(Application.persistentDataPath, fileName);
        if (File.Exists(filePath))
        {
            string json = File.ReadAllText(filePath);
            T data = JsonUtility.FromJson<T>(json);
            Debug.Log($"Data loaded from {filePath}");
            return data;
        }
        else
        {
            Debug.LogWarning($"File not found: {filePath}");
            return new T(); // Trả về một đối tượng rỗng nếu file không tồn tại
        }
    }
}