using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class DebugButton : MonoBehaviour
{
    private TextMeshProUGUI _dialogueIdText;
    public UnityEvent OnPlayerDataChanged;
    private void Start()
    {
        if (_dialogueIdText == null)
        {
            _dialogueIdText = GameObject.FindGameObjectWithTag("ChoiceOption1").GetComponent<TextMeshProUGUI>();
            _dialogueIdText.text = "Dialogue: " + GameDataManager.DialogueID;
        }
    }

    public void ClearUsedLawIds()
    {
        AudioManager.Instance.PlaySFX(SFXType.Click);
        GameDataManager.ClearUsedLawIds();
        GameDataManager.Save();
    }

    public void ResetPlayerCharacteristics()
    {
        AudioManager.Instance.PlaySFX(SFXType.Click);
        GameDataManager.ResetPlayerCharacteristics();
        GameDataManager.Save();
    }

    public void ModifyDialogueId(int value)
    {
        AudioManager.Instance.PlaySFX(SFXType.Click);
        GameDataManager.ModifyDialogueId(value);
        _dialogueIdText.text = "Dialogue: " + GameDataManager.DialogueID;
        GameDataManager.Save();

        OnPlayerDataChanged?.Invoke();
    }

    public void ResetBatchData()
    {
        AudioManager.Instance.PlaySFX(SFXType.Click);
        GameDataManager.ResetBatchData();
        GameDataManager.Save();

        //hardcoded
        OnPlayerDataChanged?.Invoke();
    }
}
