using UnityEngine;

public class LawHint : MonoBehaviour
{
    /// <summary>
    /// Defines the range within the hint will be displayed (0-1) canvas widths.
    /// The X coordinate of the vector represents the lower bound, and the Y coordinate represents the upper bound
    /// </summary>
    [SerializeField] public Vector2 ActivityRange { get; set; }
    /// <summary>
    /// The RectTransform used as a reference for calculating the position bounds within which the hint can appear
    /// </summary>
    [SerializeField] public RectTransform ParentTransform { get; set; }
    /// <summary>
    /// The RectTransform of the target UI element that the hint is associated with
    /// </summary>
    [SerializeField] public RectTransform TargetTransform { get; set; }

    private const float UPDATE_STEP = 0.01f;

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