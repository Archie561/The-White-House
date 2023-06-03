using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public Chapter CurrentChapter { get; private set; }

    public bool IsDialogueLocked { get; private set; }
    public bool IsLawLocked { get; private set; }
    public bool IsDecisionLocked {get; private set; }

    //event that triggers when lock of the mechanic changes in conditionCheck
    public event Action OnLockValueChanged;

    void Awake()
    {
        if (Instance == null)
        {
            SceneManager.sceneLoaded += OnSceneLoaded;
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

    //function that invokes every time scene loaded
    public void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.buildIndex == 0)
        {
            DataManager.LoadData();

            //check for mechanics lock
            DialogueLockCheck();
            LawLockCheck();
            DecisionLockCheck();

            //check for condition
            if (!IsDialogueLocked)
            {
                ConditionCheck(GetNextDialogue().condition, "Dialogue");
            }
            if (!IsDecisionLocked)
            {
                ConditionCheck(GetNextDecision().condition, "Decision");
            }
        }
    }

    private void DialogueLockCheck()
    {
        //if all dialogues in chapter are complete, lock dialogues
        if (DataManager.PlayerData.dialogueID >= CurrentChapter.dialogues.Length)
        {
            IsDialogueLocked = true;
            return;
        }

        Dialogue currentDialogue = GetNextDialogue();
        //if dialogue is locked by decision and decisions are not yet complete
        if (currentDialogue.lockedByDecision != -1 && !(DataManager.PlayerData.decisionID >= CurrentChapter.decisions.Length))
        {
            //if decisionID of current chapter is <= lockID, lock Dialogue
            if (!(GetNextDecision().decisionID > currentDialogue.lockedByDecision))
            {
                IsDialogueLocked = true;
                return;
            }
        }
        //if dialogue is locked by law and laws are not yet complete
        if (currentDialogue.lockedByLaw != -1 && !(DataManager.PlayerData.lawID >= CurrentChapter.laws.Length))
        {
            //if lawID of current chapter is <= lockID, lock Dialogue
            if (!(GetNextLaw().lawID > currentDialogue.lockedByLaw))
            {
                IsDialogueLocked = true;
                return;
            }
        }
        
        IsDialogueLocked = false;
    }

    //needs to be public to access from LawManager
    public void LawLockCheck()
    {
        //if all laws in chapter are complete, lock laws
        if (DataManager.PlayerData.lawID >= CurrentChapter.laws.Length)
        {
            IsLawLocked = true;
            return;
        }

        Law currentLaw = GetNextLaw();
        //if law is locked by decision and decisions are not yet complete
        if (currentLaw.lockedByDecision != -1 && !(DataManager.PlayerData.decisionID >= CurrentChapter.decisions.Length))
        {
            //if decisionID of current chapter is <= lockID, lock Law
            if (!(GetNextDecision().decisionID > currentLaw.lockedByDecision))
            {
                IsLawLocked = true;
                return;
            }
        }
        //if law is locked by dialogue and dialogues are not yet complete
        if (currentLaw.lockedByDialogue != -1 && !(DataManager.PlayerData.dialogueID >= CurrentChapter.dialogues.Length))
        {
            //if dialogueID of current chapter is <= lockID, lock Law
            if (!(GetNextDialogue().dialogueID > currentLaw.lockedByDialogue))
            {
                IsLawLocked = true;
                return;
            }
        }

        IsLawLocked = false;
    }

    private void DecisionLockCheck()
    {
        //if all decisions in chapter are complete, lock decisions
        if (DataManager.PlayerData.decisionID >= CurrentChapter.decisions.Length)
        {
            IsDecisionLocked = true;
            return;
        }

        Decision currentDecision = GetNextDecision();
        //if decision is locked by dialogue and dialogues are not yet complete
        if (currentDecision.lockedByDialogue != -1 && !(DataManager.PlayerData.dialogueID >= CurrentChapter.dialogues.Length))
        {
            //if dialogueID of current chapter is <= lockID, lock Decision
            if (!(GetNextDialogue().dialogueID > currentDecision.lockedByDialogue))
            {
                IsDecisionLocked = true;
                return;
            }
        }
        //if decision is locked by law and laws are not yet complete
        if (currentDecision.lockedByLaw != -1 && !(DataManager.PlayerData.lawID >= CurrentChapter.laws.Length))
        {
            //if lawID of current chapter is <= lockID, lock Decision
            if (!(GetNextLaw().lawID > currentDecision.lockedByLaw))
            {
                IsDecisionLocked = true;
                return;
            }
        }

        IsDecisionLocked = false;
    }

    private void ConditionCheck(Condition condition, string mechanicName)
    {
        //if condition is not used, return
        if (!condition.isUsed)
        {
            return;
        }

        bool isConditionMet = true;

        //if condition is in characteristic value
        if (!string.IsNullOrEmpty(condition.characteristicName))
        {
            //checking for condition met: false if player characteristic is less then condition characteristic
            switch (condition.characteristicName)
            {
                case "army":
                    isConditionMet = DataManager.PlayerData.characteristics.army >= condition.characteristicValue;
                    break;
                case "economy":
                    isConditionMet = DataManager.PlayerData.characteristics.economy >= condition.characteristicValue;
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

        //if condition is not met, skip mechanic depending on its name
        if (!isConditionMet)
        {
            if (mechanicName == "Dialogue")
            {
                DataManager.PlayerData.dialogueID++;
            }
            else if (mechanicName == "Decision")
            {
                DataManager.PlayerData.decisionID++;
            }

            DataManager.SaveData();
            if (OnLockValueChanged != null)
            {
                OnLockValueChanged();
            }
        }
    }

    public void LoadMainScene()
    {
        SceneManager.LoadScene(0);
    }

    public void UpdateCharacteristics(Characteristics characteristics)
    {
        DataManager.PlayerData.characteristics.army += characteristics.army;
        DataManager.PlayerData.characteristics.economy += characteristics.economy;
    }

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

    public Choice GetChoice(sbyte ID)
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
}
