using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System;

[System.Serializable]
public class LeaderboardEntry
{
    public string playerName;
    public int score;
    public int kills;
    public float survivalTime;
    public int wave;
    public string date;
    
    public LeaderboardEntry(string name, int score, int kills, float time, int wave)
    {
        this.playerName = name;
        this.score = score;
        this.kills = kills;
        this.survivalTime = time;
        this.wave = wave;
        this.date = DateTime.Now.ToString("yyyy-MM-dd HH:mm");
    }
}

[System.Serializable]
public class LeaderboardData
{
    public List<LeaderboardEntry> entries = new List<LeaderboardEntry>();
}

/// <summary>
/// Manager untuk leaderboard system.
/// Save/load dari PlayerPrefs.
/// UNLIMITED storage - hanya display dibatasi.
/// </summary>
public class LeaderboardManager : MonoBehaviour
{
    [Header("Display Settings")]
    [SerializeField] private int displayLimit = 7; // Top 7 untuk display di UI
    
    [Header("Storage Settings")]
    [SerializeField] private bool unlimitedStorage = true; // UNLIMITED entries!
    [SerializeField] private int storageLimit = 100; // Safety limit (jika unlimited = false)
    
    private LeaderboardData leaderboardData;
    private const string LEADERBOARD_KEY = "LeaderboardData";
    
    private static LeaderboardManager instance;
    
    public static LeaderboardManager Instance
    {
        get { return instance; }
    }
    
    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
        
        LoadLeaderboard();
    }
    
    /// <summary>
    /// Add entry - SEMUA player bisa save!
    /// WITH DUPLICATE PREVENTION!
    /// </summary>
    public void AddEntry(string playerName, int score, int kills, float survivalTime, int wave)
    {
        // CHECK FOR EXACT DUPLICATES (same score & name & kills at same time)
        var recentDuplicate = leaderboardData.entries.FindLast(e => 
            e.playerName == playerName && 
            e.score == score && 
            e.kills == kills &&
            Mathf.Abs(e.survivalTime - survivalTime) < 1f // Within 1 second
        );
        
        if (recentDuplicate != null)
        {
            Debug.LogWarning($"?? DUPLICATE ENTRY DETECTED! Ignoring: {playerName} - {score}");
            Debug.LogWarning($"   This entry already exists in leaderboard!");
            return; // DON'T ADD DUPLICATE!
        }
        
        LeaderboardEntry newEntry = new LeaderboardEntry(playerName, score, kills, survivalTime, wave);
        leaderboardData.entries.Add(newEntry);
        
        // Sort by score (descending) - highest first
        leaderboardData.entries = leaderboardData.entries
            .OrderByDescending(e => e.score)
            .ToList();
        
        // Apply storage limit (if not unlimited)
        if (!unlimitedStorage && leaderboardData.entries.Count > storageLimit)
        {
            leaderboardData.entries = leaderboardData.entries
                .Take(storageLimit)
                .ToList();
            
            Debug.Log($"Leaderboard trimmed to {storageLimit} entries");
        }
        
        SaveLeaderboard();
        
        Debug.Log($"? Leaderboard entry added: {playerName} - Score: {score}");
        Debug.Log($"   Total entries in database: {leaderboardData.entries.Count}");
    }
    
    /// <summary>
    /// Hapus entry berdasarkan index (0-based, setelah sorting by score)
    /// </summary>
    public bool DeleteEntry(int index)
    {
        var sortedEntries = leaderboardData.entries
            .OrderByDescending(e => e.score)
            .ToList();
        
        if (index < 0 || index >= sortedEntries.Count)
        {
            Debug.LogWarning($"Invalid index: {index}. Total entries: {sortedEntries.Count}");
            return false;
        }
        
        LeaderboardEntry toDelete = sortedEntries[index];
        
        // Hapus dari list asli
        bool removed = leaderboardData.entries.Remove(toDelete);
        
        if (removed)
        {
            SaveLeaderboard();
            Debug.Log($"Deleted entry #{index + 1}: {toDelete.playerName} - Score: {toDelete.score}");
        }
        
        return removed;
    }
    
    /// <summary>
    /// Hapus entry berdasarkan nama dan score (lebih akurat)
    /// </summary>
    public bool DeleteEntry(string playerName, int score)
    {
        var toDelete = leaderboardData.entries.FirstOrDefault(e => 
            e.playerName == playerName && e.score == score);
        
        if (toDelete != null)
        {
            leaderboardData.entries.Remove(toDelete);
            SaveLeaderboard();
            Debug.Log($"Deleted entry: {playerName} - Score: {score}");
            return true;
        }
        
        Debug.LogWarning($"Entry not found: {playerName} - Score: {score}");
        return false;
    }
    
    /// <summary>
    /// Get top N entries untuk display.
    /// Default: Top 7 (sesuai template Anda).
    /// </summary>
    public List<LeaderboardEntry> GetTopEntries(int count = 7)
    {
        return leaderboardData.entries
            .OrderByDescending(e => e.score)
            .Take(count)
            .ToList();
    }
    
    /// <summary>
    /// Get ALL entries (untuk admin/statistics).
    /// </summary>
    public List<LeaderboardEntry> GetAllEntries()
    {
        return leaderboardData.entries
            .OrderByDescending(e => e.score)
            .ToList();
    }
    
    /// <summary>
    /// Check if score qualifies for top display (Top 7).
    /// NOT used for allowing input - input ALWAYS allowed!
    /// </summary>
    public bool IsTopScore(int score, int topCount = 7)
    {
        var topEntries = GetTopEntries(topCount);
        
        // If less than topCount entries, always qualify
        if (topEntries.Count < topCount)
        {
            return true;
        }
        
        // Check if score beats lowest in top
        return score > topEntries.Last().score;
    }
    
    /// <summary>
    /// DEPRECATED - use IsTopScore instead.
    /// Kept for backwards compatibility.
    /// </summary>
    public bool IsHighScore(int score)
    {
        // Since we allow unlimited entries, this always returns true
        // OR you can check if it's in top 7
        return IsTopScore(score, displayLimit);
    }
    
    /// <summary>
    /// Get player's rank berdasarkan score.
    /// </summary>
    public int GetRank(int score)
    {
        int rank = 1;
        foreach (var entry in leaderboardData.entries.OrderByDescending(e => e.score))
        {
            if (score >= entry.score)
            {
                return rank;
            }
            rank++;
        }
        return rank;
    }
    
    /// <summary>
    /// Get total number of entries in database.
    /// </summary>
    public int GetTotalEntries()
    {
        return leaderboardData.entries.Count;
    }
    
    /// <summary>
    /// Check if player's score will appear in Top N display.
    /// </summary>
    public bool WillShowInLeaderboard(int score, int displayCount = 7)
    {
        return IsTopScore(score, displayCount);
    }
    
    private void SaveLeaderboard()
    {
        string json = JsonUtility.ToJson(leaderboardData);
        PlayerPrefs.SetString(LEADERBOARD_KEY, json);
        PlayerPrefs.Save();
        
        Debug.Log($"Leaderboard saved! Total entries: {leaderboardData.entries.Count}");
    }
    
    private void LoadLeaderboard()
    {
        if (PlayerPrefs.HasKey(LEADERBOARD_KEY))
        {
            string json = PlayerPrefs.GetString(LEADERBOARD_KEY);
            leaderboardData = JsonUtility.FromJson<LeaderboardData>(json);
            
            Debug.Log($"Leaderboard loaded! {leaderboardData.entries.Count} entries found");
        }
        else
        {
            leaderboardData = new LeaderboardData();
            Debug.Log("No leaderboard data found. Created new.");
        }
    }
    
    public void ClearLeaderboard()
    {
        leaderboardData.entries.Clear();
        SaveLeaderboard();
        Debug.Log("Leaderboard cleared!");
    }
    
    /// <summary>
    /// Debug - print all entries.
    /// </summary>
    [ContextMenu("Print All Entries")]
    public void PrintAllEntries()
    {
        Debug.Log($"========== ALL LEADERBOARD ENTRIES ({leaderboardData.entries.Count}) ==========");
        
        int rank = 1;
        foreach (var entry in leaderboardData.entries.OrderByDescending(e => e.score))
        {
            Debug.Log($"#{rank}: {entry.playerName} - Score: {entry.score}, Kills: {entry.kills}, Wave: {entry.wave}");
            rank++;
        }
    }
}
