using UnityEngine;
using System.Collections.Generic;

public class TestModalWindow : MonoBehaviour {

    public static string dialogkey;

    private ModalPanel modalPanel;
    private DialogGraph dg;

    void Awake()
    {
        modalPanel = ModalPanel.Instance();
        dg = DialogManager.Instance.Graph;
    }

    public void TestYNC()
    {
        DialogNode firstQ = dg.Root;
        prepareQuestion(firstQ);
    }

    public void TestResponse(string dkey)
    {
        if (dg.Nodes.ContainsKey(dkey) && ((ResponseNode)dg.Nodes[dkey]).Neighbor != null)
        {
            DialogNode newQuestionNode = ((ResponseNode)dg.Nodes[dkey]).Neighbor.Neighbor;
            prepareQuestion(newQuestionNode);
        }
        else
        {
            Debug.LogError("Dialog key not found error. Please check your dialog graph.");
        }
    }

    void prepareQuestion(DialogNode n)
    {
        List<string> dkeyStrings = new List<string>();
        List<string> textStrings = new List<string>();
        foreach (Link l in n.Neighbors)
        {
            dkeyStrings.Add(l.Neighbor.DKey);
            textStrings.Add(l.Neighbor.GetText());
        }
        
        modalPanel.Choice(n.GetText(), dkeyStrings, textStrings, () => { TestResponse(dialogkey); });
    }
}
