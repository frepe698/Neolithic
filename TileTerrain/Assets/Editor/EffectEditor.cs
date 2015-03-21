using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;
using System.IO;
using Edit;

public class EffectEditor : ObjectEditor
{
    //private static DataHolder.EffectDataHolder weaponDataHolder;

    private const int SINGLETARGET = 0;
    private const int AREAOFEFFECT = 1;
    private const int PROJECTILEEFFECT = 2;
    private const int MOVEMENTEFFECT = 3;
    private const int SELFBUFFEFFECT = 4;
    private List<SingleTargetEffectEdit> singletargets = new List<SingleTargetEffectEdit>();
    private List<AreaOfEffectEdit> areaofeffects = new List<AreaOfEffectEdit>();
    private List<ProjectileEffectEdit> projectileeffects = new List<ProjectileEffectEdit>();
    private List<MovementEffectEdit> movementeffects = new List<MovementEffectEdit>();
    private List<SelfBuffEffectEdit> selfbuffeffects = new List<SelfBuffEffectEdit>();

    private Vector2 scroll;

    private int selectedSingleTarget = -1;
    private int selectedAreaOfEffect = -1;
    private int selectedProjectile = -1;
    private int selectedMovement = -1;
    private int selectedSelfBuff = -1;

    protected static EffectEditor window;

    [MenuItem("Editor/Effect Editor")]
    static void Init()
    {
        window = (EffectEditor)EditorWindow.GetWindow(typeof(EffectEditor));
        //loadFile();
    }

    protected override string getStandardFilePath()
    {
        return Application.dataPath + "/Resources/Data/effectdata.xml";
    }

    protected override void loadFile(string filePath)
    {
        if (filePath == null)
        {
            filePath = EditorUtility.OpenFilePanel("Open effect data", Application.dataPath + "/Resources/Data", "xml");
        }

        if (filePath != null && !filePath.Equals(""))
        {
            DataHolder.EffectDataHolder effectDataHolder;

            XmlSerializer serializer = new XmlSerializer(typeof(DataHolder.EffectDataHolder));
            var stream = new FileStream(filePath, FileMode.Open);
            effectDataHolder = serializer.Deserialize(stream) as DataHolder.EffectDataHolder;
            stream.Close();

            singletargets = new List<SingleTargetEffectEdit>();
            areaofeffects = new List<AreaOfEffectEdit>();
            projectileeffects = new List<ProjectileEffectEdit>();
            movementeffects = new List<MovementEffectEdit>();
            selfbuffeffects = new List<SelfBuffEffectEdit>();
            foreach (SingleTargetEffectData mdata in effectDataHolder.singleTargetEffectData)
            {
                singletargets.Add(new SingleTargetEffectEdit(mdata));
            }
            foreach (AreaOfEffectData rdata in effectDataHolder.areaOfEffectData)
            {
                areaofeffects.Add(new AreaOfEffectEdit(rdata));
            }
            if (effectDataHolder.projectileEffectData != null)
            {
                foreach (ProjectileEffectData pdata in effectDataHolder.projectileEffectData)
                {
                    projectileeffects.Add(new ProjectileEffectEdit(pdata));
                }
            }
            if (effectDataHolder.movementEffectData != null)
            {
                foreach (MovementEffectData pdata in effectDataHolder.movementEffectData)
                {
                    movementeffects.Add(new MovementEffectEdit(pdata));
                }
            }
            if(effectDataHolder.selfBuffEffectData != null)
            {
                foreach(SelfBuffEffectData sdata in effectDataHolder.selfBuffEffectData)
                {
                    selfbuffeffects.Add(new SelfBuffEffectEdit(sdata));
                }
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

        SingleTargetEffectData[] singleTargetData = new SingleTargetEffectData[singletargets.Count];
        AreaOfEffectData[] areaOfEffectData = new AreaOfEffectData[areaofeffects.Count];
        ProjectileEffectData[] projectileEffectData = new ProjectileEffectData[projectileeffects.Count];
        MovementEffectData[] movementEffectData = new MovementEffectData[movementeffects.Count];
        SelfBuffEffectData[] selfBuffEffectData = new SelfBuffEffectData[selfbuffeffects.Count];

        int i = 0;
        foreach (SingleTargetEffectEdit medit in singletargets)
        {
            singleTargetData[i] = new SingleTargetEffectData(medit);
            i++;
        }
        i = 0;
        foreach (AreaOfEffectEdit redit in areaofeffects)
        {
            areaOfEffectData[i] = new AreaOfEffectData(redit);
            i++;
        }
        i = 0;
        foreach(ProjectileEffectEdit pedit in projectileeffects)
        {
            projectileEffectData[i] = new ProjectileEffectData(pedit);
            i++;
        }
        i = 0;
        foreach(MovementEffectEdit medit in movementeffects)
        {
            movementEffectData[i] = new MovementEffectData(medit);
            i++;
        }
        i = 0;
        foreach(SelfBuffEffectEdit sedit in selfbuffeffects)
        {
            selfBuffEffectData[i] = new SelfBuffEffectData(sedit);
            i++;
        }
        DataHolder.EffectDataHolder effectDataHolder = new DataHolder.EffectDataHolder(singleTargetData, areaOfEffectData,
                                                                                       projectileEffectData, movementEffectData,
                                                                                       selfBuffEffectData);

        using (FileStream file = new FileStream(filePath, FileMode.Create))
        {
            XmlSerializer serializer = new XmlSerializer(typeof(DataHolder.EffectDataHolder));
            serializer.Serialize(file, effectDataHolder);
            AssetDatabase.Refresh();
        }

    }



    protected override void OnGUI()
    {

        scroll = GUILayout.BeginScrollView(scroll, false, true, GUILayout.Width(160), GUILayout.Height(this.position.height - 170));

        GUILayout.Label("Single Target", EditorStyles.boldLabel);
        selectedSingleTarget = GUILayout.SelectionGrid(selectedSingleTarget, getSingleTargetStrings(), 1);
        if (selectedSingleTarget >= 0)
        {
            selectedAreaOfEffect = -1;
            selectedProjectile = -1;
            selectedMovement = -1;
            selectedSelfBuff = -1;
            if (!openWindow(SINGLETARGET, selectedSingleTarget))
            {
                WindowSettings window = new WindowSettings();
                window.objectListIndex = SINGLETARGET;
                window.objectIndex = selectedSingleTarget;
                window.windowFunc = editSingleTarget;
                window.windowRect = new Rect(0, 100, 300, this.position.height - 20);
                window.windowScroll = Vector2.zero;
                windows.Add(window);
            }
        }
        GUILayout.Label("Area of Effect", EditorStyles.boldLabel);
        selectedAreaOfEffect = GUILayout.SelectionGrid(selectedAreaOfEffect, getAoEStrings(), 1);
        if (selectedAreaOfEffect >= 0)
        {
            selectedSingleTarget = -1;
            selectedProjectile = -1;
            selectedMovement = -1;
            selectedSelfBuff = -1;

            if (!openWindow(AREAOFEFFECT, selectedAreaOfEffect))
            {
                WindowSettings window = new WindowSettings();
                window.objectListIndex = AREAOFEFFECT;
                window.objectIndex = selectedAreaOfEffect;
                window.windowFunc = editAreaOfEffect;
                window.windowRect = new Rect(0, 100, 300, this.position.height - 20);
                window.windowScroll = Vector2.zero;
                windows.Add(window);
            }
        }
        GUILayout.Label("Projectile Effects", EditorStyles.boldLabel);
        selectedProjectile = GUILayout.SelectionGrid(selectedProjectile, getProjectileStrings(), 1);
        if (selectedProjectile >= 0)
        {
            selectedAreaOfEffect = -1;
            selectedSingleTarget = -1;
            selectedMovement = -1;
            selectedSelfBuff = -1;

            if (!openWindow(PROJECTILEEFFECT, selectedProjectile)) 
            {
                WindowSettings window = new WindowSettings();
                window.objectListIndex = PROJECTILEEFFECT;
                window.objectIndex = selectedProjectile;
                window.windowFunc = editProjectileEffect;
                window.windowRect = new Rect(0, 100, 300, this.position.height - 20);
                window.windowScroll = Vector2.zero;
                windows.Add(window);
            }
        }
        GUILayout.Label("Movement Effects", EditorStyles.boldLabel);
        selectedMovement = GUILayout.SelectionGrid(selectedMovement, getMovementStrings(), 1);
        if (selectedMovement >= 0)
        {
            selectedAreaOfEffect = -1;
            selectedSingleTarget = -1;
            selectedProjectile = -1;
            selectedSelfBuff = -1;

            if (!openWindow(MOVEMENTEFFECT, selectedMovement))
            {
                WindowSettings window = new WindowSettings();
                window.objectListIndex = MOVEMENTEFFECT;
                window.objectIndex = selectedMovement;
                window.windowFunc = editMovementEffect;
                window.windowRect = new Rect(0, 100, 300, this.position.height - 20);
                window.windowScroll = Vector2.zero;
                windows.Add(window);
            }
        }
        GUILayout.Label("Self Buff Effects", EditorStyles.boldLabel);
        selectedSelfBuff = GUILayout.SelectionGrid(selectedSelfBuff, getSelfBuffStrings(), 1);
        if(selectedSelfBuff >= 0)
        {
            selectedAreaOfEffect = -1;
            selectedSingleTarget = -1;
            selectedProjectile = -1;
            selectedMovement = -1;

            if(!openWindow(SELFBUFFEFFECT, selectedSelfBuff))
            {
                WindowSettings window = new WindowSettings();
                window.objectListIndex = SELFBUFFEFFECT;
                window.objectIndex = selectedSelfBuff;
                window.windowFunc = editSelfBuffEffect;
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
        if (GUILayout.Button("+Single Target"))
        {
            singletargets.Add(new SingleTargetEffectEdit());
        }
        if (GUILayout.Button("+Area of Effect"))
        {
            areaofeffects.Add(new AreaOfEffectEdit());
        }
        if (GUILayout.Button("+Projectile Effect"))
        {
            projectileeffects.Add(new ProjectileEffectEdit());
        }
        if (GUILayout.Button("+Movement Effect"))
        {
            movementeffects.Add(new MovementEffectEdit());
        }
        if (GUILayout.Button("+Self Buff Effect"))
        {
            selfbuffeffects.Add(new SelfBuffEffectEdit());
        }
        if (GUILayout.Button("Remove"))
        {
            deleteSelected();
        }
        GUILayout.EndVertical();
        base.OnGUI();
    }

    private string[] getSingleTargetStrings()
    {
        string[] result = new string[singletargets.Count];
        int i = 0;
        foreach (SingleTargetEffectEdit edit in singletargets)
        {
            result[i] = edit.name;
            i++;
        }
        return result;
    }

    private string[] getAoEStrings()
    {
        string[] result = new string[areaofeffects.Count];
        int i = 0;
        foreach (AreaOfEffectEdit edit in areaofeffects)
        {
            result[i] = edit.name;
            i++;
        }
        return result;
    }

    private string[] getProjectileStrings()
    {
        string[] result = new string[projectileeffects.Count];
        int i = 0;
        foreach (ProjectileEffectEdit edit in projectileeffects)
        {
            result[i] = edit.name;
            i++;
        }
        return result;
    }

    private string[] getMovementStrings()
    {
        string[] result = new string[movementeffects.Count];
        int i = 0;
        foreach (MovementEffectEdit edit in movementeffects)
        {
            result[i] = edit.name;
            i++;
        }
        return result;
    }

    private string[] getSelfBuffStrings()
    {
        string[] result = new string[selfbuffeffects.Count];
        int i = 0;
        foreach(SelfBuffEffectEdit edit in selfbuffeffects)
        {
            result[i] = edit.name;
            i++;
        }
        return result;
    }

    protected void editSingleTarget(int windowID)
    {
        if (GUI.Button(closeButtonRect, "X"))
        {
            if (selectedSingleTarget == windows[windowID].objectIndex) selectedSingleTarget = -1;
            windows.RemoveAt(windowID);
            return;
        }
        WindowSettings window = windows[windowID];
        SingleTargetEffectEdit data = singletargets[window.objectIndex];
        if (data == null)
        {
            windows.RemoveAt(windowID);
        }

        EditorGUIUtility.labelWidth = 120;

        window.windowScroll = GUILayout.BeginScrollView(window.windowScroll, false, true);
        
        editEffect(data);

        GUILayout.EndScrollView();
        GUI.DragWindow();
    }

    protected void editAreaOfEffect(int windowID)
    {
        if (GUI.Button(closeButtonRect, "X"))
        {
            if (selectedAreaOfEffect == windows[windowID].objectIndex) selectedAreaOfEffect = -1;
            windows.RemoveAt(windowID);
            return;
        }
        WindowSettings window = windows[windowID];
        AreaOfEffectEdit data = areaofeffects[window.objectIndex];
        if (data == null)
        {
            windows.RemoveAt(windowID);
        }

        EditorGUIUtility.labelWidth = 120;

        window.windowScroll = GUILayout.BeginScrollView(window.windowScroll, false, true);

        editEffect(data);

        GUILayout.Space(20);
        EditorGUILayout.LabelField("Radius:", EditorStyles.boldLabel);
        data.radius = EditorGUILayout.FloatField(data.radius);

        GUILayout.EndScrollView();
        GUI.DragWindow();
    }

    protected void editProjectileEffect(int windowID)
    {
        if (GUI.Button(closeButtonRect, "X"))
        {
            if (selectedProjectile == windows[windowID].objectIndex) selectedProjectile = -1;
            windows.RemoveAt(windowID);
            return;
        }
        WindowSettings window = windows[windowID];
        ProjectileEffectEdit data = projectileeffects[window.objectIndex];
        if (data == null)
        {
            windows.RemoveAt(windowID);
        }

        EditorGUIUtility.labelWidth = 120;

        window.windowScroll = GUILayout.BeginScrollView(window.windowScroll, false, true);
        editEffect(data);

        GUILayout.Space(20);
        EditorGUILayout.LabelField("Projectile Stuff: ", EditorStyles.boldLabel);
        EditorGUIUtility.labelWidth = 180;
        data.weaponProjectile = EditorGUILayout.Toggle("Use weapon projectile", data.weaponProjectile);
        EditorGUIUtility.labelWidth = 120;
        if(!data.weaponProjectile) data.projectileName = EditorGUILayout.TextField("ProjectileName: ", data.projectileName);
        data.angle = EditorGUILayout.FloatField("Angle: ", data.angle);

        GUILayout.EndScrollView();
        GUI.DragWindow();
    }

    protected void editMovementEffect(int windowID)
    {
        if (GUI.Button(closeButtonRect, "X"))
        {
            if (selectedMovement == windows[windowID].objectIndex) selectedMovement = -1;
            windows.RemoveAt(windowID);
            return;
        }
        WindowSettings window = windows[windowID];
        MovementEffectEdit data = movementeffects[window.objectIndex];
        if (data == null)
        {
            windows.RemoveAt(windowID);
        }

        EditorGUIUtility.labelWidth = 120;

        window.windowScroll = GUILayout.BeginScrollView(window.windowScroll, false, true);
        editEffect(data);

        GUILayout.Space(20);
        EditorGUILayout.LabelField("Movement Stuff: ", EditorStyles.boldLabel);
        EditorGUIUtility.labelWidth = 180;
        data.range = EditorGUILayout.FloatField("Range: ", data.range);
        EditorGUIUtility.labelWidth = 120;
        data.travelTime = EditorGUILayout.FloatField("Travel Time: ", data.travelTime);

        GUILayout.EndScrollView();
        GUI.DragWindow();
    }

    protected void editSelfBuffEffect(int windowID)
    {
        if (GUI.Button(closeButtonRect, "X"))
        {
            if (selectedSelfBuff == windows[windowID].objectIndex) selectedSelfBuff = -1;
            windows.RemoveAt(windowID);
            return;
        }
        WindowSettings window = windows[windowID];
        SelfBuffEffectEdit data = selfbuffeffects[window.objectIndex];
        if(data == null)
        {
            windows.RemoveAt(windowID);
        }

        EditorGUIUtility.labelWidth = 120;

        window.windowScroll = GUILayout.BeginScrollView(window.windowScroll, false, true);
        editEffect(data);

        GUILayout.EndScrollView();
        GUI.DragWindow();
        
    }

    private void editEffect(AbilityEffectEdit data)
    {
        data.name = EditorGUILayout.TextField("Name: ", data.name);
        data.gameName = EditorGUILayout.TextField("Game Name: ", data.gameName);
        data.modelName = EditorGUILayout.TextField("Model Name: ", data.modelName);

        GUILayout.Space(20);
        EditorGUILayout.LabelField("Hit Damage:", EditorStyles.boldLabel);
        foreach (HitDamageEdit edit in data.hitDamages)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUIUtility.labelWidth = 100;

            EditorGUIUtility.fieldWidth = 40;
            edit.stat = (Stat)EditorGUILayout.EnumPopup(edit.stat);
            EditorGUIUtility.fieldWidth = 10;

            edit.percent = EditorGUILayout.FloatField(edit.percent);
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.BeginHorizontal();
            EditorGUIUtility.fieldWidth = 10;
            EditorGUIUtility.labelWidth = 70;
            edit.yourStat = EditorGUILayout.Toggle("Your stat: ", edit.yourStat);
            EditorGUIUtility.fieldWidth = 10;
            edit.damageSelf = EditorGUILayout.Toggle("To self: ", edit.damageSelf);


            if (GUILayout.Button("-"))
            {
                data.hitDamages.Remove(edit);
                break;
            }
            EditorGUILayout.EndHorizontal();
        }
        if (GUILayout.Button("+Damage"))
        {
            data.hitDamages.Add(new HitDamageEdit());
        }

        GUILayout.Space(20);
        EditorGUILayout.LabelField("Hit Buff:", EditorStyles.boldLabel);
        foreach(HitBuffEdit edit in data.hitBuffs)
        {
            EditorGUIUtility.labelWidth = 100;
            edit.type = (BuffType)EditorGUILayout.EnumPopup("Buff Type: ", edit.type);
            EditorGUILayout.BeginHorizontal();
            EditorGUIUtility.fieldWidth = 40;
            edit.stat = (Stat)EditorGUILayout.EnumPopup(edit.stat);
            EditorGUIUtility.fieldWidth = 10;

            edit.percent = EditorGUILayout.Toggle("Percent: ", edit.percent);
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.BeginHorizontal();
            EditorGUIUtility.fieldWidth = 10;
            EditorGUIUtility.labelWidth = 70;
            edit.amount = EditorGUILayout.FloatField("Amount: ", edit.amount);
            EditorGUIUtility.fieldWidth = 10;
            edit.duration = EditorGUILayout.FloatField("Duration: ", edit.duration);


            if (GUILayout.Button("-"))
            {
                data.hitBuffs.Remove(edit);
                break;
            }
            EditorGUILayout.EndHorizontal();
        }
        if (GUILayout.Button("+Buff"))
        {
            data.hitBuffs.Add(new HitBuffEdit());
        }
        
    }

    

    protected override GUI.WindowFunction getWindowFunc(WindowSettings window)
    {
        if (window.objectListIndex == SINGLETARGET) return editSingleTarget;
        if (window.objectListIndex == AREAOFEFFECT) return editAreaOfEffect;
        if (window.objectListIndex == PROJECTILEEFFECT) return editProjectileEffect;
        if (window.objectListIndex == MOVEMENTEFFECT) return editMovementEffect;
        if (window.objectListIndex == SELFBUFFEFFECT) return editSelfBuffEffect;
        return null;
    }

    protected override ObjectEdit getObjectEdit(int listIndex, int index)
    {
        switch (listIndex)
        {
            case (SINGLETARGET):
                return singletargets[index];
            case (AREAOFEFFECT):
                return areaofeffects[index];
            case(PROJECTILEEFFECT):
                return projectileeffects[index];
            case(MOVEMENTEFFECT):
                return movementeffects[index];
            case(SELFBUFFEFFECT):
                return selfbuffeffects[index];
            default:
                return null;
        }
    }

    protected override bool indexOutOfBounds(int listIndex, int index)
    {
        switch (listIndex)
        {
            case (SINGLETARGET):
                return index >= singletargets.Count;
            case (AREAOFEFFECT):
                return index >= areaofeffects.Count;
            case(PROJECTILEEFFECT):
                return index >= projectileeffects.Count;
            case(MOVEMENTEFFECT):
                return index >= movementeffects.Count;
            case (SELFBUFFEFFECT):
                return index >= selfbuffeffects.Count;
            default:
                return true;
        }
    }

    protected override void deleteSelected()
    {
        if (selectedSingleTarget >= 0)
        {
            deleteWindowIndex = selectedSingleTarget;
            deleteWindowListIndex = SINGLETARGET;
            GUI.BringWindowToFront(DELETEWINDOWID);
        }
        else if (selectedAreaOfEffect >= 0)
        {
            deleteWindowIndex = selectedAreaOfEffect;
            deleteWindowListIndex = AREAOFEFFECT;
            GUI.BringWindowToFront(DELETEWINDOWID);
        }
        else if (selectedProjectile >= 0)
        {
            deleteWindowIndex = selectedProjectile;
            deleteWindowListIndex = PROJECTILEEFFECT;
            GUI.BringWindowToFront(DELETEWINDOWID);
        }
        else if (selectedMovement >= 0)
        {
            deleteWindowIndex = selectedMovement;
            deleteWindowListIndex = MOVEMENTEFFECT;
            GUI.BringWindowToFront(DELETEWINDOWID);
        }
        else if (selectedSelfBuff >= 0)
        {
            deleteWindowIndex = selectedSelfBuff;
            deleteWindowListIndex = SELFBUFFEFFECT;
            GUI.BringWindowToFront(DELETEWINDOWID);
        }
       
    }

    protected override void delete(int listIndex, int index)
    {
        if (!indexOutOfBounds(listIndex, index))
        {
            switch (listIndex)
            {
                case (SINGLETARGET):
                    closeWindow(SINGLETARGET, index);
                    singletargets.RemoveAt(index);
                    if (selectedSingleTarget > 0) selectedSingleTarget--;
                    break;
                case (AREAOFEFFECT):
                    closeWindow(AREAOFEFFECT, index);
                    areaofeffects.RemoveAt(index);
                    if (selectedAreaOfEffect > 0) selectedAreaOfEffect--;
                    break;
                case(PROJECTILEEFFECT):
                    closeWindow(PROJECTILEEFFECT, index);
                    projectileeffects.RemoveAt(index);
                    if (selectedProjectile > 0) selectedProjectile--;
                    break;
                case (MOVEMENTEFFECT):
                    closeWindow(MOVEMENTEFFECT, index);
                    movementeffects.RemoveAt(index);
                    if (selectedMovement > 0) selectedMovement--;
                    break;
                case(SELFBUFFEFFECT):
                    closeWindow(SELFBUFFEFFECT, index);
                    selfbuffeffects.RemoveAt(index);
                    if (selectedSelfBuff > 0) selectedSelfBuff--;
                    break;
            }
        }
    }

    private void duplicate()
    {
        if (selectedSingleTarget >= 0)
        {
            SingleTargetEffectEdit temp = new SingleTargetEffectEdit(singletargets[selectedSingleTarget]);
            temp.name += "(Copy)";
            temp.gameName += "(Copy)";
            singletargets.Insert(selectedSingleTarget + 1, temp);
            selectedSingleTarget++;
        }
        else if (selectedAreaOfEffect >= 0)
        {
            AreaOfEffectEdit temp = new AreaOfEffectEdit(areaofeffects[selectedAreaOfEffect]);
            temp.name += "(Copy)";
            temp.gameName += "(Copy)";
            areaofeffects.Insert(selectedAreaOfEffect + 1, temp);
            selectedAreaOfEffect++;
        }
        else if (selectedProjectile >= 0)
        {
            ProjectileEffectEdit temp = new ProjectileEffectEdit(projectileeffects[selectedProjectile]);
            temp.name += "(Copy)";
            temp.gameName += "(Copy)";
            projectileeffects.Insert(selectedProjectile + 1, temp);
            selectedProjectile++;
        }
        else if (selectedMovement >= 0)
        {
            MovementEffectEdit temp = new MovementEffectEdit(movementeffects[selectedMovement]);
            temp.name += "(Copy)";
            temp.gameName += "(Copy)";
            movementeffects.Insert(selectedMovement + 1, temp);
            selectedMovement++;
        }
        else if (selectedSelfBuff >= 0)
        {
            SelfBuffEffectEdit temp = new SelfBuffEffectEdit(selfbuffeffects[selectedSelfBuff]);
            temp.name += "(Copy)";
            temp.gameName += "(Copy)";
            selfbuffeffects.Insert(selectedSelfBuff + 1, temp);
            selectedSelfBuff++;
        }
    }

    private void moveUp()
    {
        if (selectedSingleTarget > 0)
        {
            SingleTargetEffectEdit temp = singletargets[selectedSingleTarget];
            singletargets.RemoveAt(selectedSingleTarget);
            singletargets.Insert(selectedSingleTarget - 1, temp);
            selectedSingleTarget -= 1;

        }
        else if (selectedAreaOfEffect > 0)
        {
            AreaOfEffectEdit temp = areaofeffects[selectedAreaOfEffect];
            areaofeffects.RemoveAt(selectedAreaOfEffect);
            areaofeffects.Insert(selectedAreaOfEffect - 1, temp);
            selectedAreaOfEffect -= 1;
        }
        else if(selectedProjectile > 0)
        {
            ProjectileEffectEdit temp = projectileeffects[selectedProjectile];
            projectileeffects.RemoveAt(selectedProjectile);
            projectileeffects.Insert(selectedProjectile - 1, temp);
            selectedProjectile -= 1;
        }
        else if (selectedMovement > 0)
        {
            MovementEffectEdit temp = movementeffects[selectedMovement];
            movementeffects.RemoveAt(selectedMovement);
            movementeffects.Insert(selectedMovement - 1, temp);
            selectedMovement -= 1;
        }
        else if (selectedSelfBuff > 0)
        {
            SelfBuffEffectEdit temp = selfbuffeffects[selectedSelfBuff];
            selfbuffeffects.RemoveAt(selectedSelfBuff);
            selfbuffeffects.Insert(selectedSelfBuff - 1, temp);
            selectedSelfBuff -= 1;
        }
    }

    private void moveDown()
    {
        if (selectedSingleTarget >= 0 && selectedSingleTarget < singletargets.Count - 1)
        {
            SingleTargetEffectEdit temp = singletargets[selectedSingleTarget];
            singletargets.RemoveAt(selectedSingleTarget);
            singletargets.Insert(selectedSingleTarget + 1, temp);
            selectedSingleTarget += 1;
        }
        else if (selectedAreaOfEffect >= 0 && selectedAreaOfEffect < areaofeffects.Count - 1)
        {
            AreaOfEffectEdit temp = areaofeffects[selectedAreaOfEffect];
            areaofeffects.RemoveAt(selectedAreaOfEffect);
            areaofeffects.Insert(selectedAreaOfEffect + 1, temp);
            selectedAreaOfEffect += 1;
        }
        else if(selectedProjectile >= 0 && selectedProjectile < projectileeffects.Count - 1)
        {
            ProjectileEffectEdit temp = projectileeffects[selectedProjectile];
            projectileeffects.RemoveAt(selectedProjectile);
            projectileeffects.Insert(selectedProjectile + 1, temp);
            selectedProjectile += 1;
        }
        else if (selectedMovement >= 0 && selectedMovement < movementeffects.Count - 1)
        {
            MovementEffectEdit temp = movementeffects[selectedMovement];
            movementeffects.RemoveAt(selectedMovement);
            movementeffects.Insert(selectedMovement + 1, temp);
            selectedMovement += 1;
        }
        else if (selectedSelfBuff >= 0 && selectedSelfBuff < selfbuffeffects.Count - 1)
        {
            SelfBuffEffectEdit temp = selfbuffeffects[selectedSelfBuff];
            selfbuffeffects.RemoveAt(selectedSelfBuff);
            selfbuffeffects.Insert(selectedSelfBuff + 1, temp);
            selectedSelfBuff += 1;
        }
    }
}
