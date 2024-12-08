using TMPro;
using UnityEngine;

public class PatriotsPress : BaseNewsPanel
{
    [SerializeField] private TextMeshProUGUI _patriotsHeader;
    [SerializeField] private TextMeshProUGUI _patriotsDetailedText;

    public override void Initialize(News news)
    {
        _patriotsHeader.text = news.headerText;
        _patriotsDetailedText.text = news.detailedText;
    }
}