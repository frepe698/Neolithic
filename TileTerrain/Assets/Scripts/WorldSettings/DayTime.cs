using UnityEngine;
using System.Collections;

public class DayTime : TimeSetting {

    public DayTime()
        : base(120,
        new Color(0.65f, 0.65f, 0.5f),
        new Color(0.65f, 0.65f, 0.5f),
        new Vector3(70, 0, 0),
        new Vector3(110, 0, 0), 
        new Color(0,0,0), 
        new Vector3(50, 0, 50),
        new Vector3(90, 0, 90))
    {

    }
	
}
