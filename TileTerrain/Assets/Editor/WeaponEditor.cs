﻿using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;
using System.IO;
using Edit;

public class WeaponEditor : ObjectEditor {
    //private static DataHolder.WeaponDataHolder weaponDataHolder;
    
    [SerializeField]
    private List<MeleeWeaponEdit> meleeWeapons = new List<MeleeWeaponEdit>();
    [SerializeField]
    private List<RangedWeaponEdit> rangedWeapons = new List<RangedWeaponEdit>();

    private Vector2 scroll;

    [MenuItem("Editor/Weapon Editor")]
    static void Init()
    {
        WeaponEditor window = (WeaponEditor)EditorWindow.GetWindow(typeof(WeaponEditor));
        //loadFile();
    }

    protected override void loadFile()
    {
        DataHolder.WeaponDataHolder weaponDataHolder;
        TextAsset data = (TextAsset)Resources.Load("Data/weapondata");
        XmlSerializer serializer = new XmlSerializer(typeof(DataHolder.WeaponDataHolder));
        using (StringReader reader = new System.IO.StringReader(data.text))
        {
            weaponDataHolder = serializer.Deserialize(reader) as DataHolder.WeaponDataHolder;
        }

        foreach (MeleeWeaponData mdata in weaponDataHolder.meleeWeaponData)
        {
            meleeWeapons.Add(new MeleeWeaponEdit(mdata));
        }
        foreach (RangedWeaponData rdata in weaponDataHolder.rangedWeaponData)
        {
            rangedWeapons.Add(new RangedWeaponEdit(rdata));
        }
    }

    protected override void saveFile()
    {
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

        TextAsset data = (TextAsset)Resources.Load("Data/test");

        using (FileStream file = new FileStream(Application.dataPath + "/Resources/Data/test.xml", FileMode.Create))
        {
            XmlSerializer serializer = new XmlSerializer(typeof(DataHolder.WeaponDataHolder));
            serializer.Serialize(file, weaponDataHolder);
            AssetDatabase.Refresh();
        }      
    }

    protected override void OnGUI()
    {
        scroll = GUILayout.BeginScrollView(scroll, false, true, GUILayout.Width(160), GUILayout.Height(this.position.height - 80));
        GUILayout.Label("Melee Weapons", EditorStyles.boldLabel);
        for (int i = 0; i < meleeWeapons.Count; i++)
        {
            if (GUILayout.Button(meleeWeapons[i].gameName))
            {
                if (!openWindow(meleeWeapons[i]))
                {
                    WindowSettings window = new WindowSettings();
                    window.windowObject = meleeWeapons[i];
                    window.windowFunc = editMeleeWeapon;
                    window.windowRect = new Rect(0, 100, 300, this.position.height - 20);
                    window.windowScroll = Vector2.zero;
                    windows.Add(window);
                }
            }   
        }
        GUILayout.Label("Ranged Weapons", EditorStyles.boldLabel);
        for (int i = 0; i < rangedWeapons.Count; i++)
        {
            if (GUILayout.Button(rangedWeapons[i].gameName))
            {
                if (!openWindow(rangedWeapons[i]))
                {
                    WindowSettings window = new WindowSettings();
                    window.windowObject = rangedWeapons[i];
                    window.windowFunc = editRangedWeapon;
                    window.windowRect = new Rect(0, 100, 300, this.position.height - 20);
                    window.windowScroll = Vector2.zero;
                    windows.Add(window);
                }
            }
        }
       
        GUILayout.EndScrollView();
        GUILayout.BeginVertical(GUILayout.Width(160));
        GUILayout.Space(20);
        if (GUILayout.Button("Add Melee Weapon"))
        {
            meleeWeapons.Add(new MeleeWeaponEdit());
        }
        if (GUILayout.Button("Add Ranged Weapon"))
        {
            rangedWeapons.Add(new RangedWeaponEdit());
        }
        GUILayout.EndVertical();
        base.OnGUI();
    }

    protected void editMeleeWeapon(int windowID)
    {
        if (GUI.Button(closeButtonRect, "X"))
        {
            windows.RemoveAt(windowID);
            return;
        }
        WindowSettings window = windows[windowID];
        MeleeWeaponEdit data = window.windowObject as MeleeWeaponEdit;
        if (data == null)
        {
            windows.RemoveAt(windowID);
        }

        EditorGUIUtility.labelWidth = 120;

        window.windowScroll = GUILayout.BeginScrollView(window.windowScroll, false, true);
        data.name = TextField("Name: ", data.name);
        data.gameName = TextField("Game Name: ", data.gameName);

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
            windows.RemoveAt(windowID);
            return;
        }
        WindowSettings window = windows[windowID];
        RangedWeaponEdit data = window.windowObject as RangedWeaponEdit;
        if (data == null)
        {
            windows.RemoveAt(windowID);
            return;
        }

        EditorGUIUtility.labelWidth = 120;

        window.windowScroll = GUILayout.BeginScrollView(window.windowScroll, false, true);
        data.name = TextField("Name: ", data.name);
        data.gameName = TextField("Game Name: ", data.gameName);

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
}