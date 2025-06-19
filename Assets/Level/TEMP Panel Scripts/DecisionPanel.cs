using System;
using System.Collections;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// Represents the decision panel object
/// </summary>
public class DecisionPanel : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private Image _characterImage;
    [SerializeField] private TextMeshProUGUI _characterName;
    [SerializeField] private TextMeshProUGUI _decisionText;
    [SerializeField] private Button[] _optionButtons;
    [SerializeField] private TextMeshProUGUI[] _optionTexts;

    private const float BUTTON_ANIMATION_TIME = 0.3f;

    private Typewriter _typewriter;
    private int _optionsCount;
    private bool _isDecisionMade;

    public Action<int> OnDecisionMade;

    /// <summary>
    /// Initializes the decision panel with data
    /// </summary>
    /// <param name="decisionData">Data to initilize</param>
    public void Initialize(Decision decisionData)
    {
        _characterImage.sprite = Resources.Load<Sprite>("Textures/DecisionCharacters/" + decisionData.imageName);
        _characterName.text = decisionData.characterName;
        _decisionText.text = decisionData.text;

        _optionsCount = decisionData.options.Length;
        for (int i = 0; i < _optionsCount; i++)
            _optionTexts[i].text = decisionData.options[i];

        _typewriter = new Typewriter(_decisionText);
        _typewriter.OnWritingFinished += DisplayOptions;
        _typewriter.StartWriting();
    }

    //invokes an event to handle option click in the DecisionManager
    public void OptionClickHandler(int pickedOption)
    {
        if (_isDecisionMade) return;
        _isDecisionMade = true;

        OnDecisionMade?.Invoke(pickedOption);
    }

    //displays options buttons one after another
    private void DisplayOptions()
    {
        StartCoroutine(DisplayOptionsCoroutine());

        IEnumerator DisplayOptionsCoroutine()
        {
            for (int i = 0; i < _optionsCount; i++)
            {
                _optionButtons[i].transform.localScale = Vector3.zero;
                _optionButtons[i].gameObject.SetActive(true);
                _optionButtons[i].transform.DOScale(Vector3.one, BUTTON_ANIMATION_TIME).SetEase(Ease.OutQuart);
                yield return new WaitForSeconds(BUTTON_ANIMATION_TIME);
            }
        }
    }

    private void OnDestroy() => _typewriter.OnWritingFinished -= DisplayOptions;

    //skips writing animation on panel click
    public void OnPointerClick(PointerEventData eventData) => _typewriter.SkipWriting();
}