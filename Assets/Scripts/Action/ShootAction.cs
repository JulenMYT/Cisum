using System.Collections;
using UnityEngine;

public class ShootAction : MonoBehaviour, ICombatAction
{
    [SerializeField] private float damageMultiplier = 2;
    [SerializeField] private float perfectDamageMultiplier = 3;
    [SerializeField] private float perfectRadius = 0.1f;
    [SerializeField] private float radius = 0.4f;
    [SerializeField] private Animator playerAnimator;
    [SerializeField] private ShrinkCircleUI shrinkCircleUI;
    [SerializeField] private GameObject slingshot;
    [SerializeField] private int offenseBonus = 4;

    [SerializeField]
    AudioSource audioSource;
    [SerializeField]
    AudioClip audioClip;

    private string drawTriggerName = "draw";
    private string shootTriggerName = "shoot";

    private float success = 0f;

    public IEnumerator Execute(CombatLogic combatLogic)
    {
        combatLogic.playerStats.defenseMultiplier = 1;

        CameraManager.Instance.SwitchToCamera("ShootCamera");

        yield return PerformShoot(combatLogic, playerAnimator);

        CameraManager.Instance.SwitchToCamera("FightCamera");

        yield return InputManager.Instance.WaitForInput();

        combatLogic.OnActionCompleted();
    }

    private IEnumerator PerformShoot(CombatLogic combatLogic, Animator playerAnimator)
    {
        playerAnimator.SetTrigger(drawTriggerName);
        slingshot.SetActive(true);

        shrinkCircleUI.OnShootAttempt += HandleShootAttempt;
        shrinkCircleUI.ShowUI();

        while (shrinkCircleUI.isActive)
        {
            yield return null;
        }

        audioSource.PlayOneShot(audioClip);
        playerAnimator.SetTrigger(shootTriggerName);
        slingshot.GetComponent<Animator>().SetTrigger(shootTriggerName);

        HandleShootOutcome(combatLogic, playerAnimator);

        shrinkCircleUI.OnShootAttempt -= HandleShootAttempt;

        yield return null;
    }

    private void HandleShootAttempt(float result)
    {
        success = result;
    }

    private void HandleShootOutcome(CombatLogic combatLogic, Animator playerAnimator)
    {
        int damage = 0;
        string dialogue = string.Empty;
        ShootResult result = GetShootResult(success);

        switch (result)
        {
            case ShootResult.PerfectShot:
                damage = combatLogic.enemyStats.TakeDamage(combatLogic.playerStats.offense + offenseBonus, perfectDamageMultiplier);
                dialogue = $"Perfect shot! {damage} damage dealt!";
                break;

            case ShootResult.NiceShot:
                damage = combatLogic.enemyStats.TakeDamage(combatLogic.playerStats.offense + offenseBonus, damageMultiplier);
                dialogue = $"Nice shot! {damage} damage dealt!";
                break;

            case ShootResult.MissedShot:
                dialogue = "Missed shot!";
                break;
        }

        CombatUIManager.Instance.ShowDialogueUI(true, dialogue);
    }

    private ShootResult GetShootResult(float success)
    {
        if (success < perfectRadius)
            return ShootResult.PerfectShot;
        else if (success < radius)
            return ShootResult.NiceShot;
        else
            return ShootResult.MissedShot;
    }
}

public enum ShootResult
{
    MissedShot,
    NiceShot,
    PerfectShot
}
