using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ForeignPolicyPanel : BaseCharacteristicPanel
{
    [SerializeField] private Slider _europeanUnionSlider;
    [SerializeField] private TextMeshProUGUI _europeanUnioneValue;
    [SerializeField] private Slider _chinaSlider;
    [SerializeField] private TextMeshProUGUI _chinaValue;
    [SerializeField] private Slider _africaSlider;
    [SerializeField] private TextMeshProUGUI _africaValue;
    [SerializeField] private Slider _unitedKingdomSlider;
    [SerializeField] private TextMeshProUGUI _unitedKingdomValue;
    [SerializeField] private Slider _CISSlider;
    [SerializeField] private TextMeshProUGUI _CISValue;
    [SerializeField] private Slider _OPECSlider;
    [SerializeField] private TextMeshProUGUI _OPECValue;

    [SerializeField] private Slider _totalForeignlSlider;
    [SerializeField] private TextMeshProUGUI _totalForeignlValue;

    //initializes foreign policy panel with players characteristics
    public override void Initialize(Characteristic characteristics, int characteristicCriticalValue)
    {
        base.Initialize(characteristics, characteristicCriticalValue);

/*        FormatCharacteristic(characteristics.europeanUnion, _europeanUnioneValue, _europeanUnionSlider);
        FormatCharacteristic(characteristics.china, _chinaValue, _chinaSlider);
        FormatCharacteristic(characteristics.africa, _africaValue, _africaSlider);
        FormatCharacteristic(characteristics.unitedKingdom, _unitedKingdomValue, _unitedKingdomSlider);
        FormatCharacteristic(characteristics.CIS, _CISValue, _CISSlider);
        FormatCharacteristic(characteristics.OPEC, _OPECValue, _OPECSlider);

        int totalValue = (characteristics.europeanUnion + characteristics.china + characteristics.africa + characteristics.unitedKingdom +
            characteristics.CIS + characteristics.OPEC) / 6;

        FormatCharacteristic(totalValue, _totalForeignlValue, _totalForeignlSlider);*/
    }
}