using System.Collections;
using System.Collections.Generic;
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
    private GameObject _decisionModal;
    [SerializeField]
    private GameObject _lawModal;
    [SerializeField]
    private GameObject _newsModal;

    [SerializeField]
    private CanvasGroup _blackBackground;
    [SerializeField]
    private GameObject _pausePanel;

    private void Start()
    {
        //update exclamation marks visibility when lock value changes
        GameManager.Instance.OnLockValueChanged += SetMechanicMarks;
        SetMechanicMarks();

        if (DataManager.PlayerData.lawID == 0 && DataManager.PlayerData.dialogueID == 0 && DataManager.PlayerData.decisionID == 0 && !GameManager.Instance.IsNewsShown)
        {
            GameManager.Instance.IsNewsShown = true;
            _newsModal.SetActive(true);
        }
    }

    //set exclamation marks to buttons if mechanic is not locked
    private void SetMechanicMarks()
    {
        _dialoguesMark.SetActive(!GameManager.Instance.IsDialogueLocked);
        _lawsMark.SetActive(!GameManager.Instance.IsLawLocked);
        _decisionsMark.SetActive(!GameManager.Instance.IsDecisionLocked);
    }

    public void PauseClickHandler()
    {
        if (EventSystem.current.currentSelectedGameObject.CompareTag("MainMenuButton"))
        {
            SceneManager.LoadScene(3);
            return;
        }

        if (_pausePanel.activeSelf)
        {
            _blackBackground.LeanAlpha(0, 0.8f).setOnComplete(() => { _blackBackground.gameObject.SetActive(false); });
            _pausePanel.transform.LeanMoveLocalY(Screen.height, 0.8f).setEaseOutQuart().setOnComplete(() => { _pausePanel.SetActive(false); });
        }
        else
        {
            _pausePanel.SetActive(true);
            _blackBackground.gameObject.SetActive(true);
            _blackBackground.LeanAlpha(1, 0.8f);
            _pausePanel.transform.LeanMoveLocalY(0, 0.8f).setEaseOutQuart();
        }
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
            _lawModal.SetActive(true);
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
