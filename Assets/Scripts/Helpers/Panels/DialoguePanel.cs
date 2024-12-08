using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// Represents the dialogue panel object
/// </summary>
public class DialoguePanel : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private GameObject _characterNameBox;
    [SerializeField] private GameObject _replicaBox;
    [SerializeField] private GameObject _choiceBox;
    [SerializeField] private Image _characterImage;
    [SerializeField] private TextMeshProUGUI _characterName;
    [SerializeField] private TextMeshProUGUI _replicaText;
    [SerializeField] private TextMeshProUGUI _firstChoiceText;
    [SerializeField] private TextMeshProUGUI _secondChoiceText;

    private const float CHARACTER_ANIMATION_TIME = 0.3f;

    private float _characterOffscreenPositionY;
    private float _characterDefaultPositionY;

    private bool _isChoice;
    private bool _isAnimationFinished;
    private Typewriter _typewriter;

    public bool IsTextWrining => _typewriter.IsWriting;
    public event Action OnReplicaClick;
    public event Action<int> OnChoiceClick;

    //initializes fields and sets character image position offscreen
    private void Start()
    {
        _characterDefaultPositionY = _characterImage.transform.position.y;
        _characterOffscreenPositionY = _characterImage.rectTransform.rect.size.y * _characterImage.rectTransform.lossyScale.y * -0.5f;
        _typewriter = new Typewriter(_replicaText);

        _characterImage.transform.position = new Vector2(_characterImage.transform.position.x, _characterOffscreenPositionY);
    }

    /// <summary>
    /// Shows choice box with the specific parameters
    /// </summary>
    /// <param name="characterSprite">sprite for the image of the author of the choice (typically - an active president)</param>
    /// <param name="characterName">name of the author of the choice (typically - an active president)</param>
    /// <param name="firstChoice">text of the first choice</param>
    /// <param name="secondChoice">text of the second choice</param>
    public void ShowChoice(Sprite characterSprite, string characterName, string firstChoice, string secondChoice)
    {
        _isChoice = true;
        _replicaBox.SetActive(false);
        _choiceBox.SetActive(true);

        if (_characterImage.sprite != characterSprite) SwapCharacters(characterSprite);
        _characterName.text = characterName;
        _firstChoiceText.text = firstChoice;
        _secondChoiceText.text = secondChoice;
    }

    /// <summary>
    /// Shows replica box with the specific parameters
    /// </summary>
    /// <param name="characterSprite">sprite for the image of the author of the replica</param>
    /// <param name="characterName">name of the author of the replica</param>
    /// <param name="replica">text of the replica</param>
    public void ShowReplica(Sprite characterSprite, string characterName, string replica)
    {
        if (_isChoice)
        {
            _choiceBox.SetActive(false);
            _replicaBox.SetActive(true);
            _isChoice = false;
        }

        if (_characterImage.sprite != characterSprite) SwapCharacters(characterSprite);
        _characterName.text = characterName;
        _replicaText.text = replica;

        _typewriter.StartWriting();
    }

    /// <summary>
    /// Animates the hiding of the dialogue panel
    /// </summary>
    public void HidePanel()
    {
        var activeBox = _isChoice ? _choiceBox : _replicaBox;
        activeBox.LeanScale(Vector3.zero, CHARACTER_ANIMATION_TIME).setEaseInQuart();
        _characterNameBox.transform.LeanScale(Vector3.zero, CHARACTER_ANIMATION_TIME).setEaseInQuart();
        _characterImage.transform.LeanMoveY(_characterOffscreenPositionY, CHARACTER_ANIMATION_TIME).setEaseInQuart();
    }

    //animates the disappearance of the previous character and the appearance of a new one
    private void SwapCharacters(Sprite characterSprite)
    {
        _isAnimationFinished = false;

        _characterImage.transform.LeanMoveY(_characterOffscreenPositionY, CHARACTER_ANIMATION_TIME).setEaseInQuart().setOnComplete(() =>
        {
            _characterImage.sprite = characterSprite;
            _characterImage.transform.LeanMoveY(_characterDefaultPositionY, CHARACTER_ANIMATION_TIME)
            .setEaseOutQuart().setOnComplete(() => _isAnimationFinished = true);
        });
    }

    /// <summary>
    /// Handles cliks on dialogue panel while replicas showing. Ignores clicks when a choice box is active or animations are ongoing
    /// </summary>
    /// <param name="eventData"></param>
    public void OnPointerClick(PointerEventData eventData)
    {
        if (_isAnimationFinished && !_isChoice)
            OnReplicaClick?.Invoke();
    }

    /// <summary>
    /// Handles cliks on choice buttons. Ignores clicks when animation is ongoing. This function attached to the buttons from the editor
    /// </summary>
    /// <param name="pickedChoice"></param>
    public void OnChoiceButtonClick(int pickedChoice)
    {
        if (_isAnimationFinished)
            OnChoiceClick?.Invoke(pickedChoice);
    }

    /// <summary>
    /// Skips text writing animation if active
    /// </summary>
    public void SkipTextWriting() => _typewriter.SkipWriting();
}