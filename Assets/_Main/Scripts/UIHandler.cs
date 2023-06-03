using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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
    private GameObject _decisionModal;

    private void Start()
    {
        //update exclamation marks visibility when lock value changes
        GameManager.Instance.OnLockValueChanged += SetMechanicMarks;
        SetMechanicMarks();
    }

    //set exclamation marks to buttons if mechanic is not locked
    private void SetMechanicMarks()
    {
        _dialoguesMark.SetActive(!GameManager.Instance.IsDialogueLocked);
        _lawsMark.SetActive(!GameManager.Instance.IsLawLocked);
        _decisionsMark.SetActive(!GameManager.Instance.IsDecisionLocked);
    }

    public void LoadMeetingsScene()
    {
        if (!GameManager.Instance.IsDialogueLocked)
        {
            SceneManager.LoadScene(1);
        }
        else
        {
            Debug.Log("Dialogues are locked!");
        }
    }

    public void LoadLawsScene()
    {
        if (!GameManager.Instance.IsLawLocked)
        {
            SceneManager.LoadScene(2);
        }
        else
        {
            Debug.Log("Laws are locked!");
        }
    }

    public void DisplayDecisionModal()
    {
        if (!GameManager.Instance.IsDecisionLocked)
        {
            _decisionModal.SetActive(true);
        }
        else
        {
            Debug.Log("Decisions are locked!");
        }
    }

    private void OnDestroy()
    {
        GameManager.Instance.OnLockValueChanged -= SetMechanicMarks;
    }
}
