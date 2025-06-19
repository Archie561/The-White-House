using UnityEngine;

public class LawHint : MonoBehaviour
{
    [SerializeField] private Vector2 _activityRange;
    /// <summary>
    /// Defines the range within the hint will be displayed (0-1) canvas widths.
    /// The X coordinate of the vector represents the lower bound, and the Y coordinate represents the upper bound
    /// </summary>
    public Vector2 ActivityRange
    {
        get => _activityRange;
        set => _activityRange = value;
    }

    [SerializeField] private RectTransform _parentTransform;
    /// <summary>
    /// The RectTransform used as a reference for calculating the position bounds within which the hint can appear
    /// </summary>
    public RectTransform ParentTransform
    {
        get => _parentTransform;
        set => _parentTransform = value;
    }

    [SerializeField] private RectTransform _targetTransform;
    /// <summary>
    /// The RectTransform of the target UI element that the hint is associated with
    /// </summary>
    public RectTransform TargetTransform
    {
        get => _targetTransform;
        set => _targetTransform = value;
    }

    private const float UPDATE_STEP = 0.1f;

    private CanvasGroup _hintCanvasGroup;

    private void Awake() => _hintCanvasGroup = GetComponent<CanvasGroup>();

    private void Update()
    {
        if (TargetTransform == null || ParentTransform == null || ActivityRange == null) return;

        var normalizedPosition = (TargetTransform.anchoredPosition.x + ParentTransform.rect.width * 0.5f) / ParentTransform.rect.width;

        if (normalizedPosition >= ActivityRange.x && normalizedPosition <= ActivityRange.y)
            _hintCanvasGroup.alpha += UPDATE_STEP;
        else
            _hintCanvasGroup.alpha -= UPDATE_STEP;
    }
}