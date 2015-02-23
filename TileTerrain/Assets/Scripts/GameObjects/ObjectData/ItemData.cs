using UnityEngine;
using System.Collections;
using System.Xml;
using System.Xml.Serialization;
using Edit;
public class ItemData : ObjectData {

    private Sprite icon;
    [XmlElement(IsNullable = false)]
    public readonly string description;

    public ItemData()
    { 
    }

    public ItemData(ItemEdit edit)
        : base(edit)
    {
        if(edit.description != null && !edit.description.Trim().Equals("")) description = edit.description;
    }

	public virtual LootableObject getLootableObject(Vector3 position, Quaternion rotation)
	{
		return new LootableObject(position, rotation, name, modelName);
	}

    public virtual string getTooltipStatsString()
    {
        return "override this";
    }

    public void setIcon(Sprite sprite)
    {
        this.icon = sprite;
    }

    public Sprite getIcon()
    {
        return icon;
    }
}