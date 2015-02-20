using UnityEngine;
using UnityEngine.EventSystems;

public class InventoryButton : MonoBehaviour//, IPointerEnterHandler, IPointerExitHandler
{
    private int index;

    public void setIndex(int index)
    {
        this.index = index;
    }
    void OnPointerEnter(PointerEventData e)
    {
    }

    void OnPointerExit(PointerEventData e)
    { 
    }
}
