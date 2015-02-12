using UnityEngine;
using System.Collections;
using System.Xml;
using System.Xml.Serialization;

public class ItemData : ObjectData {

	public virtual LootableObject getLootableObject(Vector3 position, Quaternion rotation)
	{
		return new LootableObject(position, rotation, name);
	}
}