using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class LinePanel : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private Image _characterImage;
    [SerializeField] private TextMeshProUGUI _characterName;
    [SerializeField] private TextMeshProUGUI _lineText;
    [SerializeField] private RectTransform _rectTransform;

    private const float ANIMATION_DURATION = 0.2f;

    private bool _allowInteraction;
    private Typewriter _typewriter;

    public UnityEvent OnPanelClick;

    public bool IsTextWriting => _typewriter.IsWriting;

    void Awake() => _typewriter = new Typewriter(_lineText);

    public void Initialize(Sprite image, string name, string text)
    {
        if (_characterImage.sprite == null)
            _characterImage.sprite = image;
        else if (_characterImage.sprite != image)
            SwapCharacterImage(image);

        _characterName.text = name;
        _lineText.text = text;
        _typewriter.StartWriting();

        _allowInteraction = true;
    }

    public void SkipWriting() => _typewriter.SkipWriting();

    private void SwapCharacterImage(Sprite image)
    {
        _allowInteraction = false;
        var defaultPosition = _characterImage.rectTransform.anchoredPosition.y;
        var targetPosition = defaultPosition - _rectTransform.rect.height;

        _characterImage.rectTransform.DOAnchorPosY(targetPosition, ANIMATION_DURATION).SetEase(Ease.InQuart)
            .OnComplete(() =>
            {
                _characterImage.sprite = image;
                _characterImage.rectTransform.DOAnchorPosY(defaultPosition, ANIMATION_DURATION).SetEase(Ease.OutQuart)
                    .OnComplete(() => _allowInteraction = true);
            });
    }

    void IPointerClickHandler.OnPointerClick(PointerEventData eventData)
    {
        if (!_allowInteraction) return;
        OnPanelClick?.Invoke();
    }

    private void OnDisable()
    {
        _characterImage.sprite = null;
    }
}