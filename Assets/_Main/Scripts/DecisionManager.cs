using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DecisionManager : MonoBehaviour
{
    /*---------------------------UI Elements---------------------------*/
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
    /*---------------------------End UI Section---------------------------*/

    private Vector2 _optionButtonDefaultScale;

    //юніті має багти із слайдером якщо використовувати значення 0 у якості початкового розміру панелі, тому використовується значення приближене до нуля
    private float _decisionPanelStartSizeXY = 0.0001f;
    private float _blackBackgroundAnimationTime = 0.8f;
    private float _backButtonAnimationTime = 0.8f;
    private float _panelAnimationTime = 0.8f;
    private float _optionButtonAnimationTime = 0.3f;

    private Typewriter _typewriter;
    private Decision _currentDecision;

    private bool _isDecisionIsMade;
    private int _optionIndex;

    private void OnEnable()
    {
        //variables initialization
        _optionIndex = 0;
        _isDecisionIsMade = false;
        _optionButtonDefaultScale = _optionButtons[_optionIndex].transform.localScale;
        Vector2 backButtonDefaultPosition = _backButton.transform.position;
        Vector2 decisionPanelDefaultScale = _decisionPanel.transform.localScale;

        _typewriter = new Typewriter(_decisionText);
        _currentDecision = GameManager.Instance.GetNextDecision();

        //deision data initialization
        _characterImage.sprite = Resources.Load<Sprite>("Textures/Characters/" + _currentDecision.imageName);
        _characterName.text = _currentDecision.characterName;
        _decisionText.text = _currentDecision.text;

        //setting gameobjects & starting animations
        _backButton.transform.position = new Vector2(_backButton.transform.position.x, Screen.height + _backButton.GetComponent<RectTransform>().rect.size.y);
        _decisionPanel.transform.localScale = new Vector2(_decisionPanelStartSizeXY, _decisionPanelStartSizeXY);

        _blackBackground.gameObject.SetActive(true);
        _backButton.gameObject.SetActive(true);
        _decisionPanel.gameObject.SetActive(true);

        _blackBackground.LeanAlpha(1, _blackBackgroundAnimationTime);
        _backButton.transform.LeanMove(backButtonDefaultPosition, _backButtonAnimationTime).setEaseOutQuart();
        _decisionPanel.transform.LeanScale(decisionPanelDefaultScale, _panelAnimationTime).setEaseOutQuart();

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

        DataManager.UpdateCharacteristics(_currentDecision.characteristicUpdates[pickedOption - 1]);
        DataManager.PlayerData.madeDecisions[DataManager.PlayerData.chapterID].value[DataManager.PlayerData.decisionID] = pickedOption;
        DataManager.PlayerData.decisionID++;
        DataManager.SaveData();

        //анімація зникнення панелі різна при натисненні на кнопку варіанту і при натисненні на кнопку назад, тому LeanScale має бути у цьому методі
        _decisionPanel.transform.LeanScale(Vector2.zero, _panelAnimationTime).setEaseOutQuart();
        Exit();
    }

    private void Exit()
    {
        _typewriter.OnWritingFinished -= DisplayOptions;

        _backButton.transform.LeanMoveY(Screen.height + _backButton.GetComponent<RectTransform>().rect.size.y, _backButtonAnimationTime).setEaseOutQuart();
        _blackBackground.LeanAlpha(0, _blackBackgroundAnimationTime).setOnComplete(GameManager.Instance.LoadMainScene);
    }

    public void SkipWriting()
    {
        _typewriter.SkipWriting();
    }

    public void BackButtonClickHandler()
    {
        if (!_isDecisionIsMade)
        {
            _decisionPanel.transform.LeanMoveY(-Screen.height / 2, _panelAnimationTime).setEaseOutQuart();
            Exit();
        }
    }
}
