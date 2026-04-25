using System.Collections;
using UnityEngine;

public class AttackAction : MonoBehaviour, ICombatAction
{
    [SerializeField] private float tolerance = 0.05f;
    [SerializeField] private int maxHits = 16;
    [SerializeField] private Animator playerAnimator;
    [SerializeField] private GameObject bat;
    [SerializeField] private float attackStrength;
    [SerializeField] private int offenseBonus = 4;
    [SerializeField]
    private float[] hitStrengths = new float[]
    {
    1.0f, 0.25f, 0.15f, 0.1f, 0.08f, 0.07f, 0.06f,
    0.2f, 0.05f, 0.04f, 0.03f, 0.2f, 0.03f, 0.02f,
    0.02f, 0.2f
    };

    [SerializeField]
    AudioSource audioSource;
    [SerializeField]
    AudioClip tooEarly;
    [SerializeField]
    AudioClip tooLate;

    private string idleTriggerName = "idle";
    private string runTriggerName = "run";
    private string attackTriggerName = "attack";
    private string attackMessage = "Go with the beat!";
    private float duration = 0.6f;

    public IEnumerator Execute(CombatLogic combatLogic)
    {
        combatLogic.playerStats.defenseMultiplier = 1;

        CameraManager.Instance.SwitchToCamera("FightCamera");

        Transform enemyPosition = combatLogic.enemyPosition;
        GameObject enemyInstance = combatLogic.enemyInstance;

        Vector3 originalPosition = transform.position;
        Vector3 targetPosition = CalculateAttackPosition(enemyPosition);

        Animator enemyAnimator = GetAnimator(enemyInstance);

        yield return MoveToPosition(targetPosition, playerAnimator);

        CombatUIManager.Instance.ShowDialogueUI(true, attackMessage);

        yield return InputManager.Instance.WaitForInput();

        yield return PerformMultipleHits(combatLogic, playerAnimator, enemyAnimator);

        yield return MoveToPosition(originalPosition, playerAnimator);

        transform.LookAt(enemyPosition.position);

        yield return InputManager.Instance.WaitForInput();

        combatLogic.OnActionCompleted();
    }

    private Vector3 CalculateAttackPosition(Transform enemyPosition)
    {
        Vector3 direction = (enemyPosition.position - transform.position).normalized;
        return enemyPosition.position - direction * 1.5f;
    }

    private IEnumerator MoveToPosition(Vector3 targetPosition, Animator playerAnimator)
    {
        Vector3 startPosition = transform.position;
        float elapsedTime = 0f;

        transform.LookAt(targetPosition);

        playerAnimator.SetTrigger(runTriggerName);

        while (elapsedTime < duration)
        {
            transform.position = Vector3.Lerp(startPosition, targetPosition, elapsedTime / duration);

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.position = targetPosition;

        playerAnimator.SetTrigger(idleTriggerName);
    }

    private IEnumerator PerformMultipleHits(CombatLogic combatLogic, Animator playerAnimator, Animator enemyAnimator)
    {
        int hits = 0;
        int totalDamage = 0;
        bool keepAttacking = true;

        RhythmChecker checker = FindAnyObjectByType<RhythmChecker>();

        bat.SetActive(true);

        while (keepAttacking && hits < maxHits)
        {
            float offset = checker.CheckInputTiming();
            if ((checker.CheckIfLastBeat() || hits == 0) && Mathf.Abs(offset) <= tolerance)
            {
                float damageMultiplier = hits < hitStrengths.Length ? hitStrengths[hits] : 0.01f;
                int damage = combatLogic.enemyStats.TakeDamage(combatLogic.playerStats.offense + offenseBonus, attackStrength, damageMultiplier);
                totalDamage += damage;
                hits++;

                PlayHitAnimation(playerAnimator);


                CombatUIManager.Instance.ShowDialogueUI(true, $"Hit {hits}! {damage} damage dealt!");
            }
            else
            {
                keepAttacking = false;
                if (hits == 0)
                {
                    totalDamage = combatLogic.enemyStats.TakeDamage(combatLogic.playerStats.offense + offenseBonus, attackStrength);

                    PlayHitAnimation(playerAnimator);
                }
                else
                {
                    if (offset < 0)
                    {
                        audioSource.PlayOneShot(tooEarly);
                    }
                    else
                    {
                        audioSource.PlayOneShot(tooLate);
                    }
                }
            }

            yield return InputManager.Instance.WaitForInput();
        }

        bat.SetActive(false);

        CombatUIManager.Instance.ShowDialogueUI(true, $"You dealt {totalDamage} damage!");
    }
    private void PlayHitAnimation(Animator playerAnimator)
    {
        if (playerAnimator != null)
        {
            playerAnimator.SetTrigger(attackTriggerName);
        }
    }
    private Animator GetAnimator(GameObject enemyInstance)
    {
        Animator animator = enemyInstance.GetComponent<Animator>();
        if (animator == null)
        {
            animator = enemyInstance.GetComponentInChildren<Animator>();
        }
        return animator;
    }
}
