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
	
	public override void addValue(float add){
		value += add/100;
		if(value > maxAmount) value = maxAmount;
	}
	
	public override void multiply(){
		
	}
	
	public override float getValue(){
		return 1-value;
	}
	
	public override void addMultiplier(float add){
		value += add;
		if(value > maxAmount) value = maxAmount;
	}
	
	public override void reset(int level){
		value = 0;
		multiplier = 0;
	}
	
	
	public override string getWindowString(){
		return name+": "+(int)(value*100)+"%";
	}

}

