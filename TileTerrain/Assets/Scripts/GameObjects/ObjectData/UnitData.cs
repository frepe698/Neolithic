using UnityEngine;
using System.Collections;
using System.Xml;
using System.Xml.Serialization;
using Edit;

public class UnitData : ObjectData {

	public readonly int health;
	
	public readonly float lifegen;
	public readonly float energy;
	public readonly float energygen;
	
	public readonly float movespeed;
	
	public readonly float size;

	
    
    public UnitData()
    {
    }

    public UnitData(UnitEdit data)
        : base(data)
    {
        health = data.health;
        lifegen = data.lifegen;
        energy = data.energy;
        energygen = data.energygen;
        movespeed = data.movespeed;
        size = data.size;
    }

    public virtual string[] getSafeDrops()
    {
        return null;
    }
    public virtual string[] getRandomDrops()
    {
        return null;
    }
    public virtual int getMinDrops()
    {
        return 0;
    }
    public virtual int getMaxDrops()
    {
        return 0;
    }

}
