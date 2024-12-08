using System.IO;
using UnityEngine;
using UnityEngine.Localization.Settings;
using UnityEngine.UI;

public class PresidentSelectorManager : MonoBehaviour
{
    [SerializeField] private PresidentData[] _presidents;
    [SerializeField] private PresidentPanel _presidentPanel;
    [SerializeField] private SurePanel _surePanel;

    [SerializeField] private Button _nextButton;
    [SerializeField] private Button _previousButton;

    private const float PANEL_ANIMATION_TIME = 0.4f;

    private bool _presidentSelected;
    private int _presidentIndex = 0;

    //loads first president data and subscribes to events
    private void Start()
    {
        LoadPresidentData(_presidents[_presidentIndex]);
        SetInteractableSelectionButtons();

        _presidentPanel.OnPlayButtonClick += StartGame;
        _presidentPanel.OnResetButtonClick += ShowResetPanel;
        _surePanel.OnConfirmButtonClick += ConfirmReset;
        _surePanel.OnCancelButtonClick += CloseResetPanel;
    }

    //initializes president panel with data and determines if its button interactable
    private void LoadPresidentData(PresidentData presidentData)
    {
        float progress = PlayerPrefs.GetFloat(presidentData.nameKey, 0);

        _presidentPanel.Initialize(presidentData, progress);
        _presidentPanel.SetPlayButtonInteractable(progress < 1);
        _presidentPanel.SetResetButtonInteractable(progress > 0);
    }

    //navigates which president should be displayed
    public void SelectionButtonClickHandler(bool next)
    {
        if (_presidentSelected) return;

        AudioManager.Instance.PlaySFX("button");

        _presidentIndex += next ? 1 : -1;

        LoadPresidentData(_presidents[_presidentIndex]);
        SetInteractableSelectionButtons();
    }

    //initializes player and chapter data managers with current selected president data and starts the game
    private void StartGame()
    {
        if (_presidentSelected) return;
        _presidentSelected = true;

        AudioManager.Instance.StopPlaying();
        AudioManager.Instance.PlaySFX("button");

        var selectedPresident = _presidents[_presidentIndex].nameKey;
        var playerDataManager = new PlayerDataManager(selectedPresident, _presidents[_presidentIndex].chaptersAmount);
        var chapterDataManager = new ChapterDataManager(selectedPresident, playerDataManager.ChapterID);

        ServiceLocator.RegisterService(playerDataManager);
        ServiceLocator.RegisterService(chapterDataManager);

        string localizedPresidentName = LocalizationSettings.StringDatabase.GetLocalizedString("UILocalization", selectedPresident);
        string localizedMonthName = LocalizationSettings.StringDatabase.GetLocalizedString("UILocalization", $"Month{playerDataManager.ChapterID}");

        LevelManager.Instance.LoadScene("Main", localizedPresidentName, localizedMonthName);
    }

    //animates reset panel appearing on click on reset button
    private void ShowResetPanel()
    {
        if (_presidentSelected) return;
        _presidentSelected = true;

        AudioManager.Instance.PlaySFX("button");

        _surePanel.gameObject.SetActive(true);
        _surePanel.transform.localScale = Vector2.zero;
        _surePanel.transform.LeanScale(Vector2.one, PANEL_ANIMATION_TIME).setEaseOutQuart();
    }

    //deletes president data file on click on confirm reset button
    public void ConfirmReset()
    {
        File.Delete(Application.persistentDataPath + $"/{_presidents[_presidentIndex].nameKey}PlayerData.json");
        PlayerPrefs.SetFloat(_presidents[_presidentIndex].nameKey, 0);
        PlayerPrefs.Save();

        LoadPresidentData(_presidents[_presidentIndex]);
        CloseResetPanel();
    }

    //animates reset panel disappearing on click on cancel reset button
    private void CloseResetPanel()
    {
        AudioManager.Instance.PlaySFX("button");

        _surePanel.transform.LeanScale(Vector2.zero, PANEL_ANIMATION_TIME).setEaseOutQuart().setOnComplete(() =>
        {
            _surePanel.gameObject.SetActive(false);
            _presidentSelected = false;
        });
    }

    //blocks interaction with selection buttons depending on the president index value
    private void SetInteractableSelectionButtons()
    {
        _previousButton.interactable = _presidentIndex > 0;
        _nextButton.interactable = _presidentIndex < _presidents.Length - 1;
    }

    //unsubscribes from events
    private void OnDestroy()
    {
        _presidentPanel.OnPlayButtonClick -= StartGame;
        _presidentPanel.OnResetButtonClick -= ShowResetPanel;
        _surePanel.OnConfirmButtonClick -= ConfirmReset;
        _surePanel.OnCancelButtonClick -= CloseResetPanel;
    }
}