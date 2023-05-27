using System.IO;
using UnityEngine;

public static class DataManager
{
    public static PlayerData PlayerData { get; private set; }

    public static void LoadData()
    {
        string playerDataPath = Application.dataPath + "/playerData.json";

        if (File.Exists(playerDataPath))
        {
            PlayerData = JsonUtility.FromJson<PlayerData>(File.ReadAllText(playerDataPath));
        }
        else
        {
            PlayerData = new PlayerData();

            PlayerData.chapterIndex = 0;
            PlayerData.dialogueIndex = 0;
            PlayerData.lawIndex = 0;
            PlayerData.decisionIndex = 0;

            //hardcoded
            PlayerData.madeChoices = new int[100];
            PlayerData.madeDecisions = new int[100];

            for (int i = 0; i < 100; i++)
            {
                PlayerData.madeChoices[i] = -1;
                PlayerData.madeDecisions[i] = -1;
            }

            SaveData();
        }
    }

    public static void SaveData()
    {
        File.WriteAllText(Application.dataPath + "/playerData.json", JsonUtility.ToJson(PlayerData, true));
    }

    public static Chapter GetCurrentChapter()
    {
        string chaptersDataPath = Application.dataPath + "/chapters.json";

        if (File.Exists(chaptersDataPath))
        {
            return JsonHelper.FromJson<Chapter>(File.ReadAllText(chaptersDataPath))[PlayerData.chapterIndex];
        }
        else
        {
            Debug.LogError($"File does not exist: {chaptersDataPath}");
            return null;
        }
    }
}
