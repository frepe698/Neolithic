using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;
using System.IO;
using Edit;

public class WeaponEditor : ObjectEditor {
    //private static DataHolder.WeaponDataHolder weaponDataHolder;

    private const int MELEE = 0;
    private const int RANGED = 1;
    private List<MeleeWeaponEdit> meleeWeapons = new List<MeleeWeaponEdit>();
    private List<RangedWeaponEdit> rangedWeapons = new List<RangedWeaponEdit>();

    private Vector2 scroll;

    private int selectedmelee = -1;
    private int selectedranged = -1;


    [MenuItem("Editor/Weapon Editor")]
    static void Init()
    {
        WeaponEditor window = (WeaponEditor)EditorWindow.GetWindow(typeof(WeaponEditor));
        //loadFile();
    }

    protected override string getStandardFilePath()
    {
        return Application.dataPath + "/Resources/Data/weapondata.xml";
    }

    protected override void loadFile(string filePath)
    {
        if (filePath == null)
        {
            filePath = EditorUtility.OpenFilePanel("Open weapon data", Application.dataPath + "/Resources/Data", "xml");
        }

        if (filePath != null && !filePath.Equals(""))
        {
            DataHolder.WeaponDataHolder weaponDataHolder;

            XmlSerializer serializer = new XmlSerializer(typeof(DataHolder.WeaponDataHolder));
            var stream = new FileStream(filePath, FileMode.Open);
            weaponDataHolder = serializer.Deserialize(stream) as DataHolder.WeaponDataHolder;
            stream.Close();

            meleeWeapons = new List<MeleeWeaponEdit>();
            rangedWeapons = new List<RangedWeaponEdit>();
            foreach (MeleeWeaponData mdata in weaponDataHolder.meleeWeaponData)
            {
                meleeWeapons.Add(new MeleeWeaponEdit(mdata));
            }
            foreach (RangedWeaponData rdata in weaponDataHolder.rangedWeaponData)
            {
                rangedWeapons.Add(new RangedWeaponEdit(rdata));
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
        
        MeleeWeaponData[] meleeData = new MeleeWeaponData[meleeWeapons.Count];
        RangedWeaponData[] rangedData = new RangedWeaponData[rangedWeapons.Count];

        int i = 0;
        foreach (MeleeWeaponEdit medit in meleeWeapons)
        {
            meleeData[i] = new MeleeWeaponData(medit);
            i++;
        }
        i = 0;
        foreach (RangedWeaponEdit redit in rangedWeapons)
        {
            rangedData[i] = new RangedWeaponData(redit);
            i++;
        }

        DataHolder.WeaponDataHolder weaponDataHolder = new DataHolder.WeaponDataHolder(meleeData, rangedData);

        using (FileStream file = new FileStream(filePath, FileMode.Create))
        {
            XmlSerializer serializer = new XmlSerializer(typeof(DataHolder.WeaponDataHolder));
            serializer.Serialize(file, weaponDataHolder);
            AssetDatabase.Refresh();
        }  
        
    }



    protected override void OnGUI()
    {

        scroll = GUILayout.BeginScrollView(scroll, false, true, GUILayout.Width(160), GUILayout.Height(this.position.height - 150));

        GUILayout.Label("Melee Weapons", EditorStyles.boldLabel);
        selectedmelee = GUILayout.SelectionGrid(selectedmelee, getMeleeStrings(), 1);
        if (selectedmelee >= 0)
        {
            selectedranged = -1;
            if (!openWindow(MELEE, selectedmelee))
            {
                WindowSettings window = new WindowSettings();
                window.objectListIndex = MELEE;
                window.objectIndex = selectedmelee;
                window.windowFunc = editMeleeWeapon;
                window.windowRect = new Rect(0, 100, 300, this.position.height - 20);
                window.windowScroll = Vector2.zero;
                windows.Add(window);
            }
        }
        GUILayout.Label("Ranged Weapons", EditorStyles.boldLabel);
        selectedranged = GUILayout.SelectionGrid(selectedranged, getRangedStrings(), 1);
        if (selectedranged >= 0)
        {
            selectedmelee = -1;
            if (!openWindow(RANGED, selectedranged))
            {
                WindowSettings window = new WindowSettings();
                window.objectListIndex = RANGED;
                window.objectIndex = selectedranged;
                window.windowFunc = editRangedWeapon;
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
        if (GUILayout.Button("+Melee"))
        {
            meleeWeapons.Add(new MeleeWeaponEdit());
        }
        if (GUILayout.Button("+Ranged"))
        {
            rangedWeapons.Add(new RangedWeaponEdit());
        }
        if (GUILayout.Button("Remove"))
        {
            deleteSelected();
        }
        GUILayout.EndVertical();
        base.OnGUI();
    }

    private string[] getMeleeStrings()
    {
        string[] result = new string[meleeWeapons.Count];
        int i = 0;
        foreach (MeleeWeaponEdit edit in meleeWeapons)
        {
            result[i] = edit.name;
            i++;
        }
        return result;
    }

    private string[] getRangedStrings()
    {
        string[] result = new string[rangedWeapons.Count];
        int i = 0;
        foreach (RangedWeaponEdit edit in rangedWeapons)
        {
            result[i] = edit.name;
            i++;
        }
        return result;
    }

    protected void editMeleeWeapon(int windowID)
    {
        if (GUI.Button(closeButtonRect, "X"))
        {
            if (selectedmelee == windows[windowID].objectIndex) selectedmelee = -1;
            windows.RemoveAt(windowID);
            return;
        }
        WindowSettings window = windows[windowID];
        MeleeWeaponEdit data = meleeWeapons[window.objectIndex];
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
        EditorGUILayout.LabelField("Damage:  DPS:", EditorStyles.boldLabel);
        data.damage[0] = IntField("Combat:    " + data.damage[0]*data.attackSpeed, data.damage[0]);
        data.damage[1] = IntField("Tree:         " + data.damage[1]*data.attackSpeed, data.damage[1]);
        data.damage[2] = IntField("Stone:       " + data.damage[2]*data.attackSpeed, data.damage[2]);

        data.attackSpeed = FloatField("Attack speed", data.attackSpeed);

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Weapon Stuff:", EditorStyles.boldLabel);
        data.weaponAttackAnim = TextField("Weapon Atk Anim: ", data.weaponAttackAnim);
        data.rightHand = EditorGUILayout.Toggle("Hold Right Hand: ", data.rightHand);

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Attack Animations:", EditorStyles.boldLabel);
        data.attackAnims[0] = TextField("Combat: ", data.attackAnims[0]);
        data.attackAnims[1] = TextField("Tree: ", data.attackAnims[1]);
        data.attackAnims[2] = TextField("Stone: ", data.attackAnims[2]);

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Other Animations:", EditorStyles.boldLabel);
        data.idleAnim = TextField("Idle: ", data.idleAnim);
        data.runAnim = TextField("Run: ", data.runAnim);
        data.lootAnim = TextField("Loot: ", data.lootAnim);

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Attack Sounds:", EditorStyles.boldLabel);
        data.attackSounds[0] = TextField("Combat: ", data.attackSounds[0]);
        data.attackSounds[1] = TextField("Tree: ", data.attackSounds[1]);
        data.attackSounds[2] = TextField("Stone: ", data.attackSounds[2]);

        EditorGUILayout.Space();
        data.durability = IntField("Durability: ", data.durability);

        GUILayout.EndScrollView();
        GUI.DragWindow();
    }

    protected void editRangedWeapon(int windowID)
    {
        if (GUI.Button(closeButtonRect, "X"))
        {
            if (selectedranged == windows[windowID].objectIndex) selectedranged = -1;
            windows.RemoveAt(windowID);
            return;
        }
        WindowSettings window = windows[windowID];
        RangedWeaponEdit data = rangedWeapons[window.objectIndex];
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
        EditorGUILayout.LabelField("Attack: \t DPS: " + data.damage*data.attackSpeed, EditorStyles.boldLabel);
        data.damage = IntField("Damage: ", data.damage);
        data.attackSpeed = FloatField("Attack speed", data.attackSpeed);
        data.attackAnim = TextField("Animation: ", data.attackAnim);
        data.attackSound = TextField("Sound: ", data.attackSound);

        

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Weapon Stuff:", EditorStyles.boldLabel);
        data.weaponAttackAnim = TextField("Weapon Atk Anim: ", data.weaponAttackAnim);
        data.rightHand = EditorGUILayout.Toggle("Hold Right Hand: ", data.rightHand);

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Projectile:", EditorStyles.boldLabel);
        data.projectileName = TextField("Name: ", data.projectileName);
        data.projectileModelName = TextField("Model Name: ", data.projectileModelName);

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Other Animations:", EditorStyles.boldLabel);
        data.idleAnim = TextField("Idle: ", data.idleAnim);
        data.runAnim = TextField("Run: ", data.runAnim);
        data.lootAnim = TextField("Loot: ", data.lootAnim);

        EditorGUILayout.Space();
        data.durability = IntField("Durability: ", data.durability);

        GUILayout.EndScrollView();
        GUI.DragWindow();
    }

    protected override GUI.WindowFunction getWindowFunc(WindowSettings window)
    {
        if (window.objectListIndex == MELEE) return editMeleeWeapon;
        if (window.objectListIndex == RANGED) return editRangedWeapon;
        return null;
    }

    protected override ObjectEdit getObjectEdit(int listIndex, int index)
    {
        switch (listIndex)
        {
            case (MELEE):
                return meleeWeapons[index];
            case (RANGED):
                return rangedWeapons[index];
            default:
                return null;
        }
    }

    protected override bool indexOutOfBounds(int listIndex, int index)
    {
        switch (listIndex)
        {
            case (MELEE):
                return index >= meleeWeapons.Count;
            case (RANGED):
                return index >= rangedWeapons.Count;
            default:
                return true;
        }
    }

    protected override void deleteSelected()
    {
        if (selectedmelee >= 0)
        {
            deleteWindowIndex = selectedmelee;
            deleteWindowListIndex = MELEE;
            GUI.BringWindowToFront(DELETEWINDOWID);
        }
        else if(selectedranged >= 0)
        {
            deleteWindowIndex = selectedranged;
            deleteWindowListIndex = RANGED;
            GUI.BringWindowToFront(DELETEWINDOWID);
        }
    }

    protected override void delete(int listIndex, int index)
    {
        if (!indexOutOfBounds(listIndex, index))
        {
            switch (listIndex)
            {
                case (MELEE):
                    closeWindow(MELEE, index);
                    meleeWeapons.RemoveAt(index);
                    if (selectedmelee > 0) selectedmelee--;
                    break;
                case (RANGED):
                    closeWindow(RANGED, index);
                    rangedWeapons.RemoveAt(index);
                    if (selectedranged > 0) selectedranged--;
                    break;
            }
        }
    }

    private void duplicate()
    {
        if (selectedmelee >= 0)
        {
            MeleeWeaponEdit temp = new MeleeWeaponEdit(meleeWeapons[selectedmelee]);
            temp.name += "(Copy)";
            temp.gameName += "(Copy)";
            meleeWeapons.Insert(selectedmelee + 1, temp);
            selectedmelee++;
        }
        else if (selectedranged >= 0)
        {
            RangedWeaponEdit temp = new RangedWeaponEdit(rangedWeapons[selectedranged]);
            temp.name += "(Copy)";
            temp.gameName += "(Copy)";
            rangedWeapons.Insert(selectedranged + 1, temp);
            selectedranged++;
        }
    }

    private void moveUp()
    {
        if (selectedmelee > 0)
        {
            MeleeWeaponEdit temp = meleeWeapons[selectedmelee];
            meleeWeapons.RemoveAt(selectedmelee);
            meleeWeapons.Insert(selectedmelee - 1, temp);
            selectedmelee -= 1;

        }
        else if (selectedranged > 0)
        {
            RangedWeaponEdit temp = rangedWeapons[selectedranged];
            rangedWeapons.RemoveAt(selectedranged);
            rangedWeapons.Insert(selectedranged - 1, temp);
            selectedranged -= 1;
        }
    }

    private void moveDown()
    {
        if (selectedmelee >= 0 && selectedmelee < meleeWeapons.Count - 1)
        {
            MeleeWeaponEdit temp = meleeWeapons[selectedmelee];
            meleeWeapons.RemoveAt(selectedmelee);
            meleeWeapons.Insert(selectedmelee + 1, temp);
            selectedmelee += 1;
        }
        else if (selectedranged >= 0 && selectedranged < rangedWeapons.Count-1)
        {
            RangedWeaponEdit temp = rangedWeapons[selectedranged];
            rangedWeapons.RemoveAt(selectedranged);
            rangedWeapons.Insert(selectedranged+1, temp);
            selectedranged += 1;
        }
    }
}
