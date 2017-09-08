[System.Serializable]
public class DialogNode
{
    private string dKey;
    private string strKey;
    private AdjacencyList neighbors;

    public DialogNode (string dKey, string strKey)
    {
        this.dKey = dKey;
        this.strKey = strKey;
        neighbors = new AdjacencyList();
    }

    public string StrKey
    {
        get
        {
            return strKey;
        }
        set
        {
            strKey = value;
        }
    }

    public string DKey
    {
        get
        {
            return dKey;
        }
    }

    public AdjacencyList Neighbors
    {
        get
        {
            return neighbors;
        }
    }

    public virtual string GetText()
    {
        return LanguageController.Instance.GetText(strKey);
    }

    public virtual bool Equals(DialogNode n)
    {
        if (dKey == n.DKey && n.Neighbors.Count == neighbors.Count)
        {
            for(int i = 0; i < neighbors.Count; ++i)
            {
                if (!neighbors[i].Neighbor.DKey.Equals(n.Neighbors[i].Neighbor.DKey))
                {
                    return false;
                }
            }
            return true;
        }
        else
        {
            return false;
        }
    }

    protected virtual internal void AddLink(DialogNode d)
    {
        AddLink(new Link(d));  
    }

    protected virtual internal void AddLink(Link l)
    {
        neighbors.Add(l);
    }

    protected virtual internal void RemoveLink(DialogNode d)
    {
        RemoveLink(new Link(d));
    }

    protected virtual internal void RemoveLink(Link l)
    {
        neighbors.Remove(l);
    }
}

[System.Serializable]
public class ResponseNode: DialogNode
{
    private string dKey;
    private string strKey;
    private Link neighbor;

    public ResponseNode(string dKey, string strKey) : base(dKey, strKey)
    {
        this.dKey = dKey;
        this.strKey = strKey;
    }

    protected internal override void AddLink(DialogNode d)
    {
        neighbor = new Link(d);
    }

    protected override internal void AddLink(Link l)
    {
        neighbor = l;
    }

    public Link Neighbor
    {
        get
        {
            return neighbor;
        }
    }
    
    protected virtual internal void RemoveLink()
    {
        neighbor = null;
    }
}
