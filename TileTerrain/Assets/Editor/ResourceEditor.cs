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
    private List<ResourceEdit> resources = new List<ResourceEdit>();

    private Vector2 scroll;

    private int selectedres = -1;


    [MenuItem("Editor/Resource Editor")]
    static void Init()
    {
        ResourceEditor window = (ResourceEditor)EditorWindow.GetWindow(typeof(ResourceEditor));
    }

    protected override void loadFile()
    {
        filePath = null;
        filePath = EditorUtility.OpenFilePanel("Open resource data", Application.dataPath + "/Resources/Data", "xml");
        if (filePath != null)
        {
            DataHolder.ResourceDataHolder resourceDataHolder;

            XmlSerializer serializer = new XmlSerializer(typeof(DataHolder.ResourceDataHolder));
            var stream = new FileStream(filePath, FileMode.Open);
            resourceDataHolder = serializer.Deserialize(stream) as DataHolder.ResourceDataHolder;
            stream.Close();

            resources = new List<ResourceEdit>();

            foreach (ResourceData rdata in resourceDataHolder.resourceData)
            {
                resources.Add(new ResourceEdit(rdata));
            }
        }

    }

    protected override void saveFile()
    {
        if (filePath == null)
        {
            saveAsFile();
            return;
        }
        ResourceData[] resourceData = new ResourceData[resources.Count];

        int i = 0;
        foreach (ResourceEdit redit in resources)
        {
            resourceData[i] = new ResourceData(redit);
            i++;
        }

        DataHolder.ResourceDataHolder resourceDataHolder = new DataHolder.ResourceDataHolder(resourceData);

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
            //selectedother = -1;
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
            result[i] = edit.gameName;
            i++;
        }
        return result;
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
        Vector2 amount = EditorGUILayout.Vector2Field("Min/Max", new Vector2(data.minDrops, data.maxDrops));
        data.minDrops = (int)amount.x;
        data.maxDrops = (int)amount.y;

        GUILayout.EndScrollView();
        GUI.DragWindow();
    }

    protected override GUI.WindowFunction getWindowFunc(WindowSettings window)
    {
        if (window.objectListIndex == RESOURCES) return editResource;
        return null;
    }

    protected override ObjectEdit getObjectEdit(int listIndex, int index)
    {
        switch (listIndex)
        {
            case (RESOURCES):
                return resources[index];
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
            }
        }
    }

    private void duplicate()
    {
        if (selectedres >= 0)
        {
            ResourceEdit temp = new ResourceEdit(resources[selectedres]);
            temp.gameName += "(Copy)";
            resources.Insert(selectedres + 1, temp);
            selectedres++;
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
    }
}
