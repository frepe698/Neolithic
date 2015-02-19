using UnityEngine;
using System.Collections;

public class TimeManager{

    private readonly static int DAY = 0;
    private readonly static int DUSK = 1;
    private readonly static int NIGHT = 2;
    private readonly static int DAWN = 3;

    private static TimeSetting[] times = new TimeSetting[]{new DayTime(), new DuskTime(), new NightTime(), new DuskTime()};
    private TimeSetting currentTime;
    private int currentTimeIndex = 0;
    private int nextTimeIndex = 1;

    private readonly float intermissionTime = 5;
    private float intermissionTimer;

    private Vector3 intermissionSunSpeed;
    private Vector3 intermissionMoonSpeed;
    private Color intermissionSunLightChange;
    private Color intermissionMoonLightChange;

    
    private static object syncRoot = new System.Object();
    private static volatile TimeManager instance;

    private Light sun;
    private Light moon;
    
    public TimeManager()
    {
        currentTime = times[0];
        currentTime.start();
        sun = new GameObject().AddComponent<Light>();
        sun.transform.eulerAngles = currentTime.getSunRotation();
        sun.color = currentTime.getSunLightColor();
        sun.type = LightType.Directional;
        sun.shadows = LightShadows.Hard;
        sun.shadowStrength = 0.4f;
        
        moon = new GameObject().AddComponent<Light>();
        moon.transform.eulerAngles = currentTime.getMoonRotation();
        moon.color = currentTime.getMoonLightColor();
        moon.type = LightType.Directional;
        moon.shadows = LightShadows.Hard;
        moon.shadowStrength = 0.4f;

        updateIntermissionValues();
    }
    
    

    public void update()
    {
        currentTime.update();
        if(currentTime.isAlive())
        {
            
            //set sun and moon properties
            sun.color = currentTime.getSunLightColor();
            moon.color = currentTime.getMoonLightColor();

            sun.transform.eulerAngles = currentTime.getSunRotation();
            moon.transform.eulerAngles = currentTime.getMoonRotation();
        }
        else 
        { 
            if(intermissionTimer < intermissionTime)
            {
                intermissionTimer += Time.deltaTime;
                //interpolate sun and moon properties

                sun.color += intermissionSunLightChange*Time.deltaTime;
                moon.color += intermissionMoonLightChange*Time.deltaTime;

                sun.transform.eulerAngles += intermissionSunSpeed*Time.deltaTime;
                moon.transform.eulerAngles += intermissionMoonSpeed*Time.deltaTime;
            }
            else 
            {
                Debug.Log("new light");
                intermissionTimer = 0;
                currentTimeIndex = (currentTimeIndex + 1) % 4;
                nextTimeIndex = (currentTimeIndex + 1) % 4;
                currentTime = times[currentTimeIndex];
                currentTime.start();
                updateIntermissionValues();

                
            }
        }
    }

    public void updateIntermissionValues()
    {
        intermissionSunSpeed = (times[nextTimeIndex].getSunStartRotation() - times[currentTimeIndex].getSunEndRotation()) / intermissionTime;
        intermissionMoonSpeed = (times[nextTimeIndex].getMoonStartRotation() - times[currentTimeIndex].getMoonEndRotation()) / intermissionTime;
        Debug.Log(intermissionSunSpeed);
        intermissionSunLightChange = (times[nextTimeIndex].getSunLightColor() - times[currentTimeIndex].getSunLightColor()) / intermissionTime;
        intermissionMoonLightChange = (times[nextTimeIndex].getMoonLightColor() - times[currentTimeIndex].getMoonLightColor()) / intermissionTime;
                
    }
    public static TimeManager Instance
    {
        get
        {
            //check to see if it doesnt exist
            if (instance == null)
            {
                //lock access, if it is already locked, wait.
                lock (syncRoot)
                {
                    //the instance could have been made between
                    //checking and waiting for a lock to release.
                    if (instance == null)
                    {
                        //create a new instance
                        instance = new TimeManager();
                    }
                }
            }
            //return either the new instance or the already built one.
            return instance;
        }
    }
	
}
