using UnityEngine;
using System.Collections;
using System.Xml;
using System.Xml.Serialization;
using Edit;
public class ItemData : ObjectData {

    public ItemData()
    { 
    }

    public ItemData(ItemEdit edit)
        : base(edit)
    { 
    }

	public virtual LootableObject getLootableObject(Vector3 position, Quaternion rotation)
	{
		return new LootableObject(position, rotation, name, modelName);
	}
}