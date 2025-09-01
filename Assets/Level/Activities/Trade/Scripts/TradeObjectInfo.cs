using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// represents players trade object info in UI
/// </summary>
public class TradeObjectInfo : MonoBehaviour
{
    [SerializeField] private TradeObjectType _objectType;
    [SerializeField] private string _objectName;
    [SerializeField] private Image _objectIcon;
    [SerializeField] private TextMeshProUGUI _amountText;

    public void UpdateAmountUI(TradeObjectType type, TradeObjectData data)
    {
        if (type == _objectType)
            _amountText.text = $"{data.Amount}/{data.Capacity}";
    }
}
