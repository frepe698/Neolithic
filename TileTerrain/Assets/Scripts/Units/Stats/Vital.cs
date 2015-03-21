using UnityEngine;
using System.Collections;

public class Vital : BaseStat {
	
	private float curValue;
	private float percent;
	
	private readonly int levelAdd;

	public Vital(string name, int baseValue, int levelAdd) : base(name, baseValue) {
		curValue = 0;
        percent = -1;
		this.levelAdd = levelAdd;
	}
	
	public override void reset(int level){
        if (percent < 0) percent = 1;
        else percent = getCurValue() / (getValue() + 0.00001f);
		
		value = baseValue + levelAdd*level;
		multiplier = 1;
	}
	
	public override void multiply(){
		value = value*multiplier;
		curValue = value*percent;
	}
	
	public float getCurValue(){
		return curValue;
	}
	
	public void setCurValue(float value){
		curValue = value;
		if(curValue > value) curValue = value;
	}
	
	public void addCurValue(float add){
		curValue += add;
		if(curValue > value) curValue = value;
	}
	
	public int getBase(){
		return baseValue;
	}
	
	public int getLevelAdd(){
		return levelAdd;
	}
	
	public override string getWindowString(){
		return name+": "+(int)curValue+"/"+(int)value;
	}
}

