﻿using UnityEngine;
using System.Collections;

public class BaseStat {
	protected float value;
	protected float multiplier;
	protected string name;
	
	protected readonly int baseValue;
	
	public BaseStat(){
		baseValue = 0;
	}
	
	public BaseStat(string name, int baseValue){
		this.name = name;
		this.value = 0;
		multiplier = 1;
		this.baseValue = baseValue;
	}
	
	public void multiply(){
		value = value*multiplier;
	}
	
	public float getValue(){
		return value;
	}
	public float getMultiValue(){
		return value-1;
	}
	
	public void reset(int level){
		value = baseValue;
		multiplier = 1;
	}
	
	public void addValue(float add){
		value += add;
	}
	
	public void setMultiplier(float value){
		multiplier = value;
	}
	
	public void addMultiplier(float add){
		multiplier += add;
	}
	
	public string getName(){
		return name;
	}
	
	public string getWindowString(){
		return name+": "+(int)value;
	}

}
