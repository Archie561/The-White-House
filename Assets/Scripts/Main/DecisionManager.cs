using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DecisionManager : MonoBehaviour
{
    /*---------------------------UI SECTION---------------------------*/
    [SerializeField]
    private CanvasGroup _blackBackground;
    [SerializeField]
    private Button _backButton;

    [SerializeField]
    private Image _decisionPanel;
    [SerializeField]
    private Image _characterImage;
    [SerializeField]
    private TextMeshProUGUI _characterName;
    [SerializeField]
    private TextMeshProUGUI _decisionText;
    [SerializeField]
    private Button[] _optionButtons;
    [SerializeField]
    private TextMeshProUGUI[] _optionTexts;
    /*---------------------------END UI SECTION---------------------------*/

    /*--------------------ANIMATION PARAMETERS SECTION--------------------*/
    private Vector2 _optionButtonDefaultScale;
    private Vector2 _decisionPanelDefaultScale;
    private Vector2 _decisionPanelDefaultPosition;
    private Vector2 _backButtonDefaultPosition;

    private float _decisionPanelStartSizeXY = 0.0001f;
    private float _blackBackgroundAnimationTime = 0.8f;
    private float _backButtonAnimationTime = 0.8f;
    private float _panelAnimationTime = 0.8f;
    private float _optionButtonAnimationTime = 0.3f;
    /*------------------END ANIMATION PARAMETERS SECTION------------------*/

    /*----------------------OTHER PARAMETERS SECTION----------------------*/
    [SerializeField]
    private UIHandler _UIHandler;

    private Typewriter _typewriter;
    private Decision _currentDecision;

    private bool _isDecisionIsMade;
    private int _optionIndex;
    /*--------------------END OTHER PARAMETERS SECTION--------------------*/

    private void OnEnable()
    {
        //variables initialization
        _optionIndex = 0;
        _isDecisionIsMade = false;
        _optionButtonDefaultScale = _optionButtons[_optionIndex].transform.localScale;
        _decisionPanelDefaultScale = _decisionPanel.transform.localScale;
        _decisionPanelDefaultPosition = _decisionPanel.transform.position;
        _backButtonDefaultPosition = _backButton.transform.position;

        _typewriter = new Typewriter(_decisionText);
        _currentDecision = GameManager.Instance.GetNextDecision();

        //deision data initialization
        _characterImage.sprite = Resources.Load<Sprite>("Textures/Characters/" + _currentDecision.imageName);
        _characterName.text = _currentDecision.characterName;
        _decisionText.text = _currentDecision.text;

        //setting gameobjects & starting animations
        _backButton.transform.position = new Vector2(_backButton.transform.position.x, Screen.height + _backButton.GetComponent<RectTransform>().rect.size.y * _backButton.GetComponent<RectTransform>().lossyScale.y);
        _decisionPanel.transform.localScale = new Vector2(_decisionPanelStartSizeXY, _decisionPanelStartSizeXY);

        _blackBackground.gameObject.SetActive(true);
        _backButton.gameObject.SetActive(true);
        _decisionPanel.gameObject.SetActive(true);

        _blackBackground.LeanAlpha(1, _blackBackgroundAnimationTime);
        _backButton.transform.LeanMove(_backButtonDefaultPosition, _backButtonAnimationTime).setEaseOutQuart();
        _decisionPanel.transform.LeanScale(_decisionPanelDefaultScale, _panelAnimationTime).setEaseOutQuart();

        _typewriter.OnWritingFinished += DisplayOptions;
        _typewriter.StartWriting();
    }

    private void DisplayOptions()
    {
        if (_optionIndex >= _currentDecision.options.Length)
        {
            return;
        }

        _optionTexts[_optionIndex].text = _currentDecision.options[_optionIndex];
        _optionButtons[_optionIndex].transform.localScale = Vector2.zero;
        _optionButtons[_optionIndex].gameObject.SetActive(true);
        _optionButtons[_optionIndex].transform.LeanScale(_optionButtonDefaultScale, _optionButtonAnimationTime).setEaseOutQuart().setOnComplete(DisplayOptions);

        _optionIndex++;
    }

    public void OptionClickHandler()
    {
        if (_isDecisionIsMade)
        {
            return;
        }
        _isDecisionIsMade = true;

        int pickedOption;
        switch (EventSystem.current.currentSelectedGameObject.tag)
        {
            case "DecisionOption1":
                pickedOption = 1;
                break;
            case "DecisionOption2":
                pickedOption = 2;
                break;
            case "DecisionOption3":
                pickedOption = 3;
                break;
            case "DecisionOption4":
                pickedOption = 4;
                break;
            default:
                pickedOption = 1;
                Debug.LogError("Picked option wrong value!");
                break;
        }
        AudioManager.Instance.PlaySFX("button");

        GameManager.Instance.UpdateCharacteristics(_currentDecision.characteristicUpdates[pickedOption - 1]);
        DataManager.PlayerData.madeDecisions[DataManager.PlayerData.chapterID].value[DataManager.PlayerData.decisionID] = pickedOption;
        DataManager.PlayerData.decisionID++;
        DataManager.SaveData();

        _UIHandler.UpdateBudget(_currentDecision.characteristicUpdates[pickedOption - 1].budget);

        //animation of the panel is different when click on the option button and back button, so LeanScale should invoke in this method
        _decisionPanel.transform.LeanScale(Vector2.zero, _panelAnimationTime).setEaseOutQuart();
        Exit();
    }

    private void Exit()
    {
        GameManager.Instance.UpdateMechanicsStatus();

        _typewriter.OnWritingFinished -= DisplayOptions;

        _backButton.transform.LeanMoveY(Screen.height + _backButton.GetComponent<RectTransform>().rect.size.y * _backButton.GetComponent<RectTransform>().lossyScale.y, _backButtonAnimationTime).setEaseOutQuart();
        _blackBackground.LeanAlpha(0, _blackBackgroundAnimationTime).setOnComplete(() =>
        {
            _backButton.gameObject.SetActive(false);
            _decisionPanel.gameObject.SetActive(false);
            _blackBackground.gameObject.SetActive(false);
            for (int i = 0; i < _optionButtons.Length; i++)
            {
                _optionButtons[i].gameObject.SetActive(false);
                _optionButtons[i].transform.localScale = _optionButtonDefaultScale;
            }

            _decisionPanel.transform.localScale = _decisionPanelDefaultScale;
            _decisionPanel.transform.position = _decisionPanelDefaultPosition;
            _backButton.transform.position = _backButtonDefaultPosition;

            _UIHandler.SetMechanicMarks();
            _UIHandler.CheckEndOfChapter();

            gameObject.SetActive(false);
        });
    }

    public void SkipWriting()
    {
        _typewriter.SkipWriting();
    }

    public void BackButtonClickHandler()
    {
        if (!_isDecisionIsMade)
        {
            AudioManager.Instance.PlaySFX("button");
            _decisionPanel.transform.LeanMoveY(-Screen.height / 2, _panelAnimationTime).setEaseOutQuart();
            Exit();
        }
    }
}
