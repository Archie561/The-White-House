using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class LawManager : MonoBehaviour
{
    /*---------------------------UI Elements---------------------------*/
    [SerializeField]
    private Image _document;
    [SerializeField]
    private TextMeshProUGUI _headerText;
    [SerializeField]
    private TextMeshProUGUI _mainText;
    [SerializeField]
    private TextMeshProUGUI _detailedText;
    [SerializeField]
    private TextMeshProUGUI _preparedByText;

    [SerializeField]
    private CanvasGroup _blackBackground;
    [SerializeField]
    private Button _backButton;
    [SerializeField]
    private Button _declineButton;
    [SerializeField]
    private Button _acceptButton;
    /*---------------------------End UI Section---------------------------*/

    private Law _currentLaw;

    //field to calculate difference between pointer and document coordinates
    private Vector2 _currentPosition;

    private float _triggerArea = 800.0f;

    private bool _isAnimationFinished;

    private void OnEnable()
    {
        //display black background and back button 
        _blackBackground.gameObject.SetActive(true);
        _blackBackground.LeanAlpha(1, 0.8f);
        _backButton.transform.LeanMoveLocalY(460, 0.8f).setEaseOutQuart();

        _currentPosition = Vector2.zero;

        TryGetNextLaw();
    }

    private void TryGetNextLaw()
    {
        //if current law is locked - exit
        GameManager.Instance.LawLockCheck();
        if (GameManager.Instance.IsLawLocked)
        {
            HideAndExit(false);
            return;
        }

        //initilize UI with law data
        _currentLaw = GameManager.Instance.GetNextLaw();

        _headerText.text = _currentLaw.header;
        _mainText.text = _currentLaw.mainText;
        _detailedText.text = _currentLaw.detailedText;
        _preparedByText.text = _currentLaw.preparedBy;

        DisplayDocument();
    }

    private void DisplayDocument()
    {
        _isAnimationFinished = false;

        _document.transform.localPosition = new Vector2(0, Screen.height);
        _document.transform.LeanMoveLocalY(-50, 1).setEaseOutQuart().setOnComplete(() => { _isAnimationFinished = true; });
    }

    public void HideAndExit(bool hideDocument = true)
    {
        //if animations are not playing
        if (_isAnimationFinished)
        {
            //check if it needs to animate document disappearing
            if (hideDocument)
            {
                _document.gameObject.LeanMoveLocal(new Vector2(0, Screen.height), 0.8f).setEaseOutQuart();
            }

            //animate back button and black screen
            _backButton.transform.LeanMoveLocalY(620, 0.8f).setEaseOutQuart();
            _blackBackground.LeanAlpha(0, 0.8f).setOnComplete(() => { GameManager.Instance.LoadMainScene(); });
        }
    }

    //public void LawClickHandler()
    //{
    //    if (_isAnimationFinished)
    //    {
    //        //save data
    //        _isAnimationFinished = false;

    //        //update characteristics and play animation
    //        if (EventSystem.current.currentSelectedGameObject.CompareTag("ChoiceOption1"))
    //        {
    //            GameManager.Instance.UpdateCharacteristics(_currentLaw.characteristicsUpdateWhenApplied);
    //            _document.transform.LeanMoveLocal(new Vector2(Screen.width, _document.transform.localPosition.y), 0.8f).setEaseOutQuart().setOnComplete(() => { _isAnimationFinished = true; TryGetNextLaw(); });
    //        }
    //        else if (EventSystem.current.currentSelectedGameObject.CompareTag("ChoiceOption2"))
    //        {
    //            GameManager.Instance.UpdateCharacteristics(_currentLaw.characteristicsUpdateWhenDeclined);
    //            _document.transform.LeanMoveLocal(new Vector2(-Screen.width, _document.transform.localPosition.y), 0.8f).setEaseOutQuart().setOnComplete(() => { _isAnimationFinished = true; TryGetNextLaw(); });
    //        }
    //    }
    //}

    public void OnDocumentDown()
    {
        if (_isAnimationFinished)
        {
            _currentPosition = new Vector2(Input.mousePosition.x - _document.transform.position.x, 0);
        }
    }

    public void OnDocumentDrag()
    {
        if (_isAnimationFinished)
        {
            _document.transform.position = new Vector2(Input.mousePosition.x, _document.transform.position.y) - _currentPosition;
        }
    }

    public void OnDocumentUp()
    {
        if (!_isAnimationFinished)
        {
            return;
        }

        _isAnimationFinished = false;

        if (_document.transform.localPosition.x < -_triggerArea)
        {
            GameManager.Instance.UpdateCharacteristics(_currentLaw.characteristicsUpdateWhenDeclined);
            _document.transform.LeanMoveLocal(new Vector2(-Screen.width, _document.transform.localPosition.y), 0.8f).setEaseOutQuart().setOnComplete(() => { _isAnimationFinished = true; TryGetNextLaw(); });

            DataManager.PlayerData.lawID++;
            DataManager.SaveData();
        }

        else if (_document.transform.localPosition.x > _triggerArea)
        {
            GameManager.Instance.UpdateCharacteristics(_currentLaw.characteristicsUpdateWhenApplied);
            _document.transform.LeanMoveLocal(new Vector2(Screen.width, _document.transform.localPosition.y), 0.8f).setEaseOutQuart().setOnComplete(() => { _isAnimationFinished = true; TryGetNextLaw(); });

            DataManager.PlayerData.lawID++;
            DataManager.SaveData();
        }

        else
        {
            _document.transform.LeanMoveLocal(new Vector2(0, _document.transform.localPosition.y), 0.8f).setEaseOutQuart().setOnComplete(() => { _isAnimationFinished = true; });
        }
    }
}
