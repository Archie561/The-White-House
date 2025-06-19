using UnityEngine;

public class UIAdjuster : MonoBehaviour
{
    [SerializeField] private UnityEngine.UI.Image _leftPanel;
    [SerializeField] private UnityEngine.UI.Image _rightPanel;

    //adjusts UI to the safe zone
    private void Start()
    {
        _leftPanel.transform.position = new Vector2(Screen.safeArea.xMin -
            (_leftPanel.rectTransform.rect.width  * _leftPanel.transform.lossyScale.x * 0.5f), Screen.safeArea.center.y);
        _rightPanel.transform.position = new Vector2(Screen.safeArea.xMax +
            (_rightPanel.rectTransform.rect.width * _rightPanel.transform.lossyScale.x * 0.5f), Screen.safeArea.center.y);
    }
}