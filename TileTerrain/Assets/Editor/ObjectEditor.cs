using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using Edit;
using System;

public abstract class ObjectEditor : EditorWindow {

    protected List<WindowSettings> windows = new List<WindowSettings>();

    public static readonly Rect closeButtonRect = new Rect(278, 0, 18, 18);

    protected int deleteWindowIndex = -1;
    protected int deleteWindowListIndex = -1;

    public const int DELETEWINDOWID = 1000;


    protected string filePath;


    protected virtual void OnGUI()
    {
        BeginWindows();
        for (int i = 0; i < windows.Count; i++)
        {
            WindowSettings window = windows[i];
            if (indexOutOfBounds(window.objectListIndex, window.objectIndex))
            {
                Debug.Log("Remove Window");
                windows.RemoveAt(i);
                i--;
                continue;
            }
            else if (window.shouldReload())
            {
                Debug.Log("Reload window");
                GUI.WindowFunction func = getWindowFunc(window);
                if (func == null)
                {
                    Debug.Log("Func is null");
                    windows.RemoveAt(i);
                    i--;
                    continue;
                }

                Debug.Log("window reloaded");
                window.windowFunc = func;
            }
            Rect rect = window.windowRect;


            ObjectEdit edit = getObjectEdit(window.objectListIndex, window.objectIndex);
            rect = GUI.Window(i, rect, window.windowFunc, edit.windowTitle());
            rect = new Rect(Mathf.Clamp(rect.x, 160, this.position.width-150), 10, rect.width, this.position.height - 160);
            window.windowRect = rect;
        }
        if (!indexOutOfBounds(deleteWindowListIndex, deleteWindowIndex)) GUI.Window(DELETEWINDOWID, new Rect(200, this.position.height - 150, 200, 100), deleteWindow, "Delete");
        EndWindows();

        if (GUI.Button(new Rect(150, this.position.height - 40, 50, 30), "SAVE"))
        {
            saveFile();
        }
        if (GUI.Button(new Rect(210, this.position.height - 40, 70, 30), "SAVE AS"))
        {
            saveAsFile();
        }
        if (GUI.Button(new Rect(290, this.position.height - 40, 50, 30), "LOAD"))
        {
            loadFile();
        }

        if(filePath != null) GUI.Label(new Rect(350, this.position.height - 35, 800, 20), "Filepath: " + filePath);

    }

    private void deleteWindow(int windowID)
    {
        ObjectEdit edit = getObjectEdit(deleteWindowListIndex, deleteWindowIndex);
        GUILayout.Label("Are you sure you want to\nremove " + edit.gameName + "?");
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("YES!"))
        {
            delete(deleteWindowListIndex, deleteWindowIndex);
            deleteWindowIndex = -1;
            deleteWindowListIndex = -1;
        }
        if (GUILayout.Button("NO!"))
        {
            deleteWindowIndex = -1;
            deleteWindowListIndex = -1;
        }
        GUILayout.EndHorizontal();
    }

    protected bool hasWindow(int listIndex, int index)
    {
        foreach (WindowSettings window in windows)
        {
            if (window.objectListIndex == listIndex && window.objectIndex == index)
            {
                return true;
            }
        }
        return false;
    }

    protected bool openWindow(int listIndex, int index)
    {
        for (int i = 0; i < windows.Count; i++)
        {
            WindowSettings window = windows[i];
            if (window.objectListIndex == listIndex && window.objectIndex == index)
            {
                GUI.BringWindowToFront(i);
                return true;
            }
        }
        return false;
    }

    protected bool closeWindow(int listIndex, int index)
    {
        for (int i = 0; i < windows.Count; i++)
        {
            WindowSettings window = windows[i];
            if (window.objectListIndex == listIndex && window.objectIndex == index)
            {
                windows.RemoveAt(i);
                return true;
            }
        }
        return false;
    }
    

    protected static string TextField(string label, string text)
    {
        return EditorGUILayout.TextField(label, text);
    }

    protected static int IntField(string label, int number)
    {
        return EditorGUILayout.IntField(label, number);
    }

    protected static float FloatField(string label, float number)
    {
        return EditorGUILayout.FloatField(label, number);
    }

    protected abstract GUI.WindowFunction getWindowFunc(WindowSettings window);

    protected abstract ObjectEdit getObjectEdit(int listIndex, int index);

    protected abstract bool indexOutOfBounds(int listIndex, int index);

    protected virtual void delete(int listIndex, int index)
    { 
    }
    protected virtual void deleteSelected()
    {
    }


    [Serializable]
    protected class WindowSettings
    {
        public int objectListIndex;
        public int objectIndex;

        public GUI.WindowFunction windowFunc;

        public Rect windowRect;

        public Vector2 windowScroll;
        public bool shouldReload()
        {
            return (windowFunc == null);
        }
    }

    protected virtual void loadFile()
    { 
    }

    protected virtual void saveFile()
    { 
    }
    protected virtual void saveAsFile()
    {
        filePath = null;
        filePath = EditorUtility.SaveFilePanel("Save as", Application.dataPath + "/Resources/Data", "", "xml");
        if (filePath != null)
        {
            saveFile();
        }
    }

}
