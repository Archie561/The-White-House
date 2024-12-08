using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Localization.Settings;
using UnityEngine.UI;

public class StartScreenHandler : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private RawImage _startingImage;
    [SerializeField] private CanvasGroup _blackoutScreen;
    [SerializeField] private CanvasGroup _tapToContinueText;

    private const float BLACKOUT_SCREEN_ANIMATION_TIME = 0.8f;
    private const float TEXT_ANIMATION_TIME = 1.5f;
    private const float SOUND_DEFAULT_VOLUME = 0.8f;

    private bool _isClicked;

    private void Start() => SetUpGameParameters();

    //initializes localization and sound game parameters
    private void SetUpGameParameters()
    {
        LocalizeGame();

        AudioManager.Instance.SetSFXVolume(PlayerPrefs.GetFloat("sfxVolume", SOUND_DEFAULT_VOLUME));
        AudioManager.Instance.SetMusicVolume(PlayerPrefs.GetFloat("musicVolume", SOUND_DEFAULT_VOLUME));
        AudioManager.Instance.SetMusic("main-theme");

        _blackoutScreen.LeanAlpha(0, BLACKOUT_SCREEN_ANIMATION_TIME);
        _tapToContinueText.LeanAlpha(0, TEXT_ANIMATION_TIME).setLoopPingPong();
    }

    //determines the system language for game translation and translates game
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
                    PlayerPrefs.SetString("gameLanguage", "en");
                    break;
                default:
                    PlayerPrefs.SetString("gameLanguage", "en");
                    break;
            }

            PlayerPrefs.Save();
        }

        StartCoroutine(SetLanguage(PlayerPrefs.GetString("gameLanguage")));
    }

    private IEnumerator SetLanguage(string code)
    {
        yield return LocalizationSettings.InitializationOperation;
        LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.GetLocale(code);
    }

    //removes starting screen after click on it
    public void OnPointerClick(PointerEventData eventData)
    {
        if (_isClicked) return;
        _isClicked = true;

         _blackoutScreen.LeanAlpha(1, BLACKOUT_SCREEN_ANIMATION_TIME).setOnComplete(() =>
        {
            LeanTween.cancel(_tapToContinueText.gameObject);
            _tapToContinueText.gameObject.SetActive(false);
            _startingImage.gameObject.SetActive(false);

            _blackoutScreen.LeanAlpha(0, BLACKOUT_SCREEN_ANIMATION_TIME).setOnComplete(() => gameObject.SetActive(false)) ;
        });
    }
}