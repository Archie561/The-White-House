using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public Chapter CurrentChapter { get; private set; }

    void Awake()
    {
        if (Instance == null)
        {
            DataManager.LoadData();
            CurrentChapter = DataManager.GetCurrentChapter();

            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public Dialogue GetNextDialogue()
    {
        return CurrentChapter.dialogues[DataManager.PlayerData.dialogueIndex];
    }

    public Choice GetChoice(sbyte index)
    {
        return CurrentChapter.choices[index];
    }
}
