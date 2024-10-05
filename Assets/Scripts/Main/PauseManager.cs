using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Localization.Settings;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseManager : MonoBehaviour
{
    /*-----------------------------UI SECTION-----------------------------*/
    [SerializeField]
    private CanvasGroup _loadingScreen;
    [SerializeField]
    private CanvasGroup _blackoutScreen;
    [SerializeField]
    private Button _backButton;

    [SerializeField]
    private GameObject _pausePanel;
    [SerializeField]
    private Slider _sfxSlider;
    [SerializeField]
    private Slider _musicSlider;
    /*---------------------------END UI SECTION---------------------------*/

    /*--------------------ANIMATION PARAMETERS SECTION--------------------*/
    private bool _isAnimationFinished;
    private float _loadingScreenAnimationTime = 0.8f;
    private float _blackoutScreenAnimationTime = 0.8f;
    private float _panelAnimationTime = 0.8f;
    private float _backButtonAnimationTime = 0.8f;
    /*------------------END ANIMATION PARAMETERS SECTION------------------*/

    /*----------------------OTHER PARAMETERS SECTION----------------------*/
    private bool _languageChangingIsInProcess;
    private int _languageIndex;
    private string _languageCode;
    /*--------------------END OTHER PARAMETERS SECTION--------------------*/

    void OnEnable()
    {
        _languageIndex = LocalizationSettings.AvailableLocales.Locales.IndexOf(LocalizationSettings.SelectedLocale);
        _languageCode = PlayerPrefs.GetString("gameLanguage");

        _sfxSlider.value = PlayerPrefs.GetFloat("sfxVolume", 0.8f);
        _musicSlider.value = PlayerPrefs.GetFloat("musicVolume", 0.8f);

        _isAnimationFinished = false;

        //blackout screen animation
        _blackoutScreen.gameObject.SetActive(true);
        _blackoutScreen.LeanAlpha(1, _blackoutScreenAnimationTime);

        //back button animation
        Vector2 _backButtonDefaultPosition = _backButton.transform.position;
        _backButton.transform.position = new Vector2(_backButton.transform.position.x, Screen.height + _backButton.GetComponent<RectTransform>().rect.size.y * _backButton.GetComponent<RectTransform>().lossyScale.y);
        _backButton.gameObject.SetActive(true);
        _backButton.transform.LeanMove(_backButtonDefaultPosition, _backButtonAnimationTime).setEaseOutQuart();

        //pause panel animation
        _pausePanel.transform.localScale = Vector2.zero;
        _pausePanel.gameObject.SetActive(true);
        _pausePanel.LeanScale(Vector2.one, _panelAnimationTime).setEaseOutQuart().setOnComplete(() => _isAnimationFinished = true);
    }

    public void RightLanguageButtonClickHandler()
    {
        if (_languageChangingIsInProcess)
        {
            return;
        }
        AudioManager.Instance.PlaySFX("button");
        _languageIndex = _languageIndex == LocalizationSettings.AvailableLocales.Locales.Count - 1 ? 0 : _languageIndex + 1;
        StartCoroutine(ChangeLanguage(_languageIndex));
    }
    public void LeftLanguageButtonClickHandler()
    {
        if (_languageChangingIsInProcess)
        {
            return;
        }
        AudioManager.Instance.PlaySFX("button");
        _languageIndex = _languageIndex == 0 ? LocalizationSettings.AvailableLocales.Locales.Count - 1 : _languageIndex - 1;
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

    public void BackButtonClickHandler()
    {
        if (!_isAnimationFinished || _languageChangingIsInProcess)
        {
            return;
        }
        _isAnimationFinished = false;
        AudioManager.Instance.PlaySFX("button");

        //if language was changed - reload scene and load translated chapter
        if (_languageCode != PlayerPrefs.GetString("gameLanguage"))
        {
            ReloadSceneAfterLanguageChange();
            return;
        }

        //blackout screen animation
        _blackoutScreen.LeanAlpha(0, _blackoutScreenAnimationTime);

        //back button animation
        Vector2 backButtonDefaultPosition = _backButton.transform.position;
        _backButton.transform.LeanMoveY(Screen.height + _backButton.GetComponent<RectTransform>().rect.size.y * _backButton.GetComponent<RectTransform>().lossyScale.y, _backButtonAnimationTime).setEaseOutQuart();

        //pause panel animation
        Vector2 panelDefaultPosition = _pausePanel.transform.position;
        _pausePanel.LeanMoveY(-Screen.height / 2, _panelAnimationTime).setEaseOutQuart().setOnComplete(() =>
        {
            _blackoutScreen.gameObject.SetActive(false);
            _backButton.gameObject.SetActive(false);
            _pausePanel.SetActive(false);
            
            _pausePanel.transform.position = panelDefaultPosition;
            _backButton.transform.position = backButtonDefaultPosition;

            gameObject.SetActive(false);
        });
    }

    public void ExitToMenuClickHandler()
    {
        AudioManager.Instance.PlaySFX("button");

        _loadingScreen.gameObject.SetActive(true);
        _loadingScreen.LeanAlpha(1, _loadingScreenAnimationTime).setOnComplete(() =>
        {
            AudioManager.Instance.StopPlaying();
            SceneManager.LoadScene(0);
            Destroy(GameManager.Instance.gameObject);
        });
    }

    private void ReloadSceneAfterLanguageChange()
    {
        _loadingScreen.gameObject.SetActive(true);
        _loadingScreen.LeanAlpha(1, _loadingScreenAnimationTime).setOnComplete(() =>
        {
            AudioManager.Instance.StopPlaying();
            SceneManager.LoadScene(1);
            Destroy(GameManager.Instance.gameObject);
        });
    }
}
