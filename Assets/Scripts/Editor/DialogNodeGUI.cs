using UnityEngine;
using UnityEditor;

public class DialogNodeGUI: Editor
{
    private DialogNode node;
    private int windowId;
    private Rect shape;

    public DialogNodeGUI(DialogNode node, int id, Vector2 pos)
    {
        this.node = node;
        windowId = id;
        shape = new Rect(pos, new Vector2(100,100));
    }

    public void init(DialogNode node, int id, Vector2 pos)
    {
        this.node = node;
        windowId = id;
        shape = new Rect(pos, new Vector2(100, 100));
    }
    
    public void DrawNodeWindow(int windowId)
    {
        string dialogStr = node.StrKey + "=" + node.GetText();

        GUILayout.Label(dialogStr.Length > 30 ? dialogStr.Substring(0, 30) + "..." : dialogStr);

        if (GUILayout.Button("Attatch"))
        {
            DialogManagerEditor.nodesToLink.Push(node);
        }

        AdjacencyList responses = node.Neighbors;
        
        GUILayout.BeginVertical();

        foreach (Link l in responses)
        {
            
            ResponseNode resNode = (ResponseNode)l.Neighbor;
            
            string resStr = resNode.DKey + ":" + resNode.StrKey + "=" + resNode.GetText();

            GUILayout.BeginHorizontal();

            GUILayout.Box(resStr.Length > 30 ? resStr.Substring(0, 30) + "..." : resStr, GUILayout.MinWidth(230));

            int select = GUILayout.Toolbar(-1, new string[] { "A", "x" }, GUILayout.MaxWidth(45));
            if (select == 0)
            {
                DialogManagerEditor.nodesToLink.Push(resNode);
            }
            else if (select == 1)
            {
                DialogManagerEditor.resToRemove = resNode;
            }

            GUILayout.EndHorizontal();
        }

        GUILayout.EndVertical();

        Event currentEvent = Event.current;

        if (currentEvent.type == EventType.MouseUp && currentEvent.button == 1)
        {
            GenericMenu menu = new GenericMenu();
            menu.AddItem(new GUIContent("Add new response"), false, AddNewResponse, WindowId);
            menu.AddItem(new GUIContent("Remove the dialog"), false, RemoveDialog, WindowId);
            menu.AddItem(new GUIContent("Set as starting dialog"), false, SetStartDialog, WindowId);
            menu.ShowAsContext();
            currentEvent.Use();
        }
        
        GUI.DragWindow();
    }

    void AddNewResponse(object obj)
    {
        DialogManagerEditor.resToAdd = windowId;
    }

    void RemoveDialog(object obj)
    {
        DialogManagerEditor.dialogToRemove = windowId;
    }

    void SetStartDialog(object obj)
    {
        DialogManagerEditor.startDialog = windowId;
    }

    public void DrawWindow(int id)
    {
        GUI.DragWindow();
    }

    public Rect Shape
    {
        get
        {
            return shape;
        }
        set
        {
            shape = value;
        }
    }

    public DialogNode Node
    {
        get
        {
            return node;
        }
        set
        {
            node = value;
        }
    }

    public int WindowId
    {
        get
        {
            return windowId;
        }
    }
}
