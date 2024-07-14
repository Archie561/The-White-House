using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoryFlowManager : MonoBehaviour
{
    void Start()
    {
        //катсцени можна добавити у сцені загрузки
        //if cutscenes - showCunscenes();
        //if news - showNews();

        UpdateMechanicsStatus();
        //check if it is the end of chapter
    }

    //функція для оновлення статусу механік (заблокована/розблокована)
    public void UpdateMechanicsStatus()
    {
        //якщо діалоги не заблоковані, але умова для показу блоку діалогу не виконується - пропустити діалог
        if (!GameManager.Instance.IsDialogueLocked() && !GameManager.Instance.IsConditionMet(GameManager.Instance.GetNextDialogue().condition))
        {
            DataManager.PlayerData.dialogueID++;
            DataManager.SaveData();
            UpdateMechanicsStatus();
        }
        if (!GameManager.Instance.IsDecisionLocked() && !GameManager.Instance.IsConditionMet(GameManager.Instance.GetNextDecision().condition))
        {
            DataManager.PlayerData.decisionID++;
            DataManager.SaveData();
            UpdateMechanicsStatus();
        }

        GameObject.Find("Canvas").GetComponent<UIHandler>().SetMechanicMarks();
    }

}
