using UnityEngine;
using System.Collections;

public class DuskTime : TimeSetting {

    public DuskTime(int length = 60)
        : base(length,
        new Color(0.6f, 0.5f, 0.3f),
        new Color(0,0,0),
        new Vector3(125, 25, 0),
        new Vector3(155, 25, 0), 
        new Color(0,0,0),
        new Color(0.1f, 0.1f, 0.2f), 
        new Vector3(50, 0, 50),
        new Vector3(50, 0, 50))
    {

    }
	
}
