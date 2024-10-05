using System;
using System.IO;
using UnityEngine;
using UnityEngine.Playables;

public static class DataManager
{
    public static PlayerData PlayerData { get; private set; }

    public static string CurrentSelectedPresident { get; set; }

    public static int ChaptersAmount = 2;

    public static void LoadPlayerData()
    {
        string playerDataPath = System.IO.Directory.GetCurrentDirectory() + "/Assets/Story/" + CurrentSelectedPresident + "/PlayerData.json";

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


            MadeAction[] madeActions = new MadeAction[ChaptersAmount];

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
        File.WriteAllText(Directory.GetCurrentDirectory() + "/Assets/Story/" + CurrentSelectedPresident + "/PlayerData.json", JsonUtility.ToJson(PlayerData, true));
        PlayerPrefs.SetFloat(CurrentSelectedPresident, PlayerData.chapterID / (float)ChaptersAmount);

        if (PlayerData.chapterID == 0)
        {
            PlayerPrefs.SetFloat(CurrentSelectedPresident, 0.04f);
        }
    }

    public static Chapter GetCurrentChapter()
    {
        string chapterDataPath = Directory.GetCurrentDirectory() + "/Assets/Story/" + CurrentSelectedPresident + "/" + PlayerPrefs.GetString("gameLanguage", "en") + "/" + PlayerData.chapterID + ".json";

        if (File.Exists(chapterDataPath))
        {
            try
            {
                return JsonUtility.FromJson<Chapter>(File.ReadAllText(chapterDataPath));
            }
            catch (Exception e)
            {
                Debug.LogError(e.Message + " Something went wrong! Maybe chapter is in incorrect format?");
                return null;
            }
        }
        else
        {
            Debug.LogError($"File does not exist: {chapterDataPath}");
            return null;
        }
    }
}
