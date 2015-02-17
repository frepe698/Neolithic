using UnityEngine;
using System.Collections;
using System.Xml;
using System.Xml.Serialization;
using System.IO;

[XmlRoot("DataHolder")]
public class DataHolder {

	[XmlRoot("UnitsRoot")]
	public class UnitDataHolder
	{
		[XmlArray("Units"), XmlArrayItem("UnitData")]
		public readonly UnitData[] unitData;
	}

	[XmlRoot("WeaponsRoot")]
	public class WeaponDataHolder
	{
        public WeaponDataHolder() { }
        public WeaponDataHolder(MeleeWeaponData[] meleeData, RangedWeaponData[] rangedData)
        {
            this.meleeWeaponData = meleeData;
            this.rangedWeaponData = rangedData;
        }
		[XmlArray("MeleeWeapons"), XmlArrayItem("MeleeWeaponData")]
		public readonly MeleeWeaponData[] meleeWeaponData;
		
		[XmlArray("RangedWeapons"), XmlArrayItem("RangedWeaponData")]
		public readonly RangedWeaponData[] rangedWeaponData;
	}

	[XmlRoot("ConsumableItemsRoot")]
	public class ConsumableItemDataHolder
	{
		[XmlArray("ConsumableItems"), XmlArrayItem("ConsumableItemData")]
		public readonly ConsumableItemData[] consumableItemData;
	}

	[XmlRoot("MaterialsRoot")]
	public class MaterialDataHolder
	{
		[XmlArray("Materials"), XmlArrayItem("MaterialData")]
		public readonly MaterialData[] materialData;
	}

	[XmlRoot("RecipesRoot")]
	public class RecipeDataHolder
	{
		[XmlArray("ItemRecipes"), XmlArrayItem("ItemRecipeData")]
		public readonly ItemRecipeData[] itemRecipeData;
		
		[XmlArray("MaterialRecipes"), XmlArrayItem("MaterialRecipeData")]
		public readonly MaterialRecipeData[] materialRecipeData;
	}

	[XmlRoot("ResourcesRoot")]
	public class ResourceDataHolder
	{
		[XmlArray("Resources"), XmlArrayItem("ResourceData")]
		public readonly ResourceData[] resourceData;
	}

	[XmlRoot("ProjectilesRoot")]
	public class ProjectileDataHolder
	{
		[XmlArray("Projectiles"), XmlArrayItem("ProjectileData")]
		public readonly ProjectileData[] projectileData;
	}

	private UnitDataHolder unitDataHolder;

	private WeaponDataHolder weaponDataHolder;

	private ConsumableItemDataHolder consumableItemDataHolder;

	private MaterialDataHolder materialDataHolder;

	private RecipeDataHolder recipeDataHolder;

	private ResourceDataHolder resourceDataHolder;

	private ProjectileDataHolder projectileDataHolder;



	//object for locking
	private static object syncRoot = new System.Object();
	private static volatile DataHolder instance;

	public static void loadData()
	{
//		XmlSerializer serializer = new XmlSerializer(typeof(DataHolder));
//		using(FileStream stream = new FileStream(Application.dataPath+"\\data.xml", FileMode.Open))
//		{
//			instance = serializer.Deserialize(stream) as DataHolder;
//			Debug.Log ("loaded data");
//		}
//		TextAsset data = (TextAsset)Resources.Load ("data");
//		XmlSerializer serializer = new XmlSerializer(typeof(DataHolder));
//		using(StringReader reader = new System.IO.StringReader(data.text))
//		{
//			instance = serializer.Deserialize(reader) as DataHolder;
//		}

		instance = new DataHolder();

		TextAsset data = (TextAsset)Resources.Load ("Data/unitdata");
		XmlSerializer serializer = new XmlSerializer(typeof(UnitDataHolder));
		using(StringReader reader = new System.IO.StringReader(data.text))
		{
			instance.unitDataHolder = serializer.Deserialize(reader) as UnitDataHolder;
		}

		data = (TextAsset)Resources.Load ("Data/weapondata");
		serializer = new XmlSerializer(typeof(WeaponDataHolder));
		using(StringReader reader = new System.IO.StringReader(data.text))
		{
			instance.weaponDataHolder = serializer.Deserialize(reader) as WeaponDataHolder;
		}

		data = (TextAsset)Resources.Load ("Data/consumableitemdata");
		serializer = new XmlSerializer(typeof(ConsumableItemDataHolder));
		using(StringReader reader = new System.IO.StringReader(data.text))
		{
			instance.consumableItemDataHolder = serializer.Deserialize(reader) as ConsumableItemDataHolder;
		}

		data = (TextAsset)Resources.Load ("Data/materialdata");
		serializer = new XmlSerializer(typeof(MaterialDataHolder));
		using(StringReader reader = new System.IO.StringReader(data.text))
		{
			instance.materialDataHolder = serializer.Deserialize(reader) as MaterialDataHolder;
		}

		data = (TextAsset)Resources.Load ("Data/recipedata");
		serializer = new XmlSerializer(typeof(RecipeDataHolder));
		using(StringReader reader = new System.IO.StringReader(data.text))
		{
			instance.recipeDataHolder = serializer.Deserialize(reader) as RecipeDataHolder;
		}

		data = (TextAsset)Resources.Load ("Data/resourcedata");
		serializer = new XmlSerializer(typeof(ResourceDataHolder));
		using(StringReader reader = new System.IO.StringReader(data.text))
		{
			instance.resourceDataHolder = serializer.Deserialize(reader) as ResourceDataHolder;
		}

		data = (TextAsset)Resources.Load ("Data/projectiledata");
		serializer = new XmlSerializer(typeof(ProjectileDataHolder));
		using(StringReader reader = new System.IO.StringReader(data.text))
		{
			instance.projectileDataHolder = serializer.Deserialize(reader) as ProjectileDataHolder;
		}

	}



	public ResourceData getResourceData(string name)
	{
		foreach(ResourceData data in resourceDataHolder.resourceData)
		{
			if(data.name.Equals(name)) return data;
		}
		return null;
	}

	public ItemData getItemData(string name)
	{
		foreach(MaterialData data in materialDataHolder.materialData)
		{
			if(data.name.Equals(name)) return data;
		}
		foreach(MeleeWeaponData data in weaponDataHolder.meleeWeaponData)
		{
			if(data.name.Equals(name)) return data;
		}
		foreach(RangedWeaponData data in weaponDataHolder.rangedWeaponData)
		{
			if(data.name.Equals(name)) return data;
		}
		foreach(ConsumableItemData data in consumableItemDataHolder.consumableItemData)
		{
			if(data.name.Equals(name)) return data;
		}
		return null;
	}

	public CraftedItemData getCraftedItemData(string name)
	{
		foreach(MeleeWeaponData data in weaponDataHolder.meleeWeaponData)
		{
			if(data.name.Equals(name)) return data;
		}
		foreach(RangedWeaponData data in weaponDataHolder.rangedWeaponData)
		{
			if(data.name.Equals(name)) return data;
		}

		return null;
	}

	public WeaponData getWeaponData(string name)
	{
		foreach(MeleeWeaponData data in weaponDataHolder.meleeWeaponData)
		{
			if(data.name.Equals(name)) return data;
		}
		foreach(RangedWeaponData data in weaponDataHolder.rangedWeaponData)
		{
			if(data.name.Equals(name)) return data;
		}
		
		return null;
	}

	public ConsumableItemData getConsumableItemData(string name)
	{
		foreach(ConsumableItemData data in consumableItemDataHolder.consumableItemData)
		{
			if(data.name.Equals(name)) return data;
		}
		return null;
	}

	public MaterialData getMaterialData(string name)
	{
		foreach(MaterialData data in materialDataHolder.materialData)
		{
			if(data.name.Equals(name)) return data;
		}
		return null;
	}
	
	public ItemRecipeData getItemRecipeData(string name)
	{
		foreach(ItemRecipeData data in recipeDataHolder.itemRecipeData)
		{
			if(data.name.Equals(name)) return data;
		}
		return null;
	}

	public ItemRecipeData[] getItemRecipeData()
	{
		return recipeDataHolder.itemRecipeData;
	}

	
	public MaterialRecipeData getMaterialRecipeData(string name)
	{
		foreach(MaterialRecipeData data in recipeDataHolder.materialRecipeData)
		{
			if(data.name.Equals(name)) return data;
		}
		return null;
	}

	public MaterialRecipeData[] getMaterialRecipeData()
	{
		return recipeDataHolder.materialRecipeData;
	}
	
	
	public ProjectileData getProjectileData(string name)
	{
		foreach(ProjectileData data in projectileDataHolder.projectileData)
		{
			if(data.name.Equals(name)) return data;
		}
		return null;
	}

	public UnitData getUnitData(string name)
	{
		foreach(UnitData data in unitDataHolder.unitData)
		{
			if(data.name.Equals(name)) return data;
		}
		return null;
	}


	public static DataHolder Instance
	{
		get
		{
			//check to see if it doesnt exist
			if (instance == null)
			{
				loadData();
				//lock access, if it is already locked, wait.
				lock (syncRoot)
				{
					//the instance could have been made between
					//checking and waiting for a lock to release.
					if (instance == null)
					{
						//create a new instance
						loadData();
					}
				}
			}
			//return either the new instance or the already built one.
			return instance;
		}
	}

}
