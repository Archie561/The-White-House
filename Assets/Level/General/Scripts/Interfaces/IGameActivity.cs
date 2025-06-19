public interface IGameActivity<T>
{
    bool IsPending(PlayerData playerData, ChapterBatch batch);
    bool TryGetData(PlayerData playerData, ChapterBatch batch, out T data);
}

public interface IResultableActivity<T> : IGameActivity<T>
{
    void ApplyResult(PlayerData playerData, T data, object context);
}
