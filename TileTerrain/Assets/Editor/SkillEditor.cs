using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;
using System.IO;
using Edit;

public class SkillEditor : ObjectEditor
{

    private const int SKILLS = 0;
    private List<SkillEdit> skills = new List<SkillEdit>();

    private Vector2 scroll;

    private int selectedskill = -1;

    protected static SkillEditor window;

    [MenuItem("Editor/Skill Editor")]
    static void Init()
    {
        window = (SkillEditor)EditorWindow.GetWindow(typeof(SkillEditor));
    }

    protected override string getStandardFilePath()
    {
        return Application.dataPath + "/Resources/Data/skilldata.xml";
    }

    protected override void loadFile(string filePath)
    {
        if (filePath == null)
        {
            filePath = EditorUtility.OpenFilePanel("Open skill data", Application.dataPath + "/Resources/Data", "xml");
        }

        if (filePath != null && !filePath.Equals(""))
        {
            DataHolder.SkillDataHolder skillDataHolder;

            XmlSerializer serializer = new XmlSerializer(typeof(DataHolder.SkillDataHolder));
            var stream = new FileStream(filePath, FileMode.Open);
            skillDataHolder = serializer.Deserialize(stream) as DataHolder.SkillDataHolder;
            stream.Close();

            skills = new List<SkillEdit>();

            foreach (SkillData rdata in skillDataHolder.skillData)
            {
                skills.Add(new SkillEdit(rdata));
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
        SkillData[] skillData = new SkillData[skills.Count];

        int i = 0;
        foreach (SkillEdit redit in skills)
        {
            skillData[i] = new SkillData(redit);
            i++;
        }

        DataHolder.SkillDataHolder skillDataHolder = new DataHolder.SkillDataHolder(skillData);

        using (FileStream file = new FileStream(filePath, FileMode.Create))
        {
            XmlSerializer serializer = new XmlSerializer(typeof(DataHolder.SkillDataHolder));
            serializer.Serialize(file, skillDataHolder);
            AssetDatabase.Refresh();
        }
    }

    protected override void OnGUI()
    {

        scroll = GUILayout.BeginScrollView(scroll, false, true, GUILayout.Width(160), GUILayout.Height(this.position.height - 150));

        GUILayout.Label("Skills", EditorStyles.boldLabel);
        selectedskill = GUILayout.SelectionGrid(selectedskill, getSkillStrings(), 1);
        if (selectedskill >= 0)
        {
            //selectedother = -1;
            if (!openWindow(SKILLS, selectedskill))
            {
                WindowSettings window = new WindowSettings();
                window.objectListIndex = SKILLS;
                window.objectIndex = selectedskill;
                window.windowFunc = editSkill;
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
        if (GUILayout.Button("+Skill"))
        {
            skills.Add(new SkillEdit());
        }
        if (GUILayout.Button("Remove"))
        {
            deleteSelected();
        }
        GUILayout.EndVertical();
        base.OnGUI();
    }

    private string[] getSkillStrings()
    {
        string[] result = new string[skills.Count];
        int i = 0;
        foreach (SkillEdit edit in skills)
        {
            result[i] = edit.name;
            i++;
        }
        return result;
    }


    protected void editSkill(int windowID)
    {
        if (GUI.Button(closeButtonRect, "X"))
        {
            if (selectedskill == windows[windowID].objectIndex) selectedskill = -1;
            windows.RemoveAt(windowID);
            return;
        }
        WindowSettings window = windows[windowID];
        SkillEdit data = skills[window.objectIndex];
        if (data == null)
        {
            windows.RemoveAt(windowID);
        }

        EditorGUIUtility.labelWidth = 120;

        window.windowScroll = GUILayout.BeginScrollView(window.windowScroll, false, true);
        data.name = TextField("Name: ", data.name);
        data.gameName = TextField("Game Name: ", data.gameName);
        //data.modelName = TextField("Model Name: ", data.modelName);

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Levels: ", EditorStyles.boldLabel);
        data.reqExpFolded = EditorGUILayout.Foldout(data.reqExpFolded, "Req Exp");

        if (data.reqExpFolded)
        {
            EditorGUIUtility.labelWidth = 80;
            for (int i = 0; i < data.requiredExp.Length; i++)
            {
                data.requiredExp[i] = IntField("Level " + (i + 1) + ": ", data.requiredExp[i]);
            }
            if (GUILayout.Button("reset"))
            {
                data.resetRequiredExp();
            }
        }

        GUILayout.Space(20);
        EditorGUILayout.LabelField("Stats per level:", EditorStyles.boldLabel);
        foreach (PassiveStat edit in data.statsPerLevel)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUIUtility.labelWidth = 100;
            EditorGUIUtility.fieldWidth = 40;
            edit.stat = (Stat)EditorGUILayout.EnumPopup(edit.stat);
            EditorGUIUtility.fieldWidth = 10;
            edit.amount = EditorGUILayout.FloatField(edit.amount);

            EditorGUIUtility.labelWidth = 30;
            EditorGUILayout.PrefixLabel("is%");
            edit.multiplier = EditorGUILayout.Toggle(edit.multiplier);
            
            if (GUILayout.Button("-"))
            {
                data.statsPerLevel.Remove(edit);
                break;
            }
            EditorGUILayout.EndHorizontal();
        }
        if (GUILayout.Button("+Stat"))
        {
            data.statsPerLevel.Add(new PassiveStat());
        }

        GUILayout.Space(20);
        EditorGUILayout.LabelField("Learnable abilities:", EditorStyles.boldLabel);
        foreach (LearnableAbility edit in data.abilities)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUIUtility.fieldWidth = 50;
            EditorGUIUtility.labelWidth = 50;
            edit.name = EditorGUILayout.TextField(edit.name);
            EditorGUIUtility.fieldWidth = 10;
            EditorGUIUtility.labelWidth = 50;
            EditorGUILayout.PrefixLabel("Req Lvl:");
            edit.reqLevel = EditorGUILayout.IntField(edit.reqLevel);
            if (GUILayout.Button("-"))
            {
                data.abilities.Remove(edit);
                break;
            }
            EditorGUILayout.EndHorizontal();
        }
        if (GUILayout.Button("+Ability"))
        {
            data.abilities.Add(new LearnableAbility());
        }

        GUILayout.EndScrollView();
        GUI.DragWindow();
    }

    protected override GUI.WindowFunction getWindowFunc(WindowSettings window)
    {
        if (window.objectListIndex == SKILLS) return editSkill;
        return null;
    }

    protected override ObjectEdit getObjectEdit(int listIndex, int index)
    {
        switch (listIndex)
        {
            case (SKILLS):
                return skills[index];
            default:
                return null;
        }
    }

    protected override bool indexOutOfBounds(int listIndex, int index)
    {
        switch (listIndex)
        {
            case (SKILLS):
                return index >= skills.Count;
            default:
                return true;
        }
    }

    protected override void deleteSelected()
    {
        if (selectedskill >= 0)
        {
            deleteWindowIndex = selectedskill;
            deleteWindowListIndex = SKILLS;
            GUI.BringWindowToFront(DELETEWINDOWID);
        }
    }

    protected override void delete(int listIndex, int index)
    {
        if (!indexOutOfBounds(listIndex, index))
        {
            switch (listIndex)
            {
                case (SKILLS):
                    closeWindow(SKILLS, index);
                    skills.RemoveAt(index);
                    if (selectedskill > 0) selectedskill--;
                    break;
            }
        }
    }

    private void duplicate()
    {
        if (selectedskill >= 0)
        {
            SkillEdit temp = new SkillEdit(skills[selectedskill]);
            temp.name += "(Copy)";
            temp.gameName += "(Copy)";
            skills.Insert(selectedskill + 1, temp);
            selectedskill++;
        }
    }

    private void moveUp()
    {
        if (selectedskill > 0)
        {
            SkillEdit temp = skills[selectedskill];
            skills.RemoveAt(selectedskill);
            skills.Insert(selectedskill - 1, temp);
            selectedskill -= 1;

        }
    }

    private void moveDown()
    {
        if (selectedskill >= 0 && selectedskill < skills.Count - 1)
        {
            SkillEdit temp = skills[selectedskill];
            skills.RemoveAt(selectedskill);
            skills.Insert(selectedskill + 1, temp);
            selectedskill += 1;
        }
    }
}
