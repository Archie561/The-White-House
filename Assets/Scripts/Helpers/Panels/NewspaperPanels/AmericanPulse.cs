using TMPro;
using UnityEngine;

public class AmericanPulse : BaseNewsPanel
{
    [SerializeField] private TextMeshProUGUI _pulseHeader;
    [SerializeField] private TextMeshProUGUI _pulseDetailedText;

    public override void Initialize(News news)
    {
        _pulseHeader.text = news.headerText;
        _pulseDetailedText.text = news.detailedText;
    }
}