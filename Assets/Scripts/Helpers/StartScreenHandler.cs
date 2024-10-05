using System.Collections;
using UnityEngine;
using UnityEngine.Localization.Settings;

public class StartScreenHandler : MonoBehaviour
{
    [SerializeField]
    private CanvasGroup _loadingScreen;
    private CanvasGroup _tapToContinueText;

    private float _loadingScreenAnimationTime = 0.8f;
    private float _textAnimationTime = 1.5f; 

    private void Start()
    {
        LocalizeGame();

        _tapToContinueText = GetComponentInChildren<CanvasGroup>();
        _tapToContinueText.LeanAlpha(0, _textAnimationTime).setLoopPingPong();
    }

    private void LocalizeGame()
    {
        if (string.IsNullOrEmpty(PlayerPrefs.GetString("gameLanguage")))
        {
            switch (UnityEngine.Device.Application.systemLanguage)
            {
                case SystemLanguage.English:
                    PlayerPrefs.SetString("gameLanguage", "en");
                    break;
                case SystemLanguage.Ukrainian:
                    PlayerPrefs.SetString("gameLanguage", "uk");
                    break;
                default:
                    PlayerPrefs.SetString("gameLanguage", "en");
                    break;
            }
        }
        StartCoroutine(SetLanguage(PlayerPrefs.GetString("gameLanguage")));
    }

    private IEnumerator SetLanguage(string code)
    {
        yield return LocalizationSettings.InitializationOperation;
        LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.GetLocale(code);
        SetupGameParameters();
    }

    private void SetupGameParameters()
    {
        _loadingScreen.LeanAlpha(0, _loadingScreenAnimationTime).setOnComplete(() => _loadingScreen.gameObject.SetActive(false));

        AudioManager.Instance.SetSFXVolume(PlayerPrefs.GetFloat("sfxVolume", 0.8f));
        AudioManager.Instance.SetMusicVolume(PlayerPrefs.GetFloat("musicVolume", 0.8f));
        AudioManager.Instance.SetMusic("main-theme");
    }

    public void ClickHandler()
    {
        _loadingScreen.gameObject.SetActive(true);
        _loadingScreen.LeanAlpha(1, _loadingScreenAnimationTime).setOnComplete(() =>
        {
            LeanTween.cancel(_tapToContinueText.gameObject);
            gameObject.SetActive(false);
            _loadingScreen.LeanAlpha(0, _loadingScreenAnimationTime).setOnComplete(() => _loadingScreen.gameObject.SetActive(false));
        });
    }
}
