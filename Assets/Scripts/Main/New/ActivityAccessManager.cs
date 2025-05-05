using System.Collections.Generic;
using UnityEngine;

public class ActivityAccessManager : MonoBehaviour
{
    [SerializeField] private List<BaseActivityButton> _activityButtons;

    private void Awake()
    {
        GameManager.Instance.OnGameStateChanged += OnStateChanged;

        //_chapterDataManager = ServiceLocator.GetService<ChapterDataManager>();
        //_playerDataManager = ServiceLocator.GetService<PlayerDataManager>();
    }

    private void OnStateChanged(GameState state)
    {
        if (state == GameState.Default)
            UpdateActivitiesStatus();
    }

    private void UpdateActivitiesStatus()
    {
        //var batchIDs = _chapterDataManager.GetBatch(_playerDataManager.BatchID);

        foreach (var button  in _activityButtons)
        {
            var activity = button.ActivityType;

            //if (batchIDs.TryGetValue(activity, out int maxID))
            //{
                //button.SetInteractable(_playerDataManager.GetActivityID(activity) < maxID);
            //}
            //else
            {
                Debug.LogWarning($"There is no batch data for activity: {activity}");
            }
        }
    }
}