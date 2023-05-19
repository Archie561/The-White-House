using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class DataManager : MonoBehaviour
{
    public static DataManager Instance { get; private set; }

    public Dialogue[] Dialogues;
    public Choice[] Choices;

    void Awake()
    {
        if (Instance == null)
        {
            LoadData();

            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void LoadData()
    {
        string dialoguePath = Application.dataPath + "/dialogues.json";
        string choicesPath = Application.dataPath + "/choices.json";

        if (File.Exists(dialoguePath) && File.Exists(choicesPath))
        {
            string dialoguesJson = File.ReadAllText(dialoguePath);
            string choicesJson = File.ReadAllText(choicesPath);

            Dialogues = JsonHelper.FromJson<Dialogue>(dialoguesJson);
            Choices = JsonHelper.FromJson<Choice>(choicesJson);
        }
        else
        {
            Debug.LogError($"File(s) does not exist: {dialoguePath} || {choicesPath}");
        }
    }

    public Dialogue GetFirstDialogue()
    {
        return Dialogues[0];
    }

    public Choice GetFirstChoice()
    {
        return Choices[0];
    }
}
