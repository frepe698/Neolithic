using UnityEngine;
using System.Collections;
using System.Xml.Serialization;
using System.Xml;
using Edit;
public class ObjectData {

	[XmlAttribute("name")]
	public readonly string name;

	[XmlAttribute("gameName")]
	public readonly string gameName;

    public ObjectData()
    { 
    }

    public ObjectData(ObjectEdit edit)
    {
        name = edit.name;
        gameName = edit.gameName;
    }
}
