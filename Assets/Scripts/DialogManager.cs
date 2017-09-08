using UnityEngine;
using System.Collections;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System;

public class DialogManager : ScriptableObject {

    private static DialogManager instance = null;
    private string dataPath;

    private DialogGraph graph;

    public static DialogManager Instance
    {
        get
        {
            if (!instance)
            {
                instance = CreateInstance<DialogManager>();
                instance.LoadDialogGraph();
            }
            return instance;
        }
    }
    
    void LoadDialogGraph()
    {
        dataPath = Application.dataPath + "/Resources/dialogGraph.dat";

        if (File.Exists(dataPath))
        {
            try
            {
                BinaryFormatter bf = new BinaryFormatter();
                FileStream fs = File.Open(dataPath, FileMode.Open);

                graph = (DialogGraph)bf.Deserialize(fs);
                fs.Close();
            }
            catch (Exception e)
            {
                Debug.Log(e.Message);
                initDialogGraph();
            }
        }
        else
        {
            initDialogGraph();
        }
    }

    public void SaveDialogGraph()
    {
        BinaryFormatter bf = new BinaryFormatter();
        FileStream fs = File.Create(dataPath);

        bf.Serialize(fs, graph);
        fs.Close();
    }

    void initDialogGraph()
    {
        graph = new DialogGraph();

        DialogNode d1 = new DialogNode("npc1q1", "where");
        DialogNode d2 = new ResponseNode("q1a1", "bigbrother");
        DialogNode d3 = new ResponseNode("q1a2", "here");
        DialogNode d4 = new DialogNode("npc1q2", "voteout");
        DialogNode d5 = new ResponseNode("q2a1", "watchfordrama");
        DialogNode d6 = new DialogNode("npc1q3", "blind");
        DialogNode d7 = new ResponseNode("q3a1", "surprised");
        DialogNode d11 = new ResponseNode("q3a2", "getperscription");
        DialogNode d8 = new DialogNode("npc1q4", "thedrama");
        DialogNode d9 = new DialogNode("npc1q5", "uhh");
        DialogNode d10 = new DialogNode("npc1q6", "notold");
        graph.Root = d1;
        graph.Add(d1);
        graph.Add(d2);
        graph.Add(d3);
        graph.Add(d4);
        graph.Add(d5);
        graph.Add(d6);
        graph.Add(d7);
        graph.Add(d8);
        graph.Add(d9);
        graph.Add(d10);
        graph.Add(d11);
        graph.AddLink(d1, d2);
        graph.AddLink(d1, d3);
        graph.AddLink(d2, d4);
        graph.AddLink(d4, d5);
        graph.AddLink(d5, d8);
        graph.AddLink(d3, d6);
        graph.AddLink(d6, d7);
        graph.AddLink(d6, d11);
        graph.AddLink(d7, d9);
        graph.AddLink(d11, d10);
        //graph.AddLink(d11, d1);
    }

    public DialogGraph Graph
    {
        get
        {
            return graph;
        }
    }
}
