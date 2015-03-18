using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;
using System.IO;
using Edit;

public class AbilityEditor : ObjectEditor
{

    private const int ABILITIES = 0;
    private List<AbilityEdit> abilities = new List<AbilityEdit>();

    private Vector2 scroll;

    private int selectedability = -1;

    protected static AbilityEditor window;

    [MenuItem("Editor/Ability Editor")]
    static void Init()
    {
        window = (AbilityEditor)EditorWindow.GetWindow(typeof(AbilityEditor));
    }

    protected override string getStandardFilePath()
    {
        return Application.dataPath + "/Resources/Data/abilitydata.xml";
    }

    protected override void loadFile(string filePath)
    {
        if (filePath == null)
        {
            filePath = EditorUtility.OpenFilePanel("Open ability data", Application.dataPath + "/Resources/Data", "xml");
        }

        if (filePath != null && !filePath.Equals(""))
        {
            DataHolder.AbilityDataHolder AbilityDataHolder;

            XmlSerializer serializer = new XmlSerializer(typeof(DataHolder.AbilityDataHolder));
            var stream = new FileStream(filePath, FileMode.Open);
            AbilityDataHolder = serializer.Deserialize(stream) as DataHolder.AbilityDataHolder;
            stream.Close();

            abilities = new List<AbilityEdit>();

            foreach (AbilityData rdata in AbilityDataHolder.abilityData)
            {
                abilities.Add(new AbilityEdit(rdata));
            }

            this.filePath = filePath;
        }

    }

    protected override void saveFile()
    {
        if (filePath == null && !filePath.Equals(""))
        {
            saveAsFile();
            return;
        }
        AbilityData[] AbilityData = new AbilityData[abilities.Count];

        int i = 0;
        foreach (AbilityEdit redit in abilities)
        {
            AbilityData[i] = new AbilityData(redit);
            i++;
        }

        DataHolder.AbilityDataHolder AbilityDataHolder = new DataHolder.AbilityDataHolder(AbilityData);

        using (FileStream file = new FileStream(filePath, FileMode.Create))
        {
            XmlSerializer serializer = new XmlSerializer(typeof(DataHolder.AbilityDataHolder));
            serializer.Serialize(file, AbilityDataHolder);
            AssetDatabase.Refresh();
        }
    }

    protected override void OnGUI()
    {

        scroll = GUILayout.BeginScrollView(scroll, false, true, GUILayout.Width(160), GUILayout.Height(this.position.height - 150));

        GUILayout.Label("Abilities", EditorStyles.boldLabel);
        selectedability = GUILayout.SelectionGrid(selectedability, getAbilityStrings(), 1);
        if (selectedability >= 0)
        {
            //selectedother = -1;
            if (!openWindow(ABILITIES, selectedability))
            {
                WindowSettings window = new WindowSettings();
                window.objectListIndex = ABILITIES;
                window.objectIndex = selectedability;
                window.windowFunc = editAbility;
                window.windowRect = new Rect(0, 100, 300, this.position.height - 20);
                window.windowScroll = Vector2.zero;
                windows.Add(window);
            }
        }

        GUILayout.EndScrollView();
        GUILayout.BeginVertical(GUILayout.Width(120));
        GUILayout.Space(20);
        if (GUILayout.Button("Move up"))
        {
            moveUp();
        }
        if (GUILayout.Button("Move down"))
        {
            moveDown();
        }
        if (GUILayout.Button("Duplicate"))
        {
            duplicate();
        }
        if (GUILayout.Button("+Ability"))
        {
            abilities.Add(new AbilityEdit());
        }
        if (GUILayout.Button("Remove"))
        {
            deleteSelected();
        }
        GUILayout.EndVertical();
        base.OnGUI();
    }

    private string[] getAbilityStrings()
    {
        string[] result = new string[abilities.Count];
        int i = 0;
        foreach (AbilityEdit edit in abilities)
        {
            result[i] = edit.name;
            i++;
        }
        return result;
    }


    protected void editAbility(int windowID)
    {
        if (GUI.Button(closeButtonRect, "X"))
        {
            if (selectedability == windows[windowID].objectIndex) selectedability = -1;
            windows.RemoveAt(windowID);
            return;
        }
        WindowSettings window = windows[windowID];
        AbilityEdit data = abilities[window.objectIndex];
        if (data == null)
        {
            windows.RemoveAt(windowID);
        }

        EditorGUIUtility.labelWidth = 120;

        window.windowScroll = GUILayout.BeginScrollView(window.windowScroll, false, true);
        data.name = TextField("Name: ", data.name);
        data.gameName = TextField("Game Name: ", data.gameName);
        data.modelName = TextField("Icon Name: ", data.modelName);

        GUILayout.Space(20);

        data.energyCost = EditorGUILayout.IntField("Energy Cost: ", data.energyCost);
        data.healthCost = EditorGUILayout.IntField("Health Cost: ",data.healthCost);
        data.cooldown = EditorGUILayout.FloatField("Cooldown: ", data.cooldown);

        GUILayout.Space(20);
        data.totalTime = EditorGUILayout.FloatField("Total Time: ", data.totalTime);

        EditorGUILayout.LabelField("Effects:", EditorStyles.boldLabel);
        foreach(AbilityEffectAndTime edit in data.effects)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUIUtility.labelWidth = 100;
            EditorGUIUtility.fieldWidth = 40;
            edit.name = EditorGUILayout.TextField(edit.name);
            EditorGUIUtility.fieldWidth = 10;
            edit.time = EditorGUILayout.FloatField(edit.time);

            if (GUILayout.Button("-"))
            {
                data.effects.Remove(edit);
                break;
            }
            EditorGUILayout.EndHorizontal();
        }
        if (GUILayout.Button("+Effect"))
        {
            data.effects.Add(new AbilityEffectAndTime());
        }

        EditorGUILayout.LabelField("Animation:         Time:       Speed:", EditorStyles.boldLabel);
        foreach (AbilityAnimation edit in data.animations)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUIUtility.labelWidth = 100;
            EditorGUIUtility.fieldWidth = 40;
            edit.name = EditorGUILayout.TextField(edit.name);
            EditorGUIUtility.fieldWidth = 10;
            edit.time = EditorGUILayout.FloatField(edit.time);
            edit.speed = EditorGUILayout.FloatField(edit.speed);


            if (GUILayout.Button("-"))
            {
                data.animations.Remove(edit);
                break;
            }
            EditorGUILayout.EndHorizontal();
        }
        if (GUILayout.Button("+Animation"))
        {
            data.animations.Add(new AbilityAnimation());
        }


        GUILayout.EndScrollView();
        GUI.DragWindow();
    }

    protected override GUI.WindowFunction getWindowFunc(WindowSettings window)
    {
        if (window.objectListIndex == ABILITIES) return editAbility;
        return null;
    }

    protected override ObjectEdit getObjectEdit(int listIndex, int index)
    {
        switch (listIndex)
        {
            case (ABILITIES):
                return abilities[index];
            default:
                return null;
        }
    }

    protected override bool indexOutOfBounds(int listIndex, int index)
    {
        switch (listIndex)
        {
            case (ABILITIES):
                return index >= abilities.Count;
            default:
                return true;
        }
    }

    protected override void deleteSelected()
    {
        if (selectedability >= 0)
        {
            deleteWindowIndex = selectedability;
            deleteWindowListIndex = ABILITIES;
            GUI.BringWindowToFront(DELETEWINDOWID);
        }
    }

    protected override void delete(int listIndex, int index)
    {
        if (!indexOutOfBounds(listIndex, index))
        {
            switch (listIndex)
            {
                case (ABILITIES):
                    closeWindow(ABILITIES, index);
                    abilities.RemoveAt(index);
                    if (selectedability > 0) selectedability--;
                    break;
            }
        }
    }

    private void duplicate()
    {
        if (selectedability >= 0)
        {
            AbilityEdit temp = new AbilityEdit(abilities[selectedability]);
            temp.name += "(Copy)";
            temp.gameName += "(Copy)";
            abilities.Insert(selectedability + 1, temp);
            selectedability++;
        }
    }

    private void moveUp()
    {
        if (selectedability > 0)
        {
            AbilityEdit temp = abilities[selectedability];
            abilities.RemoveAt(selectedability);
            abilities.Insert(selectedability - 1, temp);
            selectedability -= 1;

        }
    }

    private void moveDown()
    {
        if (selectedability >= 0 && selectedability < abilities.Count - 1)
        {
            AbilityEdit temp = abilities[selectedability];
            abilities.RemoveAt(selectedability);
            abilities.Insert(selectedability + 1, temp);
            selectedability += 1;
        }
    }
}
