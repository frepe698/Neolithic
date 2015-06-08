﻿using UnityEngine;
using System.Collections;

public class NightTime : TimeSetting {

    public NightTime(int length = 120)
        : base(length,
        new Color(0,0,0),
        new Color(0,0,0),
        new Vector3(270, 25, 0),
        new Vector3(270, 25, 0),
        new Color(0.1f, 0.1f, 0.2f),
        new Color(0.1f, 0.1f, 0.2f), 
        new Vector3(50, 0, 50), 
        new Vector3(50, 0, 50),
        -4)
    {

    }

    public override void start()
    {
        base.start();
        GameMaster.respawnAllNightSpawners();
        GameMaster.requestLaneSpawningStart();
    }

    public override float getTimeOfDay()
    {
        return base.getTimeOfDay() / 3.0f + 4.0f / 6.0f;
    }
}
