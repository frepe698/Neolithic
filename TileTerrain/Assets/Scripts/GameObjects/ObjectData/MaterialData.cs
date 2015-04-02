using UnityEngine;
using System.Collections;
using Edit;

public class MaterialData : ItemData{

    public MaterialData()
    { 
    }

    public MaterialData(MaterialEdit edit)
        : base(edit)
    {

    }

    public override string getTooltipStatsString()
    {
        return "";
    }

    public override Item getItem()
    {
        return new MaterialItem(name);
    }
}
