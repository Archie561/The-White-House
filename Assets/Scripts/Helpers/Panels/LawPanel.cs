using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// Represents the law panel object
/// </summary>
public class LawPanel : MonoBehaviour, IDragHandler, IPointerDownHandler, IPointerUpHandler
{
    [SerializeField] private Image _blankImage;
    [SerializeField] private Sprite[] _blankTypes;
    [SerializeField] private TextMeshProUGUI _headerText;
    [SerializeField] private TextMeshProUGUI _mainText;
    [SerializeField] private TextMeshProUGUI _detailedText;
    [SerializeField] private TextMeshProUGUI _signatureText;
    [SerializeField] private Color _armyBlankFontColor;

    public event Action OnPanelDrag;
    public event Action OnPanelUp;

    public void OnDrag(PointerEventData eventData) => OnPanelDrag?.Invoke();

    public void OnPointerUp(PointerEventData eventData) => OnPanelUp?.Invoke();

    /// <summary>
    /// Initializes the decision panel with data
    /// </summary>
    /// <param name="law">Data to initialize</param>
    public void Initialize(Law law)
    {
        _blankImage.sprite = _blankTypes[law.lawType - 1];

        //if it is army blank, set different font color
        if (law.lawType == 3)
        {
            _headerText.color = _armyBlankFontColor;
            _mainText.color = _armyBlankFontColor;
            _detailedText.color = _armyBlankFontColor;
            _signatureText.color = _armyBlankFontColor;
        }

        _headerText.text = law.header;
        _mainText.text = law.mainText;
        _detailedText.text = law.detailedText;
        _signatureText.text = law.preparedBy;
    }

    //due to a bug in unity, OnPointerUp event is not called if the class doesn't implement the IPointerDownHandler interface
    public void OnPointerDown(PointerEventData eventData) { }
}