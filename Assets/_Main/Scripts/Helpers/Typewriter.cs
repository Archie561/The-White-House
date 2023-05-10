using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Typewriter
{
    public TextMeshProUGUI Text { get; set; }
    public float WritingSpeed { get; set; } = 0.02f;
    public bool IsWriting { get; private set; } = false;

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

            yield return new WaitForSeconds(WritingSpeed);
        }

        IsWriting = false;
    }
}
