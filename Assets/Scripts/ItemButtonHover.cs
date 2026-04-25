using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

public class ItemButtonHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public Item item;

    public void OnPointerEnter(PointerEventData eventData)
    {
        CombatUIManager.Instance.ShowItemDescription(item.description);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        CombatUIManager.Instance.HideItemDescription();
    }
}
