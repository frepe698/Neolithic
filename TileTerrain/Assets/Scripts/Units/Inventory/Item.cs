using UnityEngine;
using System.Collections;

public abstract class Item {

	private string name;

	public Item(string name)
	{
		this.name = name;
	}

	public string getName()
	{
		return name;
	}

	public virtual string getGameName()
	{
		return "gameName";
	}

	public virtual string getInventoryDisplay()
	{
		return name;
	}
}
