using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization.Settings;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour
{
    [SerializeField] private BudgetBox _budgetBox;
    [SerializeField] private RawImage _backgroundImage;
    [SerializeField] private DialoguePanel _dialoguePanel;

    private bool _shouldDisplayChoice;
    private bool _shouldDisplaySubreplica;

    private int _replicaID;
    private int _subreplicaID;

    private Dialogue _currentDialogue;
    private SubReplica[] _currentSubreplicas;

    private string _localizedPresidentName;
    private PlayerDataManager _playerDataManager;
    private ChapterDataManager _chapterDataManager;
    private Dictionary<string, Sprite> _characterSprites;

    private void Start()
    {
        _playerDataManager = ServiceLocator.GetService<PlayerDataManager>();
        _chapterDataManager = ServiceLocator.GetService<ChapterDataManager>();

        SetUpMechanic();
    }

    //initializes variables and subscribes to events
    private void SetUpMechanic()
    {
        _replicaID = 0;
        //_currentDialogue = _chapterDataManager.GetDialogue(_playerDataManager.DialogueID);
        _localizedPresidentName = LocalizationSettings.StringDatabase.GetLocalizedString("UILocalization", _playerDataManager.ActivePresident);

        LoadCharactersSprites();

        if (!string.IsNullOrEmpty(_currentDialogue.backgroundMusic))
            AudioManager.Instance.SetMusic(_currentDialogue.backgroundMusic);
        if (!string.IsNullOrEmpty(_currentDialogue.backgroundImageName))
            _backgroundImage.texture = Resources.Load<Texture>($"Textures/BackgroundImages/{_currentDialogue.backgroundImageName}");

        _dialoguePanel.OnChoiceClick += ChoiceClickHandler;
        _dialoguePanel.OnReplicaClick += ReplicaClickHandler;

        ReplicaClickHandler();
    }

    //loads all character sprites used in the dialog for quick access to them
    private void LoadCharactersSprites()
    {
        _characterSprites = new Dictionary<string, Sprite>();

        foreach (var replica in _currentDialogue.replicas)
        {
            LoadSprite(replica.imageName);

            foreach (var subreplica in replica.subreplicasOption1)
                LoadSprite(subreplica.imageName);

            foreach (var subreplica in replica.subreplicasOption2)
                LoadSprite(subreplica.imageName);
        }
        LoadSprite(_playerDataManager.ActivePresident);

        void LoadSprite(string name)
        {
            if (_characterSprites.ContainsKey(name)) return;

            var sprite = Resources.Load<Sprite>($"Textures/Characters/{name}");
            if (sprite != null) _characterSprites.Add(name, sprite);
            else Debug.LogError($"Sprite not found: {name}");
        }
    }

    //handles click on dialogue panel while replicas showning: if text writing animation is not finished - skip it,
    //if needs to display choice after current replica - show choice, if needs to display subreplicas after made choice
    // - show subreplica. Else - show replica. Handler ignores clicks when choice box is active or character image animation is not finished
    private void ReplicaClickHandler()
    {
        if (_dialoguePanel.IsTextWrining)
        {
            _dialoguePanel.SkipTextWriting();
            return;
        }

        if (_shouldDisplayChoice)
        {
            ShowChoice();
            return;
        }

        if (_shouldDisplaySubreplica)
        {
            ShowSubreplica();
            return;
        }

        ShowReplica();
    }

    //shows choice box with current choice
    private void ShowChoice()
    {
        var choice = _chapterDataManager.GetChoice(_currentDialogue.replicas[_replicaID].choiceID);
        _dialoguePanel.ShowChoice(_characterSprites[_playerDataManager.ActivePresident], _localizedPresidentName, choice.option1, choice.option2);
    }

    //shows replica box with current subreplicas. If all subreplicas for this replica is shown - move to the next replica
    private void ShowSubreplica()
    {
        var subreplica = _currentSubreplicas[_subreplicaID];
        _dialoguePanel.ShowReplica(_characterSprites[subreplica.imageName], subreplica.characterName, subreplica.subReplicaText);
        AudioManager.Instance.PlaySFX("typing");

        _subreplicaID++;
        _shouldDisplaySubreplica = _subreplicaID < _currentSubreplicas.Length;
        if (!_shouldDisplaySubreplica) _replicaID++;
    }

    //shows replica box with current replica and detects if needs to display choice after it. If all replicas is shown - exit mechanic
    private void ShowReplica()
    {
        if (_replicaID >= _currentDialogue.replicas.Length)
        {
            Exit();
            return;
        }

        var replica = _currentDialogue.replicas[_replicaID];
        _dialoguePanel.ShowReplica(_characterSprites[replica.imageName], replica.characterName, replica.replicaText);
        AudioManager.Instance.PlaySFX("typing");

        _shouldDisplayChoice = _currentDialogue.replicas[_replicaID].choiceID != -1;
        if (!_shouldDisplayChoice) _replicaID++;
    }

    //saves the made choice and updates the characteristics depending on it.
    //Detects if needs to display subreplicas after current choice: if not - move to the next replica
    private void ChoiceClickHandler(int pickedChoice)
    {
        _shouldDisplayChoice = false;
        AudioManager.Instance.PlaySFX("button");

        var replica = _currentDialogue.replicas[_replicaID];
        var choice = _chapterDataManager.GetChoice(replica.choiceID);

        var characteristicsToUpdate = pickedChoice == 1 ? choice.characteristicsUpdateOption1 : choice.characteristicsUpdateOption2;

/*        _playerDataManager.SaveChoice(choice.choiceID, pickedChoice);
        _playerDataManager.UpdateCharacteristics(characteristicsToUpdate);*/
        //_budgetBox.UpdateBudget(characteristicsToUpdate.budget);

        _currentSubreplicas = pickedChoice == 1 ? replica.subreplicasOption1 : replica.subreplicasOption2;

        _subreplicaID = 0;
        _shouldDisplaySubreplica = _currentSubreplicas.Length > 0;
        if (!_shouldDisplaySubreplica) _replicaID++;

        ReplicaClickHandler();
    }

    //hides dialogue panel, unsubscribes from events and updates dialogue id, then - loads main scene
    private void Exit()
    {
        _dialoguePanel.HidePanel();

        _dialoguePanel.OnReplicaClick -= ReplicaClickHandler;
        _dialoguePanel.OnChoiceClick -= ChoiceClickHandler;

        //_playerDataManager.UpdateDialogueID(_currentDialogue.dialogueID + 1);
        LevelManager.Instance.LoadScene("Main");
    }
}