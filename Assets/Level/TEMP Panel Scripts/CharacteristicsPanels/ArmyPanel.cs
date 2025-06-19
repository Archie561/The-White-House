using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ArmyPanel : BaseCharacteristicPanel
{
    [SerializeField] private Slider _navySlider;
    [SerializeField] private TextMeshProUGUI _navyValue;
    [SerializeField] private Slider _airForcesSlider;
    [SerializeField] private TextMeshProUGUI _airForcesValue;
    [SerializeField] private Slider _infantrySlider;
    [SerializeField] private TextMeshProUGUI _infantryValue;
    [SerializeField] private Slider _machinerySlider;
    [SerializeField] private TextMeshProUGUI _machineryValue;

    [SerializeField] private Slider _totalArmySlider;
    [SerializeField] private TextMeshProUGUI _totalArmyValue;

    //initializes army panel with players characteristics
    public override void Initialize(Characteristic characteristics, int characteristicCriticalValue)
    {
        base.Initialize(characteristics, characteristicCriticalValue);

/*        FormatCharacteristic(characteristics.navy, _navyValue, _navySlider);
        FormatCharacteristic(characteristics.airForces, _airForcesValue, _airForcesSlider);
        FormatCharacteristic(characteristics.infantry, _infantryValue, _infantrySlider);
        FormatCharacteristic(characteristics.machinery, _machineryValue, _machinerySlider);

        int totalValue = (characteristics.navy + characteristics.airForces + characteristics.infantry + characteristics.machinery) / 4;
        FormatCharacteristic(totalValue, _totalArmyValue, _totalArmySlider);*/
    }
}