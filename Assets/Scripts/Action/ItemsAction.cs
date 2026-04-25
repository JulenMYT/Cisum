using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemsAction : MonoBehaviour
{
    public void Execute(CombatLogic combatLogic)
    {
        CombatUIManager.Instance.ShowInventoryUI(true);
        CombatUIManager.Instance.EnableButtons(false);
    }
}
