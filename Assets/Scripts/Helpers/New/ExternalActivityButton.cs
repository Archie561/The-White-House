using UnityEngine;
using UnityEngine.EventSystems;

public class ExternalActivityButton : BaseActivityButton
{
    [SerializeField] private string _activitySceneName;

    //if interaction is allowed - loads scene _activitySceneName
    public override void OnPointerClick(PointerEventData eventData)
    {
        if (!_interactable) return;

        if (_activitySceneName != null)
            LevelManager.Instance.LoadScene(_activitySceneName);
    }
}