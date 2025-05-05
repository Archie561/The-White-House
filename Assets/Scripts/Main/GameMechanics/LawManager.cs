using UnityEngine;

/*public class LawManager : MonoBehaviour
{
    [SerializeField] private BudgetBox _budgetBox;
    [SerializeField] private GameObject _lawsMark;
    [SerializeField] private BlackoutScreen _blackoutScreen;
    [SerializeField] private CanvasGroup _acceptHint;
    [SerializeField] private CanvasGroup _declineHint;
    [SerializeField] private LawPanel _lawPanelPrefab;
    [SerializeField] private Transform _lawsPanelsTransform;

    private const float PANEL_ANIMATION_TIME = 0.6f;
    private const float HINT_ANIMATION_TIME = 0.8f;

    private readonly float SCREEN_CENTER = Screen.width * 0.5f;
    private readonly float PANEL_OFFSCREEN_LEFT = Screen.width * -0.5f;
    private readonly float PANEL_OFFSCREEN_RIGHT = Screen.width * 1.5f;

    private readonly float ACCEPT_TRIGGER_AREA = Screen.width * 0.6f;
    private readonly float DECLINE_TRIGGER_AREA = Screen.width * 0.4f;
    private readonly float ACCEPT_HINT_PEAK_AREA = Screen.width * 0.8f;
    private readonly float DECLINE_HINT_PEAK_AREA = Screen.width * 0.2f;

    private bool _isAnimationFinished;
    private bool _isDragPositionSet;
    private bool _isLawLocked;

    //field to calculate difference between pointer and law panel coordinates
    private float _cursorOffset;
    private Law _lawData;
    private LawPanel _currentLawPanel;

    private PlayerDataManager _playerDataManager;
    private ChapterDataManager _chapterDataManager;

    private void Awake()
    {
        GameManager.Instance.OnGameStateChanged += OnStateChanged;

        _playerDataManager = ServiceLocator.GetService<PlayerDataManager>();
        _chapterDataManager = ServiceLocator.GetService<ChapterDataManager>();
    }

    private void OnDestroy() => GameManager.Instance.OnGameStateChanged -= OnStateChanged;

    //sets game state to ActiveLaws on button click if laws are not locked and current game state is Default
    public void LawClickHandler()
    {
        if (!_isLawLocked && GameManager.Instance.State == GameState.Default)
        {
            AudioManager.Instance.PlaySFX("document");
            GameManager.Instance.ChangeGameState(GameState.ActiveLaws);
        }
    }

    //handles game state change. If Default - updates laws status, if ActiveLaws - initializes laws, else - return
    private void OnStateChanged(GameState state)
    {
        if (state == GameState.Default) UpdateLawsStatus();

        else if (state == GameState.ActiveLaws) SetUpMechanic();
    }

    //initializes laws mechanic: sets blackout screen, hint objects and invokes method to load law data
    private void SetUpMechanic()
    {
        _acceptHint.gameObject.SetActive(true);
        _declineHint.gameObject.SetActive(true);

        _blackoutScreen.SetPosition(BlackoutScreenPosition.underBudgetBox);
        _blackoutScreen.OnClick += Exit;
        _blackoutScreen.FadeIn();

        LoadNextLaw();
    }

    //Creates the next law panel and initializes it with data. Exits if all laws is displayed
    private void LoadNextLaw()
    {
        if (_isLawLocked)
        {
            Exit();
            return;
        }

        //_lawData = _chapterDataManager.GetLaw(_playerDataManager.LawID);
        _currentLawPanel = Instantiate(_lawPanelPrefab, _lawsPanelsTransform);
        _currentLawPanel.Initialize(_lawData);
        _currentLawPanel.OnPanelDrag += OnPanelDrag;
        _currentLawPanel.OnPanelUp += OnPanelUp;

        ShowLawPanel();
    }

    //animates law panel appearing
    private void ShowLawPanel()
    {
        _isAnimationFinished = false;
        _currentLawPanel.transform.localScale = Vector3.zero;
        _currentLawPanel.transform.LeanScale(Vector3.one, PANEL_ANIMATION_TIME).setEaseOutQuart().setOnComplete(() => _isAnimationFinished = true);
    }

    //handles panel drag by setting its position to cursor possition and updates the visibility of hints
    public void OnPanelDrag()
    {
        if (!_isAnimationFinished) return;

        if (!_isDragPositionSet)
        {
            _cursorOffset = Input.mousePosition.x - _currentLawPanel.transform.position.x;
            _isDragPositionSet = true;
        }

        _currentLawPanel.transform.position = new Vector2(Input.mousePosition.x - _cursorOffset, _currentLawPanel.transform.position.y);
        UpdateHints();
    }

    //handles events when releasing panel
    public void OnPanelUp()
    {
        if (!_isAnimationFinished) return;

        _isAnimationFinished = false;
        _isDragPositionSet = false;

        Transform panelTransform = _currentLawPanel.transform;

        //if law panel is in the neutral zone, reset its position, else - handle signing or declining of the law
        if (panelTransform.position.x > DECLINE_TRIGGER_AREA && panelTransform.position.x < ACCEPT_TRIGGER_AREA) CancelLawSigning();
        else HandleLawSigning(panelTransform.position.x >= ACCEPT_TRIGGER_AREA);

        //hides hints after releasing the panel, regardless of whether the law is in the neutral zone, signed or declined
        if (_acceptHint.alpha > 0) _acceptHint.LeanAlpha(0, HINT_ANIMATION_TIME);
        if (_declineHint.alpha > 0) _declineHint.LeanAlpha(0, HINT_ANIMATION_TIME);
    }

    //resets position of the panel
    private void CancelLawSigning() => _currentLawPanel.transform.LeanMoveX(SCREEN_CENTER, PANEL_ANIMATION_TIME)
        .setEaseOutQuart().setOnComplete(() => _isAnimationFinished = true);

    //updates the player characteristics and status of laws mechanic depending on the signing of the law and plays animation
    private void HandleLawSigning(bool isSigned)
    {
        AudioManager.Instance.PlaySFX(isSigned ? "signed-1" : "decline");
        //Characteristic characteristics = isSigned ? _lawData.characteristicsUpdateWhenApplied : _lawData.characteristicsUpdateWhenDeclined;

*//*        _playerDataManager.UpdateLawID(_lawData.lawID + 1);
        _playerDataManager.UpdateCharacteristics(characteristics);
        _budgetBox.UpdateBudget(characteristics.budget);*//*
        UpdateLawsStatus();

        _currentLawPanel.transform.LeanMoveX(isSigned ? PANEL_OFFSCREEN_RIGHT : PANEL_OFFSCREEN_LEFT, PANEL_ANIMATION_TIME)
            .setEaseOutQuart().setOnComplete(() =>
            {
                _isAnimationFinished = true;
                DestroyLawPanel();
                LoadNextLaw();
            });
    }

    //defines whether decision is locked or not
    public void UpdateLawsStatus()
    {
        *//*int lawID = _playerDataManager.LawID;

        _isLawLocked =
            lawID >= _chapterDataManager.GetLawsLength() ||
            _chapterDataManager.GetLaw(lawID).lockedByDialogue >= _playerDataManager.DialogueID ||
            _chapterDataManager.GetLaw(lawID).lockedByDecision >= _playerDataManager.DecisionID;

        _lawsMark.SetActive(!_isLawLocked);*//*
    }

    //unsubscribes from events, disables hints, destroys law panel and sets default game state
    private void Exit()
    {
        if (!_isAnimationFinished) return;

        //if law panel still exists - hide and destroy it
        if (_currentLawPanel != null)
            _currentLawPanel.transform.LeanScale(Vector3.zero, PANEL_ANIMATION_TIME).setEaseOutQuart().setOnComplete(DestroyLawPanel);

        _acceptHint.gameObject.SetActive(false);
        _declineHint.gameObject.SetActive(false);

        _blackoutScreen.OnClick -= Exit;
        _blackoutScreen.FadeOut(() => GameManager.Instance.ChangeGameState(GameState.Default));
    }

    //destroys law panel gameobject and unsubscribes from panel events 
    private void DestroyLawPanel()
    {
        _currentLawPanel.OnPanelDrag -= OnPanelDrag;
        _currentLawPanel.OnPanelUp -= OnPanelUp;
        Destroy(_currentLawPanel.gameObject);
    }

    //updates the transparency of hints: if the panel is closer to the left edge of the screen, increases the alpha of the decline hint
    //else - increases the alpha of the accept hint
    private void UpdateHints()
    {
        float panelPositionX = _currentLawPanel.transform.position.x;

        _acceptHint.alpha = Mathf.Clamp01((panelPositionX - SCREEN_CENTER) / (ACCEPT_HINT_PEAK_AREA - SCREEN_CENTER));
        _declineHint.alpha = Mathf.Clamp01((panelPositionX - SCREEN_CENTER) / (DECLINE_HINT_PEAK_AREA - SCREEN_CENTER));
    }
}*/
