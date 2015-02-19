using UnityEngine;
using System.Collections;

public class NightTime : TimeSetting {

    public NightTime()
        : base(15, 
        new Color(0.0f, 0.0f, 0.0f),
        new Vector3(270, 0, 0),
        new Vector3(25, 0, 0), 
        new Color(0.00f, 0.00f, 0.0f), 
        new Vector3(50, 0, 50), 
        new Vector3(90, 0, 90))
    {

    }
}
