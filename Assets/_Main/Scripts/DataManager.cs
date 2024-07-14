using System;
using System.IO;
using UnityEngine;

public static class DataManager
{
    public static PlayerData PlayerData { get; private set; }

    public static void LoadData()
    {
        //string playerDataPath = Application.dataPath + "/playerData.json";
        string playerDataPath = System.IO.Directory.GetCurrentDirectory() + "/playerData.json";

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
        //File.WriteAllText(Application.dataPath + "/playerData.json", JsonUtility.ToJson(PlayerData, true));
        File.WriteAllText(System.IO.Directory.GetCurrentDirectory() + "/playerData.json", JsonUtility.ToJson(PlayerData, true));
    }

    public static Chapter GetCurrentChapter()
    {
        //!!! Chapters file path
        //string chaptersDataPath = Application.dataPath + "/chaptersTest.json";
        string chaptersDataPath = System.IO.Directory.GetCurrentDirectory() + "/chaptersTest.json";

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

    public static void UpdateCharacteristics(Characteristics characteristics)
    {
        PlayerData.characteristics.budget += characteristics.budget;

        PlayerData.characteristics.navy = PlayerData.characteristics.navy + characteristics.navy >= 100 ? 100 : PlayerData.characteristics.navy + characteristics.navy <= 0 ? 0 : PlayerData.characteristics.navy + characteristics.navy;
        PlayerData.characteristics.airForces = PlayerData.characteristics.airForces + characteristics.airForces >= 100 ? 100 : PlayerData.characteristics.airForces + characteristics.airForces <= 0 ? 0 : PlayerData.characteristics.airForces + characteristics.airForces;
        PlayerData.characteristics.infantry = PlayerData.characteristics.infantry + characteristics.infantry >= 100 ? 100 : PlayerData.characteristics.infantry + characteristics.infantry <= 0 ? 0 : PlayerData.characteristics.infantry + characteristics.infantry;
        PlayerData.characteristics.machinery = PlayerData.characteristics.machinery + characteristics.machinery >= 100 ? 100 : PlayerData.characteristics.machinery + characteristics.machinery <= 0 ? 0 : PlayerData.characteristics.machinery + characteristics.machinery;
        PlayerData.characteristics.europeanUnion = PlayerData.characteristics.europeanUnion + characteristics.europeanUnion >= 100 ? 100 : PlayerData.characteristics.europeanUnion + characteristics.europeanUnion <= 0 ? 0 : PlayerData.characteristics.europeanUnion + characteristics.europeanUnion;
        PlayerData.characteristics.china = PlayerData.characteristics.china + characteristics.china >= 100 ? 100 : PlayerData.characteristics.china + characteristics.china <= 0 ? 0 : PlayerData.characteristics.china + characteristics.china;
        PlayerData.characteristics.africa = PlayerData.characteristics.africa + characteristics.africa >= 100 ? 100 : PlayerData.characteristics.africa + characteristics.africa <= 0 ? 0 : PlayerData.characteristics.africa + characteristics.africa;
        PlayerData.characteristics.unitedKingdom = PlayerData.characteristics.unitedKingdom + characteristics.unitedKingdom >= 100 ? 100 : PlayerData.characteristics.unitedKingdom + characteristics.unitedKingdom <= 0 ? 0 : PlayerData.characteristics.unitedKingdom + characteristics.unitedKingdom;
        PlayerData.characteristics.CIS = PlayerData.characteristics.CIS + characteristics.CIS >= 100 ? 100 : PlayerData.characteristics.CIS + characteristics.CIS <= 0 ? 0 : PlayerData.characteristics.CIS + characteristics.CIS;
        PlayerData.characteristics.OPEC = PlayerData.characteristics.OPEC + characteristics.OPEC >= 100 ? 100 : PlayerData.characteristics.OPEC + characteristics.OPEC <= 0 ? 0 : PlayerData.characteristics.OPEC + characteristics.OPEC;
        PlayerData.characteristics.science = PlayerData.characteristics.science + characteristics.science >= 100 ? 100 : PlayerData.characteristics.science + characteristics.science <= 0 ? 0 : PlayerData.characteristics.science + characteristics.science;
        PlayerData.characteristics.welfare = PlayerData.characteristics.welfare + characteristics.welfare >= 100 ? 100 : PlayerData.characteristics.welfare + characteristics.welfare <= 0 ? 0 : PlayerData.characteristics.welfare + characteristics.welfare;
        PlayerData.characteristics.education = PlayerData.characteristics.education + characteristics.education >= 100 ? 100 : PlayerData.characteristics.education + characteristics.education <= 0 ? 0 : PlayerData.characteristics.education + characteristics.education;
        PlayerData.characteristics.medicine = PlayerData.characteristics.medicine + characteristics.medicine >= 100 ? 100 : PlayerData.characteristics.medicine + characteristics.medicine <= 0 ? 0 : PlayerData.characteristics.medicine + characteristics.medicine;
        PlayerData.characteristics.ecology = PlayerData.characteristics.ecology + characteristics.ecology >= 100 ? 100 : PlayerData.characteristics.ecology + characteristics.ecology <= 0 ? 0 : PlayerData.characteristics.ecology + characteristics.ecology;
        PlayerData.characteristics.infrastructure = PlayerData.characteristics.infrastructure + characteristics.infrastructure >= 100 ? 100 : PlayerData.characteristics.infrastructure + characteristics.infrastructure <= 0 ? 0 : PlayerData.characteristics.infrastructure + characteristics.infrastructure;
    }
}
