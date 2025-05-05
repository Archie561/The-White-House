using DG.Tweening;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// Represents a draggable panel for displaying law information.
/// </summary>
public class LawPanel : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [Header("UI References")]
    [SerializeField] private Image _backgroundImage;
    [SerializeField] private TextMeshProUGUI _headerText;
    [SerializeField] private TextMeshProUGUI _mainText;
    [SerializeField] private TextMeshProUGUI _detailsText;
    [SerializeField] private TextMeshProUGUI _signatureText;

    public Law LawData {  get; private set; }

    private RectTransform _rectTransform;
    private Canvas _parentCanvas;
    private Vector2 _dragOffset;
    private bool _isDragging;
    private bool _isInteractable;

    public event Action DragEnded;

    private void Awake()
    {
        _rectTransform = GetComponent<RectTransform>();
        _parentCanvas = GetComponentInParent<Canvas>();
    }

    public void Initialize(Law law)
    {
        LawData = law;

        _headerText.text = $"{LawData.header} (Law ID: {LawData.lawID})";
        _mainText.text = LawData.mainText;
        _detailsText.text = LawData.detailedText;
        _signatureText.text = LawData.preparedBy;
    }

    public void AnimateAppearing(float duration)
    {
        _isInteractable = false;
        _rectTransform.localScale = Vector3.zero;
        _rectTransform.DOScale(Vector3.one, duration).SetEase(Ease.OutQuart).OnComplete(() => _isInteractable = true);
    }

    public void MoveTo(Vector2 position, float duration, bool allowInterruption = false, Action onComplete = null)
    {
        _isInteractable = allowInterruption;
        _rectTransform.DOAnchorPos(position, duration).SetEase(Ease.OutQuart).OnComplete(() =>
        {
            _isInteractable = true;
            onComplete?.Invoke();
        });
    }

    void IBeginDragHandler.OnBeginDrag(PointerEventData eventData)
    {
        if (!_isInteractable) return;
        _isDragging = true;

        if (DOTween.IsTweening(_rectTransform))
            _rectTransform.DOKill();

        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            _rectTransform,
            eventData.position,
            eventData.pressEventCamera,
            out _dragOffset
        );
    }

    void IDragHandler.OnDrag(PointerEventData eventData)
    {
        if (!_isDragging) return;

        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
            _parentCanvas.transform as RectTransform,
            eventData.position,
            eventData.pressEventCamera,
            out var localPoint))
        {
            _rectTransform.anchoredPosition = new Vector2(localPoint.x - _dragOffset.x, _rectTransform.anchoredPosition.y);
        }
    }

    void IEndDragHandler.OnEndDrag(PointerEventData eventData)
    {
        if (!_isDragging) return;

        _isDragging = false;
        DragEnded?.Invoke();
    }
}