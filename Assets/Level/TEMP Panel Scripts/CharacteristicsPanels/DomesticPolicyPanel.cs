using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DomesticPolicyPanel : BaseCharacteristicPanel
{
    [SerializeField] private Slider _scienceSlider;
    [SerializeField] private TextMeshProUGUI _scienceValue;
    [SerializeField] private Slider _welfareSlider;
    [SerializeField] private TextMeshProUGUI _welfareValue;
    [SerializeField] private Slider _educationSlider;
    [SerializeField] private TextMeshProUGUI _educationValue;
    [SerializeField] private Slider _medicineSlider;
    [SerializeField] private TextMeshProUGUI _medicineValue;
    [SerializeField] private Slider _ecologySlider;
    [SerializeField] private TextMeshProUGUI _ecologyValue;
    [SerializeField] private Slider _infrastructureSlider;
    [SerializeField] private TextMeshProUGUI _infrastructureValue;

    [SerializeField] private Slider _totalDomesticSlider;
    [SerializeField] private TextMeshProUGUI _totalDomesticValue;

    //initializes domestic policy panel with players characteristics
    public override void Initialize(Characteristic characteristics, int characteristicCriticalValue)
    {
        base.Initialize(characteristics, characteristicCriticalValue);

/*        FormatCharacteristic(characteristics.science, _scienceValue, _scienceSlider);
        FormatCharacteristic(characteristics.welfare, _welfareValue, _welfareSlider);
        FormatCharacteristic(characteristics.education, _educationValue, _educationSlider);
        FormatCharacteristic(characteristics.medicine, _medicineValue, _medicineSlider);
        FormatCharacteristic(characteristics.ecology, _ecologyValue, _ecologySlider);
        FormatCharacteristic(characteristics.infrastructure, _infrastructureValue, _infrastructureSlider);

        int totalValue = (characteristics.science + characteristics.welfare + characteristics.education + characteristics.medicine +
            characteristics.ecology + characteristics.infrastructure) / 6;

        FormatCharacteristic(totalValue, _totalDomesticValue, _totalDomesticSlider);*/
    }
}