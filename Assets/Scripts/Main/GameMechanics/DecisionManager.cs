using UnityEngine;

public class DecisionManager : MonoBehaviour
{
    [SerializeField] private BudgetBox _budgetBox;
    [SerializeField] private BlackoutScreen _blackoutScreen;
    [SerializeField] private GameObject _decisionsMark;
    [SerializeField] private Transform _decisionsPanelsTransform;
    [SerializeField] private DecisionPanel _decisionPanelPrefab;

    private const float PANEL_ANIMATION_TIME = 0.8f;

    private ChapterDataManager _chapterDataManager;
    private PlayerDataManager _playerDataManager;
    private DecisionPanel _currentDecisionPanel;
    private Decision _decisionData;
    private bool _isDecisionLocked;

    private void Awake()
    {
        GameManager.Instance.OnGameStateChanged += OnStateChanged;

        _chapterDataManager = ServiceLocator.GetService<ChapterDataManager>();
        _playerDataManager = ServiceLocator.GetService<PlayerDataManager>();
    }

    private void OnDestroy() => GameManager.Instance.OnGameStateChanged -= OnStateChanged;

    //sets game state to ActiveDecisions on button click if decisions are not locked and current game state is Default
    public void DecisionsClickHandler()
    {
        if (!_isDecisionLocked && GameManager.Instance.State == GameState.Default)
        {
            AudioManager.Instance.PlaySFX("phone");
            GameManager.Instance.ChangeGameState(GameState.ActiveDecisions);
        }
    }

    //handles game state change. If Default - updates decisions status, if ActiveDecisions - initializes decision, else - return
    private void OnStateChanged(GameState state)
    {
        if (state == GameState.Default) UpdateDecisionsStatus();

        else if (state == GameState.ActiveDecisions) SetUpMechanic();
    }

    //initializes decision mechanic: sets blackout screen and invokes method to load decision data
    private void SetUpMechanic()
    {
        _blackoutScreen.SetPosition(BlackoutScreenPosition.underBudgetBox);
        _blackoutScreen.OnClick += Exit;
        _blackoutScreen.FadeIn();

        LoadDecision();
    }

    //initializes decision panel with data
    private void LoadDecision()
    {
        _decisionData = _chapterDataManager.GetDecision(_playerDataManager.DecisionID);
        _currentDecisionPanel = Instantiate(_decisionPanelPrefab, _decisionsPanelsTransform);
        _currentDecisionPanel.Initialize(_decisionData);
        _currentDecisionPanel.OnDecisionMade += OptionClickHandler;

        ShowDecisionPanel();
    }

    //animates decision panel appearing
    private void ShowDecisionPanel()
    {
        _currentDecisionPanel.transform.localScale = Vector3.zero;
        _currentDecisionPanel.transform.LeanScale(Vector3.one, PANEL_ANIMATION_TIME).setEaseOutQuart();
    }

    //saves data after player clicked on option button
    private void OptionClickHandler(int pickedOption)
    {
        AudioManager.Instance.PlaySFX("button");

        _playerDataManager.SaveDecision(_decisionData.decisionID, pickedOption);
        _playerDataManager.UpdateDecisionID(_decisionData.decisionID + 1);
        _playerDataManager.UpdateCharacteristics(_decisionData.characteristicUpdates[pickedOption - 1]);
        _budgetBox.UpdateBudget(_decisionData.characteristicUpdates[pickedOption - 1].budget);

        Exit();
    }

    //unsubscribes from events, closes decision panel and sets default game state
    private void Exit()
    {
        _currentDecisionPanel.OnDecisionMade -= OptionClickHandler;
        _currentDecisionPanel.transform.LeanScale(Vector3.zero, PANEL_ANIMATION_TIME).setEaseOutQuart()
            .setOnComplete(() => Destroy(_currentDecisionPanel.gameObject));

        _blackoutScreen.OnClick -= Exit;
        _blackoutScreen.FadeOut(() => GameManager.Instance.ChangeGameState(GameState.Default));
    }

    //defines whether decision is locked or not: if decision isn't locked but condition for it isn't met, increment decision id and invokes method again
    private void UpdateDecisionsStatus()
    {
        int decisionID = _playerDataManager.DecisionID;

        _isDecisionLocked =
            decisionID >= _chapterDataManager.GetDecisionsLength() ||
            _chapterDataManager.GetDecision(decisionID).lockedByLaw >= _playerDataManager.LawID ||
            _chapterDataManager.GetDecision(decisionID).lockedByDialogue >= _playerDataManager.DialogueID;

        if (!_isDecisionLocked && !_playerDataManager.IsConditionMet(_chapterDataManager.GetDecision(decisionID).condition))
        {
            _playerDataManager.UpdateDecisionID(decisionID + 1);
            UpdateDecisionsStatus();
            return;
        }

        _decisionsMark.SetActive(!_isDecisionLocked);
    }
}