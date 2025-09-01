using System;
using System.Collections.Generic;
using UnityEngine;

public class ActivityAccessManager : MonoBehaviour
{
    [SerializeField] private List<ActivityButton> _activityButtons;

    private void Start()
    {
        UpdatePendingIndicators();

        if (IsChapterFinished())
        {
            if (GameDataManager.TryMoveToNextBatch())
                UpdatePendingIndicators();
            else
                Debug.Log("Всі батчі пройдені, потрібно перейти до наступної глави");
        }
    }

    //temp public, потім поміняти на прайвет
    public void UpdatePendingIndicators()
    {
        foreach (ActivityButton button in _activityButtons)
        {
            button.SetPendingIndicator(GameDataManager.IsPending(button.ActivityType));
        }
    }

    private bool IsChapterFinished()
    {
        foreach (ActivityType activity in Enum.GetValues(typeof(ActivityType)))
        {
            if (GameDataManager.IsPending(activity))
                return false;
        }

        return true;
    }
}
