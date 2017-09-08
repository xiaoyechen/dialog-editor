using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System;

public class DialogManagerEditor : EditorWindow
{
    private DialogManager dm;

    private List<Rect> windows;
    private int sideWindowWidth = 400;
    private Rect sideWindowRect { get { return new Rect(position.width - sideWindowWidth, 0, sideWindowWidth, position.height); } }
    private List<DialogNodeGUI> nodeWindows;

    private float panX=0, panY = 0;
    private bool addButton = false;
    private Vector2 scroll;
    private string _newDKey, _newStrKey, _newdialog;

    // set from DialogNodeGUI
    public static Stack<DialogNode> nodesToLink;
    public static ResponseNode resToRemove;
    public static int resToAdd = -1, dialogToRemove = -1, startDialog = -1;

    [MenuItem("Window/Dialog Manager")]
    private static void Init()
    {
        DialogManagerEditor editor = (DialogManagerEditor)GetWindow(typeof(DialogManagerEditor));
        editor.titleContent = new GUIContent("Dialog Manager");
        editor.minSize = new Vector2(700,500);
        editor.Show();
    }    

    void OnEnable()
    {
        dm = DialogManager.Instance;
        windows = new List<Rect>();
        nodeWindows = new List<DialogNodeGUI>();
        nodesToLink = new Stack<DialogNode>();

        BuildNodeWindows();
    }

    void OnGUI()
    {
        Event currentEvent = Event.current;
        
        if (currentEvent.type == EventType.MouseDrag)
        {
            panX += Event.current.delta.x;
            panY += Event.current.delta.y;
        }
        
        DrawLinks();

        if (nodesToLink.Count >= 2)
        {
            UpdateLink();
        }

        if (resToRemove != null)
        {
            dm.Graph.Remove(resToRemove);
            resToRemove = null;
        }
        
        if (dialogToRemove > -1)
        {
            RemoveDialog();
            dialogToRemove = -1;
        }
        if (startDialog != -1)
        {
            SetStartDialog();
        }
        if (resToAdd != -1)
        {
            AddNewResponse();
        }

        //GUI.BeginGroup(new Rect(panX, panY, 10000, 10000));
        BeginWindows();
        
        for (int i = 0; i < windows.Count; ++i)
        {
            windows[i] = GUILayout.Window(nodeWindows[i].WindowId, windows[i], nodeWindows[i].DrawNodeWindow, nodeWindows[i].Node.DKey);
        }
        
        if (addButton)
        {
            AddNewNode();
        }
        
        EndWindows();
        //GUI.EndGroup();

        // Draw Side Window
        sideWindowWidth = Math.Min(600, Math.Max(200, (int)(position.width / 5)));
        GUILayout.BeginArea(sideWindowRect, GUI.skin.box);
        DrawSideWindow();
        GUILayout.EndArea();
    }

    void UpdateLink()
    {
        DialogNode linkTo = nodesToLink.Pop();
        DialogNode linking = nodesToLink.Pop();
   
        if (!linkTo.Equals(linking) && linking.GetType() == typeof(ResponseNode))
        {
            ToggleLink((ResponseNode)linking, linkTo);
        }
    }

    void ToggleLink(ResponseNode linking, DialogNode linkTo)
    {
        if (dm.Graph.ContainsLink(linking, linkTo))
        {
            dm.Graph.RemoveLink(linking, linkTo);
        }
        else
        {
            dm.Graph.AddLink(linking, linkTo);
        }
    }
    
    void RemoveDialog()
    {
        int index = nodeWindows.FindIndex(x => x.WindowId.Equals(dialogToRemove));
        dialogToRemove = -1;

        if (index >= 0)
        {
            if (nodeWindows[index].Node.Equals(dm.Graph.Root))
            {
                Debug.Log("You removed the starting dialog. Please set a new starting dialog.");
            }

            dm.Graph.Remove(nodeWindows[index].Node);
            windows.RemoveAt(index);
            nodeWindows.RemoveAt(index);
        }
        else
        {
            Debug.Log("Cannot find the node");
        }
    }

    void SetStartDialog()
    {
        DialogNode startNode = nodeWindows.Find(x => x.WindowId == startDialog).Node;
        startDialog = -1;

        if (dm.Graph.Contains(startNode))
        {
            dm.Graph.Root = startNode;
            
        }
        else
        {
            Debug.Log("Cannot find the dialog node");
        }
    }

    void AddNewResponse()
    {
        int pnodeid = nodeWindows.FindIndex(x => x.WindowId == resToAdd);
        DialogNode parentNode = nodeWindows[pnodeid].Node;
        resToAdd = -1;

        if (_newDKey == null || _newDKey == "" || _newStrKey == null || _newStrKey == "")
        {
            Debug.Log("Error adding response: missing dialog key and/or string id key");
            return;
        }

        ResponseNode newNode = new ResponseNode(_newDKey, _newStrKey);

        if (dm.Graph.Contains(newNode))
        {
            Debug.Log("Key already exist! Please enter another key");
            return;
        }

        dm.Graph.Add(newNode);
        dm.Graph.AddLink(parentNode, newNode);

        LanguageController.Instance.AddWord(_newStrKey, _newdialog);
    }

    void BuildNodeWindows()
    {
        int nodeId = 0;
        foreach (KeyValuePair<string, DialogNode> n in dm.Graph.Nodes)
        {
            DialogNode node = n.Value;

            // don't build a window for a response dialog because it's included in a dialog node window
            if(node.GetType() == typeof(ResponseNode))
            {
                continue;
            }

            DialogNodeGUI dngui = CreateInstance<DialogNodeGUI>();
            dngui.init(node, nodeId, new Vector2(nodeId % ((1000 - sideWindowWidth)/220) * 220, nodeId / ((1000 - sideWindowWidth) / 220) * 120));
            nodeWindows.Add(dngui);
            windows.Add(dngui.Shape);
            
            ++nodeId;
        }
    }

    void DrawLinks()
    {
        int numNodes = windows.Count;

        for (int i = 0; i < numNodes; ++i)
        {
            Rect parentNodeShape = windows[i];
            DialogNodeGUI parentNodeWindow = nodeWindows[i];

            AdjacencyList adjLinks = parentNodeWindow.Node.Neighbors;

            int resIndex = 0;

            while (resIndex < adjLinks.Count)
            {
                ResponseNode resNode = (ResponseNode)adjLinks[resIndex].Neighbor;

                // Remove this link if linking node is removed
                if (!dm.Graph.Contains(resNode))
                {
                    adjLinks.RemoveAt(resIndex);
                    continue;
                }

                // skip current node if this response node does not connect to any dialog
                if (resNode.Neighbor == null)
                {
                    ++resIndex;
                    continue;
                }
                else if (!dm.Graph.Contains(resNode.Neighbor.Neighbor))
                {// if the node to link with does not exist, then remove the link and move to next response node
                    dm.Graph.RemoveLink(resNode, resNode.Neighbor.Neighbor);
                    ++resIndex;
                    continue;
                }

                int nextNodeIndex = nodeWindows.FindIndex(x => x.Node == resNode.Neighbor.Neighbor);

                if (nextNodeIndex < 0)
                {
                    Debug.Log("Cannot find the node to link with. Are you trying to link a response dialog with another response?");
                    dm.Graph.RemoveLink(resNode, resNode.Neighbor.Neighbor);
                    ++resIndex;
                    continue;
                }
                
                DrawNodeCurve(parentNodeShape, windows[nextNodeIndex], adjLinks.Count, resIndex);

                ++resIndex;
            }
        }
    }

    void DrawSideWindow()
    {
        GUILayout.Label("Control", EditorStyles.boldLabel);

        EditorGUI.BeginChangeCheck();

        EditorGUILayout.LabelField("Dialog Key:");
        _newDKey = EditorGUILayout.TextField(_newDKey, GUILayout.MinWidth(100));

        EditorGUILayout.LabelField("String ID Key:");
        _newStrKey = EditorGUILayout.TextField(_newStrKey, GUILayout.MinWidth(100));

        EditorGUILayout.LabelField("Dialog:");
        _newdialog = EditorGUILayout.TextArea(_newdialog, GUILayout.Height(60));

        EditorGUI.EndChangeCheck();

        EditorGUILayout.Separator();

        addButton = GUILayout.Button("Add node", GUILayout.MinWidth(100));

        EditorGUILayout.Separator();

        if (GUILayout.Button("Save dialog graph", GUILayout.MinWidth(100)))
        {
            dm.SaveDialogGraph();
            LanguageController.Instance.SaveLanguage();
        }

        GUILayout.Box("Current starting dialog: " + dm.Graph.Root.DKey + ":" + dm.Graph.Root.StrKey + " - " + dm.Graph.Root.GetText());

        EditorGUILayout.HelpBox("Remember to save your changes before running the main scene for the changes to take effect", MessageType.Info);
    }

    void AddNewNode()
    {
        if (_newDKey == null || _newDKey == "" || _newStrKey == null || _newStrKey == "")
        {
            Debug.Log("Can't add new node. Invalid dialog ID key and/or string ID key.");
            return;
        }

        DialogNode newNode = new DialogNode(_newDKey, _newStrKey);

        if (dm.Graph.Contains(newNode))
        {
            Debug.Log("Dialog ID key already exist! Please enter another key.");
            return;
        }

        dm.Graph.Add(newNode);
        
        LanguageController.Instance.AddWord(_newStrKey, _newdialog);

        DialogNodeGUI newNodeWindow = CreateInstance<DialogNodeGUI>();

        int newWindowId = nodeWindows[nodeWindows.Count - 1].WindowId + 1;
        newNodeWindow.init(newNode, newWindowId, new Vector2(100, 100));
        windows.Add(newNodeWindow.Shape);
        nodeWindows.Add(newNodeWindow);

        windows[windows.Count-1] = GUILayout.Window(newNodeWindow.WindowId, windows[windows.Count-1], newNodeWindow.DrawNodeWindow, newNodeWindow.Node.StrKey);
    }

    void DrawNodeCurve(Rect start, Rect end, int totalRes, int resIndex)
    {
        bool startRight = start.xMax < end.xMin;
        bool endRight = start.xMin > end.xMax;
        float slice = start.height / (totalRes + 3);
        float startY = totalRes <= 1 ? 
                        (start.yMin + start.height / 2 + 20) : 
                        (start.yMin + slice * (resIndex+4) - slice / 2);

        Vector3 startPos = new Vector3(startRight ? start.xMax:start.xMin, startY);
        Vector3 endPos = new Vector3(endRight ? end.xMax: end.xMin, end.yMin + 5);
        Vector3 startTangent = startPos + (startRight ? Vector3.right:Vector3.left) * 50;
        Vector3 endTangent = endPos + (endRight ? Vector3.right:Vector3.left) * 50;
        
        Handles.DrawBezier(startPos, endPos, startTangent, endTangent, Color.white, null, 4);
    }
}
