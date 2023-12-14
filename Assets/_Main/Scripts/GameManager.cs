using System;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    //додати знаки оклику до характеристик
    //додати систему звуків
    //додати катсцени

    public static GameManager Instance { get; private set; }

    public Chapter CurrentChapter { get; private set; }

    public bool IsDialogueLocked { get; private set; }
    public bool IsLawLocked { get; private set; }
    public bool IsDecisionLocked {get; private set; }
    public bool IsNewsShown { get; set; }

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
        if (scene.buildIndex == 1)
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

        //initilizeBudget if it is not main menu scene
        if (scene.buildIndex != 0)
        {
            UpdateBudget();
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

        //if condition is not met, skip mechanic depending on its name
        if (!isConditionMet)
        {
            if (mechanicName == "Dialogue")
            {
                DataManager.PlayerData.dialogueID++;
                DialogueLockCheck();
            }
            else if (mechanicName == "Decision")
            {
                DataManager.PlayerData.decisionID++;
                DecisionLockCheck();
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
        SceneManager.LoadScene(1);
    }

    public void UpdateCharacteristics(Characteristics characteristics)
    {
        DataManager.PlayerData.characteristics.budget += characteristics.budget;
        UpdateBudget();

        DataManager.PlayerData.characteristics.navy += characteristics.navy;
        DataManager.PlayerData.characteristics.airForces += characteristics.airForces;
        DataManager.PlayerData.characteristics.infantry += characteristics.infantry;
        DataManager.PlayerData.characteristics.machinery += characteristics.machinery;
        DataManager.PlayerData.characteristics.europeanUnion += characteristics.europeanUnion;
        DataManager.PlayerData.characteristics.china += characteristics.china;
        DataManager.PlayerData.characteristics.africa += characteristics.africa;
        DataManager.PlayerData.characteristics.unitedKingdom += characteristics.unitedKingdom;
        DataManager.PlayerData.characteristics.CIS += characteristics.CIS;
        DataManager.PlayerData.characteristics.OPEC += characteristics.OPEC;
        DataManager.PlayerData.characteristics.science += characteristics.science;
        DataManager.PlayerData.characteristics.welfare += characteristics.welfare;
        DataManager.PlayerData.characteristics.education += characteristics.education;
        DataManager.PlayerData.characteristics.medicine += characteristics.medicine;
        DataManager.PlayerData.characteristics.ecology += characteristics.ecology;
        DataManager.PlayerData.characteristics.infrastructure += characteristics.infrastructure;
    }

    private void UpdateBudget()
    {
        string budget = Math.Abs(DataManager.PlayerData.characteristics.budget).ToString();
        int budgetLength = budget.Length;

        if (budget == "0")
        {
            GameObject.Find("BudgetBox").GetComponentInChildren<TextMeshProUGUI>().text = "0 $";
            return;
        }

        if (budget.Length > 6)
        {
            GameObject.Find("BudgetBox").GetComponentInChildren<TextMeshProUGUI>().text = (DataManager.PlayerData.characteristics.budget < 0 ? "-" : "") + "999,999,999,999+ $";
            return;
        }

        for (int i = budgetLength - 1; i >= 0; i--)
        {
            if ((budgetLength - i) % 3 == 0 && i != 0)
            {
                budget = budget.Insert(i, ",");
            }
        }

        //!!!HARDCODED
        if (SceneManager.GetActiveScene().buildIndex != 3)
            GameObject.Find("BudgetBox").GetComponentInChildren<TextMeshProUGUI>().text = (DataManager.PlayerData.characteristics.budget < 0 ? "-" : "") + budget + ",000,000 $";
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
}
