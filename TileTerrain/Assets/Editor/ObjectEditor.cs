using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;
using System.IO;

public class ObjectEditor : EditorWindow {

    protected List<WindowSettings> windows = new List<WindowSettings>();



    protected virtual void OnGUI()
    {
        BeginWindows();
        for (int i = 0; i < windows.Count; i++)
        {
            WindowSettings window = windows[i];
            Rect rect = window.windowRect;
            
            rect = GUI.Window(i, rect, window.windowFunc, window.windowObject.name);
            rect = new Rect(Mathf.Clamp(rect.x, 160, this.position.width-150), 10, rect.width, this.position.height - 60);
            window.windowRect = rect;
        }
        
        EndWindows();


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

    protected abstract class ObjectEdit
    {
        public string name;
        public string gameName;

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


}
