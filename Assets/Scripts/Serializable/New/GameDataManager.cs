using Newtonsoft.Json;
using System;
using System.IO;
using UnityEngine;

[Serializable]
public static class GameDataManager
{
    public static PlayerData PlayerData { get; private set; }
    public static string CurrentActivePresident {  get; private set; }

    public static void Initialize(string presidentName)
    {
        CurrentActivePresident = presidentName;
        var playerDataPath = Application.persistentDataPath + $"/{CurrentActivePresident}PlayerData.json";

        if (File.Exists(playerDataPath))
            PlayerData = JsonConvert.DeserializeObject<PlayerData>(File.ReadAllText(playerDataPath));
        else
            PlayerData = new PlayerData();
    }

    public static void SavePlayerData()
    {
        var playerDataPath = Application.persistentDataPath + $"/{CurrentActivePresident}PlayerData.json";
        var jsonData = JsonConvert.SerializeObject(PlayerData, Formatting.Indented);

        File.WriteAllText(playerDataPath, jsonData);
    }
}