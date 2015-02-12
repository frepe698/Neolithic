using UnityEngine;
using System.Collections;
using System.Xml;
using System.Xml.Serialization;

public class UnitData : ObjectData {

	[XmlAttribute("modelName")]
	public readonly string modelname;

	public readonly int health;
	public readonly int damage;
	public readonly float lifegen;
	public readonly float energy;
	public readonly float energygen;
	public readonly float attackspeed = 1;
	public readonly float movespeed;
	public readonly bool hostile;
	public readonly int lineofsight;
	public readonly float size;

	[XmlArray("safeDrops")]
	public readonly string[] safeDrops;

	[XmlArray("randomDrops")]
	public readonly string[] randomDrops;

	public readonly int minDrops;
	public readonly int maxDrops;
}
