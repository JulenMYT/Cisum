using System.Collections;
using UnityEngine;

public class GuardAction : MonoBehaviour, ICombatAction
{
    [SerializeField] private Animator playerAnimator;

    float defenseMultiplier = 3;
    string defenseMessage = "You put yourself in defense position.";
    string animationTriggerName = "guard";

    public IEnumerator Execute(CombatLogic combatLogic)
    {
        playerAnimator.SetTrigger(animationTriggerName);

        combatLogic.playerStats.defenseMultiplier = defenseMultiplier;

        CombatUIManager.Instance.ShowDialogueUI(true, defenseMessage);

        yield return InputManager.Instance.WaitForInput();

        combatLogic.OnActionCompleted();
    }
}
