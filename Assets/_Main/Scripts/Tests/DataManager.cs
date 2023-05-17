using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataManager : MonoBehaviour
{
    public static DataManager Instance;

    public Dialogue currentDialogue;
    public Choice currentChoice;
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        currentDialogue = new Dialogue();

        string[] characterNames = { "Alice Rothwell", "Alice Rothwell", "Lyle Ellington", "Roberta Horton", "Alice Rothwell", "Lyle Ellington" };
        string[] imageNames = { "AliceRothwell", "AliceRothwell", "LyleEllington", "RobertaHorton", "AliceRothwell", "LyleEllington" };
        string[] replicas =
        {
            "Lorem ipsum, or lipsum as it is sometimes known, is dummy text used in laying out print, graphic or web designs.",
            "The passage is attributed to an unknown typesetter in the 15th century who is thought to have scrambled parts of Cicero's De Finibus Bonorum et Malorum for use in a type specimen book.",
            "The purpose of lorem ipsum is to create a natural looking block of text (sentence, paragraph, page, etc.)",
            "Lorem ipsum, or lipsum as it is sometimes known, is dummy text used in laying out print, graphic or web designs.",
            "The passage is attributed to an unknown typesetter in the 15th century who is thought to have scrambled parts of Cicero's De Finibus Bonorum et Malorum for use in a type specimen book.",
            "The purpose of lorem ipsum is to create a natural looking block of text (sentence, paragraph, page, etc.)"
        };
        bool[] isChoices = { false, true, false, true, false, false };

        currentDialogue.characterName = characterNames;
        currentDialogue.imageName = imageNames;
        currentDialogue.replicas = replicas;
        currentDialogue.isChoice = isChoices;

        currentChoice = new Choice();

        string[] choiceNames = { "firstChoice", "secondChoice" };
        string[] options1 = { "looking block of text", "attributed to an unknown typesetter" };
        string[] options2 = { "Lorem ipsum, or lipsum as it is", "who is thought to have scrambled" };
        bool[] mustBeSaved = { false, false };

        currentChoice.choiceName = choiceNames;
        currentChoice.option1 = options1;
        currentChoice.option2 = options2;
        currentChoice.mustBeSaved = mustBeSaved;
    }
}
