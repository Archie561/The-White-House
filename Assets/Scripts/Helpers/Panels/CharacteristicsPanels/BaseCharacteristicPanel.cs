using TMPro;
using UnityEngine;
using UnityEngine.UI;

public abstract class BaseCharacteristicPanel : MonoBehaviour
{
    private readonly Color WARNING_CHARACTERISTIC_COLOR = new Color(0.623f, 0.133f, 0.157f);
    private readonly Color DEFAULT_CHARACTERISTIC_COLOR = new Color(0.831f, 0.788f, 0.659f);

    protected Characteristics _characteristics;
    protected int _characteristicCriticalValue;

    /// <summary>
    /// Initializes the panel with data
    /// </summary>
    /// <param name="characteristics">Players characteristics</param>
    /// <param name="characteristicCriticalValue">If the characteristic is less than it, a warning is displayed and the color changes to red</param>
    public virtual void Initialize(Characteristics characteristics, int characteristicCriticalValue)
    {
        _characteristics = characteristics;
        _characteristicCriticalValue = characteristicCriticalValue;
    }

    //formats the characteristic data for display on the panel and sets its color depending on the value
    protected void FormatCharacteristic(int characteristicValue, TextMeshProUGUI characteristicText, Slider characteristicSlider)
    {
        characteristicText.text = $"{characteristicValue}%";
        characteristicSlider.value = characteristicValue / 100f;
        
        if (characteristicValue <= _characteristicCriticalValue)
        {
            characteristicText.color = WARNING_CHARACTERISTIC_COLOR;
            characteristicSlider.fillRect.GetComponent<Image>().color = WARNING_CHARACTERISTIC_COLOR;
        }
        else
        {
            characteristicText.color = DEFAULT_CHARACTERISTIC_COLOR;
            characteristicSlider.fillRect.GetComponent<Image>().color = Color.white;
        }
    }
}