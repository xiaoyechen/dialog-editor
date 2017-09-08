using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections.Generic;

public class ModalPanel : MonoBehaviour {
    
    public Text question;
    public Image img;
    public Button responseButtonObj;

    public GameObject modalPanelObj;
    public GameObject buttonPanelObj;

    private static ModalPanel modalPanel;

    public static ModalPanel Instance()
    {
        if (!modalPanel)
        {
            modalPanel = FindObjectOfType(typeof(ModalPanel)) as ModalPanel;
            if (!modalPanel)
                Debug.LogError("There needs to be one active ModalPanel script on a GameObject in your scene.");
        }

        return modalPanel;
    }

    public void Choice(string question, List<string> answerKeys, List<string> answers, UnityAction callback)
    {
        modalPanelObj.SetActive(true);
        this.question.text = question;
        img.gameObject.SetActive(false);

        foreach(Button oldButton in buttonPanelObj.GetComponentsInChildren<Button>())
        {
            Destroy(oldButton.gameObject);
        }

        for (int i = 0; i < answerKeys.Count; ++i)
        {
            Button newButton = Instantiate(responseButtonObj) as Button;
            newButton.transform.SetParent(buttonPanelObj.transform, false);
            newButton.gameObject.GetComponentInChildren<Text>().text = answers[i];

            newButton.onClick.RemoveAllListeners();

            string answerKey = answerKeys[i];
            newButton.onClick.AddListener(() => { setResponseKey(answerKey); });
            newButton.onClick.AddListener(callback);
            
            newButton.gameObject.SetActive(true);
        }

        if (answers.Count <= 0)
        {
            Button newButton = Instantiate(responseButtonObj) as Button;
            newButton.transform.SetParent(buttonPanelObj.transform, false);
            newButton.gameObject.GetComponentInChildren<Text>().text = "End Conversation";

            newButton.onClick.RemoveAllListeners();
            newButton.onClick.AddListener(ClosePanel);

            newButton.gameObject.SetActive(true);
        }
    }

    void setResponseKey(string key)
    {
        TestModalWindow.dialogkey = key;
    }

    void ClosePanel()
    {
        modalPanelObj.SetActive(false);
    }
}
