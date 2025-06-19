using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LawActivity : IResultableActivity<Law>
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

    public bool TryGetData(PlayerData playerData, ChapterBatch batch, out Law data)
    {
        if (IsPending(playerData, batch))
        {
            int lastLawId = PlayerPrefs.GetInt("lastLawId", -1);
            data = _laws.FirstOrDefault(l => l.lawID == lastLawId)
                ?? _laws[Random.Range(0, _laws.Count)];

            PlayerPrefs.SetInt("lastLawId", data.lawID);
            PlayerPrefs.Save();

            return true;
        }
        else
        {
            data = null;
            return false;
        }
    }

    public void ApplyResult(PlayerData playerData, Law data, object context)
    {
        if (context is not bool accepted)
        {
            Debug.LogError("LawActivity ApplyResult: context must be a boolean indicating acceptance");
            return;
        }

        foreach (var (characteristic, value) in data.affectedCharacteristics)
        {
            int delta = accepted ? value : -value;
            playerData.Characteristics[characteristic] = Mathf.Clamp(
                playerData.Characteristics[characteristic] + delta, 0, 100);
        }

        _laws.Remove(data);
        playerData.UsedLawIds.Add(data.lawID);
        playerData.ProceededLawsCount++;
    }
}