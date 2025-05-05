using UnityEngine;
using UnityEngine.Localization.Settings;

public class ChapterTransitionManager : MonoBehaviour
{
    [SerializeField] private BlackoutScreen _blackoutScreen;
    [SerializeField] private GameObject _endOfChapterPanel;

    private const float PANEL_ANIMATION_TIME = 0.8f;

    private PlayerDataManager _playerDataManager;
    private ChapterDataManager _chapterDataManager;

    private void Awake()
    {
        GameManager.Instance.OnGameStateChanged += OnStateChanged;

        _playerDataManager = ServiceLocator.GetService<PlayerDataManager>();
        _chapterDataManager = ServiceLocator.GetService<ChapterDataManager>();
    }

    private void OnDestroy() => GameManager.Instance.OnGameStateChanged -= OnStateChanged;

    private void OnStateChanged(GameState state)
    {
        if (state == GameState.EndOfChapter) ShowEndOfChapterPanel();
    }

    private void ShowEndOfChapterPanel()
    {
        _blackoutScreen.SetPosition(BlackoutScreenPosition.overBudgetBox);
        _blackoutScreen.FadeIn();

        _endOfChapterPanel.SetActive(true);
        _endOfChapterPanel.transform.localScale = Vector3.zero;
        _endOfChapterPanel.transform.LeanScale(Vector3.one, PANEL_ANIMATION_TIME).setEaseOutQuart();
    }

    public void ContinueClickHandler()
    {
/*        _playerDataManager.UpdateGameOver(IsGameOver());

        //TEMPORARY CHECK
        if (_playerDataManager.ChapterID == _playerDataManager.ChaptersAmount - 1)
        {
            UpdateChapterData();
            LevelManager.Instance.LoadScene("Main");
            return;
        }

        if (_playerDataManager.GameOver || _chapterDataManager.GetEndingCutscenes().Length > 0)
            LevelManager.Instance.LoadScene("Cutscene");
        else
            LoadNextChapter();*/
    }

    private bool IsGameOver()
    {
        int criticalValue = CharacteristicsManager.CHARACTERISTIC_CRITICAL_VALUE;

        bool budgetFailure = GameDataManager.PlayerData.GetCharacteristic(Characteristic.Budget) < 0;

        /*        bool domesticFailure = (characteristics.science + characteristics.medicine + characteristics.welfare + characteristics.ecology +
                    characteristics.education + characteristics.infrastructure) / 6 < criticalValue;

                bool foreignFailure = (characteristics.europeanUnion + characteristics.unitedKingdom + characteristics.china + characteristics.CIS +
                    characteristics.africa + characteristics.OPEC) / 6 < criticalValue;

                bool armyFailure = (characteristics.infantry + characteristics.airForces + characteristics.machinery + characteristics.navy)
                    / 4 < criticalValue;*/

        return budgetFailure; //|| domesticFailure || foreignFailure || armyFailure;
    }

    private void LoadNextChapter()
    {
        AudioManager.Instance.StopPlaying();
        UpdateChapterData();
/*
        var newChapter = new ChapterDataManager(_playerDataManager.ActivePresident, _playerDataManager.ChapterID);
        ServiceLocator.RegisterService(newChapter);

        string localizedPresidentName = LocalizationSettings.StringDatabase.GetLocalizedString("UILocalization", _playerDataManager.ActivePresident);
        string localizedMonthName = LocalizationSettings.StringDatabase.GetLocalizedString("UILocalization", $"Month{_playerDataManager.ChapterID}");*/

        //LevelManager.Instance.LoadScene("Main", localizedPresidentName, localizedMonthName);
    }

    private void UpdateChapterData()
    {
/*        _playerDataManager.UpdateChapterID(_playerDataManager.ChapterID + 1);
        _playerDataManager.UpdateDialogueID(0);
        _playerDataManager.UpdateLawID(0);
        _playerDataManager.UpdateDecisionID(0);
        _playerDataManager.UpdateStartingCutscenesShown(false);

        _playerDataManager.SaveData();*/
    }
}