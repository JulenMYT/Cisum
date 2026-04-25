using System.Collections;
using UnityEngine;

[CreateAssetMenu(fileName = "New Item", menuName = "Inventory/Item")]
public class Item : ScriptableObject
{
    public string itemName;
    [TextArea]
    public string description;
    public ItemType type;
    public int parameter;

    public enum ItemType
    {
        Heal,
        Damage,
        Buff
    }

    public void Use(CharacterStats target)
    {
        if (type == ItemType.Heal)
        {
            target.Heal(parameter);
            CombatUIManager.Instance.UpdateHealth(target);
        }
        else if (type == ItemType.Damage)
        {
            target.TakeDamage(parameter, 1f, 1f, true);
        }
    }
}