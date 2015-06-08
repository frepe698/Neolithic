using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class HeroHealthbarController : HealthbarController {

    public RectTransform energyMask;
    public Text level;

    void Start()
    {
        rectTransform.localScale = new Vector3(1, 1, 1);
    }

    public override void update(Actor actor)
    {
        healthMask.sizeDelta = new Vector2(actor.getHealth() / actor.getMaxHealth() * 50, healthMask.sizeDelta.y);
        energyMask.sizeDelta = new Vector2(actor.getUnitStats().getCurEnergy() / actor.getUnitStats().getMaxEnergy() * 50, energyMask.sizeDelta.y);
        level.text = actor.getUnitStats().getDisplayLevel().ToString();
        Vector2 screenPos = RectTransformUtility.WorldToScreenPoint(Camera.main, actor.getPosition() + new Vector3(0, 3f, 0));

        //screenPos = Camera.main.WorldToScreenPoint(unit.getPosition()) + new Vector3(0, 50, 0);
        screenPos = screenPos / GameMaster.getGUIManager().getCanvasCanvas().scaleFactor - new Vector2(320, 180); ;
        screenPos.x *= 0.9f;
        rectTransform.anchoredPosition = screenPos;
    }
	
}
