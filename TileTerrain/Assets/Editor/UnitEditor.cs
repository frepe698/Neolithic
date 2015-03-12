using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;
using System.IO;
using Edit;

public class UnitEditor : ObjectEditor {

    private const int HERO = 0;
    private const int AI = 1;
    private List<HeroEdit> heroes = new List<HeroEdit>();
    private List<AIUnitEdit> aiUnits = new List<AIUnitEdit>();

    private Vector2 scroll;

    private int selectedhero = -1;
    private int selectedai = -1;

    protected static UnitEditor window;


    [MenuItem("Editor/Unit Editor")]
    static void Init()
    {
        window = (UnitEditor)EditorWindow.GetWindow(typeof(UnitEditor));
    }

    protected override string getStandardFilePath()
    {
        return Application.dataPath + "/Resources/Data/unitdata.xml";
    }

    protected override void loadFile(string filePath)
    {
        if (filePath == null)
        {
            filePath = EditorUtility.OpenFilePanel("Open unit data", Application.dataPath + "/Resources/Data", "xml");
        }

        if (filePath != null && !filePath.Equals(""))
        {
            DataHolder.UnitDataHolder unitDataHolder;

            XmlSerializer serializer = new XmlSerializer(typeof(DataHolder.UnitDataHolder));
            var stream = new FileStream(filePath, FileMode.Open);
            unitDataHolder = serializer.Deserialize(stream) as DataHolder.UnitDataHolder;
            stream.Close();

            heroes = new List<HeroEdit>();
            aiUnits = new List<AIUnitEdit>();
            foreach (HeroData mdata in unitDataHolder.heroData)
            {
                heroes.Add(new HeroEdit(mdata));
            }
            foreach (AIUnitData rdata in unitDataHolder.aiUnitData)
            {
                aiUnits.Add(new AIUnitEdit(rdata));
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
        HeroData[] heroData = new HeroData[heroes.Count];
        AIUnitData[] aiUnitData = new AIUnitData[aiUnits.Count];

        int i = 0;
        foreach (HeroEdit medit in heroes)
        {
            heroData[i] = new HeroData(medit);
            i++;
        }
        i = 0;
        foreach (AIUnitEdit redit in aiUnits)
        {
            aiUnitData[i] = new AIUnitData(redit);
            i++;
        }

        DataHolder.UnitDataHolder unitDataHolder = new DataHolder.UnitDataHolder(heroData, aiUnitData);

        using (FileStream file = new FileStream(filePath, FileMode.Create))
        {
            XmlSerializer serializer = new XmlSerializer(typeof(DataHolder.UnitDataHolder));
            serializer.Serialize(file, unitDataHolder);
            AssetDatabase.Refresh();
        }
    }

    protected override void OnGUI()
    {

        scroll = GUILayout.BeginScrollView(scroll, false, true, GUILayout.Width(160), GUILayout.Height(this.position.height - 150));

        GUILayout.Label("Heroes", EditorStyles.boldLabel);
        selectedhero = GUILayout.SelectionGrid(selectedhero, getHeroStrings(), 1);
        if (selectedhero >= 0)
        {
            selectedai = -1;
            if (!openWindow(HERO, selectedhero))
            {
                WindowSettings window = new WindowSettings();
                window.objectListIndex = HERO;
                window.objectIndex = selectedhero;
                window.windowFunc = editHero;
                window.windowRect = new Rect(0, 100, 300, this.position.height - 20);
                window.windowScroll = Vector2.zero;
                windows.Add(window);
            }
        }
        GUILayout.Label("AI Units", EditorStyles.boldLabel);
        selectedai = GUILayout.SelectionGrid(selectedai, getAIStrings(), 1);
        if (selectedai >= 0)
        {
            selectedhero = -1;
            if (!openWindow(AI, selectedai))
            {
                WindowSettings window = new WindowSettings();
                window.objectListIndex = AI;
                window.objectIndex = selectedai;
                window.windowFunc = editAIUnit;
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
        if (GUILayout.Button("+Hero"))
        {
            heroes.Add(new HeroEdit());
        }
        if (GUILayout.Button("+AI"))
        {
            aiUnits.Add(new AIUnitEdit());
        }
        if (GUILayout.Button("Remove"))
        {
            deleteSelected();
        }
        GUILayout.EndVertical();
        base.OnGUI();
    }

    private string[] getHeroStrings()
    {
        string[] result = new string[heroes.Count];
        int i = 0;
        foreach (HeroEdit edit in heroes)
        {
            result[i] = edit.name;
            i++;
        }
        return result;
    }

    private string[] getAIStrings()
    {
        string[] result = new string[aiUnits.Count];
        int i = 0;
        foreach (AIUnitEdit edit in aiUnits)
        {
            result[i] = edit.name;
            i++;
        }
        return result;
    }

    protected void editHero(int windowID)
    {
        if (GUI.Button(closeButtonRect, "X"))
        {
            if (selectedhero == windows[windowID].objectIndex) selectedhero = -1;
            windows.RemoveAt(windowID);
            return;
        }
        WindowSettings window = windows[windowID];
        HeroEdit data = heroes[window.objectIndex];
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
        EditorGUILayout.LabelField("Resources: ", EditorStyles.boldLabel);
        data.health = IntField("Health: ", data.health);
        data.lifegen = FloatField("Lifegen: ", data.lifegen);
        data.energy = FloatField("Energy: ", data.energy);
        data.energygen = FloatField("Energegen: ", data.energygen);

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Shoe size:", EditorStyles.boldLabel);
        data.movespeed = FloatField("Move speed", data.movespeed);
        data.size = FloatField("Size", data.size);

        GUILayout.EndScrollView();
        GUI.DragWindow();
    }

    protected void editAIUnit(int windowID)
    {
        if (GUI.Button(closeButtonRect, "X"))
        {
            if (selectedai == windows[windowID].objectIndex) selectedai = -1;
            windows.RemoveAt(windowID);
            return;
        }
        WindowSettings window = windows[windowID];
        AIUnitEdit data = aiUnits[window.objectIndex];
        if (data == null)
        {
            windows.RemoveAt(windowID);
            return;
        }

        EditorGUIUtility.labelWidth = 120;

        window.windowScroll = GUILayout.BeginScrollView(window.windowScroll, false, true);
        data.name = TextField("Name: ", data.name);
        data.gameName = TextField("Game Name: ", data.gameName);
        data.modelName = TextField("Model Name: ", data.modelName);

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Resources: ", EditorStyles.boldLabel);
        data.health = IntField("Health: ", data.health);
        data.lifegen = FloatField("Lifegen: ", data.lifegen);
        data.energy = FloatField("Energy: ", data.energy);
        data.energygen = FloatField("Energegen: ", data.energygen);

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Shoe size:", EditorStyles.boldLabel);
        data.movespeed = FloatField("Move speed", data.movespeed);
        data.size = FloatField("Size", data.size);

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Attack: \t DPS: " + data.damage * data.attackSpeed, EditorStyles.boldLabel);
        data.damage = IntField("Damage: ", data.damage);
        data.attackSpeed = FloatField("Attack speed", data.attackSpeed);
        data.attackSound = TextField("Sound: ", data.attackSound);

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Other: ", EditorStyles.boldLabel);
        data.hostile = EditorGUILayout.Toggle("Hostile: ", data.hostile);
        data.lineofsight = IntField("Line of sight: ", data.lineofsight);

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Safe Drops: ", EditorStyles.boldLabel);
        data.safeDrops = EditorGUILayout.TextArea(data.safeDrops, GUILayout.ExpandHeight(true));
        EditorGUILayout.LabelField("Random Drops: ", EditorStyles.boldLabel);
        data.randomDrops = EditorGUILayout.TextArea(data.randomDrops, GUILayout.ExpandHeight(true));
        Vector2 amount = EditorGUILayout.Vector2Field("Min/Max", new Vector2(data.minDrops, data.maxDrops));
        data.minDrops = (int)amount.x;
        data.maxDrops = (int)amount.y;
        GUILayout.EndScrollView();
        GUI.DragWindow();
    }

    protected override GUI.WindowFunction getWindowFunc(WindowSettings window)
    {
        if (window.objectListIndex == HERO) return editHero;
        if (window.objectListIndex == AI) return editAIUnit;
        return null;
    }

    protected override ObjectEdit getObjectEdit(int listIndex, int index)
    {
        switch (listIndex)
        {
            case (HERO):
                return heroes[index];
            case (AI):
                return aiUnits[index];
            default:
                return null;
        }
    }

    protected override bool indexOutOfBounds(int listIndex, int index)
    {
        switch (listIndex)
        {
            case (HERO):
                return index >= heroes.Count;
            case (AI):
                return index >= aiUnits.Count;
            default:
                return true;
        }
    }

    protected override void deleteSelected()
    {
        if (selectedhero >= 0)
        {
            deleteWindowIndex = selectedhero;
            deleteWindowListIndex = HERO;
            GUI.BringWindowToFront(DELETEWINDOWID);
        }
        else if (selectedai >= 0)
        {
            deleteWindowIndex = selectedai;
            deleteWindowListIndex = AI;
            GUI.BringWindowToFront(DELETEWINDOWID);
        }
    }

    protected override void delete(int listIndex, int index)
    {
        if (!indexOutOfBounds(listIndex, index))
        {
            switch (listIndex)
            {
                case (HERO):
                    closeWindow(HERO, index);
                    heroes.RemoveAt(index);
                    if (selectedhero > 0) selectedhero--;
                    break;
                case (AI):
                    closeWindow(AI, index);
                    aiUnits.RemoveAt(index);
                    if (selectedai > 0) selectedai--;
                    break;
            }
        }
    }

    private void duplicate()
    {
        if (selectedhero >= 0)
        {
            HeroEdit temp = new HeroEdit(heroes[selectedhero]);
            temp.name += "(Copy)";
            temp.gameName += "(Copy)";
            heroes.Insert(selectedhero + 1, temp);
            selectedhero++;
        }
        else if (selectedai >= 0)
        {
            AIUnitEdit temp = new AIUnitEdit(aiUnits[selectedai]);
            temp.name += "(Copy)";
            temp.gameName += "(Copy)";
            aiUnits.Insert(selectedai + 1, temp);
            selectedai++;
        }
    }

    private void moveUp()
    {
        if (selectedhero > 0)
        {
            HeroEdit temp = heroes[selectedhero];
            heroes.RemoveAt(selectedhero);
            heroes.Insert(selectedhero - 1, temp);
            selectedhero -= 1;

        }
        else if (selectedai > 0)
        {
            AIUnitEdit temp = aiUnits[selectedai];
            aiUnits.RemoveAt(selectedai);
            aiUnits.Insert(selectedai - 1, temp);
            selectedai -= 1;
        }
    }

    private void moveDown()
    {
        if (selectedhero >= 0 && selectedhero < heroes.Count - 1)
        {
            HeroEdit temp = heroes[selectedhero];
            heroes.RemoveAt(selectedhero);
            heroes.Insert(selectedhero + 1, temp);
            selectedhero += 1;
        }
        else if (selectedai >= 0 && selectedai < aiUnits.Count - 1)
        {
            AIUnitEdit temp = aiUnits[selectedai];
            aiUnits.RemoveAt(selectedai);
            aiUnits.Insert(selectedai + 1, temp);
            selectedai += 1;
        }
    }
}
