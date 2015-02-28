using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;
using System.IO;
using Edit;

public class ItemEditor : ObjectEditor
{

    private const int MATERIAL = 0;
    private const int CONSUMABLE = 1;
    private List<MaterialEdit> materials = new List<MaterialEdit>();
    private List<ConsumableEdit> consumables = new List<ConsumableEdit>();

    private Vector2 scroll;

    private int selectedmaterial = -1;
    private int selectedconsumable = -1;


    [MenuItem("Editor/Item Editor")]
    static void Init()
    {
        ItemEditor window = (ItemEditor)EditorWindow.GetWindow(typeof(ItemEditor));
        //loadFile();
    }

    protected override string getStandardFilePath()
    {
        return Application.dataPath + "/Resources/Data/itemdata.xml";
    }

    protected override void loadFile(string filePath)
    {
        Debug.Log("load items " + filePath);
        if (filePath == null)
        {
            Debug.Log("file path is null");
            filePath = EditorUtility.OpenFilePanel("Open item data", Application.dataPath + "/Resources/Data", "xml");
        }
        
        if (filePath != null && !filePath.Equals(""))
        {
            Debug.Log("load items");
            DataHolder.ItemDataHolder itemDataHolder;

            XmlSerializer serializer = new XmlSerializer(typeof(DataHolder.ItemDataHolder));
            var stream = new FileStream(filePath, FileMode.Open);
            itemDataHolder = serializer.Deserialize(stream) as DataHolder.ItemDataHolder;
            stream.Close();

            materials = new List<MaterialEdit>();
            consumables = new List<ConsumableEdit>();
            foreach (MaterialData mdata in itemDataHolder.materialData)
            {
                materials.Add(new MaterialEdit(mdata));
            }
            foreach (ConsumableItemData rdata in itemDataHolder.consumableItemData)
            {
                consumables.Add(new ConsumableEdit(rdata));
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

        MaterialData[] materialData = new MaterialData[materials.Count];
        ConsumableItemData[] consumableData = new ConsumableItemData[consumables.Count];

        int i = 0;
        foreach (MaterialEdit medit in materials)
        {
            materialData[i] = new MaterialData(medit);
            i++;
        }
        i = 0;
        foreach (ConsumableEdit redit in consumables)
        {
            consumableData[i] = new ConsumableItemData(redit);
            i++;
        }

        DataHolder.ItemDataHolder ItemDataHolder = new DataHolder.ItemDataHolder(materialData, consumableData);

        using (FileStream file = new FileStream(filePath, FileMode.Create))
        {
            XmlSerializer serializer = new XmlSerializer(typeof(DataHolder.ItemDataHolder));
            serializer.Serialize(file, ItemDataHolder);
            AssetDatabase.Refresh();
        }

    }



    protected override void OnGUI()
    {

        scroll = GUILayout.BeginScrollView(scroll, false, true, GUILayout.Width(160), GUILayout.Height(this.position.height - 150));

        GUILayout.Label("Materials", EditorStyles.boldLabel);
        selectedmaterial = GUILayout.SelectionGrid(selectedmaterial, getMaterialStrings(), 1);
        if (selectedmaterial >= 0)
        {
            selectedconsumable = -1;
            if (!openWindow(MATERIAL, selectedmaterial))
            {
                WindowSettings window = new WindowSettings();
                window.objectListIndex = MATERIAL;
                window.objectIndex = selectedmaterial;
                window.windowFunc = editMaterial;
                window.windowRect = new Rect(0, 100, 300, this.position.height - 20);
                window.windowScroll = Vector2.zero;
                windows.Add(window);
            }
        }
        GUILayout.Label("Consumables", EditorStyles.boldLabel);
        selectedconsumable = GUILayout.SelectionGrid(selectedconsumable, getConsumableStrings(), 1);
        if (selectedconsumable >= 0)
        {
            selectedmaterial = -1;
            if (!openWindow(CONSUMABLE, selectedconsumable))
            {
                WindowSettings window = new WindowSettings();
                window.objectListIndex = CONSUMABLE;
                window.objectIndex = selectedconsumable;
                window.windowFunc = editConsumable;
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
        if (GUILayout.Button("+Material"))
        {
            materials.Add(new MaterialEdit());
        }
        if (GUILayout.Button("+Consumable"))
        {
            consumables.Add(new ConsumableEdit());
        }
        if (GUILayout.Button("Remove"))
        {
            deleteSelected();
        }
        GUILayout.EndVertical();
        base.OnGUI();
    }

    private string[] getMaterialStrings()
    {
        string[] result = new string[materials.Count];
        int i = 0;
        foreach (MaterialEdit edit in materials)
        {
            result[i] = edit.name;
            i++;
        }
        return result;
    }

    private string[] getConsumableStrings()
    {
        string[] result = new string[consumables.Count];
        int i = 0;
        foreach (ConsumableEdit edit in consumables)
        {
            result[i] = edit.name;
            i++;
        }
        return result;
    }

    protected void editMaterial(int windowID)
    {
        if (GUI.Button(closeButtonRect, "X"))
        {
            if (selectedmaterial == windows[windowID].objectIndex) selectedmaterial = -1;
            windows.RemoveAt(windowID);
            return;
        }
        WindowSettings window = windows[windowID];
        MaterialEdit data = materials[window.objectIndex];
        if (data == null)
        {
            windows.RemoveAt(windowID);
        }

        EditorGUIUtility.labelWidth = 120;

        window.windowScroll = GUILayout.BeginScrollView(window.windowScroll, false, true);
        data.name = TextField("Name: ", data.name);
        data.gameName = TextField("Game Name: ", data.gameName);
        data.modelName = TextField("Model Name: ", data.modelName);

        EditorGUILayout.PrefixLabel("Description:");
        data.description = EditorGUILayout.TextField(data.description);

        GUILayout.EndScrollView();
        GUI.DragWindow();
    }

    protected void editConsumable(int windowID)
    {
        if (GUI.Button(closeButtonRect, "X"))
        {
            if (selectedconsumable == windows[windowID].objectIndex) selectedconsumable = -1;
            windows.RemoveAt(windowID);
            return;
        }
        WindowSettings window = windows[windowID];
        ConsumableEdit data = consumables[window.objectIndex];
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

        EditorGUILayout.PrefixLabel("Description:");
        data.description = EditorGUILayout.TextField(data.description);

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Effects: ", EditorStyles.boldLabel);
        data.hungerChange = IntField("Hunger: ", data.hungerChange);

        GUILayout.EndScrollView();
        GUI.DragWindow();
    }

    protected override GUI.WindowFunction getWindowFunc(WindowSettings window)
    {
        if (window.objectListIndex == MATERIAL) return editMaterial;
        if (window.objectListIndex == CONSUMABLE) return editConsumable;
        return null;
    }

    protected override ObjectEdit getObjectEdit(int listIndex, int index)
    {
        switch (listIndex)
        {
            case (MATERIAL):
                return materials[index];
            case (CONSUMABLE):
                return consumables[index];
            default:
                return null;
        }
    }

    protected override bool indexOutOfBounds(int listIndex, int index)
    {
        switch (listIndex)
        {
            case (MATERIAL):
                return index >= materials.Count;
            case (CONSUMABLE):
                return index >= consumables.Count;
            default:
                return true;
        }
    }

    protected override void deleteSelected()
    {
        if (selectedmaterial >= 0)
        {
            deleteWindowIndex = selectedmaterial;
            deleteWindowListIndex = MATERIAL;
            GUI.BringWindowToFront(DELETEWINDOWID);
        }
        else if (selectedconsumable >= 0)
        {
            deleteWindowIndex = selectedconsumable;
            deleteWindowListIndex = CONSUMABLE;
            GUI.BringWindowToFront(DELETEWINDOWID);
        }
    }

    protected override void delete(int listIndex, int index)
    {
        if (!indexOutOfBounds(listIndex, index))
        {
            switch (listIndex)
            {
                case (MATERIAL):
                    closeWindow(MATERIAL, index);
                    materials.RemoveAt(index);
                    if (selectedmaterial > 0) selectedmaterial--;
                    break;
                case (CONSUMABLE):
                    closeWindow(CONSUMABLE, index);
                    consumables.RemoveAt(index);
                    if (selectedconsumable > 0) selectedconsumable--;
                    break;
            }
        }
    }

    private void duplicate()
    {
        if (selectedmaterial >= 0)
        {
            MaterialEdit temp = new MaterialEdit(materials[selectedmaterial]);
            temp.gameName += "(Copy)";
            materials.Insert(selectedmaterial + 1, temp);
            selectedmaterial++;
        }
        else if (selectedconsumable >= 0)
        {
            ConsumableEdit temp = new ConsumableEdit(consumables[selectedconsumable]);
            temp.gameName += "(Copy)";
            consumables.Insert(selectedconsumable + 1, temp);
            selectedconsumable++;
        }
    }

    private void moveUp()
    {
        if (selectedmaterial > 0)
        {
            MaterialEdit temp = materials[selectedmaterial];
            materials.RemoveAt(selectedmaterial);
            materials.Insert(selectedmaterial - 1, temp);
            selectedmaterial -= 1;

        }
        else if (selectedconsumable > 0)
        {
            ConsumableEdit temp = consumables[selectedconsumable];
            consumables.RemoveAt(selectedconsumable);
            consumables.Insert(selectedconsumable - 1, temp);
            selectedconsumable -= 1;
        }
    }

    private void moveDown()
    {
        if (selectedmaterial >= 0 && selectedmaterial < materials.Count - 1)
        {
            MaterialEdit temp = materials[selectedmaterial];
            materials.RemoveAt(selectedmaterial);
            materials.Insert(selectedmaterial + 1, temp);
            selectedmaterial += 1;
        }
        else if (selectedconsumable >= 0 && selectedconsumable < consumables.Count - 1)
        {
            ConsumableEdit temp = consumables[selectedconsumable];
            consumables.RemoveAt(selectedconsumable);
            consumables.Insert(selectedconsumable + 1, temp);
            selectedconsumable += 1;
        }
    }
}
