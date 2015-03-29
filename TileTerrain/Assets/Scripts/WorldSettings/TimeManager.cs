using UnityEngine;
using System.Collections;

public class TimeManager{

    private readonly static int DAY = 0;
    private readonly static int DUSK = 1;
    private readonly static int NIGHT = 2;
    private readonly static int DAWN = 3;

    private readonly static float TIMESPEED = 1;

    private static TimeSetting[] times = new TimeSetting[]{new DayTime(12), new DuskTime(6), new NightTime(12), new DawnTime(6)};
    private TimeSetting currentTime;
    private int currentTimeIndex = 0;
    private int nextTimeIndex = 1;

    private readonly float intermissionTime = 10;
    private float intermissionTimer;

    private Vector3 intermissionSunSpeed;
    private Vector3 intermissionMoonSpeed;
    private Color intermissionSunLightChange;
    private Color intermissionMoonLightChange;

    private Vector3 sunRotation;
    private Vector3 moonRotation;

    private Color sunColor;
    private Color moonColor;

    private static readonly Color black = new Color(0, 0, 0);

    private static object syncRoot = new System.Object();
    private static volatile TimeManager instance;

    private Light sun;
    private Light moon;

    private bool inDoors = false;
    public TimeManager()
    {
        currentTime = times[DAY];
        currentTime.start();
        sun = new GameObject("sun").AddComponent<Light>();
        sunRotation = currentTime.getSunRotation();
        sunColor = currentTime.getSunLightColor();
        sun.color = sunColor;
        sun.type = LightType.Directional;
        sun.shadows = LightShadows.Hard;
        sun.shadowStrength = 0.4f;
        sun.intensity = 1.5f;
        
        moon = new GameObject("moon").AddComponent<Light>();
        moonRotation = currentTime.getMoonRotation();
        moonColor = currentTime.getMoonLightColor();
        moon.color = moonColor;
        moon.type = LightType.Directional;
        moon.shadows = LightShadows.Hard;
        moon.shadowStrength = 0.4f;
        moon.intensity = 1.5f;
        updateIntermissionValues();
    }
    
    

    public void update()
    {
        sun.transform.eulerAngles = sunRotation;
        moon.transform.eulerAngles = moonRotation;

        float deltaTime = Time.deltaTime * TIMESPEED;
        currentTime.update(deltaTime);
        if(currentTime.isAlive())
        {
            
            //set sun and moon properties
            sunColor = currentTime.getSunLightColor();
            moonColor = currentTime.getMoonLightColor();

            sunRotation = currentTime.getSunRotation();
            moonRotation = currentTime.getMoonRotation();
        }
        else 
        { 
            if(intermissionTimer < intermissionTime)
            {
               
                intermissionTimer += deltaTime;
                //interpolate sun and moon properties
                sunColor += intermissionSunLightChange * deltaTime;
                moonColor += intermissionMoonLightChange * deltaTime;

                sunRotation += intermissionSunSpeed * deltaTime;
                sun.transform.eulerAngles = sunRotation;

                moonRotation += intermissionMoonSpeed * deltaTime;
                moon.transform.eulerAngles = moonRotation;
            }
            else 
            {
                intermissionTimer = 0;

                currentTimeIndex = (currentTimeIndex + 1) % 4;
                nextTimeIndex = (currentTimeIndex + 1) % 4;
                currentTime = times[currentTimeIndex];
                currentTime.start();
                updateIntermissionValues();
                
                
            }
        }
        if(!inDoors)
        {
            sun.color = sunColor;
            moon.color = moonColor;
        }
        else 
        {
            sun.color = black;
            moon.color = black;
        }

    }

    public void updateIntermissionValues()
    {
        intermissionSunSpeed = (times[nextTimeIndex].getSunStartRotation() - times[currentTimeIndex].getSunEndRotation()) / intermissionTime;
        intermissionMoonSpeed = (times[nextTimeIndex].getMoonStartRotation() - times[currentTimeIndex].getMoonEndRotation()) / intermissionTime;
        intermissionSunLightChange = (times[nextTimeIndex].getSunStartColor() - times[currentTimeIndex].getSunEndColor()) / intermissionTime;
        intermissionMoonLightChange = (times[nextTimeIndex].getMoonStartColor() - times[currentTimeIndex].getMoonEndColor()) / intermissionTime;
                
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

    public static void removeInstance()
    {
        instance = null;
    }

    public void setIndoors(bool indoors)
    {
        inDoors = indoors;
    }

    public void toggleIndoors()
    {
        inDoors = !inDoors;
    }
	
}
