using UnityEngine;

public class CharacteristicsManager : MonoBehaviour
{
    [SerializeField] private BaseCharacteristicPanel[] _characteristicsPanels;
    [SerializeField] private GameObject _domesticPolicyMark;
    [SerializeField] private GameObject _foreignPolicyMark;
    [SerializeField] private GameObject _armyMark;
    [SerializeField] private BlackoutScreen _blackoutScreen;

    public static int CHARACTERISTIC_CRITICAL_VALUE = 10;
    private const float PANEL_ANIMATION_TIME = 0.8f;

    private BaseCharacteristicPanel _currentCharacteristicPanel;
    private PlayerDataManager _playerDataManager;

    private void Start()
    {
        GameManager.Instance.OnGameStateChanged += OnStateChanged;
        _playerDataManager = ServiceLocator.GetService<PlayerDataManager>();
    }

    private void OnDestroy() => GameManager.Instance.OnGameStateChanged -= OnStateChanged;

    //handles game state change. If Default - updates characteristics marks, if ActiveCharacteristics - shows required characteristics panel
    //which is already initialized on characteristics button click, else - quit
    private void OnStateChanged(GameState state)
    {
        if (state == GameState.Default) UpdateCharacteristicsMarks();

        else if (state == GameState.ActiveCharacteristics) SetUpMechanic();
    }

    //initializes the characteristics data for the required panel and sets game state to ActiveCharacteristics if current state is Default
    public void CharacteristicsClickHandler(int characteristicsGroupId)
    {
        if (GameManager.Instance.State == GameState.Default)
        {
            AudioManager.Instance.PlaySFX("button");

            _currentCharacteristicPanel = _characteristicsPanels[characteristicsGroupId];
            _currentCharacteristicPanel.Initialize(_playerDataManager.Characteristics, CHARACTERISTIC_CRITICAL_VALUE);

            GameManager.Instance.ChangeGameState(GameState.ActiveCharacteristics);
        }
    }

    //sets blackout screen and invokes method to start appearing animation of the panel
    private void SetUpMechanic()
    {
        _blackoutScreen.SetPosition(BlackoutScreenPosition.overBudgetBox);
        _blackoutScreen.OnClick += Exit;
        _blackoutScreen.FadeIn();

        ShowCharacteristicsPanel();
    }

    //animates the appearing of characteristics panel
    private void ShowCharacteristicsPanel()
    {
        _currentCharacteristicPanel.gameObject.SetActive(true);
        _currentCharacteristicPanel.transform.localScale = Vector3.zero;
        _currentCharacteristicPanel.transform.LeanScale(Vector3.one, PANEL_ANIMATION_TIME).setEaseOutQuart();
    }

    //activates the warning sign next to the button of each characteristics group if any of its characteristics is below the critical value
    private void UpdateCharacteristicsMarks()
    {
        Characteristics characteristics = _playerDataManager.Characteristics;

        _domesticPolicyMark.SetActive(
            characteristics.science <= CHARACTERISTIC_CRITICAL_VALUE ||
            characteristics.welfare <= CHARACTERISTIC_CRITICAL_VALUE ||
            characteristics.education <= CHARACTERISTIC_CRITICAL_VALUE ||
            characteristics.medicine <= CHARACTERISTIC_CRITICAL_VALUE ||
            characteristics.ecology <= CHARACTERISTIC_CRITICAL_VALUE ||
            characteristics.infrastructure <= CHARACTERISTIC_CRITICAL_VALUE);

        _foreignPolicyMark.SetActive(
            characteristics.europeanUnion <= CHARACTERISTIC_CRITICAL_VALUE ||
            characteristics.china <= CHARACTERISTIC_CRITICAL_VALUE ||
            characteristics.africa <= CHARACTERISTIC_CRITICAL_VALUE ||
            characteristics.unitedKingdom <= CHARACTERISTIC_CRITICAL_VALUE ||
            characteristics.CIS <= CHARACTERISTIC_CRITICAL_VALUE ||
            characteristics.OPEC <= CHARACTERISTIC_CRITICAL_VALUE);

        _armyMark.SetActive(
            characteristics.navy <= CHARACTERISTIC_CRITICAL_VALUE ||
            characteristics.airForces <= CHARACTERISTIC_CRITICAL_VALUE ||
            characteristics.infantry <= CHARACTERISTIC_CRITICAL_VALUE ||
            characteristics.machinery <= CHARACTERISTIC_CRITICAL_VALUE);
    }

    //closes characteristics panel and sets default game state
    public void Exit()
    {
        _currentCharacteristicPanel.transform.LeanScale(Vector3.zero, PANEL_ANIMATION_TIME).setEaseOutQuart()
            .setOnComplete(() => _currentCharacteristicPanel.gameObject.SetActive(false));

        _blackoutScreen.OnClick -= Exit;
        _blackoutScreen.FadeOut(() => GameManager.Instance.ChangeGameState(GameState.Default));
    }
}