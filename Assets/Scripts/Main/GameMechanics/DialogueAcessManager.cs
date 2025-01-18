using UnityEngine;

public class DialogueAcessManager : MonoBehaviour
{
    [SerializeField] private GameObject _dialoguesMark;

    private bool _isDialogueLocked;

    private ChapterDataManager _chapterDataManager;
    private PlayerDataManager _playerDataManager;

    private void Awake()
    {
        GameManager.Instance.OnGameStateChanged += OnStateChanged;

        _chapterDataManager = ServiceLocator.GetService<ChapterDataManager>();
        _playerDataManager = ServiceLocator.GetService<PlayerDataManager>();
    }

    private void OnDestroy() => GameManager.Instance.OnGameStateChanged -= OnStateChanged;

    //handles game state change. If Default - updates dialogues status, if ActiveDialogues - loads dialogue scene, else - quit
    private void OnStateChanged(GameState state)
    {
        if (state == GameState.Default) UpdateDialoguesStatus();

        else if (state == GameState.ActiveDialogues) LevelManager.Instance.LoadScene("Dialogue");
    }

    //sets game state to ActiveDialogues on button click if dialogues are not locked and current game state is Default
    public void DialogueClickHandler()
    {
        if (!_isDialogueLocked && GameManager.Instance.State == GameState.Default)
        {
            AudioManager.Instance.PlaySFX("door");
            GameManager.Instance.ChangeGameState(GameState.ActiveDialogues);
        }
    }

    //defines whether dialogue is locked or not: if dialogue isn't locked but condition for it isn't met, increment dialogue id and invokes method again
    private void UpdateDialoguesStatus()
    {
        int dialogueID = _playerDataManager.DialogueID;

        _isDialogueLocked =
            dialogueID >= _chapterDataManager.GetDialoguesLength() ||
            _chapterDataManager.GetDialogue(dialogueID).lockedByLaw >= _playerDataManager.LawID ||
            _chapterDataManager.GetDialogue(dialogueID).lockedByDecision >= _playerDataManager.DecisionID;

        if (!_isDialogueLocked && !_playerDataManager.IsConditionMet(_chapterDataManager.GetDialogue(dialogueID).condition))
        {
            _playerDataManager.UpdateDialogueID(dialogueID + 1);
            UpdateDialoguesStatus();
            return;
        }

        _dialoguesMark.SetActive(!_isDialogueLocked);
    }
}