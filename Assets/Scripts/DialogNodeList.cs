using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class DialogNodeList : IEnumerable {

    private Dictionary<string, DialogNode> dialogData = new Dictionary<string, DialogNode>();

    public virtual void Add(DialogNode n)
    {
        dialogData.Add(n.DKey, n);
    }

    public virtual void Remove(DialogNode n)
    {
        dialogData.Remove(n.DKey);
    }

    public virtual bool ContainsKey(string dkey)
    {
        return dialogData.ContainsKey(dkey);
    }

    public virtual void Clear()
    {
        dialogData.Clear();
    }

    public IEnumerator GetEnumerator()
    {
        return ((IEnumerable)dialogData).GetEnumerator();
    }

    public virtual DialogNode this[string key]
    {
        get
        {
            return dialogData[key];
        }
    }

    public int Count
    {
        get
        {
            return dialogData.Count;
        }
    }
}
