using UnityEngine;
using System.Collections;
using System.Xml.Serialization;
using System.Xml;

public class ObjectData {

	[XmlAttribute("name")]
	public readonly string name;

	[XmlAttribute("gameName")]
	public readonly string gameName;
	
}
