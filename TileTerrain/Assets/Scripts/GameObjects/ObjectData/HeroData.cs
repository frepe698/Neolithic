using UnityEngine;
using System.Collections;
using System.Xml;
using System.Xml.Serialization;
using Edit;

public class HeroData : UnitData {

    public HeroData(HeroEdit data)
        : base(data)
    { 
    }

    public HeroData()
    { 
    }

}
