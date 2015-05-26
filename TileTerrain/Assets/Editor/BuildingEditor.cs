using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;
using System.IO;
using Edit;

public class BuildingEditor : ObjectEditor
{

    private const int BUILDING = 0;
    private const int TOWER = 1;
    private List<BuildingEdit> buildings = new List<BuildingEdit>();
    private List<TowerEdit> towers = new List<TowerEdit>();

    private Vector2 scroll;

    private int selectedbuilding = -1;
    private int selectedtower = -1;

    protected static BuildingEditor window;


    [MenuItem("Editor/Building Editor")]
    static void Init()
    {
        window = (BuildingEditor)EditorWindow.GetWindow(typeof(BuildingEditor));
    }

    protected override string getStandardFilePath()
    {
        return Application.dataPath + "/Resources/Data/buildingdata.xml";
    }

    protected override void loadFile(string filePath)
    {
        if (filePath == null)
        {
            filePath = EditorUtility.OpenFilePanel("Open building data", Application.dataPath + "/Resources/Data", "xml");
        }

        if (filePath != null && !filePath.Equals(""))
        {
            DataHolder.BuildingDataHolder buildingDataHolder;

            XmlSerializer serializer = new XmlSerializer(typeof(DataHolder.BuildingDataHolder));
            var stream = new FileStream(filePath, FileMode.Open);
            buildingDataHolder = serializer.Deserialize(stream) as DataHolder.BuildingDataHolder;
            stream.Close();

            buildings = new List<BuildingEdit>();
            towers = new List<TowerEdit>();
            foreach (BuildingData mdata in buildingDataHolder.buildingData)
            {
                buildings.Add(new BuildingEdit(mdata));
            }
            foreach (TowerData rdata in buildingDataHolder.towerData)
            {
                towers.Add(new TowerEdit(rdata));
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
        BuildingData[] craftingBuildingData = new BuildingData[buildings.Count];
        TowerData[] towerData = new TowerData[towers.Count];

        int i = 0;
        foreach (BuildingEdit medit in buildings)
        {
            craftingBuildingData[i] = new BuildingData(medit);
            i++;
        }
        i = 0;
        foreach (TowerEdit redit in towers)
        {
            towerData[i] = new TowerData(redit);
            i++;
        }

        DataHolder.BuildingDataHolder buildingDataHolder = new DataHolder.BuildingDataHolder(craftingBuildingData, towerData);

        using (FileStream file = new FileStream(filePath, FileMode.Create))
        {
            XmlSerializer serializer = new XmlSerializer(typeof(DataHolder.BuildingDataHolder));
            serializer.Serialize(file, buildingDataHolder);
            AssetDatabase.Refresh();
        }
    }

    protected override void OnGUI()
    {

        scroll = GUILayout.BeginScrollView(scroll, false, true, GUILayout.Width(160), GUILayout.Height(this.position.height - 150));

        GUILayout.Label("Crafting Buildings", EditorStyles.boldLabel);
        selectedbuilding = GUILayout.SelectionGrid(selectedbuilding, getCraftingBuildingStrings(), 1);
        if (selectedbuilding >= 0)
        {
            selectedtower = -1;
            if (!openWindow(BUILDING, selectedbuilding))
            {
                WindowSettings window = new WindowSettings();
                window.objectListIndex = BUILDING;
                window.objectIndex = selectedbuilding;
                window.windowFunc = editBuilding;
                window.windowRect = new Rect(0, 100, 300, this.position.height - 20);
                window.windowScroll = Vector2.zero;
                windows.Add(window);
            }
        }
        GUILayout.Label("Towers", EditorStyles.boldLabel);
        selectedtower = GUILayout.SelectionGrid(selectedtower, getTowerStrings(), 1);
        if (selectedtower >= 0)
        {
            selectedbuilding = -1;
            if (!openWindow(TOWER, selectedtower))
            {
                WindowSettings window = new WindowSettings();
                window.objectListIndex = TOWER;
                window.objectIndex = selectedtower;
                window.windowFunc = editTower;
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
        if (GUILayout.Button("+Crafting Building"))
        {
            buildings.Add(new BuildingEdit());
        }
        if (GUILayout.Button("+Tower"))
        {
            towers.Add(new TowerEdit());
        }
        if (GUILayout.Button("Remove"))
        {
            deleteSelected();
        }
        GUILayout.EndVertical();
        base.OnGUI();
    }

    private string[] getCraftingBuildingStrings()
    {
        string[] result = new string[buildings.Count];
        int i = 0;
        foreach (BuildingEdit edit in buildings)
        {
            result[i] = edit.name;
            i++;
        }
        return result;
    }

    private string[] getTowerStrings()
    {
        string[] result = new string[towers.Count];
        int i = 0;
        foreach (TowerEdit edit in towers)
        {
            result[i] = edit.name;
            i++;
        }
        return result;
    }

    protected void editBuilding(BuildingEdit data)
    {
        data.name = TextField("Name: ", data.name);
        data.gameName = TextField("Game Name: ", data.gameName);
        data.modelName = TextField("Model Name: ", data.modelName);

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Defense: ", EditorStyles.boldLabel);
        data.health = IntField("Health: ", data.health);
        data.healthperlevel = IntField("Health Per Level: ", data.healthperlevel);
        data.lifegen = FloatField("Lifegen: ", data.lifegen);
        EditorGUILayout.Space();
        data.size = FloatField("Size", data.size);

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Crafting Recipe Names: ", EditorStyles.boldLabel);
        data.craftingRecipeNames = EditorGUILayout.TextArea(data.craftingRecipeNames, GUILayout.ExpandHeight(true));
    }

    protected void editBuilding(int windowID)
    {
        if (GUI.Button(closeButtonRect, "X"))
        {
            if (selectedbuilding == windows[windowID].objectIndex) selectedbuilding = -1;
            windows.RemoveAt(windowID);
            return;
        }
        WindowSettings window = windows[windowID];
        BuildingEdit data = buildings[window.objectIndex];
        if (data == null)
        {
            windows.RemoveAt(windowID);
        }

        EditorGUIUtility.labelWidth = 120;

        window.windowScroll = GUILayout.BeginScrollView(window.windowScroll, false, true);

        editBuilding(data);

        GUILayout.EndScrollView();
        GUI.DragWindow();
    }

    protected void editTower(int windowID)
    {
        if (GUI.Button(closeButtonRect, "X"))
        {
            if (selectedtower == windows[windowID].objectIndex) selectedtower = -1;
            windows.RemoveAt(windowID);
            return;
        }
        WindowSettings window = windows[windowID];
        TowerEdit data = towers[window.objectIndex];
        if (data == null)
        {
            windows.RemoveAt(windowID);
            return;
        }

        EditorGUIUtility.labelWidth = 120;

        window.windowScroll = GUILayout.BeginScrollView(window.windowScroll, false, true);

        editBuilding(data);

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Attack: \t DPS: " + data.damage * data.attackspeed, EditorStyles.boldLabel);
        data.damage = IntField("Damage: ", data.damage);
        data.attackspeed = FloatField("Attack speed", data.attackspeed);
        data.attackSound = TextField("Sound: ", data.attackSound);

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Other: ", EditorStyles.boldLabel);
        data.lineofsight = IntField("Line of sight: ", data.lineofsight);

        EditorGUILayout.LabelField("Abilties: ", EditorStyles.boldLabel);
        data.abilities = EditorGUILayout.TextArea(data.abilities, GUILayout.ExpandHeight(true));
        data.basicattack = TextField("Basic Attack: ", data.basicattack);
        GUILayout.EndScrollView();
        GUI.DragWindow();
    }

    protected override GUI.WindowFunction getWindowFunc(WindowSettings window)
    {
        if (window.objectListIndex == BUILDING) return editBuilding;
        if (window.objectListIndex == TOWER) return editTower;
        return null;
    }

    protected override ObjectEdit getObjectEdit(int listIndex, int index)
    {
        switch (listIndex)
        {
            case (BUILDING):
                return buildings[index];
            case (TOWER):
                return towers[index];
            default:
                return null;
        }
    }

    protected override bool indexOutOfBounds(int listIndex, int index)
    {
        switch (listIndex)
        {
            case (BUILDING):
                return index >= buildings.Count;
            case (TOWER):
                return index >= towers.Count;
            default:
                return true;
        }
    }

    protected override void deleteSelected()
    {
        if (selectedbuilding >= 0)
        {
            deleteWindowIndex = selectedbuilding;
            deleteWindowListIndex = BUILDING;
            GUI.BringWindowToFront(DELETEWINDOWID);
        }
        else if (selectedtower >= 0)
        {
            deleteWindowIndex = selectedtower;
            deleteWindowListIndex = TOWER;
            GUI.BringWindowToFront(DELETEWINDOWID);
        }
    }

    protected override void delete(int listIndex, int index)
    {
        if (!indexOutOfBounds(listIndex, index))
        {
            switch (listIndex)
            {
                case (BUILDING):
                    closeWindow(BUILDING, index);
                    buildings.RemoveAt(index);
                    if (selectedbuilding > 0) selectedbuilding--;
                    break;
                case (TOWER):
                    closeWindow(TOWER, index);
                    towers.RemoveAt(index);
                    if (selectedtower > 0) selectedtower--;
                    break;
            }
        }
    }

    private void duplicate()
    {
        if (selectedbuilding >= 0)
        {
            BuildingEdit temp = new BuildingEdit(buildings[selectedbuilding]);
            temp.name += "(Copy)";
            temp.gameName += "(Copy)";
            buildings.Insert(selectedbuilding + 1, temp);
            selectedbuilding++;
        }
        else if (selectedtower >= 0)
        {
            TowerEdit temp = new TowerEdit(towers[selectedtower]);
            temp.name += "(Copy)";
            temp.gameName += "(Copy)";
            towers.Insert(selectedtower + 1, temp);
            selectedtower++;
        }
    }

    private void moveUp()
    {
        if (selectedbuilding > 0)
        {
            BuildingEdit temp = buildings[selectedbuilding];
            buildings.RemoveAt(selectedbuilding);
            buildings.Insert(selectedbuilding - 1, temp);
            selectedbuilding -= 1;

        }
        else if (selectedtower > 0)
        {
            TowerEdit temp = towers[selectedtower];
            towers.RemoveAt(selectedtower);
            towers.Insert(selectedtower - 1, temp);
            selectedtower -= 1;
        }
    }

    private void moveDown()
    {
        if (selectedbuilding >= 0 && selectedbuilding < buildings.Count - 1)
        {
            BuildingEdit temp = buildings[selectedbuilding];
            buildings.RemoveAt(selectedbuilding);
            buildings.Insert(selectedbuilding + 1, temp);
            selectedbuilding += 1;
        }
        else if (selectedtower >= 0 && selectedtower < towers.Count - 1)
        {
            TowerEdit temp = towers[selectedtower];
            towers.RemoveAt(selectedtower);
            towers.Insert(selectedtower + 1, temp);
            selectedtower += 1;
        }
    }
}
