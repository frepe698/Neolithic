using UnityEngine;
using System.Collections;

public class DayTime : TimeSetting {

    public DayTime(int length = 120)
        : base(length,
        new Color(0.5f, 0.5f, 0.45f),
        new Color(0.5f, 0.5f, 0.45f),
        new Vector3(70, 20, 0),
        new Vector3(110, 20, 0),
        new Color(0, 0, 0),
        new Color(0, 0, 0),
        new Vector3(50, 0, 50),
        new Vector3(50, 0, 50),
        14)
    {

    }

    public override void start()
    {
        base.start();
        GameMaster.allNightSpawnersRemoveUnits();
        GameMaster.respawnAllDaySpawners();
        //GameMaster.requestLaneSpawningStart();
        
    }

    public override float getTimeOfDay()
    {
        return base.getTimeOfDay() / 3.0f + 1.0f / 6.0f;
    }
	
}
