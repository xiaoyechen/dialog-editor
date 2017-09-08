using UnityEngine;
using UnityEditor;
using UnityEditorInternal;

public class LanguageManagerEditor : EditorWindow {
    SerializedObject serializedObject;
    int selectedLanguage;

    private ReorderableList list;

    [MenuItem("Window/Language Manager")]
    private static void Init()
    {
        LanguageManagerEditor editor = (LanguageManagerEditor)GetWindow(typeof(LanguageManagerEditor));
        editor.Show();
    }

    void OnEnable()
    {
        serializedObject = new SerializedObject(LanguageController.Instance);

        list = new ReorderableList(serializedObject, serializedObject.FindProperty("languageList"), false, true, true, true);

        list.onAddCallback += onAddCallback;
        list.drawElementCallback += drawElementCallback;
        list.onRemoveCallback += onRemoveCallback;
        list.onSelectCallback += onSelectCallback;
        list.drawHeaderCallback += drawHeaderCallback;
    }
    
    void OnDisable()
    {
        list.onAddCallback -= onAddCallback;
        list.drawElementCallback -= drawElementCallback;
        list.onRemoveCallback -= onRemoveCallback;
        list.onSelectCallback -= onSelectCallback;
        list.drawHeaderCallback -= drawHeaderCallback;
    }
    
    void onSelectCallback(ReorderableList l)
    {
        selectedLanguage = l.index;
    }

    void onRemoveCallback(ReorderableList l)
    {
        if (EditorUtility.DisplayDialog("Confirm", "Are you sure you want to delete the language? (This will also remove the TextAsset file)", "Yes", "No"))
        {
            LanguageController.Instance.removeLanguage(l.index);
            ReorderableList.defaultBehaviours.DoRemoveButton(l);
            serializedObject.ApplyModifiedProperties();
        }
    }

    void onAddCallback(ReorderableList l)
    {
        var index = l.serializedProperty.arraySize;
        l.serializedProperty.arraySize++;
        l.index = index;
        var element = l.serializedProperty.GetArrayElementAtIndex(index);

        int temporaryCode = index;

        string newLanguage = "Language(" + temporaryCode + ")";
        while (LanguageController.Instance.languageNameExist(newLanguage))
        {
            temporaryCode++;
            newLanguage = "Language(" + temporaryCode + ")";
        }
        element.FindPropertyRelative("languageName").stringValue = newLanguage;

        LanguageController.Instance.addLanguageEmpty(newLanguage);
        serializedObject.ApplyModifiedProperties();
    }

    void drawElementCallback(Rect rect, int index, bool active, bool focused)
    {
        Language lang = LanguageController.Instance.languageList[index];
        
        EditorGUI.LabelField(new Rect(rect.x, rect.y, 20, rect.height), (index + 1) + "");
        EditorGUI.LabelField(new Rect(rect.x+25, rect.y, 100, rect.height), "Language Name:");
        
        EditorGUI.BeginChangeCheck();

        string name = EditorGUI.TextField(new Rect(rect.x + 130, rect.y, 100, rect.height), lang.languageName);
        
        if (EditorGUI.EndChangeCheck())
        {
            if (!string.IsNullOrEmpty(name) && name != lang.languageName)
            {
                if (LanguageController.Instance.languageNameExist(name))
                {
                    Debug.Log("Another text asset with the same name already exists");
                }
                else
                {
                    LanguageController.Instance.renameLanguage(index, name);
                }
            }
        }
    }

    void drawHeaderCallback(Rect rect)
    {
        GUI.Label(rect, "Language List");
    }

    void OnGUI()
    {
        GUILayout.Label("Language Manager", EditorStyles.boldLabel);
        GUILayout.Label("Current language: " + LanguageController.Instance.currentLanguage.languageName, EditorStyles.miniLabel);

        serializedObject.Update();
        list.DoLayoutList();
        serializedObject.ApplyModifiedProperties();

        bool selectLanguageButton = GUILayout.Button("Set selected as default language", GUILayout.Width(250));
        bool refreshLanguageButton = GUILayout.Button("Refresh language list", GUILayout.Width(250));

        if (selectLanguageButton)
        {
            LanguageController.Instance.setCurrentLanguage(selectedLanguage);
        }

        if (refreshLanguageButton)
        {
            LanguageController.Instance.reloadLanguageList();
        }
        
        EditorGUILayout.HelpBox("Text data files are stored inside Resources/Text\n"
                                + "Refresh language list after making changes to the text data files\n"
                                + "Text data format: {STRING_ID}={LOCALIZED_STRING}", MessageType.Info);
    }

    // Close editor window when entering play mode
    void Update()
    {
        if (EditorApplication.isPlaying)
        {
            Close();
        }
    }
}
