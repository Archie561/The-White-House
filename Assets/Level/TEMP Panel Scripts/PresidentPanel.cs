using System;
using TMPro;
using UnityEngine;
using UnityEngine.Localization.Components;
using UnityEngine.UI;

/// <summary>
/// Represents president panel from president selection screen
/// </summary>
public class PresidentPanel : MonoBehaviour
{
    [SerializeField] private Button _playButton;
    [SerializeField] private Button _resetButton;

    [SerializeField] private Image _presidentImage;
    [SerializeField] private Image _bonus1Image;
    [SerializeField] private Image _bonus2Image;

    [SerializeField] private LocalizeStringEvent _presidentName;
    [SerializeField] private LocalizeStringEvent _presidentDescription;
    [SerializeField] private LocalizeStringEvent _backstory;
    [SerializeField] private LocalizeStringEvent _bonus1Name;
    [SerializeField] private LocalizeStringEvent _bonus2Name;
    [SerializeField] private LocalizeStringEvent _bonus1Description;
    [SerializeField] private LocalizeStringEvent _bonus2Description;

    [SerializeField] private TextMeshProUGUI _progressValue;
    [SerializeField] private Slider _progressSlider;

    public Action OnPlayButtonClick;
    public Action OnResetButtonClick;

    /// <summary>
    /// Initializes the panel with president data
    /// </summary>
    /// <param name="presidentData">static information about the president</param>
    /// <param name="progress">progress within current president in 0..1 range</param>
    public void Initialize(PresidentData presidentData, float progress)
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

        _progressValue.text = Mathf.Round(progress * 100) + " %";
        _progressSlider.value = progress;
    }

    /// <summary>
    /// Sets to the button object from the editor. To determine the functionality on click, subscribe to the OnPlayButtonClick event 
    /// </summary>
    public void PlayButtonClickHandler() => OnPlayButtonClick?.Invoke();

    /// <summary>
    /// Sets to the button object from the editor. To determine the functionality on click, subscribe to the OnResetButtonClick event 
    /// </summary>
    public void ResetButtonClickHandler() => OnResetButtonClick?.Invoke();

    public void SetPlayButtonInteractable(bool interactable) => _playButton.interactable = interactable;

    public void SetResetButtonInteractable(bool interactable) => _resetButton.interactable = interactable;
}