using System;
using TMPro;
using UnityEditor.Localization.Editor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Localization.Settings;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour
{
    /*---------------------------UI SECTION---------------------------*/
    [SerializeField]
    private CanvasGroup _loadingScreen;
    [SerializeField]
    private RawImage _backgroundImage;

    [SerializeField]
    private GameObject _messageBoxes;
    [SerializeField]
    private GameObject _replicaBox;
    [SerializeField]
    private GameObject _choiceBox;

    [SerializeField]
    private Image _characterImage;
    [SerializeField]
    private TextMeshProUGUI _characterName;
    [SerializeField]
    private TextMeshProUGUI _replicaText;
    [SerializeField]
    private TextMeshProUGUI _choice1Text;
    [SerializeField]
    private TextMeshProUGUI _choice2Text;

    [SerializeField]
    private TextMeshProUGUI _budgetText;
    [SerializeField]
    private Color _defaultBudgetColor;
    [SerializeField]
    private Color _revenuesColor;
    [SerializeField]
    private Color _expensesColor;
    /*---------------------------END UI SECTION---------------------------*/

    /*--------------------ANIMATION PARAMETERS SECTION--------------------*/
    private bool _isAnimationFinished;
    private float _loadingScreenAnimationTime = 0.8f;
    private float _characterAnimationTime = 0.3f;
    private float _panelsAnimationTime = 0.6f;
    private float _budgetTextAnimationTime = 0.3f;

    private float _characterImageDefaultPositionY;
    private float _characterImageHidePositionY;
    /*------------------END ANIMATION PARAMETERS SECTION------------------*/

    /*----------------------OTHER PARAMETERS SECTION----------------------*/
    private Dialogue _currentDialogue;
    private SubReplica[] _currentSubReplicas;

    private Typewriter _typewriter;

    private bool _showSubReplicas;
    private int _replicaIndex;
    private int _subReplicaIndex;

    private string _presidentImageName;
    private string _presidentName;

    [SerializeField]
    private GameObject _pauseManager;
    /*--------------------END OTHER PARAMETERS SECTION--------------------*/

    private void Start()
    {
        //зробити зображення поточного президента
        _budgetText.text = BudgetToString(DataManager.PlayerData.characteristics.budget);
        _budgetText.color = DataManager.PlayerData.characteristics.budget < 0 ? _expensesColor : _defaultBudgetColor;

        Vector2 messageBoxesDefaultScale = _messageBoxes.transform.localScale;
        _characterImageDefaultPositionY = _characterImage.transform.position.y;
        _characterImageHidePositionY = -_characterImage.rectTransform.rect.size.y * _characterImage.rectTransform.lossyScale.y / 2;
        _presidentImageName = DataManager.CurrentSelectedPresident;
        _presidentName = LocalizationSettings.StringDatabase.GetLocalizedString("UILocalization", _presidentImageName);

        _messageBoxes.transform.localScale = Vector2.zero;
        _characterImage.transform.position = new Vector2(_characterImage.transform.position.x, _characterImageHidePositionY);

        _currentDialogue = GameManager.Instance.GetNextDialogue();
        _typewriter = new Typewriter(_replicaText);
        _replicaIndex = 0;

        _messageBoxes.SetActive(true);
        _characterImage.gameObject.SetActive(true);

        _loadingScreen.LeanAlpha(0, _loadingScreenAnimationTime).setOnComplete(() => { _loadingScreen.gameObject.SetActive(false); });
        if (!string.IsNullOrEmpty(_currentDialogue.backgroundMusic))
        {
            AudioManager.Instance.SetMusic(_currentDialogue.backgroundMusic);
        }

        /*------------------------initializing the first replica------------------------*/
        _backgroundImage.texture = Resources.Load<Texture>("Textures/BackgroundImages/" + _currentDialogue.backgroundImageName);
        _characterImage.sprite = Resources.Load<Sprite>("Textures/Characters/" + _currentDialogue.replicas[_replicaIndex].imageName);
        _characterName.text = _currentDialogue.replicas[_replicaIndex].characterName;
        _replicaText.text = _currentDialogue.replicas[_replicaIndex].replicaText;

        _messageBoxes.LeanScale(messageBoxesDefaultScale, _panelsAnimationTime).setEaseOutQuart();
        _characterImage.transform.LeanMoveY(_characterImageDefaultPositionY, _characterAnimationTime).setEaseOutQuart().setOnComplete(() => { _isAnimationFinished = true; });
        _typewriter.StartWriting();
        AudioManager.Instance.PlaySFX("typing");
        /*------------------------------------------------------------------------------*/
    }

    private void LoadNextReplica()
    {
        //if needs to show specific subreplicas after made choice
        if (_showSubReplicas)
        {
            if (_subReplicaIndex >= _currentSubReplicas.Length)
            {
                _showSubReplicas = false;
                LoadNextReplica();
                return;
            }

            if (_characterImage.sprite.name != _currentSubReplicas[_subReplicaIndex].imageName)
            {
                SwapCharacters(_currentSubReplicas[_subReplicaIndex].imageName);
            }
            _characterName.text = _currentSubReplicas[_subReplicaIndex].characterName;
            _replicaText.text = _currentSubReplicas[_subReplicaIndex].subReplicaText;

            _subReplicaIndex++;
        }
        //if needs to show main replicas chain
        else
        {
            _replicaIndex++;
            if (_replicaIndex >= _currentDialogue.replicas.Length)
            {
                Exit();
                return;
            }

            if (_characterImage.sprite.name != _currentDialogue.replicas[_replicaIndex].imageName)
            {
                SwapCharacters(_currentDialogue.replicas[_replicaIndex].imageName);
            }
            _characterName.text = _currentDialogue.replicas[_replicaIndex].characterName;
            _replicaText.text = _currentDialogue.replicas[_replicaIndex].replicaText;
        }

        _typewriter.StartWriting();
        AudioManager.Instance.PlaySFX("typing");
    }

    public void LoadNextChoice()
    {
        if (_characterImage.sprite.name != _presidentImageName)
        {
            SwapCharacters(_presidentImageName);
        }
        _characterName.text = _presidentName;
        _choice1Text.text = GameManager.Instance.GetChoice(_currentDialogue.replicas[_replicaIndex].choiceID).option1;
        _choice2Text.text = GameManager.Instance.GetChoice(_currentDialogue.replicas[_replicaIndex].choiceID).option2;
    }

    public void DialogueClickHandler()
    {
        //if animation si playing or choice modal is active - ignore clicking
        if (!_isAnimationFinished || _choiceBox.activeSelf)
        {
            return;
        }

        if (_typewriter.IsWriting)
        {
            _typewriter.SkipWriting();
            return;
        }

        //if subreplicas are not showing right now and there is choice to show - display choice modal
        if (!_showSubReplicas && _currentDialogue.replicas[_replicaIndex].choiceID != -1)
        {
            LoadNextChoice();
            _replicaBox.SetActive(false);
            _choiceBox.SetActive(true);
            return;
        }

        LoadNextReplica();
    }

    public void ChoiceClickHandler()
    {
        if (!_isAnimationFinished)
        {
            return;
        }

        AudioManager.Instance.PlaySFX("button");

        int _pickedChoiceOption = EventSystem.current.currentSelectedGameObject.tag == "ChoiceOption1" ? 1 : 2;
        Characteristics characteristicsToUpdate = _pickedChoiceOption == 1 ? GameManager.Instance.GetChoice(_currentDialogue.replicas[_replicaIndex].choiceID).characteristicsUpdateOption1 :
            GameManager.Instance.GetChoice(_currentDialogue.replicas[_replicaIndex].choiceID).characteristicsUpdateOption2;

        GameManager.Instance.UpdateCharacteristics(characteristicsToUpdate);
        UpdateBudget(characteristicsToUpdate.budget);
        DataManager.PlayerData.madeChoices[DataManager.PlayerData.chapterID].value[_currentDialogue.replicas[_replicaIndex].choiceID] = _pickedChoiceOption;

        //subreplicas numeration starts from 0 in every Replica object
        _currentSubReplicas = _pickedChoiceOption == 1 ? _currentDialogue.replicas[_replicaIndex].subreplicasOption1 : _currentDialogue.replicas[_replicaIndex].subreplicasOption2;
        _showSubReplicas = true;
        _subReplicaIndex = 0;

        //if there are replicas or subreplicas to show after choice - swap modals
        if (_replicaIndex < _currentDialogue.replicas.Length - 1 || _currentSubReplicas.Length > 0)
        {
            _choiceBox.SetActive(false);
            _replicaBox.SetActive(true);
        }
        LoadNextReplica();
    }

    public void SwapCharacters(string imageName)
    {
        _isAnimationFinished = false;

        _characterImage.transform.LeanMoveY(_characterImageHidePositionY, _characterAnimationTime).setEaseOutQuart().setOnComplete(() =>
        {
            _characterImage.sprite = Resources.Load<Sprite>("Textures/Characters/" + imageName);
            _characterImage.transform.LeanMoveY(_characterImageDefaultPositionY, _characterAnimationTime).setEaseOutQuart().setOnComplete(() => _isAnimationFinished = true);
        });
    }

    public void Exit()
    {
        DataManager.PlayerData.dialogueID++;
        DataManager.SaveData();

        GameManager.Instance.UpdateMechanicsStatus();

        _loadingScreen.gameObject.SetActive(true);
        _characterImage.transform.LeanMoveY(_characterImageHidePositionY, _characterAnimationTime).setEaseOutQuart();
        _messageBoxes.LeanScale(Vector2.zero, _panelsAnimationTime).setEaseOutQuart();
        _loadingScreen.LeanAlpha(1, _loadingScreenAnimationTime).setOnComplete(() =>
        {
            if (GameManager.Instance.IsChapterFinished)
            {
                GameManager.Instance.LoadNextChapter();
            }
            else
            {
                SceneManager.LoadScene(1);
            }
        });
    }

    /*=====================================================BUDGET BOX SECTION=======================================================*/
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
    /*===================================================END BUDGET BOX SECTION=====================================================*/
}
