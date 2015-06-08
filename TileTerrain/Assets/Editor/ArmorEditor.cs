using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;
using System.IO;
using Edit;

public class ArmorEditor : ObjectEditor
{

    private const int ARMOR = 0;
    private List<ArmorEdit> armor = new List<ArmorEdit>();

    private Vector2 scroll;

    private int selectedarmor = -1;

    protected static ArmorEditor window;


    [MenuItem("Editor/Armor Editor")]
    static void Init()
    {
        window = (ArmorEditor)EditorWindow.GetWindow(typeof(ArmorEditor));
    }

    protected override string getStandardFilePath()
    {
        return Application.dataPath + "/Resources/Data/armordata.xml";
    }

    protected override void loadFile(string filePath)
    {
        if (filePath == null)
        {
            filePath = EditorUtility.OpenFilePanel("Open armor data", Application.dataPath + "/Resources/Data", "xml");
        }

        if (filePath != null && !filePath.Equals(""))
        {
            DataHolder.ArmorDataHolder armorDataHolder;

            XmlSerializer serializer = new XmlSerializer(typeof(DataHolder.ArmorDataHolder));
            var stream = new FileStream(filePath, FileMode.Open);
            armorDataHolder = serializer.Deserialize(stream) as DataHolder.ArmorDataHolder;
            stream.Close();

            armor = new List<ArmorEdit>();

            foreach (ArmorData rdata in armorDataHolder.armorData)
            {
                armor.Add(new ArmorEdit(rdata));
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
        ArmorData[] armorData = new ArmorData[armor.Count];

        int i = 0;
        foreach (ArmorEdit redit in armor)
        {
            armorData[i] = new ArmorData(redit);
            i++;
        }

        DataHolder.ArmorDataHolder armorDataHolder = new DataHolder.ArmorDataHolder(armorData);

        using (FileStream file = new FileStream(filePath, FileMode.Create))
        {
            XmlSerializer serializer = new XmlSerializer(typeof(DataHolder.ArmorDataHolder));
            serializer.Serialize(file, armorDataHolder);
            AssetDatabase.Refresh();
        }
    }

    protected override void OnGUI()
    {

        scroll = GUILayout.BeginScrollView(scroll, false, true, GUILayout.Width(160), GUILayout.Height(this.position.height - 150));

        GUILayout.Label("Armor", EditorStyles.boldLabel);
        selectedarmor = GUILayout.SelectionGrid(selectedarmor, getArmorStrings(), 1);
        if (selectedarmor >= 0)
        {
            //selectedother = -1;
            if (!openWindow(ARMOR, selectedarmor))
            {
                WindowSettings window = new WindowSettings();
                window.objectListIndex = ARMOR;
                window.objectIndex = selectedarmor;
                window.windowFunc = editArmor;
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
        if (GUILayout.Button("+Armor"))
        {
            armor.Add(new ArmorEdit());
        }
        if (GUILayout.Button("Remove"))
        {
            deleteSelected();
        }
        GUILayout.EndVertical();
        base.OnGUI();
    }

    private string[] getArmorStrings()
    {
        string[] result = new string[armor.Count];
        int i = 0;
        foreach (ArmorEdit edit in armor)
        {
            result[i] = edit.name;
            i++;
        }
        return result;
    }


    protected void editArmor(int windowID)
    {
        if (GUI.Button(closeButtonRect, "X"))
        {
            if (selectedarmor == windows[windowID].objectIndex) selectedarmor = -1;
            windows.RemoveAt(windowID);
            return;
        }
        WindowSettings window = windows[windowID];
        ArmorEdit data = armor[window.objectIndex];
        if (data == null)
        {
            windows.RemoveAt(windowID);
        }

        EditorGUIUtility.labelWidth = 120;

        window.windowScroll = GUILayout.BeginScrollView(window.windowScroll, false, true);
        data.name = TextField("Name: ", data.name);
        data.gameName = TextField("Game Name: ", data.gameName);
        data.modelName = TextField("Model Name: ", data.modelName);

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Stats: ", EditorStyles.boldLabel);
        data.armor = IntField("Armor: ", data.armor);
        data.speedPenalty = IntField("Speed Penalty: ", data.speedPenalty);
        data.warmth = FloatField("Warmth: ", data.warmth);

        data.armorType = (ArmorType)EditorGUILayout.EnumPopup("Armor Type: ", data.armorType);
        data.durability = IntField("Durablity: ", data.durability);

        GUILayout.EndScrollView();
        GUI.DragWindow();
    }

    protected override GUI.WindowFunction getWindowFunc(WindowSettings window)
    {
        if (window.objectListIndex == ARMOR) return editArmor;
        return null;
    }

    protected override ObjectEdit getObjectEdit(int listIndex, int index)
    {
        switch (listIndex)
        {
            case (ARMOR):
                return armor[index];
            default:
                return null;
        }
    }

    protected override bool indexOutOfBounds(int listIndex, int index)
    {
        switch (listIndex)
        {
            case (ARMOR):
                return index >= armor.Count;
            default:
                return true;
        }
    }

    protected override void deleteSelected()
    {
        if (selectedarmor >= 0)
        {
            deleteWindowIndex = selectedarmor;
            deleteWindowListIndex = ARMOR;
            GUI.BringWindowToFront(DELETEWINDOWID);
        }
    }

    protected override void delete(int listIndex, int index)
    {
        if (!indexOutOfBounds(listIndex, index))
        {
            switch (listIndex)
            {
                case (ARMOR):
                    closeWindow(ARMOR, index);
                    armor.RemoveAt(index);
                    if (selectedarmor > 0) selectedarmor--;
                    break;
            }
        }
    }

    private void duplicate()
    {
        if (selectedarmor >= 0)
        {
            ArmorEdit temp = new ArmorEdit(armor[selectedarmor]);
            temp.name += "(Copy)";
            temp.gameName += "(Copy)";
            armor.Insert(selectedarmor + 1, temp);
            selectedarmor++;
        }
    }

    private void moveUp()
    {
        if (selectedarmor > 0)
        {
            ArmorEdit temp = armor[selectedarmor];
            armor.RemoveAt(selectedarmor);
            armor.Insert(selectedarmor - 1, temp);
            selectedarmor -= 1;

        }
    }

    private void moveDown()
    {
        if (selectedarmor >= 0 && selectedarmor < armor.Count - 1)
        {
            ArmorEdit temp = armor[selectedarmor];
            armor.RemoveAt(selectedarmor);
            armor.Insert(selectedarmor + 1, temp);
            selectedarmor += 1;
        }
    }
}
