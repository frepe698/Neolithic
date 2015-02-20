﻿using UnityEngine;
using System.Collections;

public class TimeSetting  {


    private readonly float lifeSpan;
    private float elapsedTime;

    private Color sunStartLightColor;
    private Color sunEndLightColor;
    private Color sunLightColor;
    private Color sunLightChange;

    private Vector3 sunStartRotation;
    private Vector3 sunEndRotation;
    private Vector3 sunDirection;
    private Vector3 sunRotation;

    private Color moonLightColor;
    private Vector3 moonStartRotation;
    private Vector3 moonEndRotation;
    private Vector3 moonDirection;
    private Vector3 moonRotation;

    private bool alive = false;

    public TimeSetting(float lifeSpan, Color sunStartLightColor, Color sunEndLightColor, Vector3 sunStartRotation, Vector3 sunEndRotation, Color moonLightColor, Vector3 moonStartRotation, Vector3 moonEndRotation)
    {
        this.lifeSpan = lifeSpan;

        this.sunStartLightColor = sunStartLightColor;
        this.sunEndLightColor = sunEndLightColor;
        this.sunLightChange = (sunEndLightColor - sunStartLightColor) / lifeSpan;

        this.sunStartRotation = sunStartRotation;
        this.sunEndRotation = sunEndRotation;
        this.sunDirection = (sunEndRotation - sunStartRotation) / lifeSpan;

        this.moonLightColor = moonLightColor;
        this.moonStartRotation = moonStartRotation;
        this.moonEndRotation = moonEndRotation;
        this.moonDirection = (moonEndRotation - moonStartRotation) / lifeSpan;
    }

    public void update(float deltaTime)
    {
        if(elapsedTime >= lifeSpan)
        {
            alive = false;
            return;
        }

        elapsedTime += deltaTime;
        this.sunRotation += sunDirection * deltaTime;
        this.moonRotation += moonDirection * deltaTime;
        this.sunLightColor += sunLightChange * deltaTime;
    }
    public void start()
    {
        this.elapsedTime = 0;
        this.sunRotation = sunStartRotation;
        this.moonRotation = moonStartRotation;
        this.sunLightColor = sunStartLightColor;
        this.alive = true;
    }



    public Color getSunLightColor()
    {
        return this.sunLightColor;
    }

    public Color getSunStartColor()
    {
        return this.sunStartLightColor;
    }

    public Color getSunEndColor()
    {
        return this.sunEndLightColor;
    }

    public Vector3 getSunRotation()
    {
        return this.sunRotation;
    }

    public Vector3 getSunStartRotation()
    {
        return this.sunStartRotation;
    }

    public Vector3 getSunEndRotation()
    {
        return this.sunEndRotation;
    }


    public Color getMoonLightColor()
    {
        return this.moonLightColor;
    }

    public Vector3 getMoonRotation()
    {
        return this.moonRotation;
    }

    public Vector3 getMoonStartRotation()
    {
        return this.moonStartRotation;
    }

    public Vector3 getMoonEndRotation()
    {
        return this.moonEndRotation;
    }

    public bool isAlive()
    {
        return this.alive;
    }
}