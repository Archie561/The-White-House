using System;
using TMPro;
using UnityEngine;
using UnityEngine.Localization.Settings;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class UIHandler : MonoBehaviour
{
    /*---------------------------UI SECTION---------------------------*/
    [SerializeField]
    private CanvasGroup _loadingScreen;
    [SerializeField]
    private CanvasGroup _chapterTitle;
    [SerializeField]
    private TextMeshProUGUI _presidentText;
    [SerializeField]
    private TextMeshProUGUI _monthText;

    [SerializeField]
    private UnityEngine.UI.Image _leftPanel;
    [SerializeField]
    private UnityEngine.UI.Image _rightPanel;

    [SerializeField]
    private GameObject _dialoguesMark;
    [SerializeField]
    private GameObject _lawsMark;
    [SerializeField]
    private GameObject _decisionsMark;
    [SerializeField]
    private GameObject _politicMark;
    [SerializeField]
    private GameObject _internationalMark;
    [SerializeField]
    private GameObject _armyMark;

    [SerializeField]
    private TextMeshProUGUI _budgetText;
    [SerializeField]
    private Color _defaultBudgetColor;
    [SerializeField]
    private Color _revenuesColor;
    [SerializeField]
    private Color _expensesColor;

    [SerializeField]
    private GameObject _decisionManager;
    [SerializeField]
    private GameObject _newsManager;
    [SerializeField]
    private GameObject _lawManager;
    [SerializeField]
    private GameObject _characteristicsManager;
    [SerializeField]
    private GameObject _pauseManager;
    /*---------------------------END UI SECTION---------------------------*/

    /*--------------------ANIMATION PARAMETERS SECTION--------------------*/
    private float _budgetTextAnimationTime = 0.3f;
    private float _loadingScreenAnimationTime = 0.8f;
    private float _firstLoadingAnimationTime = 3.0f;
    /*------------------END ANIMATION PARAMETERS SECTION------------------*/

    private int _characteristicCriticalValue = 10;

    private void Start()
    {
        //adjusting UI to the safe zone
        _leftPanel.transform.position = new Vector2(Screen.safeArea.xMin - _leftPanel.rectTransform.rect.width / 2 * _leftPanel.transform.lossyScale.x, Screen.safeArea.center.y);
        _rightPanel.transform.position = new Vector2(Screen.safeArea.xMax + _rightPanel.rectTransform.rect.width / 2 * _rightPanel.transform.lossyScale.x, Screen.safeArea.center.y);

        _budgetText.text = BudgetToString(DataManager.PlayerData.characteristics.budget);
        _budgetText.color = DataManager.PlayerData.characteristics.budget < 0 ? _expensesColor : _defaultBudgetColor;

        SetMechanicMarks();
        HandleSceneTransition();
    }

    /*===================================MECHANIC PANELS HANDLING SECTION====================================*/
    public void DisplayDialogue()
    {
        if (!GameManager.Instance.IsDialogueLocked())
        {
            AudioManager.Instance.PlaySFX("door");
            _loadingScreen.gameObject.SetActive(true);
            _loadingScreen.LeanAlpha(1, _loadingScreenAnimationTime).setOnComplete(() => { SceneManager.LoadScene(2); });
        }
        else
        {
            Debug.Log("Dialogues are locked!");
            AudioManager.Instance.PlaySFX("button");
        }
    }

    public void DisplayLaws()
    {
        if (!GameManager.Instance.IsLawLocked())
        {
            AudioManager.Instance.PlaySFX("document");
            _lawManager.SetActive(true);
        }
        else
        {
            Debug.Log("Laws are locked!");
            AudioManager.Instance.PlaySFX("button");
        }
    }

    public void DisplayDecision()
    {
        if (!GameManager.Instance.IsDecisionLocked())
        {
            AudioManager.Instance.PlaySFX("phone");
            _decisionManager.SetActive(true);
        }
        else
        {
            Debug.Log("Decisions are locked!");
            AudioManager.Instance.PlaySFX("button");
        }
    }

    public void DisplayCharacteristics()
    {
        AudioManager.Instance.PlaySFX("button");
        _characteristicsManager.SetActive(true);
    }
    /*==================================END MECHANIC PANELS HANDLING SECTION==================================*/

    /*===================================OTHER UI ELEMENTS HANDLING SECTION===================================*/
    public void SetMechanicMarks()
    {
        _dialoguesMark.SetActive(!GameManager.Instance.IsDialogueLocked());
        _lawsMark.SetActive(!GameManager.Instance.IsLawLocked());
        _decisionsMark.SetActive(!GameManager.Instance.IsDecisionLocked());

        Characteristics characteristics = DataManager.PlayerData.characteristics;

        _politicMark.SetActive(characteristics.science < _characteristicCriticalValue || characteristics.medicine < _characteristicCriticalValue
            || characteristics.welfare < _characteristicCriticalValue || characteristics.ecology < _characteristicCriticalValue
            || characteristics.education < _characteristicCriticalValue || characteristics.infrastructure < _characteristicCriticalValue);
        _internationalMark.SetActive(characteristics.europeanUnion < _characteristicCriticalValue || characteristics.unitedKingdom < _characteristicCriticalValue
            || characteristics.china < _characteristicCriticalValue || characteristics.CIS < _characteristicCriticalValue
            || characteristics.africa < _characteristicCriticalValue || characteristics.OPEC < _characteristicCriticalValue);
        _armyMark.SetActive(characteristics.infantry < _characteristicCriticalValue || characteristics.airForces < _characteristicCriticalValue
            || characteristics.machinery < _characteristicCriticalValue || characteristics.navy < _characteristicCriticalValue);
    }

    public void UpdateBudget(int value)
    {
        if (value == 0)
        {
            return;
        }

        float defaultTextPositionY = _budgetText.transform.position.y;
        _budgetText.color = value > 0 ? _revenuesColor : _expensesColor;

        _budgetText.transform.LeanMoveY(_budgetText.transform.position.y + _budgetText.rectTransform.rect.height * _budgetText.transform.lossyScale.y, _budgetTextAnimationTime).setEaseInQuart().setOnComplete(() =>
        {
            _budgetText.text = BudgetToString(DataManager.PlayerData.characteristics.budget);
            _budgetText.transform.position = new Vector2(_budgetText.transform.position.x, defaultTextPositionY - _budgetText.rectTransform.rect.height * _budgetText.transform.lossyScale.y);
            _budgetText.transform.LeanMoveY(defaultTextPositionY, _budgetTextAnimationTime).setEaseOutQuart().setOnComplete(() => { _budgetText.color = DataManager.PlayerData.characteristics.budget < 0 ? _expensesColor : _defaultBudgetColor; });
        });
    }

    private string BudgetToString(int budget)
    {
        string stringBudget = Math.Abs(budget).ToString();
        int budgetLength = stringBudget.Length;

        if (stringBudget == "0")
        {
            return "0 $";
        }

        if (stringBudget.Length > 6)
        {
            return (budget < 0 ? "-" : "") + "999,999,999,999+ $";
        }

        for (int i = budgetLength - 1; i >= 0; i--)
        {
            if ((budgetLength - i) % 3 == 0 && i != 0)
            {
                stringBudget = stringBudget.Insert(i, ",");
            }
        }

        return (budget < 0 ? "-" : "") + stringBudget + ",000,000 $";
    }

    public void DisplayPausePanel()
    {
        AudioManager.Instance.PlaySFX("button");
        _pauseManager.SetActive(true);
    }
    /*==================================END OTHER UI ELEMENTS HANDLING SECTION==================================*/

    /*=======================================TRANSITIONS HANDLING SECTION=======================================*/
    public void CheckEndOfChapter()
    {
        if (GameManager.Instance.IsChapterFinished)
        {
            _loadingScreen.LeanAlpha(1, _loadingScreenAnimationTime).setOnComplete(GameManager.Instance.LoadNextChapter);
        }
    }

    private void HandleSceneTransition()
    {
        if (GameManager.Instance.shouldDisplayChapterTitle)
        {
            GameManager.Instance.shouldDisplayChapterTitle = false;

            _presidentText.text = LocalizationSettings.StringDatabase.GetLocalizedString("UILocalization", DataManager.CurrentSelectedPresident);
            _monthText.text = GameManager.Instance.CurrentChapter.title;

            _chapterTitle.gameObject.SetActive(true);
            _chapterTitle.LeanAlpha(1, _loadingScreenAnimationTime);

            if (GameManager.Instance.shouldDisplayStartingCutscenes)
            {
                _chapterTitle.LeanAlpha(0, _loadingScreenAnimationTime).setDelay(_firstLoadingAnimationTime).setOnComplete(() => { SceneManager.LoadScene(3); });
            }
            else
            {
                _loadingScreen.LeanAlpha(0, _loadingScreenAnimationTime).setDelay(_firstLoadingAnimationTime).setOnComplete(() =>
                {
                    if (GameManager.Instance.shouldDisplayNews)
                    {
                        _newsManager.SetActive(true);
                        GameManager.Instance.shouldDisplayNews = false;
                    }
                    _chapterTitle.gameObject.SetActive(false);
                    _loadingScreen.gameObject.SetActive(false);
                    AudioManager.Instance.SetMusic(GameManager.Instance.CurrentChapter.theme);
                });
            }
        }
        else
        {
            _loadingScreen.LeanAlpha(0, _loadingScreenAnimationTime).setOnComplete(() =>
            {
                if (GameManager.Instance.shouldDisplayNews)
                {
                    _newsManager.SetActive(true);
                    GameManager.Instance.shouldDisplayNews = false;
                    AudioManager.Instance.SetMusic(GameManager.Instance.CurrentChapter.theme);
                }
                _loadingScreen.gameObject.SetActive(false);
            });
        }
    }
    /*=====================================END TRANSITIONS HANDLING SECTION=====================================*/
}
