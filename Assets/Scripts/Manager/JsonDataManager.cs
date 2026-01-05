using UnityEngine;
using System.IO;

/// <summary>
/// Alternative data persistence menggunakan JSON file.
/// Data tersimpan di Application.persistentDataPath.
/// </summary>
public class JsonDataManager : MonoBehaviour
{
    private static string SavePath => Path.Combine(Application.persistentDataPath, "gamedata.json");
    
    [System.Serializable]
    public class GameData
    {
        public LeaderboardData leaderboard = new LeaderboardData();
        public int highestWave = 0;
        public int totalKills = 0;
        public float longestSurvival = 0f;
    }
    
    /// <summary>
    /// Save all game data to JSON file.
    /// </summary>
    public static void SaveData(GameData data)
    {
        try
        {
            string json = JsonUtility.ToJson(data, true); // true = pretty print
            File.WriteAllText(SavePath, json);
            
            Debug.Log($"Data saved to: {SavePath}");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Failed to save data: {e.Message}");
        }
    }
    
    /// <summary>
    /// Load all game data from JSON file.
    /// </summary>
    public static GameData LoadData()
    {
        if (File.Exists(SavePath))
        {
            try
            {
                string json = File.ReadAllText(SavePath);
                GameData data = JsonUtility.FromJson<GameData>(json);
                
                Debug.Log($"Data loaded from: {SavePath}");
                return data;
            }
            catch (System.Exception e)
            {
                Debug.LogError($"Failed to load data: {e.Message}");
            }
        }
        
        Debug.Log("No save file found, creating new data.");
        return new GameData();
    }
    
    /// <summary>
    /// Delete save file.
    /// </summary>
    public static void DeleteSaveFile()
    {
        if (File.Exists(SavePath))
        {
            File.Delete(SavePath);
            Debug.Log("Save file deleted!");
        }
    }
    
    /// <summary>
    /// Get save file path.
    /// </summary>
    public static string GetSavePath()
    {
        return SavePath;
    }
    
    /// <summary>
    /// Check if save file exists.
    /// </summary>
    public static bool SaveFileExists()
    {
        return File.Exists(SavePath);
    }
}
