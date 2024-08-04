using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour
{
    /*---------------------------UI SECTION---------------------------*/
    [SerializeField]
    private RawImage _backgroundImage;

    [SerializeField]
    private GameObject _dialoguePanel;
    [SerializeField]
    private GameObject _choicePanel;

    [SerializeField]
    private Image _dialogueCharacterImage;
    [SerializeField]
    private TextMeshProUGUI _dialogueCharacterName;
    [SerializeField]
    private TextMeshProUGUI _dialogueText;
    [SerializeField]
    private TextMeshProUGUI _choice1Text;
    [SerializeField]
    private TextMeshProUGUI _choice2Text;
    /*---------------------------END UI SECTION---------------------------*/

    /*----------------------OTHER PARAMETERS SECTION----------------------*/
    private Dialogue _currentDialogue;

    private SubReplica[] _currentSubReplicas;

    private Typewriter _typewriter;

    private bool _showSubReplicas;
    private int _replicaIndex;
    private int _subReplicaIndex;
    /*--------------------END OTHER PARAMETERS SECTION--------------------*/

    private void Start()
    {
        _currentDialogue = GameManager.Instance.GetNextDialogue();
        _typewriter = new Typewriter(_dialogueText);
        _replicaIndex = 0;

        _backgroundImage.texture = Resources.Load<Texture>("Textures/Backgrounds/" + _currentDialogue.backgroundImageName);

        /*------------------------initializing the first replica------------------------*/
        _dialogueCharacterImage.sprite = Resources.Load<Sprite>("Textures/Characters/" + _currentDialogue.replicas[_replicaIndex].imageName);
        _dialogueCharacterName.text = _currentDialogue.replicas[_replicaIndex].characterName;
        _dialogueText.text = _currentDialogue.replicas[_replicaIndex].replicaText;
        _typewriter.StartWriting();
        /*------------------------------------------------------------------------------*/
    }

    private void LoadNextReplica()
    {
        //if needs to show specific subreplicas after made choice
        if (_showSubReplicas)
        {
            if (_subReplicaIndex >= _currentSubReplicas.Length)
            {
                _showSubReplicas = false;

                LoadNextReplica();
                return;
            }

            _dialogueCharacterImage.sprite = Resources.Load<Sprite>("Textures/Characters/" + _currentSubReplicas[_subReplicaIndex].imageName);
            _dialogueCharacterName.text = _currentSubReplicas[_subReplicaIndex].characterName;
            _dialogueText.text = _currentSubReplicas[_subReplicaIndex].subReplicaText;

            _subReplicaIndex++;
        }
        //if needs to show main replicas chain
        else
        {
            _replicaIndex++;

            if (_replicaIndex >= _currentDialogue.replicas.Length)
            {
                DataManager.PlayerData.dialogueID++;
                DataManager.SaveData();
                GameManager.Instance.LoadMainScene();
                return;
            }

            _dialogueCharacterImage.sprite = Resources.Load<Sprite>("Textures/Characters/" + _currentDialogue.replicas[_replicaIndex].imageName);
            _dialogueCharacterName.text = _currentDialogue.replicas[_replicaIndex].characterName;
            _dialogueText.text = _currentDialogue.replicas[_replicaIndex].replicaText;
        }

        _typewriter.StartWriting();
    }

    public void DialogueClickHandler()
    {
        if (_typewriter.IsWriting)
        {
            _typewriter.SkipWriting();
            return;
        }

        //if subreplicas are not showing right now and there is choice to show - display choice modal
        if (!_showSubReplicas && _currentDialogue.replicas[_replicaIndex].choiceID != -1)
        {
            _choice1Text.text = GameManager.Instance.GetChoice(_currentDialogue.replicas[_replicaIndex].choiceID).option1;
            _choice2Text.text = GameManager.Instance.GetChoice(_currentDialogue.replicas[_replicaIndex].choiceID).option2;

            _dialoguePanel.SetActive(false);
            _choicePanel.SetActive(true);

            return;
        }

        LoadNextReplica();
    }

    public void ChoiceClickHandler()
    {
        int _pickedChoiceOption = EventSystem.current.currentSelectedGameObject.tag == "ChoiceOption1" ? 1 : 2;
        Characteristics characteristicsToUpdate = _pickedChoiceOption == 1 ? GameManager.Instance.GetChoice(_currentDialogue.replicas[_replicaIndex].choiceID).characteristicsUpdateOption1 :
            GameManager.Instance.GetChoice(_currentDialogue.replicas[_replicaIndex].choiceID).characteristicsUpdateOption2;

        DataManager.UpdateCharacteristics(characteristicsToUpdate);
        DataManager.PlayerData.madeChoices[DataManager.PlayerData.chapterID].value[_currentDialogue.replicas[_replicaIndex].choiceID] = _pickedChoiceOption;

        //subreplicas numeration starts from 0 in every Replica object
        _currentSubReplicas = _pickedChoiceOption == 1 ? _currentDialogue.replicas[_replicaIndex].subreplicasOption1 : _currentDialogue.replicas[_replicaIndex].subreplicasOption2;
        _showSubReplicas = true;
        _subReplicaIndex = 0;

        _choicePanel.SetActive(false);
        _dialoguePanel.SetActive(true);

        DialogueClickHandler();

        //якщо діалог закінчуєтсья на чойсі без продовження фраз або підфраз - перед виходом зі сцени залишаються текстурки попереднього діалогу. В теорії фікситсья анімацією
    }
}
