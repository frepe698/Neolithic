using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class HealthbarController : MonoBehaviour {

    private RectTransform rectTransform;

    public RectTransform healthMask;
    public RectTransform energyMask;
    public Text level; 

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    void Start()
    {
        rectTransform.localScale = new Vector3(1, 1, 1);
    }

    public void update(Unit unit)
    {
        healthMask.sizeDelta = new Vector2(unit.getHealth() / unit.getMaxHealth() * 50, healthMask.sizeDelta.y);
        energyMask.sizeDelta = new Vector2(unit.getUnitStats().getCurEnergy() / unit.getUnitStats().getMaxEnergy() * 50, energyMask.sizeDelta.y);
        level.text = unit.getUnitStats().getDisplayLevel().ToString();
        Vector2 screenPos = RectTransformUtility.WorldToScreenPoint(Camera.main, unit.getPosition()+new Vector3(0, 3f, 0));

        //screenPos = Camera.main.WorldToScreenPoint(unit.getPosition()) + new Vector3(0, 50, 0);
        screenPos = screenPos / GameMaster.getGUIManager().getCanvasCanvas().scaleFactor - new Vector2(320, 180);;
        screenPos.x *= 0.9f;
        rectTransform.anchoredPosition = screenPos;
    }

    public void setColor(Color color)
    {
        healthMask.FindChild("Bar").GetComponent<Image>().color = color;
    }

    public void setActive(bool active)
    {
        gameObject.SetActive(active);
    }
}
