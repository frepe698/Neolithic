using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class HealthbarController : MonoBehaviour {

    protected RectTransform rectTransform;

    public RectTransform healthMask;
    

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    void Start()
    {
        rectTransform.localScale = new Vector3(0.5f, 0.5f, 1);
    }

    public virtual void update(Actor actor)
    {
        healthMask.sizeDelta = new Vector2(actor.getHealth() / actor.getMaxHealth() * 50, healthMask.sizeDelta.y);
        Vector2 screenPos = RectTransformUtility.WorldToScreenPoint(Camera.main, actor.getPosition());

        //screenPos = Camera.main.WorldToScreenPoint(unit.getPosition()) + new Vector3(0, 50, 0);
        screenPos = screenPos / GameMaster.getGUIManager().getCanvasCanvas().scaleFactor - new Vector2(320, 180);;
        screenPos.x *= 1f;
        rectTransform.anchoredPosition = screenPos + new Vector2(0, actor.getHeight());
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
