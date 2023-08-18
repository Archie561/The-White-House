using System;
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

            PlayerData.chapterID = 0;
            PlayerData.dialogueID = 0;
            PlayerData.lawID = 0;
            PlayerData.decisionID = 0;

            Characteristics characteristics = new Characteristics();
            characteristics.budget = 500000;
            characteristics.navy = 50;
            characteristics.airForces = 50;
            characteristics.infantry = 50;
            characteristics.machinery = 50;
            characteristics.europeanUnion = 50;
            characteristics.china = 50;
            characteristics.africa = 50;
            characteristics.unitedKingdom = 50;
            characteristics.CIS = 50;
            characteristics.OPEC = 50;
            characteristics.science = 50;
            characteristics.welfare = 50;
            characteristics.education = 50;
            characteristics.medicine = 50;
            characteristics.ecology = 50;
            characteristics.infrastructure = 50;

            PlayerData.characteristics = characteristics;

            //hardcoded
            MadeAction[] madeActions = new MadeAction[10];

            for (int i = 0; i < madeActions.Length; i++)
            {
                madeActions[i] = new MadeAction();
                madeActions[i].value = new int[100];

                for (int j = 0; j < madeActions[i].value.Length; j++)
                {
                    madeActions[i].value[j] = -1;
                }
            }

            PlayerData.madeDecisions = madeActions;
            PlayerData.madeChoices = madeActions;

            SaveData();
        }
    }

    public static void SaveData()
    {
        File.WriteAllText(Application.dataPath + "/playerData.json", JsonUtility.ToJson(PlayerData, true));
    }

    public static Chapter GetCurrentChapter()
    {
        //!!! Chapters file path
        string chaptersDataPath = Application.dataPath + "/chaptersTest.json";

        if (File.Exists(chaptersDataPath))
        {
            try
            {
                return JsonHelper.FromJson<Chapter>(File.ReadAllText(chaptersDataPath))[PlayerData.chapterID];
            }
            catch (Exception e)
            {
                Debug.LogError(e.Message + $" Something went wrong! Maybe chapter object with ID {PlayerData.chapterID} does not exist?");
                return null;
            }
        }
        else
        {
            Debug.LogError($"File does not exist: {chaptersDataPath}");
            return null;
        }
    }
}
