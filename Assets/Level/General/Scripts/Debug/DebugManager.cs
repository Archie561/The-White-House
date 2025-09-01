using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class DebugManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _dialogueIdText;
    public UnityEvent OnDialogueIdModified;

    private void Start()
    {
        UpdateDialogueText();
    }

    private void UpdateDialogueText() => _dialogueIdText.text = "Dialogue: " + GameDataManager.DialogueID;

    public void ModifyDialogueId(int value)
    {
        AudioManager.Instance.PlaySFX(SFXType.Click);
        GameDataManager.ModifyDialogueId(value);
        UpdateDialogueText();
        GameDataManager.Save();

        OnDialogueIdModified?.Invoke();
    }

    public void ResetPlayerData()
    {
        AudioManager.Instance.PlaySFX(SFXType.Click);
        GameDataManager.ResetPlayerData();
        GameDataManager.Save();

        LevelManager.Instance.LoadScene("Main");
    }
}
