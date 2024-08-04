using System;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    //додати знаки оклику до характеристик
    //додати систему звуків
    //оновити бюджет бокс

    public static GameManager Instance { get; private set; }

    public Chapter CurrentChapter { get; private set; }

    void Awake()
    {
        if (Instance == null)
        {
            DataManager.LoadData();
            CurrentChapter = DataManager.GetCurrentChapter();

            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void LoadMainScene()
    {
        SceneManager.LoadScene(1);
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

    public Cutscene[] GetCutscenes()
    {
        try
        {
            return CurrentChapter.cutscenes;
        }
        catch ( Exception e )
        {
            Debug.LogError(e.Message + $" Cutscenes array does not exist!");
            return null;
        }
    }

    /*==============================================================================================================*/
    /*==========================================END OF DATA FETCH SECTION===========================================*/
    /*==============================================================================================================*/

    //-------------------------------------------------------------------------------------------------------------------------------------------------------------//

    /*==============================================================================================================*/
    /*============================================MECHANIC CHECK SECTION============================================*/
    /*==============================================================================================================*/

    public bool IsDialogueLocked()
    {
        //якщо всі діалоги в місяці виконані - заблокувати діалоги
        if (DataManager.PlayerData.dialogueID >= GameManager.Instance.CurrentChapter.dialogues.Length)
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
        if (DataManager.PlayerData.lawID >= GameManager.Instance.CurrentChapter.laws.Length)
        {
            return true;
        }

        Law currentLaw = GameManager.Instance.GetNextLaw();
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
        if (DataManager.PlayerData.decisionID >= GameManager.Instance.CurrentChapter.decisions.Length)
        {
            return true;
        }

        Decision currentDecision = GameManager.Instance.GetNextDecision();
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

    /*==============================================================================================================*/
    /*========================================END OF MECHANIC CHECK SECTION=========================================*/
    /*==============================================================================================================*/

    //-------------------------------------------------------------------------------------------------------------------------------------------------------------//
}
