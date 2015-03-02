using UnityEngine;
using System.Collections;

public class NightTime : TimeSetting {

    public NightTime()
        : base(10,
        new Color(0,0,0),
        new Color(0,0,0),
        new Vector3(270, 25, 0),
        new Vector3(270, 25, 0), 
        new Color(0.1f, 0.1f, 0.2f), 
        new Vector3(50, 0, 50), 
        new Vector3(50, 0, 50))
    {

    }

    public override void start()
    {
        base.start();
        GameMaster.respawnAllSpawners();
    }
}
