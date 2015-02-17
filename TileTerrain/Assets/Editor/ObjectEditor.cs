using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;
using System.IO;
using Edit;

public class ObjectEditor : EditorWindow {

    protected List<WindowSettings> windows = new List<WindowSettings>();

    public static readonly Rect closeButtonRect = new Rect(278, 0, 18, 18);


    protected virtual void OnGUI()
    {
        BeginWindows();
        for (int i = 0; i < windows.Count; i++)
        {
            WindowSettings window = windows[i];
            Rect rect = window.windowRect;
            
            rect = GUI.Window(i, rect, window.windowFunc, window.windowObject.windowTitle());
            rect = new Rect(Mathf.Clamp(rect.x, 160, this.position.width-150), 10, rect.width, this.position.height - 60);
            window.windowRect = rect;
        }
        
        EndWindows();

        if (GUI.Button(new Rect(200, this.position.height - 40, 50, 30), "SAVE"))
        {
            saveFile();
        }
        if (GUI.Button(new Rect(260, this.position.height - 40, 50, 30), "LOAD"))
        {
            loadFile();
        }
    }

    protected bool hasWindow(ObjectEdit objectEdit)
    {
        foreach (WindowSettings window in windows)
        {
            if (window.windowObject.Equals(objectEdit))
            {
                return true;
            }
        }
        return false;
    }

    protected bool openWindow(ObjectEdit objectEdit)
    {
        for (int i = 0; i < windows.Count; i++)
        {
            WindowSettings window = windows[i];
            if (window.windowObject.Equals(objectEdit))
            {
                GUI.BringWindowToFront(i);
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

    protected class WindowSettings
    {
        public ObjectEdit windowObject;

        public GUI.WindowFunction windowFunc;

        public Rect windowRect;

        public Vector2 windowScroll;
    }

    protected virtual void loadFile()
    { 
    }

    protected virtual void saveFile()
    { 
    }


}
