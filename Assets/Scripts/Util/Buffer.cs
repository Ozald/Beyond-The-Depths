using System.Collections.Generic;

/// <summary>
/// Represents a buffer that will "cycle" items.
/// When an item is removed from the buffer, it is re-added
/// to the back of the buffer.
/// </summary>
/// <typeparam name="T">
/// The type of item in the buffer
/// </typeparam>
public class Buffer<T>
{
    /// <summary>
    /// An item in the buffer with its value
    /// </summary>
    private class BufferedItem
    {
        public T Item { get; } 
        public int Value { get; }
        
        public BufferedItem(T item, int value)
        {
            this.Item = item;
            this.Value = value;
        }
    }
    
    private Queue<BufferedItem> BufferedItems;
    
    public int Count { get { return BufferedItems.Count; } }
    
    public Buffer()
    {
        BufferedItems = new Queue<BufferedItem>();
    }
    
    /// <summary>
    /// Adds an item to the buffer. Be careful with item value
    /// for removal with budgets.
    /// </summary>
    /// <param name="item">
    /// The item to add to the buffer
    /// </param>
    /// <param name="value">
    /// The value of the item
    /// </param>
    public void Add(T item, int value = 1)
    {
        BufferedItems.Enqueue(new BufferedItem(item, value));
    }

    /// <summary>
    /// Retrieves the first item in the buffer
    /// without removal
    /// </summary>
    /// <returns>
    /// The first item in the buffer
    /// </returns>
    public T? Peek()
    {
        return BufferedItems.Count > 0 ? BufferedItems.Peek().Item : default(T);
    }
    
    /// <summary>
    /// Removes the first item from the buffer and by default
    /// adds it to the back of the buffer.
    /// </summary>
    /// <param name="reAdd">
    /// Whether to add the item back to the end of the buffer
    /// </param>
    /// <returns>
    /// The first item in the buffer
    /// </returns>
    public T? Remove(bool reAdd = true)
    {
        if(BufferedItems.Count == 0)
            return default;
        
        BufferedItem item = BufferedItems.Dequeue();
        
        if (reAdd && item.Item is not null)
            BufferedItems.Enqueue(item);

        return item.Item;
    }

    /// <summary>
    /// Removes a certain number of items if possible
    /// </summary>
    /// <param name="items">
    /// The number of items to remove
    /// </param>
    /// <param name="reAdd">
    /// Whether to re-add removed items
    /// </param>
    /// <returns>
    /// A list of removed items
    /// </returns>
    public List<T> Remove(int items, bool reAdd = true)
    {
        List<T> removed = new List<T>();

        for (int i = 0; i < items; i++)
        {
            if (BufferedItems.Count == 0)
                break;
            
            BufferedItem item = BufferedItems.Dequeue();
            removed.Add(item.Item);
            
            if(reAdd && item.Item is not null)
                BufferedItems.Enqueue(item);
        }
        
        return removed;
    }

    /// <summary>
    /// Removes as many items as possible without exceeding
    /// a specified budget value. Removal is still done in
    /// FIFO order. Be careful of item value as an infinite loop
    /// can occur if item values can never accumulate to the budget and if
    /// the queue is just cycled.
    /// </summary>
    /// <param name="budget">
    /// The maximum budget for item removal
    /// </param>
    /// <param name="reAdd">
    /// Whether to add removed items back to the buffer
    /// </param>
    /// <returns>
    /// A list of removed items
    /// </returns>
    public List<T> RemoveBudget(int budget, bool reAdd = true)
    {
        List<T> removed = new List<T>();
        int removedValue = 0;

        while (removedValue < budget && BufferedItems.Count > 0)
        {
            BufferedItem item = BufferedItems.Dequeue();
            removedValue += item.Value;
        
            if (reAdd && item.Item is not null)
                BufferedItems.Enqueue(item);
            
            removed.Add(item.Item);
        }
        
        return removed;
    }
    
    /// <summary>
    /// Removes as many items as possible without exceeding
    /// a specified budget value. Removal is still done in
    /// FIFO order. Result will not contain duplicates. Be careful of
    /// item value as an infinite loop can occur if item values can never
    /// accumulate to the budget and if the queue is just cycled.
    /// </summary>
    /// <param name="budget">
    /// The maximum budget for item removal
    /// </param>
    /// <param name="reAdd">
    /// Whether to add removed items back to the buffer
    /// </param>
    /// <returns>
    /// A hash set of removed items
    /// </returns>
    public HashSet<T> RemoveBudgetNoDuplicates(int budget, bool reAdd = true)
    {
        HashSet<T> removed = new HashSet<T>();
        int removedValue = 0;

        while (removedValue < budget && BufferedItems.Count > 0)
        {
            BufferedItem item = BufferedItems.Dequeue();
            removedValue += item.Value;
        
            if (reAdd && item.Item is not null)
                BufferedItems.Enqueue(item);
            
            removed.Add(item.Item);
        }
        
        return removed;
    }

    /// <summary>
    /// Clears the buffer
    /// </summary>
    public void Clear()
    {
        BufferedItems.Clear();
    }

    /// <summary>
    /// Checks if the buffer contains a specific item
    /// </summary>
    /// <param name="item">The item to check for</param>
    /// <returns>True if the item is in the buffer, false otherwise</returns>
    public bool Contains(T item)
    {
        foreach (BufferedItem bufferedItem in BufferedItems)
        {
            if (EqualityComparer<T>.Default.Equals(bufferedItem.Item, item))
            {
                return true;
            }
        }
        return false;
    }

    public override string ToString()
    {
        string s = "";
        
        foreach(BufferedItem item in BufferedItems)
            s += item.Item + ", ";
        
        return s.Substring(0, s.Length - 2);
    }
}