using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    //������� ������� ����������
    //audio manager, pause manager, cutscene manager
    //������ ������ �� sure ����� � �������� ����������
    //���� �� �� � ����������
    //������ ������ ���������� �� ���� ����

    public static GameManager Instance { get; private set; }

    public GameState State { get; private set; }

    public event Action<GameState> OnGameStateChanged;
    public Dictionary<Type, int> Activities = new();

    private void Awake()
    {
        if (Instance == null) Instance = this;

        Activities.Add(typeof(Dialogue), 1);

        string json = JsonConvert.SerializeObject(Activities);

        GameDataManager.Initialize("KenRothwell");
        /*
        GameDataManager.PlayerData.SaveDecision(0, 0, 1);
        GameDataManager.PlayerData.SaveChoice(0, 0, 1);
        GameDataManager.PlayerData.UpdateActivityID(ActivityType.Dialogue, 0);
        GameDataManager.PlayerData.UpdateActivityID(ActivityType.Decision, 0);

        Dictionary<Characteristic, int> characteristics = new();
        foreach (Characteristic characteristic in Enum.GetValues(typeof(Characteristic)))
            characteristics[characteristic] = 0;
        GameDataManager.PlayerData.UpdateCharacteristics(characteristics);

        GameDataManager.SavePlayerData();*/

        //_playerDataManager = ServiceLocator.GetService<PlayerDataManager>();
        //_chapterDataManager = ServiceLocator.GetService<ChapterDataManager>();

        /*        if (_playerDataManager == null && _chapterDataManager == null)
                {
                    var playerDataManager = new PlayerDataManager("KenRothwell", 12);
                    //var chapterDataManager = new ChapterDataManager("KenRothwell", playerDataManager.ChapterID);

                    ServiceLocator.RegisterService(playerDataManager);
                    //ServiceLocator.RegisterService(chapterDataManager);

                    //_playerDataManager = ServiceLocator.GetService<PlayerDataManager>();
                    //_chapterDataManager = ServiceLocator.GetService<ChapterDataManager>();
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
/*        if (!_playerDataManager.StartingCutscenesShown && _chapterDataManager.GetStartingCutscenes().Length > 0)
        {
            LevelManager.Instance.LoadScene("Cutscene");
            return;
        }*/
        
/*        if (_playerDataManager.LawID == 0 && _playerDataManager.DecisionID == 0 && _playerDataManager.DialogueID == 0)
            ChangeGameState(GameState.ActiveNews);
        else
            ChangeGameState(GameState.Default);*/
    }

    private void HandleDefault()
    {
/*        _playerDataManager.SaveData();

        bool isChapterFinished =
            _playerDataManager.DialogueID >= _chapterDataManager.GetDialoguesLength() &&
            _playerDataManager.DecisionID >= _chapterDataManager.GetDecisionsLength() &&
            _playerDataManager.LawID >= _chapterDataManager.GetLawsLength();

        if (isChapterFinished) ChangeGameState(GameState.EndOfChapter);*/
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