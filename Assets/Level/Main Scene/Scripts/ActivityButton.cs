using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ActivityButton : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private string _sceneToLoad;
    [SerializeField] private Image _pendingIndicator;
    [SerializeField] private ActivityType _activityType;

    private void Start()
    {
        UpdatePendingIndicator();
    }

    private void UpdatePendingIndicator()
    {
        switch (_activityType)
        {
            case ActivityType.Dialogue:
                _pendingIndicator.gameObject.SetActive(GameDataManager.IsPending<Dialogue>());
                break;
            case ActivityType.Law:
                _pendingIndicator.gameObject.SetActive(GameDataManager.IsPending<Law>());
                break;
            default:
                Debug.LogError($"activty {_activityType} not registered");
                break;
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        AudioManager.Instance.PlaySFX(SFXType.Click);
        if (!string.IsNullOrEmpty(_sceneToLoad))
            LevelManager.Instance.LoadScene(_sceneToLoad);
    }
}