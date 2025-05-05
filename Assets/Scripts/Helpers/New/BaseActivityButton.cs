using UnityEngine;
using UnityEngine.EventSystems;

public abstract class BaseActivityButton : MonoBehaviour, IPointerClickHandler
{
    /// <summary>
    /// type of activity for which the button is responsible
    /// </summary>
    public ActivityType ActivityType => _activityType;

    [SerializeField] protected ActivityType _activityType;
    [SerializeField] protected GameObject _availabilityIcon;
    protected bool _interactable;

    /// <summary>
    /// defines if it is possible to interact with the button
    /// </summary>
    /// <param name="interactable">allows interaction if true</param>
    public void SetInteractable(bool interactable)
    {
        _interactable = interactable;
        _availabilityIcon.SetActive(interactable);
    }

    /// <summary>
    /// defines the behavior when clicking on a button
    /// </summary>
    /// <param name="eventData"></param>
    public abstract void OnPointerClick(PointerEventData eventData);
}