using System;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.Localization.Components;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PresidentSelectorManager : MonoBehaviour
{
    [SerializeField]
    private PresidentData[] _presidents;

    [SerializeField]
    private CanvasGroup _loadingScreen;
    [SerializeField]
    private GameObject _surePanel;
    [SerializeField]
    private Button _leftButton;
    [SerializeField]
    private Button _rightButton;
    [SerializeField]
    private Button _playButton;
    [SerializeField]
    private Image _presidentImage;
    [SerializeField]
    private Image _bonus1Image;
    [SerializeField]
    private Image _bonus2Image;

    [SerializeField]
    private LocalizeStringEvent _presidentName;
    [SerializeField]
    private LocalizeStringEvent _presidentDescription;
    [SerializeField]
    private LocalizeStringEvent _backstory;
    [SerializeField]
    private LocalizeStringEvent _bonus1Name;
    [SerializeField]
    private LocalizeStringEvent _bonus2Name;
    [SerializeField]
    private LocalizeStringEvent _bonus1Description;
    [SerializeField]
    private LocalizeStringEvent _bonus2Description;

    [SerializeField]
    private TextMeshProUGUI _progressValue;
    [SerializeField]
    private Slider _progressSlider;

    private AsyncOperation _loadingSceneOperation;
    private float _loadingScreenAnimationTime = 0.4f;
    private float _panelAnimationTime = 0.4f;

    private bool _presidentSelected;
    private int _presidentIndex;

    private void Start()
    {
        _presidentIndex = 0;
        LoadPresidentData(_presidents[_presidentIndex]);

        _leftButton.interactable = _presidentIndex > 0;
        _rightButton.interactable = _presidentIndex < _presidents.Length - 1;
    }

    private void LoadPresidentData(PresidentData presidentData)
    {
        _presidentImage.sprite = presidentData.presidentSprite;
        _bonus1Image.sprite = presidentData.bonus1Sprite;
        _bonus2Image.sprite = presidentData.bonus2Sprite;

        _presidentName.StringReference.SetReference("UILocalization", presidentData.nameKey);
        _presidentDescription.StringReference.SetReference("UILocalization", presidentData.descriptionKey);
        _backstory.StringReference.SetReference("UILocalization", presidentData.backstoryKey);
        _bonus1Name.StringReference.SetReference("UILocalization", presidentData.bonus1NameKey);
        _bonus2Name.StringReference.SetReference("UILocalization", presidentData.bonus2NameKey);
        _bonus1Description.StringReference.SetReference("UILocalization", presidentData.bonus1DescriptionKey);
        _bonus2Description.StringReference.SetReference("UILocalization", presidentData.bonus2DescriptionKey);

        _progressValue.text = Math.Round(PlayerPrefs.GetFloat(presidentData.nameKey, 0) * 100) + " %";
        _progressSlider.value = PlayerPrefs.GetFloat(presidentData.nameKey, 0);

        _playButton.interactable = PlayerPrefs.GetFloat(presidentData.nameKey, 0) < 1;
    }

    public void RightButtonClickHandler()
    {
        if (_presidentIndex == _presidents.Length - 1 || _presidentSelected)
        {
            return;
        }
        AudioManager.Instance.PlaySFX("button");

        _presidentIndex++;
        LoadPresidentData(_presidents[_presidentIndex]);

        _leftButton.interactable = _presidentIndex > 0;
        _rightButton.interactable = _presidentIndex < _presidents.Length - 1;
    }

    public void LeftButtonClickHandler()
    {
        if (_presidentIndex == 0 || _presidentSelected)
        {
            return;
        }
        AudioManager.Instance.PlaySFX("button");

        _presidentIndex--;
        LoadPresidentData(_presidents[_presidentIndex]);

        _leftButton.interactable = _presidentIndex > 0;
        _rightButton.interactable = _presidentIndex < _presidents.Length - 1;
    }

    public void StartGame()
    {
        if (_presidentSelected)
        {
            return;
        }
        AudioManager.Instance.PlaySFX("button");

        _presidentSelected = true;
        DataManager.CurrentSelectedPresident = _presidents[_presidentIndex].nameKey;

        _loadingSceneOperation = SceneManager.LoadSceneAsync(1);
        _loadingSceneOperation.allowSceneActivation = false;

        _loadingScreen.gameObject.SetActive(true);
        _loadingScreen.LeanAlpha(1, _loadingScreenAnimationTime).setOnComplete(() =>
        {
            _loadingSceneOperation.allowSceneActivation = true;
            AudioManager.Instance.StopPlaying();
        });
    }

    public void ShowResetPanel()
    {
        if (_presidentSelected)
        {
            return;
        }
        AudioManager.Instance.PlaySFX("button");

        _presidentSelected = true;
        _surePanel.transform.localScale = Vector2.zero;
        _surePanel.SetActive(true);
        _surePanel.LeanScale(Vector2.one, _panelAnimationTime).setEaseOutQuart();
    }

    public void ConfirmReset()
    {
        AudioManager.Instance.PlaySFX("button");

        File.Delete(Directory.GetCurrentDirectory() + "/Assets/Story/" + _presidents[_presidentIndex].nameKey + "/PlayerData.json");
        PlayerPrefs.SetFloat(_presidents[_presidentIndex].nameKey, 0);

        LoadPresidentData(_presidents[_presidentIndex]);
        _surePanel.LeanScale(Vector2.zero, _panelAnimationTime).setEaseOutQuart().setOnComplete(() =>
        {
            _surePanel.SetActive(false);
            _presidentSelected = false;
        });
    }

    public void CancelReset()
    {
        AudioManager.Instance.PlaySFX("button");

        _surePanel.LeanScale(Vector2.zero, _panelAnimationTime).setEaseOutQuart().setOnComplete(() =>
        {
            _surePanel.SetActive(false);
            _presidentSelected = false;
        });
    }
}

[Serializable]
public class PresidentData
{
    public string nameKey;
    public string descriptionKey;
    public string backstoryKey;
    public Sprite presidentSprite;

    public Sprite bonus1Sprite;
    public Sprite bonus2Sprite;
    public string bonus1NameKey;
    public string bonus2NameKey;
    public string bonus1DescriptionKey;
    public string bonus2DescriptionKey;
}
