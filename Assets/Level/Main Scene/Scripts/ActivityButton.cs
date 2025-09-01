using UnityEngine;
using UnityEngine.UI;

public class ActivityButton : MonoBehaviour
{
    [SerializeField] private ActivityType _activityType;
    [SerializeField] private string _sceneToLoad;
    [SerializeField] private Image _pendingIndicator;

    public ActivityType ActivityType => _activityType;

    public void SetPendingIndicator(bool active)
    {
        _pendingIndicator.gameObject.SetActive(active);
    }

    public void OnButtonClick()
    {
        AudioManager.Instance.PlaySFX(SFXType.Click);
        if (!string.IsNullOrEmpty(_sceneToLoad))
            LevelManager.Instance.LoadScene(_sceneToLoad);
    }
}