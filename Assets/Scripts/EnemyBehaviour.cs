using System.Collections;
using UnityEngine;

public class EnemyBehaviour : MonoBehaviour
{
    public EnemyScriptableObject enemyData;
    public Animator enemyAnimator;
    public CharacterStats characterStats;
    private int cycleIndex = 0;

    private void Start()
    {
        characterStats.UpdateCharacterData(enemyData.name, enemyData.hp, enemyData.hp, enemyData.offense, enemyData.defense, enemyData.level);
    }

    public IEnumerator ExecuteEnemyTurn(CombatLogic combatLogic)
    {
        var selectedAction = ChooseAction();

        CombatUIManager.Instance.ShowDialogueUI(true, selectedAction.attackMessage);
        yield return InputManager.Instance.WaitForInput();

        if (selectedAction.actionID == 1)
        {
            characterStats.Heal(selectedAction.argument);
            CombatUIManager.Instance.ShowDialogueUI(true, $"{characterStats.characterName} heals {selectedAction.argument} HP.");
        }

        CameraManager.Instance.SwitchToCamera("FightCamera");

        CombatUIManager.Instance.ShowDialogueUI(false);

        if (!string.IsNullOrEmpty(selectedAction.attackTrigger))
        {
            enemyAnimator.SetTrigger(selectedAction.attackTrigger);

            yield return new WaitForSeconds(selectedAction.hitTiming);

            GetComponent<AudioSource>().PlayOneShot(selectedAction.audioClip);
            combatLogic.playerInstance.GetComponentInChildren<Animator>().SetTrigger("damaged");
            int damage = combatLogic.playerStats.TakeDamage(characterStats.offense, selectedAction.argument);

            CombatUIManager.Instance.ShowDialogueUI(true, $"You took {damage} damage!");
            combatLogic.combatUI.UpdateHealth(combatLogic.playerStats);
        }
    }

    private EnemyScriptableObject.EnemyAction ChooseAction()
    {
        switch (enemyData.actionOrder)
        {
            case EnemyScriptableObject.ActionOrder.RANDOM:
                return ChooseRandomAction();

            case EnemyScriptableObject.ActionOrder.WEIGHTED_RANDOM:
                return ChooseWeightedRandomAction();

            case EnemyScriptableObject.ActionOrder.CYCLE:
                return ChooseCycledAction();

            case EnemyScriptableObject.ActionOrder.STAGGERED:
                return ChooseStaggeredAction();

            default:
                return ChooseRandomAction();
        }
    }

    private EnemyScriptableObject.EnemyAction ChooseRandomAction()
    {
        int index = Random.Range(0, enemyData.actions.Count);
        return enemyData.actions[index];
    }

    private EnemyScriptableObject.EnemyAction ChooseWeightedRandomAction()
    {
        float[] weights;

        if (enemyData.actions.Count == 2)
        {
            weights = new float[] { 0.70f, 0.30f };
        }
        else if (enemyData.actions.Count == 3)
        {
            weights = new float[] { 0.5f, 0.3f, 0.2f };
        }
        else if (enemyData.actions.Count == 4)
        {
            weights = new float[] { 0.5f, 0.25f, 0.125f, 0.125f };
        }
        else
        {
            return enemyData.actions[0];
        }

        float randomValue = Random.Range(0f, 1f);
        float cumulativeWeight = 0f;

        for (int i = 0; i < weights.Length; i++)
        {
            cumulativeWeight += weights[i];
            if (randomValue < cumulativeWeight)
            {
                return enemyData.actions[i];
            }
        }

        return enemyData.actions[0];
    }


    private EnemyScriptableObject.EnemyAction ChooseCycledAction()
    {
        var action = enemyData.actions[cycleIndex];
        cycleIndex = (cycleIndex + 1) % enemyData.actions.Count;
        return action;
    }

    private EnemyScriptableObject.EnemyAction ChooseStaggeredAction()
    {
        EnemyScriptableObject.EnemyAction action;

        if (enemyData.actions.Count == 3)
        {
            if (cycleIndex % 2 == 0)
            {
                action = enemyData.actions[0];
            }
            else
            {
                action = enemyData.actions[Random.Range(1, 3)];
            }
        }
        else if (enemyData.actions.Count == 4)
        {
            if (cycleIndex % 2 == 0)
            {
                action = enemyData.actions[Random.Range(0, 2)];
            }
            else
            {
                action = enemyData.actions[Random.Range(2, 4)];
            }
        }
        else
        {
            throw new System.Exception("Invalid number of actions in enemyData.actions");
        }

        cycleIndex++;
        return action;
    }
}
