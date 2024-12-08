using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class BlackoutScreen : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private CanvasGroup _blackoutScreen;
    [SerializeField] private Transform _underBudgetTransform;
    [SerializeField] private Transform _overBudgetTransform;

    private const float BLACKOUT_SCREEN_ANIMATION_TIME = 0.8f;

    private bool _isAnimationFinished;
    private BlackoutScreenPosition _currentPosition = BlackoutScreenPosition.underBudgetBox;

    /// <summary>
    /// Invokes on click on the blackout screen or exit button, if it is enabled
    /// </summary>
    public event Action OnClick;

    /// <summary>
    /// Blocks raycasts and starts fade in animation. Invokes callback on complete if set
    /// </summary>
    /// <param name="callback">Callback to invoke</param>
    public void FadeIn(Action callback = null)
    {
        _blackoutScreen.blocksRaycasts = true;
        _isAnimationFinished = false;

        _blackoutScreen.LeanAlpha(1, BLACKOUT_SCREEN_ANIMATION_TIME).setOnComplete(() =>
        {
            callback?.Invoke();
            _isAnimationFinished = true;
        });
    }

    /// <summary>
    /// Starts fade out animation if animation is not playing. Invokes callback on complete if set and resets blackout screen parameters to default
    /// </summary>
    /// <param name="callback">Callback to invoke</param>
    public void FadeOut(Action callback = null)
    {
        if (!_isAnimationFinished) return;
        _isAnimationFinished = false;

        _blackoutScreen.LeanAlpha(0, BLACKOUT_SCREEN_ANIMATION_TIME).setOnComplete(() =>
        {
            callback?.Invoke();
            _blackoutScreen.blocksRaycasts = false;
            _isAnimationFinished = true;
        });
    }

    /// <summary>
    /// Handles clicks on blackout screen by involing onClick event
    /// </summary>
    /// <param name="eventData"></param>
    public void OnPointerClick(PointerEventData eventData)
    {
        if (!_isAnimationFinished) return;
        OnClick?.Invoke();
    }

    /// <summary>
    /// Sets the position of the blackout screen. In some cases, it has to overlap the budget box, in others it doesn't
    /// </summary>
    /// <param name="position">Position to set</param>
    public void SetPosition(BlackoutScreenPosition position)
    {
        if (_currentPosition == position) return;

        switch (position)
        {
            case BlackoutScreenPosition.underBudgetBox:
                transform.SetParent(_underBudgetTransform);
                break;
            case BlackoutScreenPosition.overBudgetBox:
                transform.SetParent(_overBudgetTransform);
                break;
        }

        _currentPosition = position;
    }
}

public enum BlackoutScreenPosition
{
    underBudgetBox,
    overBudgetBox
}