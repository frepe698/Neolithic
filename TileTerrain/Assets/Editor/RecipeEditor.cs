using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;
using System.IO;
using Edit;

public class RecipeEditor : ObjectEditor
{
    //private static DataHolder.RecipeDataHolder weaponDataHolder;

    private const int EQUIPMENT = 0;
    private const int MATERIAL = 1;
    private const int CONSUMABLE = 2;
    private List<EquipmentRecipeEdit> equipment = new List<EquipmentRecipeEdit>();
    private List<MaterialRecipeEdit> materials = new List<MaterialRecipeEdit>();
    private List<ConsumableRecipeEdit> consumable = new List<ConsumableRecipeEdit>();

    private Vector2 scroll;

    private int selectedequipment = -1;
    private int selectedmaterial = -1;
    private int selectedconsumable = -1;

    protected static RecipeEditor window;

    [MenuItem("Editor/Recipe Editor")]
    static void Init()
    {
        window = (RecipeEditor)EditorWindow.GetWindow(typeof(RecipeEditor));
        //loadFile();
    }

    protected override string getStandardFilePath()
    {
        return Application.dataPath + "/Resources/Data/recipedata.xml";
    }

    protected override void loadFile(string filePath)
    {
        if (filePath == null)
        {
            filePath = EditorUtility.OpenFilePanel("Open recipe data", Application.dataPath + "/Resources/Data", "xml");
        }

        if (filePath != null && !filePath.Equals(""))
        {
            DataHolder.RecipeDataHolder recipeDataHolder;

            XmlSerializer serializer = new XmlSerializer(typeof(DataHolder.RecipeDataHolder));
            var stream = new FileStream(filePath, FileMode.Open);
            recipeDataHolder = serializer.Deserialize(stream) as DataHolder.RecipeDataHolder;
            stream.Close();

            equipment = new List<EquipmentRecipeEdit>();
            materials = new List<MaterialRecipeEdit>();
            consumable = new List<ConsumableRecipeEdit>();
            foreach (EquipmentRecipeData mdata in recipeDataHolder.equipmentRecipeData)
            {
                equipment.Add(new EquipmentRecipeEdit(mdata));
            }
            foreach (MaterialRecipeData rdata in recipeDataHolder.materialRecipeData)
            {
                materials.Add(new MaterialRecipeEdit(rdata));
            }
            foreach (ConsumableRecipeData cdata in recipeDataHolder.consumableRecipeData)
            {
                consumable.Add(new ConsumableRecipeEdit(cdata));
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

        EquipmentRecipeData[] equipmentData = new EquipmentRecipeData[equipment.Count];
        MaterialRecipeData[] materialData = new MaterialRecipeData[materials.Count];
        ConsumableRecipeData[] consumableData = new ConsumableRecipeData[consumable.Count];

        int i = 0;
        foreach (EquipmentRecipeEdit medit in equipment)
        {
            equipmentData[i] = new EquipmentRecipeData(medit);
            i++;
        }
        i = 0;
        foreach (MaterialRecipeEdit redit in materials)
        {
            materialData[i] = new MaterialRecipeData(redit);
            i++;
        }
        i = 0;
        foreach (ConsumableRecipeEdit redit in consumable)
        {
            consumableData[i] = new ConsumableRecipeData(redit);
            i++;
        }
        DataHolder.RecipeDataHolder weaponDataHolder = new DataHolder.RecipeDataHolder(equipmentData, materialData, consumableData);

        using (FileStream file = new FileStream(filePath, FileMode.Create))
        {
            XmlSerializer serializer = new XmlSerializer(typeof(DataHolder.RecipeDataHolder));
            serializer.Serialize(file, weaponDataHolder);
            AssetDatabase.Refresh();
        }

    }



    protected override void OnGUI()
    {

        scroll = GUILayout.BeginScrollView(scroll, false, true, GUILayout.Width(160), GUILayout.Height(this.position.height - 170));

        GUILayout.Label("Equipment Recipes", EditorStyles.boldLabel);
        selectedequipment = GUILayout.SelectionGrid(selectedequipment, getEquipmentStrings(), 1);
        if (selectedequipment >= 0)
        {
            selectedmaterial = -1;
            selectedconsumable = -1;
            if (!openWindow(EQUIPMENT, selectedequipment))
            {
                WindowSettings window = new WindowSettings();
                window.objectListIndex = EQUIPMENT;
                window.objectIndex = selectedequipment;
                window.windowFunc = editEquipmentRecipe;
                window.windowRect = new Rect(0, 100, 300, this.position.height - 20);
                window.windowScroll = Vector2.zero;
                windows.Add(window);
            }
        }
        GUILayout.Label("Material Recipes", EditorStyles.boldLabel);
        selectedmaterial = GUILayout.SelectionGrid(selectedmaterial, getMaterialStrings(), 1);
        if (selectedmaterial >= 0)
        {
            selectedequipment = -1;
            selectedconsumable = -1;
            if (!openWindow(MATERIAL, selectedmaterial))
            {
                WindowSettings window = new WindowSettings();
                window.objectListIndex = MATERIAL;
                window.objectIndex = selectedmaterial;
                window.windowFunc = editMaterialRecipe;
                window.windowRect = new Rect(0, 100, 300, this.position.height - 20);
                window.windowScroll = Vector2.zero;
                windows.Add(window);
            }
        }
        GUILayout.Label("Consumable Recipes", EditorStyles.boldLabel);
        selectedconsumable = GUILayout.SelectionGrid(selectedconsumable, getConsumableStrings(), 1);
        if (selectedconsumable >= 0)
        {
            selectedequipment = -1;
            selectedmaterial = -1;
            if (!openWindow(CONSUMABLE, selectedconsumable))
            {
                WindowSettings window = new WindowSettings();
                window.objectListIndex = CONSUMABLE;
                window.objectIndex = selectedconsumable;
                window.windowFunc = editConsumableRecipe;
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
        if (GUILayout.Button("+Equipment Recipe"))
        {
            equipment.Add(new EquipmentRecipeEdit());
        }
        if (GUILayout.Button("+Material Recipe"))
        {
            materials.Add(new MaterialRecipeEdit());
        }
        if (GUILayout.Button("+Consumable Recipe"))
        {
            consumable.Add(new ConsumableRecipeEdit());
        }
        if (GUILayout.Button("Remove"))
        {
            deleteSelected();
        }
        GUILayout.EndVertical();
        base.OnGUI();
    }

    private string[] getEquipmentStrings()
    {
        string[] result = new string[equipment.Count];
        int i = 0;
        foreach (EquipmentRecipeEdit edit in equipment)
        {
            result[i] = edit.name;
            i++;
        }
        return result;
    }

    private string[] getMaterialStrings()
    {
        string[] result = new string[materials.Count];
        int i = 0;
        foreach (MaterialRecipeEdit edit in materials)
        {
            result[i] = edit.name;
            i++;
        }
        return result;
    }

    private string[] getConsumableStrings()
    {
        string[] result = new string[consumable.Count];
        int i = 0;
        foreach (ConsumableRecipeEdit edit in consumable)
        {
            result[i] = edit.name;
            i++;
        }
        return result;
    }

    protected void editEquipmentRecipe(int windowID)
    {
        if (GUI.Button(closeButtonRect, "X"))
        {
            if (selectedequipment == windows[windowID].objectIndex) selectedequipment = -1;
            windows.RemoveAt(windowID);
            return;
        }
        WindowSettings window = windows[windowID];
        EquipmentRecipeEdit data = equipment[window.objectIndex];
        if (data == null)
        {
            windows.RemoveAt(windowID);
        }

        EditorGUIUtility.labelWidth = 120;

        window.windowScroll = GUILayout.BeginScrollView(window.windowScroll, false, true);

        editRecipe(data);

        GUILayout.EndScrollView();
        GUI.DragWindow();
    }

    protected void editMaterialRecipe(int windowID)
    {
        if (GUI.Button(closeButtonRect, "X"))
        {
            if (selectedmaterial == windows[windowID].objectIndex) selectedmaterial = -1;
            windows.RemoveAt(windowID);
            return;
        }
        WindowSettings window = windows[windowID];
        MaterialRecipeEdit data = materials[window.objectIndex];
        if (data == null)
        {
            windows.RemoveAt(windowID);
        }

        EditorGUIUtility.labelWidth = 120;

        window.windowScroll = GUILayout.BeginScrollView(window.windowScroll, false, true);

        editRecipe(data);

        GUILayout.EndScrollView();
        GUI.DragWindow();
    }

    protected void editConsumableRecipe(int windowID)
    {
        if (GUI.Button(closeButtonRect, "X"))
        {
            if (selectedconsumable == windows[windowID].objectIndex) selectedconsumable = -1;
            windows.RemoveAt(windowID);
            return;
        }
        WindowSettings window = windows[windowID];
        ConsumableRecipeEdit data = consumable[window.objectIndex];
        if (data == null)
        {
            windows.RemoveAt(windowID);
        }

        EditorGUIUtility.labelWidth = 120;

        window.windowScroll = GUILayout.BeginScrollView(window.windowScroll, false, true);

        editRecipe(data);

        GUILayout.EndScrollView();
        GUI.DragWindow();
    }

    protected void editRecipe(RecipeEdit data)
    {
        data.name = TextField("Name: ", data.name);
        data.gameName = TextField("Game Name: ", data.gameName);
        EditorGUILayout.PrefixLabel("Description:");
        data.description = EditorGUILayout.TextField(data.description);

        GUILayout.Space(10);
        EditorGUILayout.LabelField("Skill:", EditorStyles.boldLabel);
        data.skill = (Skills)EditorGUILayout.EnumPopup("Skill: ", data.skill);
        data.expAmount = EditorGUILayout.IntField("Exp Gain: ", data.expAmount);
        data.requiredSkillLevel = EditorGUILayout.IntField("Req level: ", data.requiredSkillLevel);

        GUILayout.Space(20);
        EditorGUILayout.LabelField("Product:", EditorStyles.boldLabel);
        data.product = EditorGUILayout.TextField(data.product);

        GUILayout.Space(20);
        EditorGUILayout.LabelField("Ingredients:", EditorStyles.boldLabel);
        foreach (IngredientEdit edit in data.ingredients)
        {
            EditorGUILayout.BeginHorizontal();
            edit.name = EditorGUILayout.TextField(edit.name);
            edit.amount = EditorGUILayout.IntField(edit.amount);
            if (GUILayout.Button("-"))
            {
                data.ingredients.Remove(edit);
                break;
            }
            EditorGUILayout.EndHorizontal();
        }
        if (GUILayout.Button("+Ingredient"))
        {
            data.ingredients.Add(new IngredientEdit());
        }
    }

    protected override GUI.WindowFunction getWindowFunc(WindowSettings window)
    {
        if (window.objectListIndex == EQUIPMENT) return editEquipmentRecipe;
        if (window.objectListIndex == MATERIAL) return editMaterialRecipe;
        if (window.objectListIndex == CONSUMABLE) return editConsumableRecipe;
        return null;
    }

    protected override ObjectEdit getObjectEdit(int listIndex, int index)
    {
        switch (listIndex)
        {
            case (EQUIPMENT):
                return equipment[index];
            case (MATERIAL):
                return materials[index];
            case(CONSUMABLE):
                return consumable[index];
            default:
                return null;
        }
    }

    protected override bool indexOutOfBounds(int listIndex, int index)
    {
        switch (listIndex)
        {
            case (EQUIPMENT):
                return index >= equipment.Count;
            case (MATERIAL):
                return index >= materials.Count;
            case(CONSUMABLE):
                return index >= consumable.Count;
            default:
                return true;
        }
    }

    protected override void deleteSelected()
    {
        if (selectedequipment >= 0)
        {
            deleteWindowIndex = selectedequipment;
            deleteWindowListIndex = EQUIPMENT;
            GUI.BringWindowToFront(DELETEWINDOWID);
        }
        else if (selectedmaterial >= 0)
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
                case (EQUIPMENT):
                    closeWindow(EQUIPMENT, index);
                    equipment.RemoveAt(index);
                    if (selectedequipment > 0) selectedequipment--;
                    break;
                case (MATERIAL):
                    closeWindow(MATERIAL, index);
                    materials.RemoveAt(index);
                    if (selectedmaterial > 0) selectedmaterial--;
                    break;
                case(CONSUMABLE):
                    closeWindow(CONSUMABLE, index);
                    consumable.RemoveAt(index);
                    if (selectedconsumable > 0) selectedconsumable--;
                    break;
            }
        }
    }

    private void duplicate()
    {
        if (selectedequipment >= 0)
        {
            EquipmentRecipeEdit temp = new EquipmentRecipeEdit(equipment[selectedequipment]);
            temp.name += "(Copy)";
            temp.gameName += "(Copy)";
            equipment.Insert(selectedequipment + 1, temp);
            selectedequipment++;
        }
        else if (selectedmaterial >= 0)
        {
            MaterialRecipeEdit temp = new MaterialRecipeEdit(materials[selectedmaterial]);
            temp.name += "(Copy)";
            temp.gameName += "(Copy)";
            materials.Insert(selectedmaterial + 1, temp);
            selectedmaterial++;
        }
        else if (selectedconsumable >= 0)
        {
            ConsumableRecipeEdit temp = new ConsumableRecipeEdit(consumable[selectedconsumable]);
            temp.name += "(Copy)";
            temp.gameName += "(Copy)";
            consumable.Insert(selectedconsumable + 1, temp);
            selectedconsumable++;
        }
    }

    private void moveUp()
    {
        if (selectedequipment > 0)
        {
            EquipmentRecipeEdit temp = equipment[selectedequipment];
            equipment.RemoveAt(selectedequipment);
            equipment.Insert(selectedequipment - 1, temp);
            selectedequipment -= 1;

        }
        else if (selectedmaterial > 0)
        {
            MaterialRecipeEdit temp = materials[selectedmaterial];
            materials.RemoveAt(selectedmaterial);
            materials.Insert(selectedmaterial - 1, temp);
            selectedmaterial -= 1;
        }
        else if (selectedconsumable > 0)
        {
            ConsumableRecipeEdit temp = consumable[selectedconsumable];
            consumable.RemoveAt(selectedconsumable);
            consumable.Insert(selectedconsumable - 1, temp);
            selectedconsumable -= 1;
        }
    }

    private void moveDown()
    {
        if (selectedequipment >= 0 && selectedequipment < equipment.Count - 1)
        {
            EquipmentRecipeEdit temp = equipment[selectedequipment];
            equipment.RemoveAt(selectedequipment);
            equipment.Insert(selectedequipment + 1, temp);
            selectedequipment += 1;
        }
        else if (selectedmaterial >= 0 && selectedmaterial < materials.Count - 1)
        {
            MaterialRecipeEdit temp = materials[selectedmaterial];
            materials.RemoveAt(selectedmaterial);
            materials.Insert(selectedmaterial + 1, temp);
            selectedmaterial += 1;
        }
        else if (selectedconsumable >= 0 && selectedconsumable < consumable.Count -1)
        {
            ConsumableRecipeEdit temp = consumable[selectedconsumable];
            consumable.RemoveAt(selectedconsumable);
            consumable.Insert(selectedconsumable + 1, temp);
            selectedconsumable += 1;
        }
    }
}
