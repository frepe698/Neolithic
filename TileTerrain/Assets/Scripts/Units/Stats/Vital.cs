using UnityEngine;
using System.Collections;

public class Vital : BaseStat {
	
	private float curValue;
	private float percent;
	
	private readonly int levelMulti;

	public Vital(string name, int baseValue, int levelMulti) : base(name, baseValue) {
		curValue = 0;
		this.levelMulti = levelMulti;
	}
	
	public void reset(int level){
		percent = getCurValue()/getValue();
		value = baseValue + levelMulti*level;
		multiplier = 1;
	}
	
	public void multiply(){
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
	
	public int getLevelMulti(){
		return levelMulti;
	}
	
	public string getWindowString(){
		return name+": "+(int)curValue+"/"+(int)value;
	}
}

