using UnityEngine;
using System.Collections;

public class DawnTime : TimeSetting {

    public DawnTime()
        : base(60,
        new Color(0,0,0),
        new Color(0.6f, 0.6f, 0.4f), 
        new Vector3(25, 0, 0), 
        new Vector3(55, 0, 0), 
        new Color(0,0,0),
        new Vector3(50, 0, 50),
        new Vector3(50, 0, 50))
    {

    }
}
