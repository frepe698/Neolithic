using UnityEngine;
using System.Collections;

public class DefStat : BaseStat{
	
	private float maxAmount;
	
	public DefStat(string name, float maxAmount){
		this.name = name;
		this.value = 0;
		this.multiplier = 1;
		this.maxAmount = maxAmount;
	}
	
	public void addValue(float add){
		value += add;
	}
	
	public void multiply(){
		value = value*multiplier;
		if(value > maxAmount) value = maxAmount;
	}
	
	public void reset(int level){
		value = 0;
		multiplier = 1;
	}
	
	public string getWindowString(){
		return name+": "+(int)(value*100)+"%";
	}
}
