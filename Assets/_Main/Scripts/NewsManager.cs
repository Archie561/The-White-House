using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class NewsManager : MonoBehaviour
{
    /*--------------------------UI ELEMENTS--------------------------*/
    [SerializeField]
    private CanvasGroup _blackBackground;

    [SerializeField]
    private Image _newsLiberty;
    [SerializeField]
    private Image _newsPatriot;
    [SerializeField]
    private Image _newsPulse;
    [SerializeField]
    private Image _photo;

    [SerializeField]
    private TextMeshProUGUI _libertyHeaderText;
    [SerializeField]
    private TextMeshProUGUI _patriotHeaderText;
    [SerializeField]
    private TextMeshProUGUI _pulseHeaderText;
    [SerializeField]
    private TextMeshProUGUI _photoText;

    [SerializeField]
    private TextMeshProUGUI _libertyDetailedText;
    [SerializeField]
    private TextMeshProUGUI _patriotDetailedText;
    [SerializeField]
    private TextMeshProUGUI _pulseDetailedText;

    [SerializeField]
    private Image _libertyImage;
    [SerializeField]
    private Image _photoImage;
    /*-------------------------END UI SECTION-------------------------*/

    private News[] _news;

    private int _newsIndex;

    private bool _isAnimationFinished;

    private void OnEnable()
    {
        _blackBackground.gameObject.SetActive(true);
        _blackBackground.LeanAlpha(1, 0.8f);
        _isAnimationFinished = true;

        _news = GameManager.Instance.GetNews();
        _newsIndex = 0;

        GetNextNews();
    }

    private void GetNextNews()
    {
        //if all news were shown - exit
        if (_newsIndex >= _news.Length)
        {
            _blackBackground.LeanAlpha(0, 0.8f).setOnComplete(() => { GameManager.Instance.LoadMainScene(); });
            return;
        }

        if (!IsConditionMet())
        {
            _newsIndex++;
            GetNextNews();
            return;
        }

        switch (_news[_newsIndex].newsPaperType)
        {
            case 1:
                LoadLiberty();
                break;
            case 2:
                LoadPatriot();
                break;
            case 3:
                LoadPulse();
                break;
            case 4:
                LoadPhoto();
                break;
            default:
                Debug.LogError("Unknown newspaper type!");
                break;
        }
    }

    private bool IsConditionMet()
    {
        //if condition is not used, return
        if (!_news[_newsIndex].condition.isUsed)
        {
            return true;
        }

        bool isConditionMet = true;

        //if condition is in characteristic value
        if (!string.IsNullOrEmpty(_news[_newsIndex].condition.characteristicName))
        {
            //checking for condition met: false if player characteristic is less then condition characteristic
            switch (_news[_newsIndex].condition.characteristicName)
            {
                case "navy":
                    isConditionMet = DataManager.PlayerData.characteristics.navy >= _news[_newsIndex].condition.characteristicValue;
                    break;
                case "airForces":
                    isConditionMet = DataManager.PlayerData.characteristics.airForces >= _news[_newsIndex].condition.characteristicValue;
                    break;
                case "infantry":
                    isConditionMet = DataManager.PlayerData.characteristics.infantry >= _news[_newsIndex].condition.characteristicValue;
                    break;
                case "machinery":
                    isConditionMet = DataManager.PlayerData.characteristics.machinery >= _news[_newsIndex].condition.characteristicValue;
                    break;
                case "europeanUnion":
                    isConditionMet = DataManager.PlayerData.characteristics.europeanUnion >= _news[_newsIndex].condition.characteristicValue;
                    break;
                case "china":
                    isConditionMet = DataManager.PlayerData.characteristics.china >= _news[_newsIndex].condition.characteristicValue;
                    break;
                case "africa":
                    isConditionMet = DataManager.PlayerData.characteristics.africa >= _news[_newsIndex].condition.characteristicValue;
                    break;
                case "unitedKingdom":
                    isConditionMet = DataManager.PlayerData.characteristics.unitedKingdom >= _news[_newsIndex].condition.characteristicValue;
                    break;
                case "CIS":
                    isConditionMet = DataManager.PlayerData.characteristics.CIS >= _news[_newsIndex].condition.characteristicValue;
                    break;
                case "OPEC":
                    isConditionMet = DataManager.PlayerData.characteristics.OPEC >= _news[_newsIndex].condition.characteristicValue;
                    break;
                case "science":
                    isConditionMet = DataManager.PlayerData.characteristics.science >= _news[_newsIndex].condition.characteristicValue;
                    break;
                case "welfare":
                    isConditionMet = DataManager.PlayerData.characteristics.welfare >= _news[_newsIndex].condition.characteristicValue;
                    break;
                case "education":
                    isConditionMet = DataManager.PlayerData.characteristics.education >= _news[_newsIndex].condition.characteristicValue;
                    break;
                case "medicine":
                    isConditionMet = DataManager.PlayerData.characteristics.medicine >= _news[_newsIndex].condition.characteristicValue;
                    break;
                case "ecology":
                    isConditionMet = DataManager.PlayerData.characteristics.ecology >= _news[_newsIndex].condition.characteristicValue;
                    break;
                case "infrastructure":
                    isConditionMet = DataManager.PlayerData.characteristics.infrastructure >= _news[_newsIndex].condition.characteristicValue;
                    break;
                default:
                    isConditionMet = true;
                    break;
            }
        }

        //if condition is in made choice
        if (isConditionMet && _news[_newsIndex].condition.choiceChapterID != -1)
        {
            //checking for condition met: false if player choice is not equal to condition choice
            if (DataManager.PlayerData.madeChoices[_news[_newsIndex].condition.choiceChapterID].value[_news[_newsIndex].condition.choiceID] != _news[_newsIndex].condition.choiceValue)
            {
                isConditionMet = false;
            }
        }

        //if condition is in made decision
        if (isConditionMet && _news[_newsIndex].condition.decisionChapterID != -1)
        {
            //checking for condition met: false if player decision is not equal to condition decision
            if (DataManager.PlayerData.madeDecisions[_news[_newsIndex].condition.decisionChapterID].value[_news[_newsIndex].condition.decisionID] != _news[_newsIndex].condition.decisionValue)
            {
                isConditionMet = false;
            }
        }

        return isConditionMet;
    }

    public void NewsClickHandler()
    {
        if (_isAnimationFinished)
        {
            Image currentNews = _newsLiberty.IsActive() ?  _newsLiberty : _newsPatriot.IsActive() ? _newsPatriot : _newsPulse.IsActive() ? _newsPulse : _photo;
            _isAnimationFinished = false;

            currentNews.transform.LeanMoveLocalY(-Screen.height, 1).setEaseOutQuart().setOnComplete(() => { 
                _isAnimationFinished = true;

                _newsLiberty.gameObject.SetActive(false);
                _newsPatriot.gameObject.SetActive(false);
                _newsPulse.gameObject.SetActive(false);
                _photo.gameObject.SetActive(false);

                _newsIndex++;

                GetNextNews();
            });
        }
    }

    private void LoadLiberty()
    {
        _newsLiberty.gameObject.SetActive(true);
        _libertyImage.sprite = Resources.Load<Sprite>("Textures/NewsImages/" + _news[_newsIndex].imageName);
        _libertyHeaderText.text = _news[_newsIndex].headerText;
        _libertyDetailedText.text = _news[_newsIndex].detailedText;

        _isAnimationFinished = false;

        _newsLiberty.transform.localPosition = new Vector2(0, Screen.height);
        _newsLiberty.transform.Rotate(Vector3.forward, Random.Range(-3.0f, 3.0f));
        _newsLiberty.transform.LeanMoveLocalY(-50, 1).setEaseOutQuart().setOnComplete(() => { _isAnimationFinished = true; });
    }

    private void LoadPatriot()
    {
        _newsPatriot.gameObject.SetActive(true);
        _patriotHeaderText.text = _news[_newsIndex].headerText;
        _patriotDetailedText.text = _news[_newsIndex].detailedText;

        _isAnimationFinished = false;

        _newsPatriot.transform.localPosition = new Vector2(0, Screen.height);
        _newsPatriot.transform.Rotate(Vector3.forward, Random.Range(-3.0f, 3.0f));
        _newsPatriot.transform.LeanMoveLocalY(-50, 1).setEaseOutQuart().setOnComplete(() => { _isAnimationFinished = true; });
    }

    private void LoadPulse()
    {
        _newsPulse.gameObject.SetActive(true);
        _pulseHeaderText.text = _news[_newsIndex].headerText;
        _pulseDetailedText.text = _news[_newsIndex].detailedText;

        _isAnimationFinished = false;

        _newsPulse.transform.localPosition = new Vector2(0, Screen.height);
        _newsPulse.transform.Rotate(Vector3.forward, Random.Range(-3.0f, 3.0f));
        _newsPulse.transform.LeanMoveLocalY(-50, 1).setEaseOutQuart().setOnComplete(() => { _isAnimationFinished = true; });
    }

    private void LoadPhoto()
    {
        _photo.gameObject.SetActive(true);
        _photoImage.sprite = Resources.Load<Sprite>("Textures/NewsImages/" + _news[_newsIndex].imageName);
        _photoText.text = _news[_newsIndex].headerText;

        _photo.transform.localPosition = new Vector2(0, Screen.height);
        _photo.transform.Rotate(Vector3.forward, Random.Range(-3.0f, 3.0f));
        _photo.transform.LeanMoveLocalY(-50, 1).setEaseOutQuart().setOnComplete(() => { _isAnimationFinished = true; });
    }
}
