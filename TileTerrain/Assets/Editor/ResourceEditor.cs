using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;
using System.IO;
using Edit;

public class ResourceEditor : ObjectEditor
{

    private const int RESOURCES = 0;
    private const int HARVESTABLE = 1;
    private List<ResourceEdit> resources = new List<ResourceEdit>();
    private List<HarvestableEdit> harvestables = new List<HarvestableEdit>();

    private Vector2 scroll;

    private int selectedres = -1;
    private int selectedhar = -1;

    protected static ResourceEditor window;

    [MenuItem("Editor/Resource Editor")]
    static void Init()
    {
        window = (ResourceEditor)EditorWindow.GetWindow(typeof(ResourceEditor));
    }

    protected override string getStandardFilePath()
    {
        return Application.dataPath + "/Resources/Data/resourcedata.xml";
    }

    protected override void loadFile(string filePath)
    {
        if (filePath == null)
        {
            filePath = EditorUtility.OpenFilePanel("Open item data", Application.dataPath + "/Resources/Data", "xml");
        }

        if (filePath != null && !filePath.Equals(""))
        {
            DataHolder.ResourceDataHolder resourceDataHolder;

            XmlSerializer serializer = new XmlSerializer(typeof(DataHolder.ResourceDataHolder));
            var stream = new FileStream(filePath, FileMode.Open);
            resourceDataHolder = serializer.Deserialize(stream) as DataHolder.ResourceDataHolder;
            stream.Close();

            resources = new List<ResourceEdit>();
            harvestables = new List<HarvestableEdit>();

            foreach (ResourceData rdata in resourceDataHolder.resourceData)
            {
                resources.Add(new ResourceEdit(rdata));
            }
            foreach (HarvestableData hdata in resourceDataHolder.harvestableData)
            {
                harvestables.Add(new HarvestableEdit(hdata));
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
        ResourceData[] resourceData = new ResourceData[resources.Count];
        HarvestableData[] harvestableData = new HarvestableData[harvestables.Count];

        int i = 0;
        foreach (ResourceEdit redit in resources)
        {
            resourceData[i] = new ResourceData(redit);
            i++;
        }
        i = 0;
        foreach (HarvestableEdit hedit in harvestables)
        {
            harvestableData[i] = new HarvestableData(hedit);
            i++;
        }

        DataHolder.ResourceDataHolder resourceDataHolder = new DataHolder.ResourceDataHolder(resourceData, harvestableData);

        using (FileStream file = new FileStream(filePath, FileMode.Create))
        {
            XmlSerializer serializer = new XmlSerializer(typeof(DataHolder.ResourceDataHolder));
            serializer.Serialize(file, resourceDataHolder);
            AssetDatabase.Refresh();
        }
    }

    protected override void OnGUI()
    {
        scroll = GUILayout.BeginScrollView(scroll, false, true, GUILayout.Width(160), GUILayout.Height(this.position.height - 150));

        GUILayout.Label("Resources", EditorStyles.boldLabel);
        selectedres = GUILayout.SelectionGrid(selectedres, getResStrings(), 1);
        if (selectedres >= 0)
        {
            selectedhar = -1;
            if (!openWindow(RESOURCES, selectedres))
            {
                WindowSettings window = new WindowSettings();
                window.objectListIndex = RESOURCES;
                window.objectIndex = selectedres;
                window.windowFunc = editResource;
                window.windowRect = new Rect(0, 100, 300, this.position.height - 20);
                window.windowScroll = Vector2.zero;
                windows.Add(window);
            }
        }
        GUILayout.Label("Harvestables", EditorStyles.boldLabel);
        selectedhar = GUILayout.SelectionGrid(selectedhar, getHarStrings(), 1);
        if (selectedhar >= 0)
        {
            selectedres = -1;
            if (!openWindow(HARVESTABLE, selectedhar))
            {
                WindowSettings window = new WindowSettings();
                window.objectListIndex = HARVESTABLE;
                window.objectIndex = selectedhar;
                window.windowFunc = editHarvestable;
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
        if (GUILayout.Button("+Res"))
        {
            resources.Add(new ResourceEdit());
        }
        if (GUILayout.Button("+Harvestable"))
        {
            harvestables.Add(new HarvestableEdit());
        }
        if (GUILayout.Button("Remove"))
        {
            deleteSelected();
        }
        GUILayout.EndVertical();
        base.OnGUI();
    }

    private string[] getResStrings()
    {
        string[] result = new string[resources.Count];
        int i = 0;
        foreach (ResourceEdit edit in resources)
        {
            result[i] = edit.name;
            i++;
        }
        return result;
    }

    private string[] getHarStrings()
    {
        string[] result = new string[harvestables.Count];
        int i = 0;
        foreach (HarvestableEdit edit in harvestables)
        {
            result[i] = edit.name;
            i++;
        }
        return result;
    }

    private void editResource(ResourceEdit data)
    {
        data.name = TextField("Name: ", data.name);
        data.gameName = TextField("Game Name: ", data.gameName);
        data.modelName = TextField("Model Name: ", data.modelName);

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Resources: ", EditorStyles.boldLabel);
        data.health = IntField("Health: ", data.health);
        data.damageType = (DamageType.dtype)EditorGUILayout.EnumPopup("Damage Type: ", data.damageType);

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Other:", EditorStyles.boldLabel);
        data.variances = IntField("Variances", data.variances);
        data.hitParticle = TextField("Hit Particle", data.hitParticle);
        data.blocksProjectile = EditorGUILayout.Toggle("Block Proj: ", data.blocksProjectile);

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Safe Drops: ", EditorStyles.boldLabel);
        data.safeDrops = EditorGUILayout.TextArea(data.safeDrops, GUILayout.ExpandHeight(true));
        EditorGUILayout.LabelField("Random Drops: ", EditorStyles.boldLabel);
        data.randomDrops = EditorGUILayout.TextArea(data.randomDrops, GUILayout.ExpandHeight(true));
        EditorGUILayout.LabelField("Rare Drops: ", EditorStyles.boldLabel);
        data.rareDrops = EditorGUILayout.TextArea(data.rareDrops, GUILayout.ExpandHeight(true));
        Vector2 amount = EditorGUILayout.Vector2Field("Min/Max", new Vector2(data.minDrops, data.maxDrops));
        data.minDrops = (int)amount.x;
        data.maxDrops = (int)amount.y;
    }

    protected void editResource(int windowID)
    {
        if (GUI.Button(closeButtonRect, "X"))
        {
            if (selectedres == windows[windowID].objectIndex) selectedres = -1;
            windows.RemoveAt(windowID);
            return;
        }
        WindowSettings window = windows[windowID];
        ResourceEdit data = resources[window.objectIndex];
        if (data == null)
        {
            windows.RemoveAt(windowID);
        }

        EditorGUIUtility.labelWidth = 120;
        window.windowScroll = GUILayout.BeginScrollView(window.windowScroll, false, true);

        editResource(data);

        GUILayout.EndScrollView();
        GUI.DragWindow();
    }

    protected void editHarvestable(int windowID)
    {
        if (GUI.Button(closeButtonRect, "X"))
        {
            if (selectedhar == windows[windowID].objectIndex) selectedhar = -1;
            windows.RemoveAt(windowID);
            return;
        }
        WindowSettings window = windows[windowID];
        HarvestableEdit data = harvestables[window.objectIndex];
        if (data == null)
        {
            windows.RemoveAt(windowID);
        }

        EditorGUIUtility.labelWidth = 120;
        window.windowScroll = GUILayout.BeginScrollView(window.windowScroll, false, true);

        editResource(data);

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Harvest stuff:", EditorStyles.boldLabel);

        data.harvestDrop = EditorGUILayout.TextField("Drop name: ", data.harvestDrop);
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Drop amount min/max");
        data.minHarvest = EditorGUILayout.IntField(data.minHarvest);
        data.maxHarvest = EditorGUILayout.IntField(data.maxHarvest);
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Drop respawn time min/max");
        data.minRespawnTime = EditorGUILayout.FloatField(data.minRespawnTime);
        data.maxRespawnTime = EditorGUILayout.FloatField(data.maxRespawnTime);
        EditorGUILayout.EndHorizontal();

        


        GUILayout.EndScrollView();
        GUI.DragWindow();
    }

    protected override GUI.WindowFunction getWindowFunc(WindowSettings window)
    {
        if (window.objectListIndex == RESOURCES) return editResource;
        if (window.objectListIndex == HARVESTABLE) return editHarvestable;
        return null;
    }

    protected override ObjectEdit getObjectEdit(int listIndex, int index)
    {
        switch (listIndex)
        {
            case (RESOURCES):
                return resources[index];
            case (HARVESTABLE):
                return harvestables[index];
            default:
                return null;
        }
    }

    protected override bool indexOutOfBounds(int listIndex, int index)
    {
        switch (listIndex)
        {
            case (RESOURCES):
                return index >= resources.Count;
            case (HARVESTABLE):
                return index >= harvestables.Count;
            default:
                return true;
        }
    }

    protected override void deleteSelected()
    {
        if (selectedres >= 0)
        {
            deleteWindowIndex = selectedres;
            deleteWindowListIndex = RESOURCES;
            GUI.BringWindowToFront(DELETEWINDOWID);
        }
        else if (selectedhar >= 0)
        {
            deleteWindowIndex = selectedhar;
            deleteWindowListIndex = HARVESTABLE;
            GUI.BringWindowToFront(DELETEWINDOWID);
        }
    }

    protected override void delete(int listIndex, int index)
    {
        if (!indexOutOfBounds(listIndex, index))
        {
            switch (listIndex)
            {
                case (RESOURCES):
                    closeWindow(RESOURCES, index);
                    resources.RemoveAt(index);
                    if (selectedres > 0) selectedres--;
                    break;
                case (HARVESTABLE):
                    closeWindow(HARVESTABLE, index);
                    harvestables.RemoveAt(index);
                    if (selectedhar > 0) selectedhar--;
                    break;
            }
        }
    }

    private void duplicate()
    {
        if (selectedres >= 0)
        {
            ResourceEdit temp = new ResourceEdit(resources[selectedres]);
            temp.name += "(Copy)";
            temp.gameName += "(Copy)";
            resources.Insert(selectedres + 1, temp);
            selectedres++;
        }
        else if (selectedhar >= 0)
        {
            HarvestableEdit temp = new HarvestableEdit(harvestables[selectedhar]);
            temp.name += "(Copy)";
            temp.gameName += "(Copy)";
            harvestables.Insert(selectedhar + 1, temp);
            selectedhar++;
        }
    }

    private void moveUp()
    {
        if (selectedres > 0)
        {
            ResourceEdit temp = resources[selectedres];
            resources.RemoveAt(selectedres);
            resources.Insert(selectedres - 1, temp);
            selectedres -= 1;
        }
        else if (selectedhar > 0)
        {
            HarvestableEdit temp = harvestables[selectedhar];
            harvestables.RemoveAt(selectedhar);
            harvestables.Insert(selectedhar - 1, temp);
            selectedhar -= 1;
        }
    }

    private void moveDown()
    {
        if (selectedres >= 0 && selectedres < resources.Count - 1)
        {
            ResourceEdit temp = resources[selectedres];
            resources.RemoveAt(selectedres);
            resources.Insert(selectedres + 1, temp);
            selectedres += 1;
        }
        else if (selectedhar >= 0 && selectedhar < harvestables.Count - 1)
        {
            HarvestableEdit temp = harvestables[selectedhar];
            harvestables.RemoveAt(selectedhar);
            harvestables.Insert(selectedhar + 1, temp);
            selectedhar += 1;
        }
    }
}
