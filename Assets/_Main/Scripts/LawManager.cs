using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LawManager : MonoBehaviour
{
    /*---------------------------UI SECTION---------------------------*/
    [SerializeField]
    private Image _lawPanel;

    [SerializeField]
    private Sprite _blankType1;
    [SerializeField]
    private Sprite _blankType2;
    [SerializeField]
    private Sprite _blankType3;

    [SerializeField]
    private TextMeshProUGUI _lawHeaderText;
    [SerializeField]
    private TextMeshProUGUI _lawMainText;
    [SerializeField]
    private TextMeshProUGUI _lawDetailedText;
    [SerializeField]
    private TextMeshProUGUI _lawPreparedByText;

    [SerializeField]
    private CanvasGroup _blackBackground;
    [SerializeField]
    private CanvasGroup _acceptHint;
    [SerializeField]
    private CanvasGroup _declineHint;
    [SerializeField]
    private Button _backButton;
    /*---------------------------END UI SECTION---------------------------*/

    /*--------------------ANIMATION PARAMETERS SECTION--------------------*/
    private bool _isAnimationFinished;
    private bool _isDragPositionSet;

    private float _blackBackgroundAnimationTime = 0.8f;
    private float _backButtonAnimationTime = 0.8f;
    private float _panelAppearingAnimationTime = 0.8f;
    private float _panelDisappearingAnimationTime = 0.8f;
    private float _hintAnimationTime = 0.5f;
    /*------------------END ANIMATION PARAMETERS SECTION------------------*/

    /*----------------------OTHER PARAMETERS SECTION----------------------*/
    //field to calculate difference between pointer and document coordinates
    private Vector2 _currentPosition;
    private Vector2 _lawPanelDefaultScale;
    private Vector2 _lawPanelDefaultPosition;

    private float _acceptTriggerArea;
    private float _declineTriggerArea;
    /*--------------------END OTHER PARAMETERS SECTION--------------------*/

    private Law _currentLaw;

    private void OnEnable()
    {
        //variables initialization
        _isAnimationFinished = true;
        _lawPanelDefaultScale = _lawPanel.transform.localScale;
        _lawPanelDefaultPosition = _lawPanel.transform.position;
        Vector2 _backButtonDefaultPosition = _backButton.transform.position;

        _acceptTriggerArea = _acceptHint.transform.position.x - _acceptHint.GetComponent<RectTransform>().rect.width * _acceptHint.transform.lossyScale.x / 2;
        _declineTriggerArea = _declineHint.transform.position.x + _declineHint.GetComponent<RectTransform>().rect.width * _declineHint.transform.lossyScale.x / 2;

        //setting gameobjects & starting animations
        _backButton.transform.position = new Vector2(_backButton.transform.position.x, Screen.height + _backButton.GetComponent<RectTransform>().rect.size.y);

        _backButton.gameObject.SetActive(true);
        _blackBackground.gameObject.SetActive(true);
        _lawPanel.gameObject.SetActive(true);
        _acceptHint.gameObject.SetActive(true);
        _declineHint.gameObject.SetActive(true);

        _backButton.transform.LeanMove(_backButtonDefaultPosition, _backButtonAnimationTime).setEaseOutQuart();
        _blackBackground.LeanAlpha(1, _blackBackgroundAnimationTime);

        //getting first law
        ResetPanelPosition();
        GetNextLaw();
    }

    private void Update()
    {
        if (_isAnimationFinished && _lawPanel.transform.position.x < _declineTriggerArea)
        {
            _acceptHint.alpha -= Time.deltaTime / _hintAnimationTime;
            _declineHint.alpha += Time.deltaTime / _hintAnimationTime;

            return;
        }

        if (_isAnimationFinished && _lawPanel.transform.position.x > _acceptTriggerArea)
        {
            _acceptHint.alpha += Time.deltaTime / _hintAnimationTime;
            _declineHint.alpha -= Time.deltaTime / _hintAnimationTime;

            return;
        }

        _acceptHint.alpha -= Time.deltaTime / _hintAnimationTime;
        _declineHint.alpha -= Time.deltaTime / _hintAnimationTime;
    }

    private void GetNextLaw()
    {
        if (GameManager.Instance.IsLawLocked())
        {
            Exit();
            return;
        }

        _currentLaw = GameManager.Instance.GetNextLaw();
        ShowLawPanel();
    }

    private void ShowLawPanel()
    {
        _isAnimationFinished = false;

        _lawPanel.sprite = _currentLaw.lawType == 1 ? _blankType1 : _currentLaw.lawType == 2 ? _blankType2 : _blankType3;
        _lawHeaderText.text = _currentLaw.header;
        _lawMainText.text = _currentLaw.mainText;
        _lawDetailedText.text = _currentLaw.detailedText;
        _lawPreparedByText.text = _currentLaw.preparedBy;

        _lawPanel.transform.LeanScale(_lawPanelDefaultScale, _panelAppearingAnimationTime).setEaseOutQuart().setOnComplete(() => { _isAnimationFinished = true; });
    }

    private void ResetPanelPosition()
    {
        _lawPanel.transform.localScale = Vector2.zero;
        _lawPanel.transform.position = _lawPanelDefaultPosition;
    }

    private void Exit()
    {
        _backButton.transform.LeanMoveY(Screen.height + _backButton.GetComponent<RectTransform>().rect.size.y, _backButtonAnimationTime).setEaseOutQuart();
        _blackBackground.LeanAlpha(0, _blackBackgroundAnimationTime).setOnComplete(() => { GameManager.Instance.LoadMainScene(); });
    }

    public void OnBlankDrag()
    {
        if (!_isAnimationFinished)
        {
            return;
        }

        if (!_isDragPositionSet)
        {
            _isDragPositionSet = true;
            _currentPosition = new Vector2(Input.mousePosition.x - _lawPanel.transform.position.x, 0);
        }

        _lawPanel.transform.position = new Vector2(Input.mousePosition.x, _lawPanel.transform.position.y) - _currentPosition;
    }

    public void OnBlanktUp()
    {
        if (!_isAnimationFinished)
        {
            return;
        }

        _isAnimationFinished = false;
        _isDragPositionSet = false;

        if (_lawPanel.transform.position.x < _declineTriggerArea)
        {
            DataManager.UpdateCharacteristics(_currentLaw.characteristicsUpdateWhenDeclined);
            DataManager.PlayerData.lawID++;
            DataManager.SaveData();

            if (_declineHint.alpha < 1)
            {
                _declineHint.LeanAlpha(1, _hintAnimationTime);
            }

            _lawPanel.transform.LeanMoveX(-Screen.width/ 2, _panelDisappearingAnimationTime).setEaseOutQuart().setOnComplete(() =>
            {
                ResetPanelPosition();

                _isAnimationFinished = true; 
                GetNextLaw();
            });

            return;
        }

        if (_lawPanel.transform.position.x > _acceptTriggerArea)
        {
            DataManager.UpdateCharacteristics(_currentLaw.characteristicsUpdateWhenApplied);
            DataManager.PlayerData.lawID++;
            DataManager.SaveData();

            if (_acceptHint.alpha < 1)
            {
                _acceptHint.LeanAlpha(1, _hintAnimationTime);
            }

            _lawPanel.transform.LeanMoveX(Screen.width + Screen.width / 2, _panelDisappearingAnimationTime).setEaseOutQuart().setOnComplete(() =>
            {
                ResetPanelPosition();

                _isAnimationFinished = true;
                GetNextLaw();
            });

            return;
        }

        _lawPanel.transform.LeanMove(_lawPanelDefaultPosition, _panelDisappearingAnimationTime).setEaseOutQuart().setOnComplete(() => { _isAnimationFinished = true; });
    }

    public void BackButtonClickHandler()
    {
        if (_isAnimationFinished)
        {
            _lawPanel.gameObject.LeanMoveY(-Screen.height / 2, _panelDisappearingAnimationTime).setEaseOutQuart();
            Exit();
        }
    }
}
