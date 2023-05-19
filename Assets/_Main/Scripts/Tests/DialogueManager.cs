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
    private Choice _currentChoice;

    private int _dialogueIndex;
    private int _choiceIndex;

    void Start()
    {
        _dialogueIndex = 0;
        _choiceIndex = 0;

        _currentDialogue = DataManager.Instance.GetFirstDialogue();
        _currentChoice = DataManager.Instance.GetFirstChoice();

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

        if (_currentDialogue.isChoice[_dialogueIndex])
        {
            ChangeModals();
            ShowNextChoice();
        }
        else
        {
            ShowNextReplica();
        }

        _dialogueIndex += _dialogueIndex < _currentDialogue.replicas.Length - 1 ? 1 : 0;
    }

    private void ShowNextReplica()
    {
        _characterImage.sprite = Resources.Load<Sprite>("Textures/Characters/" + _currentDialogue.imageName[_dialogueIndex]);
        _characterName.text = _currentDialogue.characterName[_dialogueIndex];
        _dialogueText.text = _currentDialogue.replicas[_dialogueIndex];

        _typewriter.StartWriting();
    }

    public void ChoiceClickHandler()
    {
        //saving choice system

        _choiceIndex += _choiceIndex < _currentChoice.option1.Length - 1 ? 1 : 0;
        ChangeModals();
        DialogueClickHandler();
    }
    void ShowNextChoice()
    {
        _option1Text.text = _currentChoice.option1[_choiceIndex];
        _option2Text.text = _currentChoice.option2[_choiceIndex];
    }

    private void ChangeModals()
    {
        _dialogueBox.SetActive(!_dialogueBox.activeSelf);
        _choiceBox.SetActive(!_choiceBox.activeSelf);
    }
}
