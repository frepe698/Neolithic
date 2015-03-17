using UnityEngine;
using System.Collections;
using System.Xml;
using System.Xml.Serialization;
using System.IO;

[XmlRoot("DataHolder")]
public class DataHolder {

    [XmlRoot("AbilityRoot")]
    public class AbilityDataHolder
    {
        public AbilityDataHolder() { }
        public AbilityDataHolder(AbilityData[] abilityData)
        {
            this.abilityData = abilityData;
        }
        [XmlArray("Abilities"), XmlArrayItem("AbilityData")]
        public readonly AbilityData[] abilityData;
    }

	[XmlRoot("UnitsRoot")]
	public class UnitDataHolder
	{
        public UnitDataHolder() { }
        public UnitDataHolder(HeroData[] heroData, AIUnitData[] aiUnitData)
        {
            this.heroData = heroData;
            this.aiUnitData = aiUnitData;
        }
        [XmlArray("Heroes"), XmlArrayItem("HeroData")]
        public readonly HeroData[] heroData;
	
        [XmlArray("AIUnits"), XmlArrayItem("AIUnitData")]
		public readonly AIUnitData[] aiUnitData;
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

    [XmlRoot("ArmorRoot")]
    public class ArmorDataHolder
    {
        public ArmorDataHolder() { }
        public ArmorDataHolder(ArmorData[] armorData)
        {
            this.armorData = armorData;
        }

        [XmlArray("Armor"), XmlArrayItem("ArmorData")]
        public readonly ArmorData[] armorData;
    }

	[XmlRoot("ConsumableItemsRoot")]
	public class ConsumableItemDataHolder
	{
		
	}

	[XmlRoot("ItemRoot")]
	public class ItemDataHolder
	{
        public ItemDataHolder() { }
        public ItemDataHolder(MaterialData[] materialData, ConsumableItemData[] consumableItemData) 
        {
            this.materialData = materialData;
            this.consumableItemData = consumableItemData;
        }
		[XmlArray("Materials"), XmlArrayItem("MaterialData")]
		public readonly MaterialData[] materialData;

        [XmlArray("ConsumableItems"), XmlArrayItem("ConsumableItemData")]
        public readonly ConsumableItemData[] consumableItemData;
	}

	[XmlRoot("RecipesRoot")]
	public class RecipeDataHolder
	{
        public RecipeDataHolder() { }
        public RecipeDataHolder(EquipmentRecipeData[] equipment, MaterialRecipeData[] material, ConsumableRecipeData[] consumable)
        {
            this.equipmentRecipeData = equipment;
            this.materialRecipeData = material;
            this.consumableRecipeData = consumable;
        }
		[XmlArray("EquipmentRecipes"), XmlArrayItem("EquipmentRecipeData")]
		public readonly EquipmentRecipeData[] equipmentRecipeData;
		
		[XmlArray("MaterialRecipes"), XmlArrayItem("MaterialRecipeData")]
		public readonly MaterialRecipeData[] materialRecipeData;

        [XmlArray("ConsumableRecipes"), XmlArrayItem("ConsumableRecipeData")]
        public readonly ConsumableRecipeData[] consumableRecipeData;
	}

    [XmlRoot("EffectsRoot")]
    public class EffectDataHolder
    {
        public EffectDataHolder() { }
        public EffectDataHolder(SingleTargetEffectData[] singletargets, AreaOfEffectData[] aoe)
        {
            this.singleTargetEffectData = singletargets;
            this.areaOfEffectData = aoe;
        }
        [XmlArray("SingleTargets"), XmlArrayItem("SingleTargetEffectData")]
        public readonly SingleTargetEffectData[] singleTargetEffectData;

        [XmlArray("AreaOfEffects"), XmlArrayItem("AreaOfEffectData")]
        public readonly AreaOfEffectData[] areaOfEffectData;
    }

	[XmlRoot("ResourcesRoot")]
	public class ResourceDataHolder
	{
        public ResourceDataHolder() { }
        public ResourceDataHolder(ResourceData[] resourceData)
        {
            this.resourceData = resourceData;
        }
		[XmlArray("Resources"), XmlArrayItem("ResourceData")]
		public readonly ResourceData[] resourceData;
	}

	[XmlRoot("ProjectilesRoot")]
	public class ProjectileDataHolder
	{
		[XmlArray("Projectiles"), XmlArrayItem("ProjectileData")]
		public readonly ProjectileData[] projectileData;
	}

    [XmlRoot("SkillsRoot")]
    public class SkillDataHolder
    {
        public SkillDataHolder() { }
        public SkillDataHolder(SkillData[] skillData)
        {
            this.skillData = skillData;
        }
        [XmlArray("Skills"), XmlArrayItem("SkillData")]
        public readonly SkillData[] skillData;
    }

    private AbilityDataHolder abilityDataHolder;

	private UnitDataHolder unitDataHolder;

	private WeaponDataHolder weaponDataHolder;

    private ArmorDataHolder armorDataHolder;

	private ItemDataHolder itemDataHolder;

	private RecipeDataHolder recipeDataHolder;

	private ResourceDataHolder resourceDataHolder;

	private ProjectileDataHolder projectileDataHolder;

    private SkillDataHolder skillDataHolder;

    private EffectDataHolder effectDataHolder;



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

        data = (TextAsset)Resources.Load("Data/abilitydata");
        serializer = new XmlSerializer(typeof(AbilityDataHolder));
        using (StringReader reader = new System.IO.StringReader(data.text))
        {
            instance.abilityDataHolder = serializer.Deserialize(reader) as AbilityDataHolder;
        }

        data = (TextAsset)Resources.Load("Data/effectdata");
        serializer = new XmlSerializer(typeof(EffectDataHolder));
        using (StringReader reader = new System.IO.StringReader(data.text))
        {
            instance.effectDataHolder = serializer.Deserialize(reader) as EffectDataHolder;
        }

		data = (TextAsset)Resources.Load ("Data/weapondata");
		serializer = new XmlSerializer(typeof(WeaponDataHolder));
		using(StringReader reader = new System.IO.StringReader(data.text))
		{
			instance.weaponDataHolder = serializer.Deserialize(reader) as WeaponDataHolder;
		}

        data = (TextAsset)Resources.Load("Data/armordata");
        serializer = new XmlSerializer(typeof(ArmorDataHolder));
        using (StringReader reader = new System.IO.StringReader(data.text))
        {
            instance.armorDataHolder = serializer.Deserialize(reader) as ArmorDataHolder;
        }

		data = (TextAsset)Resources.Load ("Data/itemdata");
		serializer = new XmlSerializer(typeof(ItemDataHolder));
		using(StringReader reader = new System.IO.StringReader(data.text))
		{
			instance.itemDataHolder = serializer.Deserialize(reader) as ItemDataHolder;
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

        data = (TextAsset)Resources.Load("Data/skillData");
        serializer = new XmlSerializer(typeof(SkillDataHolder));
        using(StringReader reader = new System.IO.StringReader(data.text))
        {
            instance.skillDataHolder = serializer.Deserialize(reader) as SkillDataHolder;
        }

        instance.initItemIcons();

	}

    private void initItemIcons()
    {
        Sprite[] icons = Resources.LoadAll<Sprite>("GUI/itemicons");

        foreach (MaterialData data in itemDataHolder.materialData)
        {
            data.setIcon(findSprite(icons, data.modelName));
        }
        foreach (MeleeWeaponData data in weaponDataHolder.meleeWeaponData)
        {
            data.setIcon(findSprite(icons, data.modelName));
        }
        foreach (RangedWeaponData data in weaponDataHolder.rangedWeaponData)
        {
            data.setIcon(findSprite(icons, data.modelName));
        }
        foreach (ArmorData data in armorDataHolder.armorData)
        {
            data.setIcon(findSprite(icons, data.modelName));
        }
        foreach (ConsumableItemData data in itemDataHolder.consumableItemData)
        {
            data.setIcon(findSprite(icons, data.modelName));
        }
    }

    private Sprite findSprite(Sprite[] sprites, string name)
    {
        foreach (Sprite s in sprites)
        {
            if (s.name.ToLower().Equals(name.ToLower()))
            {
                return s;
            }
        }
        return null;
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
		foreach(MaterialData data in itemDataHolder.materialData)
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
        foreach (ArmorData data in armorDataHolder.armorData)
        {
            if (data.name.Equals(name)) return data;
        }
		foreach(ConsumableItemData data in itemDataHolder.consumableItemData)
		{
			if(data.name.Equals(name)) return data;
		}
		return null;
	}

    public AbilityEffectData getEffectData(string name)
    {
        foreach(SingleTargetEffectData data in effectDataHolder.singleTargetEffectData)
        {
            if (data.name.Equals(name)) return data;
        }
        foreach (AreaOfEffectData data in effectDataHolder.areaOfEffectData)
        {
            if (data.name.Equals(name)) return data;
        }
        return null;
    }

    public AbilityData getAbilityData(string name)
    {
        foreach(AbilityData data in abilityDataHolder.abilityData)
        {
            if (data.name.Equals(name)) return data;
        }
        return null;
    }
	public EquipmentData getEquipmentData(string name)
	{
		foreach(MeleeWeaponData data in weaponDataHolder.meleeWeaponData)
		{
			if(data.name.Equals(name)) return data;
		}
		foreach(RangedWeaponData data in weaponDataHolder.rangedWeaponData)
		{
			if(data.name.Equals(name)) return data;
		}
        foreach (ArmorData data in armorDataHolder.armorData)
        {
            if (data.name.Equals(name)) return data;
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

    public ArmorData getArmorData(string name)
    {
        foreach (ArmorData data in armorDataHolder.armorData)
        {
            if (data.name.Equals(name)) return data;
        }
        return null;
    }

	public ConsumableItemData getConsumableItemData(string name)
	{
		foreach(ConsumableItemData data in itemDataHolder.consumableItemData)
		{
			if(data.name.Equals(name)) return data;
		}
		return null;
	}

	public MaterialData getMaterialData(string name)
	{
		foreach(MaterialData data in itemDataHolder.materialData)
		{
			if(data.name.Equals(name)) return data;
		}
		return null;
	}

    public RecipeData getRecipeData(string name)
    {
        foreach (EquipmentRecipeData data in recipeDataHolder.equipmentRecipeData)
        {
            if (data.name.Equals(name)) return data;
        }
        foreach (MaterialRecipeData data in recipeDataHolder.materialRecipeData)
        {
            if (data.name.Equals(name)) return data;
        }
        foreach (ConsumableRecipeData data in recipeDataHolder.consumableRecipeData)
        {
            if (data.name.Equals(name)) return data;
        }
        return null;
    }

    public RecipeData getRecipeData(int index)
    {
        int recipeIndex = index;
        if (recipeIndex < recipeDataHolder.equipmentRecipeData.Length)
        {
            return recipeDataHolder.equipmentRecipeData[recipeIndex];
        }
        recipeIndex -= recipeDataHolder.equipmentRecipeData.Length;

        if (recipeIndex < recipeDataHolder.materialRecipeData.Length)
        {
            return recipeDataHolder.materialRecipeData[recipeIndex];
        }
        recipeIndex -= recipeDataHolder.materialRecipeData.Length;

        if (recipeIndex < recipeDataHolder.consumableRecipeData.Length)
        {
            return recipeDataHolder.consumableRecipeData[recipeIndex];
        }

        return null;
    }

    public EquipmentRecipeData getEquipmentRecipeData(string name)
	{
		foreach(EquipmentRecipeData data in recipeDataHolder.equipmentRecipeData)
		{
			if(data.name.Equals(name)) return data;
		}
		return null;
	}

	public EquipmentRecipeData[] getEquipmentRecipeData()
	{
		return recipeDataHolder.equipmentRecipeData;
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

    public ConsumableRecipeData getConsumableRecipeData(string name)
    {
        foreach (ConsumableRecipeData data in recipeDataHolder.consumableRecipeData)
        {
            if (data.name.Equals(name)) return data;
        }
        return null;
    }

    public ConsumableRecipeData[] getConsumableRecipeData()
    {
        return recipeDataHolder.consumableRecipeData;
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
		foreach(HeroData data in unitDataHolder.heroData)
		{
			if(data.name.Equals(name)) return data;
		}
        foreach (AIUnitData data in unitDataHolder.aiUnitData)
        {
            if (data.name.Equals(name)) return data;
        }
		return null;
	}

    public HeroData getHeroData(string name)
    {
        foreach (HeroData data in unitDataHolder.heroData)
        {
            if (data.name.Equals(name)) return data;
        }
        return null;
    }

    public AIUnitData getAIUnitData(string name)
    {
        foreach (AIUnitData data in unitDataHolder.aiUnitData)
        {
            if (data.name.Equals(name)) return data;
        }
        return null;
    }

    public SkillData getSkillData(string name)
    {
        foreach(SkillData data in skillDataHolder.skillData)
        {
            if (data.name.Equals(name)) return data;
        }
        return null;
    }

    public int getSkillIndex(string name)
    {
        for (int i = 0; i < skillDataHolder.skillData.Length; i++)
        {
            if (skillDataHolder.skillData[i].name.Equals(name)) return i;
        }
        return -1;
    }

    public SkillData[] getAllSkillData()
    {
        return skillDataHolder.skillData;
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
