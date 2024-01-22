using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CharacteristicsManager : MonoBehaviour
{
    /*-------------------UI ELEMENTS-------------------*/
    [SerializeField]
    private Slider _scienceSlider;
    [SerializeField]
    private TextMeshProUGUI _scienceValue;
    [SerializeField]
    private Slider _navySlider;
    [SerializeField]
    private TextMeshProUGUI _navyValue;
    [SerializeField]
    private Slider _airForcesSlider;
    [SerializeField]
    private TextMeshProUGUI _airForcesValue;
    [SerializeField]
    private Slider _infantrySlider;
    [SerializeField]
    private TextMeshProUGUI _infantryValue;
    [SerializeField]
    private Slider _machinerySlider;
    [SerializeField]
    private TextMeshProUGUI _machineryValue;
    [SerializeField]
    private Slider _europeanUnionSlider;
    [SerializeField]
    private TextMeshProUGUI _europeanUnioneValue;
    [SerializeField]
    private Slider _chinaSlider;
    [SerializeField]
    private TextMeshProUGUI _chinaValue;
    [SerializeField]
    private Slider _africaSlider;
    [SerializeField]
    private TextMeshProUGUI _africaValue;
    [SerializeField]
    private Slider _unitedKingdomSlider;
    [SerializeField]
    private TextMeshProUGUI _unitedKingdomValue;
    [SerializeField]
    private Slider _CISSlider;
    [SerializeField]
    private TextMeshProUGUI _CISValue;
    [SerializeField]
    private Slider _OPECSlider;
    [SerializeField]
    private TextMeshProUGUI _OPECValue;
    [SerializeField]
    private Slider _welfareSlider;
    [SerializeField]
    private TextMeshProUGUI _welfareValue;
    [SerializeField]
    private Slider _educationSlider;
    [SerializeField]
    private TextMeshProUGUI _educationValue;
    [SerializeField]
    private Slider _medicineSlider;
    [SerializeField]
    private TextMeshProUGUI _medicineValue;
    [SerializeField]
    private Slider _ecologySlider;
    [SerializeField]
    private TextMeshProUGUI _ecologyValue;
    [SerializeField]
    private Slider _infrastructureSlider;
    [SerializeField]
    private TextMeshProUGUI _infrastructureValue;

    [SerializeField]
    private Slider _totalPoliticSlider;
    [SerializeField]
    private TextMeshProUGUI _totalPoliticsValue;
    [SerializeField]
    private Slider _totalInternationalSlider;
    [SerializeField]
    private TextMeshProUGUI _totalInternationalValue;
    [SerializeField]
    private Slider _totalArmySlider;
    [SerializeField]
    private TextMeshProUGUI _totalArmyValue;
    /*-------------------END UI SECTION-------------------*/

    [SerializeField]
    private CanvasGroup _blackBackground;
    [SerializeField]
    private Button _backButton;

    [SerializeField]
    private GameObject _politicBox;
    [SerializeField]
    private GameObject _internationalBox;
    [SerializeField]
    private GameObject _armyBox;

    public void CharacteristicClickHandler()
    {
        _blackBackground.gameObject.SetActive(true);
        _blackBackground.LeanAlpha(1, 0.8f);

        _backButton.gameObject.SetActive(true);
        _backButton.gameObject.LeanMoveLocalY(460, 0.8f).setEaseOutQuart();

        if(EventSystem.current.currentSelectedGameObject.CompareTag("PoliticButton"))
        {
            _politicBox.SetActive(true);
            LoadPolitic();
            _politicBox.LeanMoveLocalY(0, 0.8f).setEaseOutQuart();
        }
        else if (EventSystem.current.currentSelectedGameObject.CompareTag("InternationalButton"))
        {
            _internationalBox.SetActive(true);
            LoadInternational();
            _internationalBox.LeanMoveLocalY(0, 0.8f).setEaseOutQuart();
        }
        else if (EventSystem.current.currentSelectedGameObject.CompareTag("ArmyButton"))
        {
            _armyBox.SetActive(true);
            LoadArmy();
            _armyBox.LeanMoveLocalY(0, 0.8f).setEaseOutQuart();
        }
    }
    public void BackClickHandler()
    {
        if (_politicBox.activeSelf)
        {
            _politicBox.LeanMoveLocalY(-Screen.height, 0.8f).setEaseOutQuart().setOnComplete(() => { _politicBox.SetActive(false); });
        }
        else if (_internationalBox.activeSelf)
        {
            _internationalBox.LeanMoveLocalY(-Screen.height, 0.8f).setEaseOutQuart().setOnComplete(() => { _internationalBox.SetActive(false); });
        }
        else if (_armyBox.activeSelf)
        {
            _armyBox.LeanMoveLocalY(-Screen.height, 0.8f).setEaseOutQuart().setOnComplete(() => { _armyBox.SetActive(false); });
        }

        _blackBackground.LeanAlpha(0, 0.8f).setOnComplete(() => { _blackBackground.gameObject.SetActive(false); });
        _backButton.gameObject.LeanMoveLocalY(620, 0.8f).setEaseOutQuart().setOnComplete(() => { _backButton.gameObject.SetActive(false); });
    }

    private void LoadPolitic()
    {
        Characteristics characteristics = DataManager.PlayerData.characteristics;

        _scienceValue.text = characteristics.science.ToString() + '%';
        _scienceSlider.value = characteristics.science / 100f;
        _welfareValue.text = characteristics.welfare.ToString() + '%';
        _welfareSlider.value = characteristics.welfare / 100f;
        _educationValue.text = characteristics.education.ToString() + '%';
        _educationSlider.value = characteristics.education / 100f;
        _medicineValue.text = characteristics.medicine.ToString() + '%';
        _medicineSlider.value = characteristics.medicine / 100f;
        _ecologyValue.text = characteristics.ecology.ToString() + '%';
        _ecologySlider.value = characteristics.ecology / 100f;
        _infrastructureValue.text = characteristics.infrastructure.ToString() + '%';
        _infrastructureSlider.value = characteristics.infrastructure / 100f;

        int totalValue = (characteristics.science + characteristics.welfare + characteristics.education + characteristics.medicine +
            characteristics.ecology + characteristics.infrastructure) / 6;

        _totalPoliticsValue.text = totalValue.ToString() + '%';
        _totalPoliticSlider.value = totalValue / 100f;
    }

    private void LoadInternational()
    {
        Characteristics characteristics = DataManager.PlayerData.characteristics;

        _europeanUnioneValue.text = characteristics.europeanUnion.ToString() + '%';
        _europeanUnionSlider.value = characteristics.europeanUnion / 100f;
        _chinaValue.text = characteristics.china.ToString() + '%';
        _chinaSlider.value = characteristics.china / 100f;
        _africaValue.text = characteristics.africa.ToString() + '%';
        _africaSlider.value = characteristics.africa / 100f;
        _unitedKingdomValue.text = characteristics.unitedKingdom.ToString() + '%';
        _unitedKingdomSlider.value = characteristics.unitedKingdom / 100f;
        _CISValue.text = characteristics.CIS.ToString() + '%';
        _CISSlider.value = characteristics.CIS / 100f;
        _OPECValue.text = characteristics.OPEC.ToString() + '%';
        _OPECSlider.value = characteristics.OPEC / 100f;

        int totalValue = (characteristics.europeanUnion + characteristics.china + characteristics.africa + characteristics.unitedKingdom +
            characteristics.CIS + characteristics.OPEC) / 6;

        _totalInternationalValue.text = totalValue.ToString() + '%';
        _totalInternationalSlider.value = totalValue / 100f;
    }

    private void LoadArmy()
    {
        Characteristics characteristics = DataManager.PlayerData.characteristics;

        _navyValue.text = characteristics.navy.ToString() + '%';
        _navySlider.value = characteristics.navy / 100f;
        _airForcesValue.text = characteristics.airForces.ToString() + '%';
        _airForcesSlider.value = characteristics.airForces / 100f;
        _infantryValue.text = characteristics.infantry.ToString() + '%';
        _infantrySlider.value = characteristics.infantry / 100f;
        _machineryValue.text = characteristics.machinery.ToString() + '%';
        _machinerySlider.value = characteristics.machinery / 100f;

        int totalValue = (characteristics.navy + characteristics.airForces + characteristics.infantry + characteristics.machinery) / 4;

        _totalArmyValue.text = totalValue.ToString() + '%';
        _totalArmySlider.value = totalValue / 100f;
    }
}
