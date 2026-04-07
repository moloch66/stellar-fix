using System.IO;
using UnityEngine;

[System.Serializable]
public class SaveData
{
    public int   currentDay  = 1;
    public float credits     = 0f;
    public float reputation  = 0f;
}

/// <summary>
/// Salva e carrega progresso em JSON local.
/// </summary>
public static class SaveSystem
{
    private static string SavePath => Path.Combine(Application.persistentDataPath, "stellarfix_save.json");

    public static void Save(GameManager gm)
    {
        var data = gm.ToSaveData();
        var json = JsonUtility.ToJson(data, prettyPrint: true);
        File.WriteAllText(SavePath, json);
        Debug.Log($"[SaveSystem] Salvo em: {SavePath}");
    }

    public static SaveData Load()
    {
        if (!File.Exists(SavePath)) return null;
        var json = File.ReadAllText(SavePath);
        return JsonUtility.FromJson<SaveData>(json);
    }

    public static void DeleteSave()
    {
        if (File.Exists(SavePath)) File.Delete(SavePath);
        Debug.Log("[SaveSystem] Save deletado.");
    }

    public static bool HasSave() => File.Exists(SavePath);
}
