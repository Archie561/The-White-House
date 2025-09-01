public interface IGameActivity
{
    /// <summary>
    /// Determines if the activity is pending based on the player's data and the current chapter batch.
    /// </summary>
    /// <param name="playerData"></param>
    /// <param name="batch"></param>
    /// <returns></returns>
    bool IsPending(PlayerData playerData, ChapterBatch batch);
}