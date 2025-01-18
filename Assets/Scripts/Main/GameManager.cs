using System;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    //поділити таблиці локалізації
    //audio manager, pause manager, cutscene manager
    //додати написи до sure панелі в селекторі президента
    //шось не то з переходами
    //додати бінарну серіалізацію до сейв дати

    public static GameManager Instance { get; private set; }

    public GameState State { get; private set; }

    private PlayerDataManager _playerDataManager;
    private ChapterDataManager _chapterDataManager;

    public event Action<GameState> OnGameStateChanged;

    private void Awake()
    {
        if (Instance == null) Instance = this;

        _playerDataManager = ServiceLocator.GetService<PlayerDataManager>();
        _chapterDataManager = ServiceLocator.GetService<ChapterDataManager>();

/*        if (_playerDataManager == null && _chapterDataManager == null)
        {
            var playerDataManager = new PlayerDataManager("KenRothwell", 12);
            var chapterDataManager = new ChapterDataManager("KenRothwell", playerDataManager.ChapterID);

            ServiceLocator.RegisterService(playerDataManager);
            ServiceLocator.RegisterService(chapterDataManager);

            _playerDataManager = ServiceLocator.GetService<PlayerDataManager>();
            _chapterDataManager = ServiceLocator.GetService<ChapterDataManager>();
        }*/
    }

    private void Start() => ChangeGameState(GameState.FirstLoading);

    public void ChangeGameState(GameState newState)
    {
        State = newState;

        switch (newState)
        {
            case GameState.FirstLoading:
                HandleFirstLoading();
                break;
            case GameState.Default:
                HandleDefault();
                break;
        }

        OnGameStateChanged?.Invoke(newState);
    }

    private void HandleFirstLoading()
    {
        //for diploma
        if (!AudioManager.Instance.IsMusicPlaying) AudioManager.Instance.SetMusic("background-music");

        if (!_playerDataManager.StartingCutscenesShown && _chapterDataManager.GetStartingCutscenes().Length > 0)
        {
            LevelManager.Instance.LoadScene("Cutscene");
            return;
        }
        
        if (_playerDataManager.LawID == 0 && _playerDataManager.DecisionID == 0 && _playerDataManager.DialogueID == 0)
            ChangeGameState(GameState.ActiveNews);
        else
            ChangeGameState(GameState.Default);
    }

    private void HandleDefault()
    {
        _playerDataManager.SaveData();

        bool isChapterFinished =
            _playerDataManager.DialogueID >= _chapterDataManager.GetDialoguesLength() &&
            _playerDataManager.DecisionID >= _chapterDataManager.GetDecisionsLength() &&
            _playerDataManager.LawID >= _chapterDataManager.GetLawsLength();

        if (isChapterFinished) ChangeGameState(GameState.EndOfChapter);
    }
}

public enum GameState
{
    FirstLoading,
    Default,
    ActiveNews,
    ActiveLaws,
    ActiveDialogues,
    ActiveDecisions,
    ActiveCharacteristics,
    Pause,
    EndOfChapter
}