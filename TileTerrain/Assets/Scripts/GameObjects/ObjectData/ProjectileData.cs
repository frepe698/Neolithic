using UnityEngine;
using System.Collections;
using System.Xml;
using System.Xml.Serialization;

public class ProjectileData : ObjectData{

	[XmlAttribute("modelName")]
	public readonly string modelname;

	public readonly float range;
	public readonly float speed;
	public readonly float damage;

}
