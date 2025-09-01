using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class CharacteristicIcon : MonoBehaviour
{
    [SerializeField] private Characteristic _characteristic;
    [SerializeField] private Image _fillImage;

    private Color _originalIconColor;

    private const float ANIMATION_DURATION = 0.5f;

    private void Start()
    {
        _fillImage.fillAmount = GetNormalizedCharacteristicValue();
        _originalIconColor = _fillImage.color;
    }

    public void UpdateFill(Characteristic characteristic, int delta)
    {
        if (characteristic != _characteristic)
            return;

        _fillImage.color = delta > 0 ? Color.green : Color.red;
        _fillImage.DOFillAmount(GetNormalizedCharacteristicValue(), ANIMATION_DURATION).SetEase(Ease.OutQuart)
            .OnComplete(() => _fillImage.color = _originalIconColor);
    }

    //краще зробити так щоб цей клас не залежав від GameDataManager
    private float GetNormalizedCharacteristicValue() => GameDataManager.GetCharacteristicValue(_characteristic) / 100f;
}