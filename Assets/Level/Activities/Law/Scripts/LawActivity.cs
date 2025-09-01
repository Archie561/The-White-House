using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LawActivity
{
    private readonly List<Law> _laws;

    public LawActivity(List<Law> laws)
    {
        _laws = laws;
    }

    public bool IsPending(PlayerData playerData, ChapterBatch batch)
    {
        if (playerData.ProceededLawsCount >= batch.requiredProceededLaws)
            return false;

        if (_laws.Count == 0)
        {
            Debug.LogWarning("All laws were shown! Clear used law IDs");
            return false;
        }

        return true;
    }

    public bool TryGetNextLaw(PlayerData playerData, ChapterBatch batch, out Law law)
    {
        if (!IsPending(playerData, batch))
        {
            law = null;
            return false;
        }

        int lastLawId = PlayerPrefs.GetInt("lastLawId", -1);
        law = _laws.FirstOrDefault(l => l.lawID == lastLawId)
            ?? _laws[Random.Range(0, _laws.Count)];

        PlayerPrefs.SetInt("lastLawId", law.lawID);
        PlayerPrefs.Save();

        return true;
    }

    public void SaveLawDecision(PlayerData playerData, Law law, bool accepted)
    {
        foreach (var (characteristic, value) in law.affectedCharacteristics)
        {
            int delta = accepted ? value : -value;
            playerData.Characteristics[characteristic] = Mathf.Clamp(
                playerData.Characteristics[characteristic] + delta, 0, 100);
        }

        _laws.Remove(law);
        playerData.UsedLawIds.Add(law.lawID);
        playerData.ProceededLawsCount++;
    }
}