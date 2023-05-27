using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour
{
    /*---------------------------Dialogue UI Elements---------------------------*/
    [SerializeField]
    private GameObject _dialogueBox;
    [SerializeField]
    private Image _characterImage;
    [SerializeField]
    private TextMeshProUGUI _characterName;
    [SerializeField]
    private TextMeshProUGUI _dialogueText;
    /*---------------------------End Dialogue Section---------------------------*/

    /*---------------------------Choice UI Elements---------------------------*/
    [SerializeField]
    private GameObject _choiceBox;
    [SerializeField]
    private TextMeshProUGUI _option1Text;
    [SerializeField]
    private TextMeshProUGUI _option2Text;
    /*---------------------------End Choice Section---------------------------*/

    private Typewriter _typewriter;

    private Dialogue _currentDialogue;

    private int _phraseIndex;

    private bool _displayChoice;

    void Start()
    {
        _phraseIndex = 0;
        _currentDialogue = GameManager.Instance.GetNextDialogue();

        _typewriter = new Typewriter(_dialogueText);

        _dialogueBox.SetActive(true);
        DialogueClickHandler();
    }

    public void DialogueClickHandler()
    {
        if (_typewriter.IsWriting)
        {
            _typewriter.SkipWriting();
            return;
        }
        else if (_displayChoice)
        {
            _displayChoice = false;
            ChangeModals();

            return;
        }

        ShowNextReplica();

        if (_currentDialogue.choiceID[_phraseIndex] != -1)
        {
            _displayChoice = true;
            ShowNextChoice(_currentDialogue.choiceID[_phraseIndex]);
        }

        _phraseIndex += _phraseIndex < _currentDialogue.replicas.Length - 1 ? 1 : 0;
    }

    private void ShowNextReplica()
    {
        _characterImage.sprite = Resources.Load<Sprite>("Textures/Characters/" + _currentDialogue.imageName[_phraseIndex]);
        _characterName.text = _currentDialogue.characterName[_phraseIndex];
        _dialogueText.text = _currentDialogue.replicas[_phraseIndex];

        _typewriter.StartWriting();
    }

    public void ChoiceClickHandler()
    {
        //saving choice system

        ChangeModals();
        DialogueClickHandler();
    }
    void ShowNextChoice(sbyte choiceID)
    {
        _option1Text.text = GameManager.Instance.GetChoice(choiceID).option1;
        _option2Text.text = GameManager.Instance.GetChoice(choiceID).option2;
    }

    private void ChangeModals()
    {
        _dialogueBox.SetActive(!_dialogueBox.activeSelf);
        _choiceBox.SetActive(!_choiceBox.activeSelf);
    }
}
