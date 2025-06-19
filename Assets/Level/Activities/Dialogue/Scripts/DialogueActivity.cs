using System.Collections.Generic;

public class DialogueActivity : IGameActivity<Dialogue>
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

    public bool TryGetData(PlayerData playerData, ChapterBatch batch, out Dialogue data)
    {
        while (playerData.DialogueIndex < batch.requiredDialogueIndex && playerData.DialogueIndex < _dialogues.Count)
        {
            data = _dialogues[playerData.DialogueIndex];

            if (string.IsNullOrEmpty(data.requiredResponseId) || playerData.PickedResponses.Contains(data.requiredResponseId))
                return true;

            playerData.DialogueIndex++;
        }

        data = null;
        return false;
    }
}