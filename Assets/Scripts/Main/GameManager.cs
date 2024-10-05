using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public Chapter CurrentChapter { get; private set; }

    public bool IsGameOver;
    public bool IsChapterFinished;

    public bool shouldDisplayChapterTitle;
    public bool shouldDisplayNews;    
    public bool shouldDisplayStartingCutscenes;

    public bool budgetFailure;
    public bool domesticFailure;
    public bool foreignFailure;
    public bool armyFailure;

    private void Awake()
    {
        if (Instance == null)
        {
            DataManager.LoadPlayerData();
            CurrentChapter = DataManager.GetCurrentChapter();

            shouldDisplayChapterTitle = true;
            shouldDisplayNews = DataManager.PlayerData.dialogueID == 0 && DataManager.PlayerData.decisionID == 0 && DataManager.PlayerData.lawID == 0;
            shouldDisplayStartingCutscenes = shouldDisplayNews && CurrentChapter.startingCutscenes.Length > 0;

            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    /*==============================================================================================================*/
    /*=============================================DATA FETCH SECTION===============================================*/
    /*==============================================================================================================*/

    public Dialogue GetNextDialogue()
    {
        try
        {
            return CurrentChapter.dialogues[DataManager.PlayerData.dialogueID];
        }
        catch (Exception e)
        {
            Debug.LogError(e.Message + $" Dialogue object with ID {DataManager.PlayerData.dialogueID} does not exist!");
            return null;
        }
    }

    public Choice GetChoice(int ID)
    {
        try
        {
            return CurrentChapter.choices[ID];
        }
        catch (Exception e)
        {
            Debug.LogError(e.Message + $" Choise object with ID {ID} does not exist!");
            return null;
        }
    }

    public Law GetNextLaw()
    {
        try
        {
            return CurrentChapter.laws[DataManager.PlayerData.lawID];
        }
        catch (Exception e)
        {
            Debug.LogError(e.Message + $" Law object with ID {DataManager.PlayerData.lawID} does not exist!");
            return null;
        }
    }

    public Decision GetNextDecision()
    {
        try
        {
            return CurrentChapter.decisions[DataManager.PlayerData.decisionID];
        }
        catch (Exception e)
        {
            Debug.LogError(e.Message + $" Decision object with ID {DataManager.PlayerData.decisionID} does not exist!");
            return null;
        }
    }

    public News[] GetNews()
    {
        try
        {
            return CurrentChapter.news;
        }
        catch ( Exception e )
        {
            Debug.LogError(e.Message + $" News array does not exist!");
            return null;
        }
    }

    public Cutscene[] GetStartingCutscenes()
    {
        try
        {
            return CurrentChapter.startingCutscenes;
        }
        catch ( Exception e )
        {
            Debug.LogError(e.Message + $" Starting cutscenes array does not exist!");
            return null;
        }
    }

    public Cutscene[] GetEndingCutscenes()
    {
        try
        {
            return CurrentChapter.endingCutscenes;
        }
        catch (Exception e)
        {
            Debug.LogError(e.Message + $" Ending cutscenes array does not exist!");
            return null;
        }
    }

    public void LoadNextChapter()
    {
        DataManager.PlayerData.dialogueID = 0;
        DataManager.PlayerData.lawID = 0;
        DataManager.PlayerData.decisionID = 0;
        DataManager.PlayerData.chapterID++;
        DataManager.SaveData();

        AudioManager.Instance.StopPlaying();
        GameOverCheck();

        if (CurrentChapter.endingCutscenes.Length > 0 || IsGameOver)
        {
            SceneManager.LoadScene(3);
            return;
        }

        SceneManager.LoadScene(1);
        Destroy(gameObject);
    }

    /*==============================================================================================================*/
    /*==========================================END OF DATA FETCH SECTION===========================================*/
    /*==============================================================================================================*/

    //-------------------------------------------------------------------------------------------------------------------------------------------------------------//

    /*==============================================================================================================*/
    /*===========================================MECHANIC ANALYSIS SECTION==========================================*/
    /*==============================================================================================================*/

    public bool IsDialogueLocked()
    {
        //якщо всі діалоги в місяці виконані - заблокувати діалоги
        if (DataManager.PlayerData.dialogueID >= CurrentChapter.dialogues.Length)
        {
            return true;
        }

        Dialogue currentDialogue = GetNextDialogue();
        PlayerData playerData = DataManager.PlayerData;

        //якщо id останнього зробленого рішення менше за id блокуючого рішення - заблокувати діалоги
        if (playerData.decisionID <= currentDialogue.lockedByDecision)
        {
            return true;
        }
        //якщо id останнього зробленого закону менше за id блокуючого закону - заблокувати діалоги
        if (playerData.lawID <= currentDialogue.lockedByLaw)
        {
            return true;
        }

        return false;
    }

    public bool IsLawLocked()
    {
        if (DataManager.PlayerData.lawID >= CurrentChapter.laws.Length)
        {
            return true;
        }

        Law currentLaw = GetNextLaw();
        PlayerData playerData = DataManager.PlayerData;

        if (playerData.decisionID <= currentLaw.lockedByDecision)
        {
            return true;
        }

        if (playerData.dialogueID <= currentLaw.lockedByDialogue)
        {
            return true;
        }

        return false;
    }

    public bool IsDecisionLocked()
    {
        if (DataManager.PlayerData.decisionID >= CurrentChapter.decisions.Length)
        {
            return true;
        }

        Decision currentDecision = GetNextDecision();
        PlayerData playerData = DataManager.PlayerData;

        if (playerData.lawID <= currentDecision.lockedByLaw)
        {
            return true;
        }

        if (playerData.dialogueID <= currentDecision.lockedByDialogue)
        {
            return true;
        }

        return false;
    }

    public bool IsConditionMet(Condition condition)
    {
        //if condition is not used, return true
        if (!condition.isUsed)
        {
            return true;
        }

        bool isConditionMet = true;

        //if condition is in characteristic value
        if (!string.IsNullOrEmpty(condition.characteristicName))
        {
            //checking for condition met: false if player characteristic is less then condition characteristic
            switch (condition.characteristicName)
            {
                case "navy":
                    isConditionMet = DataManager.PlayerData.characteristics.navy >= condition.characteristicValue;
                    break;
                case "airForces":
                    isConditionMet = DataManager.PlayerData.characteristics.airForces >= condition.characteristicValue;
                    break;
                case "infantry":
                    isConditionMet = DataManager.PlayerData.characteristics.infantry >= condition.characteristicValue;
                    break;
                case "machinery":
                    isConditionMet = DataManager.PlayerData.characteristics.machinery >= condition.characteristicValue;
                    break;
                case "europeanUnion":
                    isConditionMet = DataManager.PlayerData.characteristics.europeanUnion >= condition.characteristicValue;
                    break;
                case "china":
                    isConditionMet = DataManager.PlayerData.characteristics.china >= condition.characteristicValue;
                    break;
                case "africa":
                    isConditionMet = DataManager.PlayerData.characteristics.africa >= condition.characteristicValue;
                    break;
                case "unitedKingdom":
                    isConditionMet = DataManager.PlayerData.characteristics.unitedKingdom >= condition.characteristicValue;
                    break;
                case "CIS":
                    isConditionMet = DataManager.PlayerData.characteristics.CIS >= condition.characteristicValue;
                    break;
                case "OPEC":
                    isConditionMet = DataManager.PlayerData.characteristics.OPEC >= condition.characteristicValue;
                    break;
                case "science":
                    isConditionMet = DataManager.PlayerData.characteristics.science >= condition.characteristicValue;
                    break;
                case "welfare":
                    isConditionMet = DataManager.PlayerData.characteristics.welfare >= condition.characteristicValue;
                    break;
                case "education":
                    isConditionMet = DataManager.PlayerData.characteristics.education >= condition.characteristicValue;
                    break;
                case "medicine":
                    isConditionMet = DataManager.PlayerData.characteristics.medicine >= condition.characteristicValue;
                    break;
                case "ecology":
                    isConditionMet = DataManager.PlayerData.characteristics.ecology >= condition.characteristicValue;
                    break;
                case "infrastructure":
                    isConditionMet = DataManager.PlayerData.characteristics.infrastructure >= condition.characteristicValue;
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
            if (DataManager.PlayerData.madeChoices[condition.choiceChapterID].value[condition.choiceID] != condition.choiceValue)
            {
                isConditionMet = false;
            }
        }

        //if condition is in made decision
        if (condition.decisionChapterID != -1 && isConditionMet)
        {
            //checking for condition met: false if player decision is not equal to condition decision
            if (DataManager.PlayerData.madeDecisions[condition.decisionChapterID].value[condition.decisionID] != condition.decisionValue)
            {
                isConditionMet = false;
            }
        }

        return isConditionMet;
    }

    public void UpdateMechanicsStatus()
    {
        //якщо діалоги не заблоковані, але умова для показу блоку діалогу не виконується - пропустити діалог
        if (!IsDialogueLocked() && !IsConditionMet(GetNextDialogue().condition))
        {
            DataManager.PlayerData.dialogueID++;
            DataManager.SaveData();
            UpdateMechanicsStatus();
            return;
        }
        if (!IsDecisionLocked() && !IsConditionMet(GetNextDecision().condition))
        {
            DataManager.PlayerData.decisionID++;
            DataManager.SaveData();
            UpdateMechanicsStatus();
            return;
        }

        IsChapterFinished = IsDecisionLocked() && IsDialogueLocked() && IsLawLocked();
    }

    public void UpdateCharacteristics(Characteristics characteristicsToAdd)
    {
        Characteristics characteristics = DataManager.PlayerData.characteristics;

        characteristics.budget += characteristicsToAdd.budget;
        characteristics.navy = characteristics.navy + characteristicsToAdd.navy >= 100 ? 100 : characteristics.navy + characteristicsToAdd.navy <= 0 ? 0 : characteristics.navy + characteristicsToAdd.navy;
        characteristics.airForces = characteristics.airForces + characteristicsToAdd.airForces >= 100 ? 100 : characteristics.airForces + characteristicsToAdd.airForces <= 0 ? 0 : characteristics.airForces + characteristicsToAdd.airForces;
        characteristics.infantry = characteristics.infantry + characteristicsToAdd.infantry >= 100 ? 100 : characteristics.infantry + characteristicsToAdd.infantry <= 0 ? 0 : characteristics.infantry + characteristicsToAdd.infantry;
        characteristics.machinery = characteristics.machinery + characteristicsToAdd.machinery >= 100 ? 100 : characteristics.machinery + characteristicsToAdd.machinery <= 0 ? 0 : characteristics.machinery + characteristicsToAdd.machinery;
        characteristics.europeanUnion = characteristics.europeanUnion + characteristicsToAdd.europeanUnion >= 100 ? 100 : characteristics.europeanUnion + characteristicsToAdd.europeanUnion <= 0 ? 0 : characteristics.europeanUnion + characteristicsToAdd.europeanUnion;
        characteristics.china = characteristics.china + characteristicsToAdd.china >= 100 ? 100 : characteristics.china + characteristicsToAdd.china <= 0 ? 0 : characteristics.china + characteristicsToAdd.china;
        characteristics.africa = characteristics.africa + characteristicsToAdd.africa >= 100 ? 100 : characteristics.africa + characteristicsToAdd.africa <= 0 ? 0 : characteristics.africa + characteristicsToAdd.africa;
        characteristics.unitedKingdom = characteristics.unitedKingdom + characteristicsToAdd.unitedKingdom >= 100 ? 100 : characteristics.unitedKingdom + characteristicsToAdd.unitedKingdom <= 0 ? 0 : characteristics.unitedKingdom + characteristicsToAdd.unitedKingdom;
        characteristics.CIS = characteristics.CIS + characteristicsToAdd.CIS >= 100 ? 100 : characteristics.CIS + characteristicsToAdd.CIS <= 0 ? 0 : characteristics.CIS + characteristicsToAdd.CIS;
        characteristics.OPEC = characteristics.OPEC + characteristicsToAdd.OPEC >= 100 ? 100 : characteristics.OPEC + characteristicsToAdd.OPEC <= 0 ? 0 : characteristics.OPEC + characteristicsToAdd.OPEC;
        characteristics.science = characteristics.science + characteristicsToAdd.science >= 100 ? 100 : characteristics.science + characteristicsToAdd.science <= 0 ? 0 : characteristics.science + characteristicsToAdd.science;
        characteristics.welfare = characteristics.welfare + characteristicsToAdd.welfare >= 100 ? 100 : characteristics.welfare + characteristicsToAdd.welfare <= 0 ? 0 : characteristics.welfare + characteristicsToAdd.welfare;
        characteristics.education = characteristics.education + characteristicsToAdd.education >= 100 ? 100 : characteristics.education + characteristicsToAdd.education <= 0 ? 0 : characteristics.education + characteristicsToAdd.education;
        characteristics.medicine = characteristics.medicine + characteristicsToAdd.medicine >= 100 ? 100 : characteristics.medicine + characteristicsToAdd.medicine <= 0 ? 0 : characteristics.medicine + characteristicsToAdd.medicine;
        characteristics.ecology = characteristics.ecology + characteristicsToAdd.ecology >= 100 ? 100 : characteristics.ecology + characteristicsToAdd.ecology <= 0 ? 0 : characteristics.ecology + characteristicsToAdd.ecology;
        characteristics.infrastructure = characteristics.infrastructure + characteristicsToAdd.infrastructure >= 100 ? 100 : characteristics.infrastructure + characteristicsToAdd.infrastructure <= 0 ? 0 : characteristics.infrastructure + characteristicsToAdd.infrastructure;

        DataManager.PlayerData.characteristics = characteristics;
    }

    private void GameOverCheck()
    {
        int criticalValue = 10;
        Characteristics characteristics = DataManager.PlayerData.characteristics;

        budgetFailure = characteristics.budget < 0;

        domesticFailure = (characteristics.science + characteristics.medicine + characteristics.welfare + characteristics.ecology +
            characteristics.education + characteristics.infrastructure) / 6 < criticalValue;

        foreignFailure = (characteristics.europeanUnion + characteristics.unitedKingdom + characteristics.china + characteristics.CIS +
            characteristics.africa + characteristics.OPEC) / 6 < criticalValue;

        armyFailure = (characteristics.infantry + characteristics.airForces + characteristics.machinery + characteristics.navy) / 4 < criticalValue;

        IsGameOver = budgetFailure || domesticFailure || foreignFailure || armyFailure || DataManager.PlayerData.chapterID >= DataManager.ChaptersAmount;
    }

    /*==============================================================================================================*/
    /*=======================================END OF MECHANIC ANALYSIS SECTION=======================================*/
    /*==============================================================================================================*/

    //-------------------------------------------------------------------------------------------------------------------------------------------------------------//
}
