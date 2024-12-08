using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class NewsPhoto : BaseNewsPanel
{
    [SerializeField] private Image _photo;
    [SerializeField] private TextMeshProUGUI _photoCaption;

    public override void Initialize(News news)
    {
        _photo.sprite = Resources.Load<Sprite>("Textures/NewsImages/" + news.imageName);
        _photoCaption.text = news.headerText;
    }
}