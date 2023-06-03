using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DecisionManager : MonoBehaviour
{
    /*---------------------------UI Elements---------------------------*/
    [SerializeField]
    private Image _characterImage;
    [SerializeField]
    private TextMeshProUGUI _characterName;
    [SerializeField]
    private TextMeshProUGUI _decisionText;
    [SerializeField]
    private Button[] _optionButton;
    [SerializeField]
    private TextMeshProUGUI[] _optionText;
    /*---------------------------End UI Section---------------------------*/

    private Typewriter _typewriter;
    private Decision _currentDecision;

    private void OnEnable()
    {
        _currentDecision = GameManager.Instance.GetNextDecision();
        _typewriter = new Typewriter(_decisionText);

        _characterImage.sprite = Resources.Load<Sprite>("Textures/Characters/" + _currentDecision.imageName);
        _characterName.text = _currentDecision.characterName;
        _decisionText.text = _currentDecision.text;

        _typewriter.OnWritingFinished += DisplayOptions;
        _typewriter.StartWriting();
    }

    private void DisplayOptions()
    {
        for (int i = 0; i < _currentDecision.options.Length; i++)
        {
            _optionButton[i].gameObject.SetActive(true);
            _optionText[i].text = _currentDecision.options[i];
        }
    }

    public void OptionClickHandler()
    {
        int pickedOption;
        switch(EventSystem.current.currentSelectedGameObject.tag)
        {
            case "DecisionOption1":
                pickedOption = 1;
                break;
            case "DecisionOption2":
                pickedOption = 2;
                break;
            case "DecisionOption3":
                pickedOption = 3;
                break;
            case "DecisionOption4":
                pickedOption = 4;
                break;
            default:
                pickedOption = -1;
                break;
        }
        GameManager.Instance.UpdateCharacteristics(_currentDecision.characteristicUpdates[pickedOption - 1]);

        DataManager.PlayerData.madeDecisions[DataManager.PlayerData.chapterID].value[DataManager.PlayerData.decisionID] = pickedOption;
        gameObject.SetActive(false);
    }

    private void OnDisable()
    {
        for (int i = 0; i < _optionButton.Length; i++)
        {
            _optionButton[i].gameObject.SetActive(false);
        }
        _typewriter.OnWritingFinished -= DisplayOptions;

        DataManager.PlayerData.decisionID++;
        DataManager.SaveData();
        GameManager.Instance.LoadMainScene();
    }
}
