using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour
{
    [SerializeField]
    private RawImage _backgroundImage;

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
    private SubDialogue _currentSubDialogue;

    private int _replicaIndex;
    private int _subReplicaIndex;
    //field to iterate within SubDialogue array in _currentDialogue object
    private int _subDialogueIndex;

    private bool _displayChoice;
    private bool _displaySubDialogue;

    void Start()
    {
        _replicaIndex = 0;
        _subDialogueIndex = 0;

        _currentDialogue = GameManager.Instance.GetNextDialogue();
        _typewriter = new Typewriter(_dialogueText);

        if (_currentDialogue.backgroundImageName != "")
        {
            _backgroundImage.texture = Resources.Load<Texture>("Textures/Backgrounds/" + _currentDialogue.backgroundImageName);
        }

        _dialogueBox.SetActive(true);
        DialogueClickHandler();
    }

    public void DialogueClickHandler()
    {
        //if text is writing, skip writing and return
        if (_typewriter.IsWriting)
        {
            _typewriter.SkipWriting();

            return;
        }

        //if needs to display choice
        if (_displayChoice)
        {
            //show choice with choiceID which is stored in choiceID[_replicaINdex] and return
            _displayChoice = false;
            ShowNextChoice(_currentDialogue.choiceID[_replicaIndex]);

            return;
        }

        //if needs to display SubDialogue
        if (_displaySubDialogue)
        {
            //if all sub replicas was displayed, set _displaySubDialogue to false and load next main dialogue replica
            if (_subReplicaIndex == _currentSubDialogue.replicas.Length)
            {
                _displaySubDialogue = false;
                _replicaIndex++;
            }
            //else show sub replica, load next sub replica and return
            else
            {
                ShowNextReplica();
                _subReplicaIndex++;

                return;
            }
        }

        /*---------------------------MAIN DIALOGUE REPLICAS BLOCK---------------------------*/
        //if all main replicas was displayed, exit scene
        if (_replicaIndex >= _currentDialogue.replicas.Length)
        {
            ExitScene();
            return;
        }
        //show next main dialogue replica
        ShowNextReplica();
        //if needs to display choice after current replica, set _displayChoice to true
        if (_currentDialogue.choiceID[_replicaIndex] != -1)
        {
            _displayChoice = true;
        }
        //else load next main dialogue replica
        else
        {
            _replicaIndex++;
        }
        /*---------------------------END BLOCK---------------------------*/
    }

    //display next SubDialogue or Dialogue replica if exists
    private void ShowNextReplica()
    {
        try
        {
            _characterImage.sprite = Resources.Load<Sprite>("Textures/Characters/" + (_displaySubDialogue ? _currentSubDialogue.imageName[_subReplicaIndex] : _currentDialogue.imageName[_replicaIndex]));
            _characterName.text = _displaySubDialogue ? _currentSubDialogue.characterName[_subReplicaIndex] : _currentDialogue.characterName[_replicaIndex];
            _dialogueText.text = _displaySubDialogue ? _currentSubDialogue.replicas[_subReplicaIndex] : _currentDialogue.replicas[_replicaIndex];
        }
        catch (Exception e)
        {
            Debug.LogError(e.Message + $" Error! Check indexes for imageName, characterName and replicas arrays in Dialogue or SubDialogue object");
            return;
        }

        _typewriter.StartWriting();
    }

    public void ChoiceClickHandler()
    {
        int pickedOption;
        sbyte choiceID = _currentDialogue.choiceID[_replicaIndex];

        //save player choice and update characteristics
        if (EventSystem.current.currentSelectedGameObject.CompareTag("ChoiceOption1"))
        {
            pickedOption = 1;
            GameManager.Instance.UpdateCharacteristics(GameManager.Instance.GetChoice(choiceID).characteristicsUpdateOption1);
        }
        else
        {
            pickedOption = 2;
            GameManager.Instance.UpdateCharacteristics(GameManager.Instance.GetChoice(choiceID).characteristicsUpdateOption2);
        }
        DataManager.PlayerData.madeChoices[DataManager.PlayerData.chapterID].value[choiceID] = pickedOption;

        //load SubDialogue after choice if exists
        try
        {
            _currentSubDialogue = pickedOption == 1 ? _currentDialogue.subDialogueOption1[_subDialogueIndex] : _currentDialogue.subDialogueOption2[_subDialogueIndex];

            //if there is no subdialogues after current choice, skip to next replica
            if (_currentSubDialogue.imageName.Length == 0)
            {
                _replicaIndex++;
            }
            else
            {
                _subReplicaIndex = 0;
                _displaySubDialogue = true;
            }

            // if there are replicas to show, change modals
            if (_replicaIndex < _currentDialogue.replicas.Length)
            {
                ChangeModals();
            }
        }
        catch (Exception e)
        {
            Debug.LogError(e.Message + $" SubDialogue object with ID {_subDialogueIndex} does not exist!");
            return;
        }

        //incement index of SubDialogue objects array
        _subDialogueIndex++;

        DialogueClickHandler();
    }

    //load text from Choice object if exists
    void ShowNextChoice(sbyte choiceID)
    {
        _option1Text.text = GameManager.Instance.GetChoice(choiceID).option1;
        _option2Text.text = GameManager.Instance.GetChoice(choiceID).option2;

        ChangeModals();
    }

    //changing between choice and dialogue modals
    void ChangeModals()
    {
        _dialogueBox.SetActive(!_dialogueBox.activeSelf);
        _choiceBox.SetActive(!_choiceBox.activeSelf);
    }

    //increment index of Dialogues array, save data into file and exit scene
    void ExitScene()
    {
        DataManager.PlayerData.dialogueID++;
        DataManager.SaveData();
        GameManager.Instance.LoadMainScene();
    }
}
