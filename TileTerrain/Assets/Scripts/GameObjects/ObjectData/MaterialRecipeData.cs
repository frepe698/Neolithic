﻿using UnityEngine;
using System.Collections;
using System.Xml;
using System.Xml.Serialization;

public class MaterialRecipeData : ObjectData {
	
	public readonly string product;
	
	[XmlArray("ingredients"), XmlArrayItem("Ingredient")]
	public readonly Ingredient[] ingredients;
	
	public readonly string tooltip;
}
