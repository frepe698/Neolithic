using UnityEngine;
using System.Collections;

public class LootId : MonoBehaviour {

	int id;

	public int getId()
	{
		return id;
	}

	public void setId(int id)
	{
		this.id = id;
	}

	public virtual Item getItem(string item)
	{
		return new MaterialItem(item);
	}
}
