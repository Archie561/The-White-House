using System;
using TMPro;
using UnityEngine;

/// <summary>
/// Represents a panel of confidence in the choice
/// </summary>
public class SurePanel : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI _headerText;
    [SerializeField] TextMeshProUGUI _detailedText;
    [SerializeField] TextMeshProUGUI _confirmButtonText;
    [SerializeField] TextMeshProUGUI _cancelButtonText;

    public Action OnConfirmButtonClick;
    public Action OnCancelButtonClick;

    /// <summary>
    /// Initializes panel with text data
    /// </summary>
    /// <param name="header">text in top-box of the panel</param>
    /// <param name="detailed">text in the middle of the panel</param>
    /// <param name="confirm">text in the confirm button</param>
    /// <param name="cancel">text in the cancel button</param>
    public void Initialize(string header, string detailed, string confirm, string cancel)
    {
        _headerText.text = header;
        _detailedText.text = detailed;
        _confirmButtonText.text = confirm;
        _cancelButtonText.text = cancel;
    }

    /// <summary>
    /// Sets to the button object from the editor. To determine the functionality on click, subscribe to the OnConfirmButtonClick event 
    /// </summary>
    public void ConfirmClickHandler() => OnConfirmButtonClick?.Invoke();

    /// <summary>
    /// Sets to the button object from the editor. To determine the functionality on click, subscribe to the OnCancelButtonClick event 
    /// </summary>
    public void CancelClickHandler() => OnCancelButtonClick?.Invoke();
}