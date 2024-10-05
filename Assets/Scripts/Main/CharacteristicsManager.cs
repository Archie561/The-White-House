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

    [SerializeField]
    private GameObject _characteristicsPanels;
    [SerializeField]
    private GameObject _domesticPolicyPanel;
    [SerializeField]
    private GameObject _foreignPolicyPanel;
    [SerializeField]
    private GameObject _armyPanel;

    [SerializeField]
    private CanvasGroup _blackBackground;
    [SerializeField]
    private Button _backButton;
    [SerializeField]
    private Color _warningColor;
    /*-------------------END UI SECTION-------------------*/

    /*--------------------ANIMATION PARAMETERS SECTION--------------------*/
    private Vector2 _panelDefaultScale;

    private float _blackBackgroundAnimationTime = 0.8f;
    private float _backButtonAnimationTime = 0.8f;
    private float _panelAppearingAnimationTime = 0.8f;
    private float _panelDisappearingAnimationTime = 0.8f;
    /*------------------END ANIMATION PARAMETERS SECTION------------------*/

    private int _characteristicCriticalValue = 10;

    public void OnEnable()
    {
        Vector2 backButtonDefaultPosition = _backButton.transform.position;
        _backButton.transform.position = new Vector2(_backButton.transform.position.x, Screen.height + _backButton.GetComponent<RectTransform>().rect.size.y * _backButton.GetComponent<RectTransform>().lossyScale.y);

        _blackBackground.gameObject.SetActive(true);
        _backButton.gameObject.SetActive(true);
        _characteristicsPanels.SetActive(true);

        _blackBackground.LeanAlpha(1, _blackBackgroundAnimationTime);
        _backButton.transform.LeanMove(backButtonDefaultPosition, _backButtonAnimationTime).setEaseOutQuart();

        if (EventSystem.current.currentSelectedGameObject.CompareTag("PoliticButton"))
        {
            DisplayDomesticPolicy();
        }
        else if (EventSystem.current.currentSelectedGameObject.CompareTag("InternationalButton"))
        {
            DisplayForeignPolicy();
        }
        else if (EventSystem.current.currentSelectedGameObject.CompareTag("ArmyButton"))
        {
            DisplayArmy();
        }
    }

    private void DisplayDomesticPolicy()
    {
        //loading characteristics data
        Characteristics characteristics = DataManager.PlayerData.characteristics;

        FormatCharacteristic(characteristics.science, _scienceValue, _scienceSlider);
        FormatCharacteristic(characteristics.welfare, _welfareValue, _welfareSlider);
        FormatCharacteristic(characteristics.education, _educationValue, _educationSlider);
        FormatCharacteristic(characteristics.medicine, _medicineValue, _medicineSlider);
        FormatCharacteristic(characteristics.ecology, _ecologyValue, _ecologySlider);
        FormatCharacteristic(characteristics.infrastructure, _infrastructureValue, _infrastructureSlider);

        int totalValue = (characteristics.science + characteristics.welfare + characteristics.education + characteristics.medicine +
            characteristics.ecology + characteristics.infrastructure) / 6;
        FormatCharacteristic(totalValue, _totalPoliticsValue, _totalPoliticSlider);

        _panelDefaultScale = _domesticPolicyPanel.transform.localScale;
        _domesticPolicyPanel.transform.localScale = Vector2.zero;
        _domesticPolicyPanel.SetActive(true);
        _domesticPolicyPanel.LeanScale(_panelDefaultScale, _panelAppearingAnimationTime).setEaseOutQuart();
    }

    private void DisplayForeignPolicy()
    {
        Characteristics characteristics = DataManager.PlayerData.characteristics;

        FormatCharacteristic(characteristics.europeanUnion, _europeanUnioneValue, _europeanUnionSlider);
        FormatCharacteristic(characteristics.china, _chinaValue, _chinaSlider);
        FormatCharacteristic(characteristics.africa, _africaValue, _africaSlider);
        FormatCharacteristic(characteristics.unitedKingdom, _unitedKingdomValue, _unitedKingdomSlider);
        FormatCharacteristic(characteristics.CIS, _CISValue, _CISSlider);
        FormatCharacteristic(characteristics.OPEC, _OPECValue, _OPECSlider);

        int totalValue = (characteristics.europeanUnion + characteristics.china + characteristics.africa + characteristics.unitedKingdom +
            characteristics.CIS + characteristics.OPEC) / 6;
        FormatCharacteristic(totalValue, _totalInternationalValue, _totalInternationalSlider);

        _panelDefaultScale = _foreignPolicyPanel.transform.localScale;
        _foreignPolicyPanel.transform.localScale = Vector2.zero;
        _foreignPolicyPanel.SetActive(true);
        _foreignPolicyPanel.LeanScale(_panelDefaultScale, _panelAppearingAnimationTime).setEaseOutQuart();
    }

    private void DisplayArmy()
    {
        Characteristics characteristics = DataManager.PlayerData.characteristics;

        FormatCharacteristic(characteristics.navy, _navyValue, _navySlider);
        FormatCharacteristic(characteristics.airForces, _airForcesValue, _airForcesSlider);
        FormatCharacteristic(characteristics.infantry, _infantryValue, _infantrySlider);
        FormatCharacteristic(characteristics.machinery, _machineryValue, _machinerySlider);

        int totalValue = (characteristics.navy + characteristics.airForces + characteristics.infantry + characteristics.machinery) / 4;
        FormatCharacteristic(totalValue, _totalArmyValue, _totalArmySlider);

        _panelDefaultScale = _armyPanel.transform.localScale;
        _armyPanel.transform.localScale = Vector2.zero;
        _armyPanel.SetActive(true);
        _armyPanel.LeanScale(_panelDefaultScale, _panelAppearingAnimationTime).setEaseOutQuart();
    }

    public void BackButtonClickHandler()
    {
        AudioManager.Instance.PlaySFX("button");
        GameObject currentPanel = _domesticPolicyPanel.activeSelf ? _domesticPolicyPanel : _foreignPolicyPanel.activeSelf ? _foreignPolicyPanel : _armyPanel;
        Vector2 panelDefaultPosition = currentPanel.transform.position;
        Vector2 backButtonDefaultPosition = _backButton.transform.position;

        currentPanel.transform.LeanMoveY(-Screen.height / 2, _panelDisappearingAnimationTime).setEaseOutQuart();
        _backButton.transform.LeanMoveY(Screen.height + _backButton.GetComponent<RectTransform>().rect.size.y * _backButton.GetComponent<RectTransform>().lossyScale.y, _backButtonAnimationTime).setEaseOutQuart();
        _blackBackground.LeanAlpha(0, _blackBackgroundAnimationTime).setOnComplete(() =>
        {
            currentPanel.SetActive(false);
            _backButton.gameObject.SetActive(false);
            _blackBackground.gameObject.SetActive(false);

            _backButton.transform.position = backButtonDefaultPosition;
            currentPanel.transform.position = panelDefaultPosition;

            gameObject.SetActive(false);
        });
    }

    private void FormatCharacteristic(int characteristicValue, TextMeshProUGUI characteristicText, Slider characteristicSlider)
    {
        characteristicText.text = characteristicValue.ToString() + '%';
        characteristicSlider.value = characteristicValue / 100f;
        if (characteristicValue <= _characteristicCriticalValue)
        {
            characteristicText.color = _warningColor;
            characteristicSlider.transform.Find("Fill Area/Fill").GetComponent<Image>().color = _warningColor;
        }
    }
}
