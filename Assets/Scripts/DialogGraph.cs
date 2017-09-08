using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/**
 * https://msdn.microsoft.com/en-us/library/aa289152(v=vs.71).aspx
 * */

[System.Serializable]
public class DialogGraph
{
    private DialogNodeList nodes;
    private DialogNode root;

    public DialogGraph()
    {
        nodes = new DialogNodeList();
    }

    public virtual void Add(DialogNode n)
    {
        if (!Contains(n))
        {
            nodes.Add(n);
        }
        else
        {
            Debug.Log("Dialog node already exists!");
        }
    }

    public virtual void AddLink(string ukey, string vkey)
    {
        if (Contains(ukey) && Contains(vkey))
        {
            AddLink(nodes[ukey], nodes[vkey]);
        }
        else
        {
            Debug.Log("Failed to add link, one or both of the keys missing.");
        }
    }

    public virtual void AddLink(DialogNode u, DialogNode v)
    {
        if (nodes.ContainsKey(u.DKey) && nodes.ContainsKey(u.DKey))
        {
            u.AddLink(v);
        }
        else
        {
            Debug.Log("Failed to add link, one or both of the keys missing.");
        }
    }

    public virtual void Remove(string key)
    {
        if (Contains(key))
        {
            Remove(nodes[key]);
        }
        else
        {
            Debug.Log("The key does not exist.");
        }
    }

    public virtual void Remove(DialogNode node)
    {
        if (nodes.ContainsKey(node.DKey))
        {
            RemoveAllLink(node);
            nodes.Remove(node);
        }
        else
        {
            Debug.Log("Error removing the node: the key does not exist.");
        }
    }

    public virtual void RemoveLink(string ukey, string vkey)
    {
        if (Contains(ukey) && Contains(vkey))
        {
            RemoveLink(nodes[ukey], nodes[vkey]);
        }
        else
        {
            Debug.Log("Failed to remove link, one or both of the keys missing.");
        }
    }

    public virtual void RemoveLink(DialogNode u, DialogNode v)
    {
        if (!Contains(u))
        {
            Debug.Log("Failed to remove link. Dialog node missing");
            return;
        }

        if (u.GetType() == typeof(ResponseNode))
        {
            ((ResponseNode)u).RemoveLink();
        }
        else if (v.GetType() == typeof(ResponseNode))
        {
            u.RemoveLink(v);
        }        
    }

    public virtual void RemoveAllLink(DialogNode d)
    {
        if (d.GetType() == typeof(ResponseNode))
        {
            ((ResponseNode)d).RemoveLink();
        }
        else
        {
            d.Neighbors.Clear();
        }
    }

    public virtual string GetText(DialogNode node)
    {
        if (Contains(node))
        {
            return node.GetText();
        }
        else
        {
            return "Missing node";
        }
    }

    public virtual bool Contains(DialogNode d)
    {
        return Contains(d.DKey);
    }

    public virtual bool Contains(string dkey)
    {
        return nodes.ContainsKey(dkey);
    }

    public virtual bool ContainsLink(ResponseNode u, DialogNode v)
    {
        return u.Neighbor != null && (u.Neighbor.Neighbor == null || u.Neighbor.Neighbor.Equals(v));
    }

    public virtual bool ContainsLink(DialogNode u, ResponseNode v)
    {
        return u.Neighbors.Contains(v);
    }

    public virtual DialogNodeList Nodes
    {
        get
        {
            return nodes;
        }
    }

    public DialogNode Root
    {
        get
        {
            return root;
        }
        set
        {
            root = value;
        }
    }
}
