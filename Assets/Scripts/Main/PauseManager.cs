using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Localization.Settings;
using UnityEngine.UI;

public class PauseManager : MonoBehaviour
{
    [SerializeField] private BlackoutScreen _blackoutScreen;
    [SerializeField] private GameObject _pausePanel;
    [SerializeField] private Slider _sfxSlider;
    [SerializeField] private Slider _musicSlider;

    private const float PANEL_ANIMATION_TIME = 0.8f;

    private bool _languageChangingIsInProcess;
    private int _languageIndex;
    private string _languageCode;
    private int languages_count;

    private void Start() => SetUpPause();

    private void SetUpPause()
    {
        languages_count = LocalizationSettings.AvailableLocales.Locales.Count;

        _languageIndex = LocalizationSettings.AvailableLocales.Locales.IndexOf(LocalizationSettings.SelectedLocale);
        _languageCode = PlayerPrefs.GetString("gameLanguage");

        _sfxSlider.value = PlayerPrefs.GetFloat("sfxVolume", 0.8f);
        _musicSlider.value = PlayerPrefs.GetFloat("musicVolume", 0.8f);
    }

    public void PauseClickHandler()
    {
        if (GameManager.Instance.State == GameState.Default)
        {
            AudioManager.Instance.PlaySFX("button");
            GameManager.Instance.ChangeGameState(GameState.Pause);
            ShowPausePanel();
        }
    }

    private void ShowPausePanel()
    {
        _blackoutScreen.SetPosition(BlackoutScreenPosition.overBudgetBox);
        _blackoutScreen.OnClick += ExitPause;
        _blackoutScreen.FadeIn();

        _pausePanel.gameObject.SetActive(true);
        _pausePanel.transform.localScale = Vector3.zero;
        _pausePanel.LeanScale(Vector3.one, PANEL_ANIMATION_TIME).setEaseOutQuart();
    }

    public void ChangeLanguageButtonClickHandler(bool leftButtonClicked)
    {
        if (_languageChangingIsInProcess) return;

        AudioManager.Instance.PlaySFX("button");

        if (leftButtonClicked) _languageIndex = (_languageIndex - 1 + languages_count) % languages_count;
        else _languageIndex = (_languageIndex + 1) % languages_count;

        StartCoroutine(ChangeLanguage(_languageIndex));
    }

    private IEnumerator ChangeLanguage(int index)
    {
        _languageChangingIsInProcess = true;

        yield return LocalizationSettings.InitializationOperation;
        LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[index];
        PlayerPrefs.SetString("gameLanguage", LocalizationSettings.AvailableLocales.Locales[index].Identifier.Code);

        _languageChangingIsInProcess = false;
    }

    public void ChangeSFXVolume()
    {
        AudioManager.Instance.SetSFXVolume(_sfxSlider.value);
        PlayerPrefs.SetFloat("sfxVolume", _sfxSlider.value);
    }

    public void ChangeMusicVolume()
    {
        AudioManager.Instance.SetMusicVolume(_musicSlider.value);
        PlayerPrefs.SetFloat("musicVolume", _musicSlider.value);
    }

    public void TestSFXVolume()
    {
        AudioManager.Instance.PlaySFX("button");
    }

    public void InstagramClickHandler()
    {
        AudioManager.Instance.PlaySFX("button");
        Application.OpenURL("https://www.instagram.com/");
    }

    public void RateUsClickHandler()
    {
        AudioManager.Instance.PlaySFX("button");
        Application.OpenURL("https://play.google.com/store/");
    }

    public void ExitPause()
    {
        if (_languageChangingIsInProcess) return;

        //if language was changed - reload scene and load translated chapter
        if (_languageCode != PlayerPrefs.GetString("gameLanguage"))
        {
            ReloadSceneAfterLanguageChange();
            return;
        }

        _pausePanel.LeanScale(Vector3.zero, PANEL_ANIMATION_TIME).setEaseOutQuart().setOnComplete(() => _pausePanel.gameObject.SetActive(true));

        _blackoutScreen.OnClick -= ExitPause;
        _blackoutScreen.FadeOut(() => GameManager.Instance.ChangeGameState(GameState.Default));
    }

    private void ReloadSceneAfterLanguageChange()
    {
        AudioManager.Instance.StopPlaying();

        var playerDataManager = ServiceLocator.GetService<PlayerDataManager>();
        //var chapterDataManager = new ChapterDataManager(playerDataManager.ActivePresident, playerDataManager.ChapterID);
        //ServiceLocator.RegisterService(chapterDataManager);

        string translatedPresidentName = LocalizationSettings.StringDatabase.GetLocalizedString("UILocalization", playerDataManager.ActivePresident);
        //string translatedChapterName = LocalizationSettings.StringDatabase.GetLocalizedString("UILocalization", $"Month{playerDataManager.ChapterID}");
        //LevelManager.Instance.LoadScene("Main", translatedPresidentName, translatedChapterName);
    }

    public void ExitToMenu()
    {
        AudioManager.Instance.PlaySFX("button");
        LevelManager.Instance.LoadScene("Menu");
    }
}
