using UnityEngine;
using System.Collections.Generic;
using UnityEditor;
using System.IO;
using UnityEditor.SceneManagement;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Runtime.Serialization.Formatters.Binary;
using System;
/**
* The language manager is under Windows menu -> Language Manager
* Features: add/remove/rename individual languages as TextAsset; predefined languages include EN,FR,DE,IT,ES,NL,PL,PT,RU,AR,JA,KO,CN.
* 
* Note: modifying the actual translation strings and keys can only be manually done in the corresponding txt files under Resources/Text;
*  the Language Manager cannot be opened during play mode.
*  
* Assumptions: the content of each language data file is lines of {STRING_KEY}={LOCALIZED_STRING};
*  each Text component in the scene has a localizeString script attached containing the string key for this component
* 
* Reference:
* Main menu materials taken from https://unity3d.com/learn/tutorials/topics/user-interface-ui/creating-main-menu?playlist=17111
* Some codings are inspired by http://blog.kjinteractive.net/2015/unity-basic-string-localization-tutorial/
*      http://answers.unity3d.com/questions/826062/re-orderable-object-lists-in-inspector.html
*  and http://va.lent.in/unity-make-your-lists-functional-with-reorderablelist/
* */
[System.Serializable]
public class Language: System.Object
{
    public string languageName;
    public Dictionary<string, string> languageData;
}

public class LanguageController : ScriptableObject
{
    public List<Language> languageList;

    public Language currentLanguage;

    const string cur_lang_key = "current_language";
    //const string lang_path = "/Resources/Text/";
    const string lang_path = "/Resources/Text/language.dat";

    // Make sure there is only 1 instance
    private static LanguageController instance = null;

    public static LanguageController Instance
    {
        get
        {
            if (instance == null)
            {
                instance = CreateInstance<LanguageController>();
                instance.initLanguageList();
            }
            return instance;
        }
    }
    
    private void loadLanguageData()
    {
        /* Object[] files = Resources.LoadAll("Text");

         string newline = System.Environment.NewLine;

         foreach (TextAsset file in files)
         {
             Language newLanguage = new Language();
             newLanguage.languageName = file.name;

             string[] lines = file.text.Split(new string[] { newline }, System.StringSplitOptions.None);

             if (lines.Length < 2)
             {
                 Debug.Log("Current text file is not using system line break. Re-reading...");
                 lines = file.text.Split('\n');
             }

             newLanguage.languageData = new Dictionary<string, string>();

             foreach (string line in lines)
             {
                 string[] test = line.Split('=');
                 if (test.Length == 2)
                 {
                     newLanguage.languageData.Add(test[0], test[1]);
                 }
             }

             languageList.Add(newLanguage);

             Resources.UnloadAsset(file);
         }
         */
        string dataPath = Application.dataPath + lang_path;

        if (File.Exists(dataPath))
        {
            try
            {
                BinaryFormatter bf = new BinaryFormatter();
                FileStream fs = File.Open(dataPath, FileMode.Open);

                languageList = (List<Language>)bf.Deserialize(fs);
                fs.Close();
            }
            catch (Exception e)
            {
                Debug.Log(e.Message);
                initLanguages();
            }
        }
        else
        {
            initLanguages();
        }
        // set the current language
        if (!EditorPrefs.HasKey(cur_lang_key))
        {
            setCurrentLanguage(0);
        }
        else
        {
            int defaultIndex = languageList.FindIndex(x => x.languageName == EditorPrefs.GetString(cur_lang_key));
            Debug.Log(languageList.Count);
            setCurrentLanguage(Math.Max(0,defaultIndex));   
        }
    }
    
    public void SaveLanguage()
    {
        /*
        string savePath = Application.dataPath + lang_path + currentLanguage.languageName + ".txt";

        StreamWriter sw = new StreamWriter(File.Open(savePath, FileMode.Create), System.Text.Encoding.UTF8);
        foreach(KeyValuePair<string, string> item in currentLanguage.languageData)
        {
            sw.WriteLine(item.Key + "=" + item.Value);
        }
        sw.Close();
        */
        string dataPath = Application.dataPath + lang_path;
        BinaryFormatter bf = new BinaryFormatter();
        FileStream fs = File.Create(dataPath);

        bf.Serialize(fs, languageList);
        fs.Close();

        AssetDatabase.Refresh();
    }

    public void AddWord(string skey, string str)
    {
        if (currentLanguage.languageData.ContainsKey(skey))
        {
            Debug.Log("String ID key [" + skey + "] already exist in current language");
        }
        else
        {
            currentLanguage.languageData.Add(skey, str);
        }
    }

    public string GetText(string skey)
    {
        if (currentLanguage.languageData.ContainsKey(skey))
        {
            return currentLanguage.languageData[skey];
        }
        else
        {
            return "Missing string ID key";
        }
    }

    public void initLanguageList()
    {
        if (languageList == null || languageList.Count <= 0)
        {
            loadLanguageData();
        }
    }

    void initLanguages()
    {
        languageList = new List<Language>();

        Language en = new Language();
        en.languageName = "EN";
        en.languageData = new Dictionary<string, string>();
        en.languageData.Add("start", "Start");
        en.languageData.Add("help", "Help");
        en.languageData.Add("audio", "Audio");
        en.languageData.Add("quit", "Quit");
        en.languageData.Add("helptext", "This is some random help text.");
        en.languageData.Add("back", "Back");
        en.languageData.Add("mastervol", "Master Volume");
        en.languageData.Add("musicvol", "Music Volume");
        en.languageData.Add("sevol", "Sound Effect Volume");
        en.languageData.Add("where", "Where have you been?");
        en.languageData.Add("bigbrother", "I've been at home watching Big Brother.");
        en.languageData.Add("here", "I've been here ther whole time!");
        en.languageData.Add("voteout", "Oh yeah, who got voted out?");
        en.languageData.Add("watchfordrama", "Voted out? I just watch it for the drama!");
        en.languageData.Add("blind", "Am I just blind ?");
        en.languageData.Add("surprised", "I'm actually surprised you noticed me at all.");
        en.languageData.Add("getperscription", "Well, I was going to tell you to get a stronger perscription.");
        en.languageData.Add("thedrama", "Sure, its the \"drama\".");
        en.languageData.Add("uhh", "Uhhhh...");
        en.languageData.Add("notold", "Come on, I'm not that old!");

        languageList.Add(en);
        SaveLanguage();
    }

    public void addLanguage(string name)
    {
         Language newLang = new Language();
         newLang.languageName = name;

         languageList.Add(newLang);
    }
    
    public void addLanguageEmpty(string name)
    {
        /*
        File.WriteAllText(Application.dataPath + "/Resources/Text/" + name + ".txt", "");
        AssetDatabase.Refresh();
        */
        Language newlanguage = new Language();
        newlanguage.languageName = name;
        newlanguage.languageData = new Dictionary<string, string>();
        languageList.Add(newlanguage);
        SaveLanguage();
    }

    public void removeLanguage(int index)
    {
        /*
        if (!AssetDatabase.MoveAssetToTrash("Assets/Resources/Text/" + languageList[index].languageName + ".txt"))
        {
            Debug.Log("error removing asset");
        }
        else
        {
            Debug.Log("successfully removed asset");
        }
        AssetDatabase.Refresh();*/
        languageList.RemoveAt(index);
        SaveLanguage();
    }

    public void renameLanguage(int index, string newName)
    {
        /*
        string originalName = languageList[index].languageName;
        AssetDatabase.RenameAsset("Assets/Resources/Text/" + originalName + ".txt", newName);

        if (EditorPrefs.GetString(cur_lang_key) == originalName){
            EditorPrefs.SetString(cur_lang_key, newName);
        }

        languageList[index].languageName = newName;
        
        AssetDatabase.Refresh();*/
        string originalName = languageList[index].languageName;
        
        if (EditorPrefs.GetString(cur_lang_key) == originalName)
        {
            EditorPrefs.SetString(cur_lang_key, newName);
        }
        
        languageList[index].languageName = newName;

        SaveLanguage();
    }

    public void reloadLanguageList()
    {
        Debug.Log("reloading language list");
        languageList.Clear();

        loadLanguageData();
    }

    public void updateGameObjectText()
    { 
        Scene currentScene = EditorSceneManager.GetActiveScene();

        GameObject[] obj = currentScene.GetRootGameObjects();
        foreach (GameObject o in obj)
        {
            // get all text components
            Text[] textObjects = o.GetComponentsInChildren<Text>(true);

            foreach (Text txt in textObjects)
            {
                // check if the Text has a stringKey variable (ie. can be translated)
                if (txt.GetComponent<LocalizeString>() != null && txt.GetComponent<LocalizeString>().stringKey != null)
                {
                    string key = txt.GetComponent<LocalizeString>().stringKey;
                    if (currentLanguage.languageData.ContainsKey(key))
                    {
                        txt.text = currentLanguage.languageData[key];
                    }
                    else
                    {
                        txt.text = "Missing translation";
                    }

                    EditorUtility.SetDirty(txt);
                    EditorSceneManager.MarkSceneDirty(currentScene);
                }
            }
        }
    }

    public void setCurrentLanguage(int index)
    {
        if (currentLanguage != null && currentLanguage.languageName == languageList[index].languageName)
        {
            return;
        }

        currentLanguage = languageList[index];
        EditorPrefs.SetString(cur_lang_key, currentLanguage.languageName);

        updateGameObjectText();
        Debug.Log("The language of the texts in current scene has been set to " + currentLanguage.languageName);
        
    }

    public bool languageNameExist(string name)
    {
        return languageList.FindIndex(x => x.languageName == name) >= 0;
    }
}
