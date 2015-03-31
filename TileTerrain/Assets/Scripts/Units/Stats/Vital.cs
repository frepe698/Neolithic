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
        else if (getCurValue() <= 0 || getValue() <= 0) percent = 0;
        else percent = getCurValue() / getValue();
		
		value = baseValue + levelAdd*level;
		multiplier = 1;
	}
	
	public override void multiply(){
		value = value*multiplier;
		curValue = value*percent;
	}

    public override void multiply(float mult)
    {
        value *= mult;
        curValue *= mult;
    }
	
	public float getCurValue(){
		return curValue;
	}
	
	public void setCurValue(float value){
		curValue = value;
        curValue = Mathf.Clamp(curValue, 0, this.value);
		
	}
	
	public void addCurValue(float add){
		curValue += add;
        curValue = Mathf.Clamp(curValue, 0, this.value);
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

