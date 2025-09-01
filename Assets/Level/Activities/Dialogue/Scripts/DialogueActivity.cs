using System;
using System.Collections.Generic;

public class DialogueActivity
{
    private readonly List<Dialogue> _dialogues;

    public DialogueActivity(List<Dialogue> dialogues)
    {
        _dialogues = dialogues;
    }

    public bool IsPending(PlayerData playerData, ChapterBatch batch)
    {
        int index = playerData.DialogueIndex;

        while (index < batch.requiredDialogueIndex && index < _dialogues.Count)
        {
            var dialogue = _dialogues[index];

            if (string.IsNullOrEmpty(dialogue.requiredResponseId) || playerData.PickedResponses.Contains(dialogue.requiredResponseId))
                return true;

            index++;
        }

        return false;
    }

    public bool TryGetNextDialogue(PlayerData playerData, ChapterBatch batch, out Dialogue dialogue)
    {
        while (playerData.DialogueIndex < batch.requiredDialogueIndex && playerData.DialogueIndex < _dialogues.Count)
        {
            dialogue = _dialogues[playerData.DialogueIndex];

            if (string.IsNullOrEmpty(dialogue.requiredResponseId) || playerData.PickedResponses.Contains(dialogue.requiredResponseId))
                return true;

            playerData.DialogueIndex++;
        }

        dialogue = null;
        return false;
    }

    public void SaveResponse(PlayerData playerData, string result)
    {
        if (string.IsNullOrEmpty(result))
        {
            throw new ArgumentException("Result must be a choice id!", nameof(result));
        }
        playerData.PickedResponses.Add(result);
    }
}