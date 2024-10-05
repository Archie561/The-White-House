using System.Collections;
using UnityEngine;
using TMPro;
using System;

public class Typewriter
{
    public TextMeshProUGUI Text { get; set; }
    public float UpdateTime { get; set; } = 0.02f;
    public bool IsWriting { get; private set; } = false;

    public event Action OnWritingFinished;

    public Typewriter()
    {
        Text = null;
    }
    public Typewriter(TextMeshProUGUI text)
    {
        Text = text;
    }
    public Typewriter(TextMeshProUGUI text, float updateTime)
    {
        Text = text;
        UpdateTime = updateTime;
    }

    public void StartWriting()
    {
        if (!IsWriting && Text != null)
        {
            Text.StartCoroutine(Write());
        }
    }

    public void SkipWriting()
    {
        if (IsWriting)
        {
            IsWriting = false;
        }
    }

    private IEnumerator Write()
    {
        IsWriting = true;
        Text.ForceMeshUpdate();

        int totalCharacterAmount = Text.textInfo.characterCount;
        int visibleCharacterAmount = 0;
        int counter = 0;

        while (visibleCharacterAmount < totalCharacterAmount)
        {
            visibleCharacterAmount = IsWriting ? counter % (totalCharacterAmount + 1) : totalCharacterAmount;

            Text.maxVisibleCharacters = visibleCharacterAmount;
            counter++;

            yield return new WaitForSeconds(UpdateTime);
        }

        IsWriting = false;

        if (OnWritingFinished != null)
        {
            OnWritingFinished();
        }
    }
}
