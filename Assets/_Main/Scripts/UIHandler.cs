using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class UIHandler : MonoBehaviour
{
    [SerializeField]
    private GameObject _dialoguesMark;
    [SerializeField]
    private GameObject _lawsMark;
    [SerializeField]
    private GameObject _decisionsMark;

    [SerializeField]
    private TextMeshProUGUI _budgetText;

    [SerializeField]
    private GameObject _decisionManager;
    [SerializeField]
    private GameObject _lawModal;
    [SerializeField]
    private GameObject _newsModal;

    [SerializeField]
    private CanvasGroup _blackBackground;
    [SerializeField]
    private GameObject _pausePanel;

    [SerializeField]
    private GameObject _newsManager;
    [SerializeField]
    private GameObject _lawManager;
    [SerializeField]
    private GameObject _characteristicsManager;

    //set exclamation marks to buttons if mechanic is not locked
    public void SetMechanicMarks()
    {
        _dialoguesMark.SetActive(!GameManager.Instance.IsDialogueLocked());
        _lawsMark.SetActive(!GameManager.Instance.IsLawLocked());
        _decisionsMark.SetActive(!GameManager.Instance.IsDecisionLocked());

        //!!!
        UpdateBudget();
    }

    private void UpdateBudget()
    {
        string budget = Math.Abs(DataManager.PlayerData.characteristics.budget).ToString();
        int budgetLength = budget.Length;

        if (budget == "0")
        {
            _budgetText.text = "0 $";
            return;
        }

        if (budget.Length > 6)
        {
            _budgetText.text = (DataManager.PlayerData.characteristics.budget < 0 ? "-" : "") + "999,999,999,999+ $";
            return;
        }

        for (int i = budgetLength - 1; i >= 0; i--)
        {
            if ((budgetLength - i) % 3 == 0 && i != 0)
            {
                budget = budget.Insert(i, ",");
            }
        }

        _budgetText.text = (DataManager.PlayerData.characteristics.budget < 0 ? "-" : "") + budget + ",000,000 $";
    }

    public void DisplayDialogue()
    {
        if (!GameManager.Instance.IsDialogueLocked())
        {
            SceneManager.LoadScene(2);
        }
        else
        {
            Debug.Log("Dialogues are locked!");
        }
    }

    public void DisplayLaws()
    {
        if (!GameManager.Instance.IsLawLocked())
        {
            _lawManager.SetActive(true);
        }
        else
        {
            Debug.Log("Laws are locked!");
        }
    }

    public void DisplayDecision()
    {
        if (!GameManager.Instance.IsDecisionLocked())
        {
            _decisionManager.SetActive(true);
        }
        else
        {
            Debug.Log("Decisions are locked!");
        }
    }

    public void DisplayCharacteristics()
    {
        _characteristicsManager.SetActive(true);
    }

    public void PauseClickHandler()
    {
        _newsManager.SetActive(true);
    }
}
