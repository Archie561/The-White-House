using System.IO;
using UnityEngine;

public class PlayerDataManager
{
    public string ActivePresident { get; private set; }
    public int ChaptersAmount { get; private set; }

    private PlayerData _playerData;

    public PlayerDataManager(string presidentName, int chaptersAmount)
    {
        ActivePresident = presidentName;
        ChaptersAmount = chaptersAmount;

        string playerDataPath = Application.persistentDataPath + $"/{presidentName}PlayerData.json";
        if (File.Exists(playerDataPath)) _playerData = JsonUtility.FromJson<PlayerData>(File.ReadAllText(playerDataPath));
        else _playerData = GenerateNewPlayerData();
    }

    public int ChapterID => _playerData.chapterID;
    public void UpdateChapterID(int ID) => _playerData.chapterID = ID;

    public int DialogueID => _playerData.dialogueID;
    public void UpdateDialogueID(int ID) => _playerData.dialogueID = ID;

    public int LawID => _playerData.lawID;
    public void UpdateLawID(int ID) => _playerData.lawID = ID;

    public int DecisionID => _playerData.decisionID;
    public void UpdateDecisionID(int ID) => _playerData.decisionID = ID;

    public bool StartingCutscenesShown => _playerData.startingCutscenesShown;
    public void UpdateStartingCutscenesShown(bool isShown) => _playerData.startingCutscenesShown = isShown;

    public bool GameOver => _playerData.gameOver;
    public void UpdateGameOver(bool isOver) => _playerData.gameOver = isOver;

    public Characteristics Characteristics => _playerData.characteristics;
    public void UpdateCharacteristics(Characteristics characteristicsToAdd)
    {
        _playerData.characteristics.budget += characteristicsToAdd.budget;
        _playerData.characteristics.navy = Mathf.Clamp(_playerData.characteristics.navy + characteristicsToAdd.navy, 0, 100);
        _playerData.characteristics.airForces = Mathf.Clamp(_playerData.characteristics.airForces + characteristicsToAdd.airForces, 0, 100);
        _playerData.characteristics.infantry = Mathf.Clamp(_playerData.characteristics.infantry + characteristicsToAdd.infantry, 0, 100);
        _playerData.characteristics.machinery = Mathf.Clamp(_playerData.characteristics.machinery + characteristicsToAdd.machinery, 0, 100);
        _playerData.characteristics.europeanUnion = Mathf.Clamp(_playerData.characteristics.europeanUnion + characteristicsToAdd.europeanUnion, 0, 100);
        _playerData.characteristics.china = Mathf.Clamp(_playerData.characteristics.china + characteristicsToAdd.china, 0, 100);
        _playerData.characteristics.africa = Mathf.Clamp(_playerData.characteristics.africa + characteristicsToAdd.africa, 0, 100);
        _playerData.characteristics.unitedKingdom = Mathf.Clamp(_playerData.characteristics.unitedKingdom + characteristicsToAdd.unitedKingdom, 0, 100);
        _playerData.characteristics.CIS = Mathf.Clamp(_playerData.characteristics.CIS + characteristicsToAdd.CIS, 0, 100);
        _playerData.characteristics.OPEC = Mathf.Clamp(_playerData.characteristics.OPEC + characteristicsToAdd.OPEC, 0, 100);
        _playerData.characteristics.science = Mathf.Clamp(_playerData.characteristics.science + characteristicsToAdd.science, 0, 100);
        _playerData.characteristics.welfare = Mathf.Clamp(_playerData.characteristics.welfare + characteristicsToAdd.welfare, 0, 100);
        _playerData.characteristics.education = Mathf.Clamp(_playerData.characteristics.education + characteristicsToAdd.education, 0, 100);
        _playerData.characteristics.medicine = Mathf.Clamp(_playerData.characteristics.medicine + characteristicsToAdd.medicine, 0, 100);
        _playerData.characteristics.ecology = Mathf.Clamp(_playerData.characteristics.ecology + characteristicsToAdd.ecology, 0, 100);
        _playerData.characteristics.infrastructure = Mathf.Clamp(_playerData.characteristics.infrastructure + characteristicsToAdd.infrastructure, 0, 100);
    }

    public void SaveChoice(int ID, int value) => _playerData.madeChoices[_playerData.chapterID].value[ID] = value;

    public void SaveDecision(int ID, int value) => _playerData.madeDecisions[_playerData.chapterID].value[ID] = value;

    //public void SaveLaw(int ID, int value) => _playerData.madeLaws[_playerData.chapterID].value[ID] = value;

    public bool IsConditionMet(Condition condition)
    {
        if (!condition.isUsed) return true;

        bool isConditionMet = true;

        //if condition is in characteristic value
        if (!string.IsNullOrEmpty(condition.characteristicName))
        {
            //checking for condition met: false if player characteristic is more then condition characteristic
            switch (condition.characteristicName)
            {
                case "navy":
                    isConditionMet = _playerData.characteristics.navy <= condition.characteristicValue;
                    break;
                case "airForces":
                    isConditionMet = _playerData.characteristics.airForces <= condition.characteristicValue;
                    break;
                case "infantry":
                    isConditionMet = _playerData.characteristics.infantry <= condition.characteristicValue;
                    break;
                case "machinery":
                    isConditionMet = _playerData.characteristics.machinery <= condition.characteristicValue;
                    break;
                case "europeanUnion":
                    isConditionMet = _playerData.characteristics.europeanUnion <= condition.characteristicValue;
                    break;
                case "china":
                    isConditionMet = _playerData.characteristics.china <= condition.characteristicValue;
                    break;
                case "africa":
                    isConditionMet = _playerData.characteristics.africa <= condition.characteristicValue;
                    break;
                case "unitedKingdom":
                    isConditionMet = _playerData.characteristics.unitedKingdom <= condition.characteristicValue;
                    break;
                case "CIS":
                    isConditionMet = _playerData.characteristics.CIS <= condition.characteristicValue;
                    break;
                case "OPEC":
                    isConditionMet = _playerData.characteristics.OPEC <= condition.characteristicValue;
                    break;
                case "science":
                    isConditionMet = _playerData.characteristics.science <= condition.characteristicValue;
                    break;
                case "welfare":
                    isConditionMet = _playerData.characteristics.welfare <= condition.characteristicValue;
                    break;
                case "education":
                    isConditionMet = _playerData.characteristics.education <= condition.characteristicValue;
                    break;
                case "medicine":
                    isConditionMet = _playerData.characteristics.medicine <= condition.characteristicValue;
                    break;
                case "ecology":
                    isConditionMet = _playerData.characteristics.ecology <= condition.characteristicValue;
                    break;
                case "infrastructure":
                    isConditionMet = _playerData.characteristics.infrastructure <= condition.characteristicValue;
                    break;
                default:
                    isConditionMet = true;
                    break;
            }
        }

        //if condition is in made choice
        if (condition.choiceChapterID != -1 && isConditionMet)
        {
            //checking for condition met: false if player choice is not equal to condition choice
            isConditionMet = _playerData.madeChoices[condition.choiceChapterID].value[condition.choiceID] == condition.choiceValue;
        }

        //if condition is in made decision
        if (condition.decisionChapterID != -1 && isConditionMet)
        {
            //checking for condition met: false if player decision is not equal to condition decision
            isConditionMet = _playerData.madeDecisions[condition.decisionChapterID].value[condition.decisionID] == condition.decisionValue;
        }

        return isConditionMet;
    }

    public void SaveData()
    {
        File.WriteAllText(Application.persistentDataPath + "/" + ActivePresident + "PlayerData.json", JsonUtility.ToJson(_playerData, true));
        PlayerPrefs.SetFloat(ActivePresident, _playerData.chapterID / (float)ChaptersAmount);

        if (_playerData.chapterID == 0)
        {
            PlayerPrefs.SetFloat(ActivePresident, 0.04f);
        }
    }

    private PlayerData GenerateNewPlayerData()
    {
        PlayerData playerData = new PlayerData();

        playerData.chapterID = 0;
        playerData.dialogueID = 0;
        playerData.lawID = 0;
        playerData.decisionID = 0;

        playerData.startingCutscenesShown = false;
        playerData.gameOver = false;

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

        playerData.characteristics = characteristics;

        MadeAction[] madeActions = new MadeAction[ChaptersAmount];

        for (int i = 0; i < madeActions.Length; i++)
        {
            madeActions[i] = new MadeAction();
            madeActions[i].value = new int[100];

            for (int j = 0; j < madeActions[i].value.Length; j++) madeActions[i].value[j] = -1;
        }

        playerData.madeDecisions = madeActions;
        playerData.madeChoices = madeActions;

        return playerData;
    }
}