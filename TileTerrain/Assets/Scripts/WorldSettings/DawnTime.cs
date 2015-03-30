﻿using UnityEngine;
using System.Collections;

public class DawnTime : TimeSetting {

    public DawnTime(int length = 60)
        : base(length,
        new Color(0,0,0),
        new Color(0.6f, 0.5f, 0.4f), 
        new Vector3(25, 25, 0), 
        new Vector3(55, 25, 0),
        new Color(0.1f, 0.1f, 0.2f), 
        new Color(0,0,0),
        new Vector3(50, 0, 50),
        new Vector3(50, 0, 50))
    {

    }

    public override void start()
    {
        base.start();
        GameMaster.getGameController().requestAILevelIncrease();
        TimeManager.Instance.addDay();
    }

    public override float getTimeOfDay()
    {
        return base.getTimeOfDay()/6.0f;
    }
}
