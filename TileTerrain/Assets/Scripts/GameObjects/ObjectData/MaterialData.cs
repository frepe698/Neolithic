using UnityEngine;
using System.Collections;

public class MaterialData : ItemData{

	public readonly string tooltip;

    public override string getTooltipStatsString()
    {
        return tooltip;
    }
}
