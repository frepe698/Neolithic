using UnityEngine;
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
	
	public virtual void multiply(){
		value = value*multiplier;
	}

    public virtual void multiply(float mult)
    {
        value *= mult;
    }
	
	public virtual float getValue(){
		return value;
	}
	public float getMultiValue(){
		return value-1;
	}
	
	public virtual void reset(int level){
		value = baseValue;
		multiplier = 1;
	}
	
	public virtual void addValue(float add){
		value += add;
	}
	
	public void setMultiplier(float value){
		multiplier = value;
	}
	
	public virtual void addMultiplier(float add){
		multiplier += add;
	}
	
	public string getName(){
		return name;
	}
	
	public virtual string getWindowString(){
		return name+": "+value;
	}

    public float getMultiplier()
    {
        return multiplier;
    }

}
