using UnityEngine;
using System.Collections;

public class PercentStat : BaseStat {
	
	private float maxAmount;
	
	public PercentStat(string name, float maxAmount){
		this.name = name;
		this.value = 0;
		this.multiplier = 0;
		this.maxAmount = maxAmount;
	}
	
	public void addValue(float add){
		value += add/100;
		if(value > maxAmount) value = maxAmount;
	}
	
	public void multiply(){
		
	}
	
	public float getValue(){
		return 1-value;
	}
	
	public void addMultiplier(float add){
		value += add;
		if(value > maxAmount) value = maxAmount;
	}
	
	public void reset(int level){
		value = 0;
		multiplier = 0;
	}
	
	
	public string getWindowString(){
		return name+": "+(int)(value*100)+"%";
	}

}

