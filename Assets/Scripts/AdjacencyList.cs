using UnityEngine;
using System.Collections;

[System.Serializable]
public class Link {
    private DialogNode neighbor;

    public Link(DialogNode neighbor)
    {
        this.neighbor = neighbor;
    }

    public virtual DialogNode Neighbor
    {
        get
        {
            return neighbor;
        }
    }
}

[System.Serializable]
public class AdjacencyList : CollectionBase {

	protected virtual internal void Add(Link l)
    {
        base.InnerList.Add(l);
    }

    protected virtual internal void Remove(Link l)
    {
        base.InnerList.Remove(l);
    }

    protected virtual internal bool Contains(DialogNode n)
    {
        foreach(Link l in base.InnerList)
        {
            if (l.Neighbor.Equals(n))
            {
                return true;
            }
        }
        return false;
    }

    public virtual Link this[int index]
    {
        get { return (Link)base.InnerList[index]; }
        set { base.InnerList[index] = value; }
    }
}
