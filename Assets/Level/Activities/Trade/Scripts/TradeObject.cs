using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TradeObject : MonoBehaviour
{
    [SerializeField] private List<Sprite> _sprites;

    [SerializeField] private Image _tradeIcon;
    [SerializeField] private TextMeshProUGUI _amountText;

    public TradeObjectType Type { get; private set; }
    public int Amount { get; private set; }

    public void SetType(TradeObjectType type)
    {
        Type = type;
        try
        {
            _tradeIcon.sprite = _sprites[(int)type];
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Error setting icon for trade object type {type}: {e.Message}");
            _tradeIcon.sprite = _sprites[0];
        }
    }

    public void SetAmount(int amount)
    {
        Amount = amount;
        _amountText.text = amount.ToString();
    }
}
