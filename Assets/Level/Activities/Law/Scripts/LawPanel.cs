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

    /// <summary>
    /// Initializes the LawPanel with the provided law data.
    /// </summary>
    /// <param name="law">Data to initialize</param>
    public void Initialize(Law law)
    {
        LawData = law;

        _headerText.text = LawData.header;
        _mainText.text = LawData.mainText;
        _detailsText.text = LawData.detailedText;
        _signatureText.text = LawData.preparedBy;
    }

    /// <summary>
    /// Plays the apeearing animation for the panel.
    /// </summary>
    /// <param name="duration"></param>
    public void AnimateAppearing(float duration)
    {
        _isInteractable = false;
        _rectTransform.localScale = Vector3.zero;
        _rectTransform.DOScale(Vector3.one, duration).SetEase(Ease.OutQuart).OnComplete(() => _isInteractable = true);
    }

    /// <summary>
    /// Moves the panel to the specified position over a given duration.
    /// </summary>
    /// <param name="position"></param>
    /// <param name="duration"></param>
    /// <param name="allowInterruption">Determines whether the animation can be interrupted</param>
    /// <param name="onComplete">Callback to invoke on complete</param>
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