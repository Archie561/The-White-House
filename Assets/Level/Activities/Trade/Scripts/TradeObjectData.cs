using System;

[Serializable]
public class TradeObjectData
{
    public int Amount { get; private set; }
    public int Capacity { get; private set; }

    public TradeObjectData(int amount, int capacity)
    {
        Amount = amount;
        Capacity = capacity;
    }

    /// <summary>
    /// Adds some value to the current amount
    /// </summary>
    /// <param name="value">Value to add</param>
    /// <returns></returns>
    public bool TryUpdateAmount(int value)
    {
        var newAmount = Math.Min(Amount + value, Capacity);

        if (newAmount < 0)
        {
            Amount = 0;
            return false;
        }
        else
        {
            Amount = newAmount;
            return true;
        }
    }

    /// <summary>
    /// Sets new capacity for this trade object
    /// </summary>
    /// <param name="newCapacity"></param>
    public void SetCapacity(int newCapacity)
    {
        if (newCapacity > 0)
            Capacity = newCapacity;
    }
}
