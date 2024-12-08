using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LibertyChronicles : BaseNewsPanel
{
    [SerializeField] private Image _newsImage;
    [SerializeField] private TextMeshProUGUI _libertyHeader;
    [SerializeField] private TextMeshProUGUI _libertyDetailedText;

    public override void Initialize(News news)
    {
        _newsImage.sprite = Resources.Load<Sprite>("Textures/NewsImages/" + news.imageName);
        _libertyHeader.text = news.headerText;
        _libertyDetailedText.text = news.detailedText;
    }
}