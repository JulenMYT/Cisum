using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public enum CombatState { START, PLAYERTURN, ENEMYTURN, WON, LOST }

public class CombatLogic : MonoBehaviour
{
    [Header("Instances")]
    public GameObject playerInstance;
    public GameObject enemyInstance;

    [Header("Positions")]
    public Transform playerPosition;
    public Transform enemyPosition;

    [Header("Stats")]
    public CharacterStats playerStats;
    public CharacterStats enemyStats;

    [Header("UI")]
    public CombatUIManager combatUI;

    [Header("Combat State")]
    public CombatState state;

    [Header("Miscellaneous")]
    [SerializeField] private CameraFade cameraFade;
    [SerializeField] private SkyboxManager skyboxManager;

    private bool actionDone = false;
    private ICombatAction currentAction;

    #region Initialization

    /// <summary>
    /// Initializes the combat system with the player and enemy prefabs.
    /// </summary>
    public void InitCombat(GameObject playerPrefab, GameObject enemyPrefab)
    {
        CameraManager.Instance.SwitchToCamera("IntroCamera");
        CombatUIManager.Instance.ShowInventoryUI(false);
        state = CombatState.START;
        StartCoroutine(Init(playerPrefab, enemyPrefab));
    }

    /// <summary>
    /// Sets up the combat scene by instantiating player and enemy instances.
    /// </summary>
    private IEnumerator Init(GameObject playerPrefab, GameObject enemyPrefab)
    {
        // Instantiate player and enemy
        playerInstance = Instantiate(playerPrefab, playerPosition);
        enemyInstance = Instantiate(enemyPrefab, enemyPosition);

        // Retrieve stats components
        playerStats = playerInstance.GetComponent<CharacterStats>();
        enemyStats = enemyInstance.GetComponent<CharacterStats>();

        // Orient instances
        playerInstance.transform.LookAt(enemyPosition);
        enemyInstance.transform.LookAt(playerPosition);

        int level = playerInstance.GetComponent<PlayerBehaviour>().playerData.level;

        skyboxManager.UpdateSkyboxColor(level);

        // Play battle music
        MusicManager.Instance.PlayMusic(enemyStats.enemyData.trackName);

        // Show encounter dialogue
        CombatUIManager.Instance.ShowDialogueUI(true, enemyStats.enemyData.encounterText);
        yield return new WaitForSeconds(2f);
        yield return InputManager.Instance.WaitForInput();

        // Update UI and start the combat loop
        combatUI.UpdateHealth(playerStats);
        state = CombatState.PLAYERTURN;
        StartCoroutine(CombatLoop());
    }

    #endregion

    #region Combat Loop

    /// <summary>
    /// Main combat loop that alternates between player and enemy turns.
    /// </summary>
    private IEnumerator CombatLoop()
    {
        while (state != CombatState.WON && state != CombatState.LOST)
        {
            switch (state)
            {
                case CombatState.PLAYERTURN:
                    yield return StartCoroutine(PlayerTurn());
                    break;

                case CombatState.ENEMYTURN:
                    yield return StartCoroutine(EnemyTurn());
                    break;
            }

            CheckCombatEndConditions();
        }

        StartCoroutine(EndCombat());
    }

    #endregion

    #region Player Turn

    /// <summary>
    /// Executes the player's turn.
    /// </summary>
    public IEnumerator PlayerTurn()
    {
        CombatUIManager.Instance.EnableButtons(true);
        CameraManager.Instance.SwitchToCamera("PlayerCamera");

        CombatUIManager.Instance.ShowDialogueUI(false);
        combatUI.ShowCombatUI(true);

        actionDone = false;
        while (!actionDone)
        {
            yield return null;
        }

        state = CombatState.ENEMYTURN;
        CheckCombatEndConditions();
    }

    /// <summary>
    /// Sets the current player action based on the chosen button.
    /// </summary>
    private void SetAction<T>() where T : Component, ICombatAction
    {
        CombatUIManager.Instance.EnableButtons(true);
        combatUI.ShowCombatUI(false);

        currentAction = playerInstance.GetComponent<T>();
        StartCoroutine(currentAction.Execute(this));
    }

    public void OnAttackButton() => SetAction<AttackAction>();
    public void OnShootButton() => SetAction<ShootAction>();
    public void OnGuardButton() => SetAction<GuardAction>();
    public void OnItemsButton()
    {
        playerInstance.GetComponent<ItemsAction>().Execute(this);
    }

    #endregion

    #region Enemy Turn

    /// <summary>
    /// Executes the enemy's turn.
    /// </summary>
    private IEnumerator EnemyTurn()
    {
        CameraManager.Instance.SwitchToCamera("EnemyCamera");

        yield return enemyInstance.GetComponent<EnemyBehaviour>().ExecuteEnemyTurn(this);
        yield return InputManager.Instance.WaitForInput();

        state = CombatState.PLAYERTURN;
        CheckCombatEndConditions();
    }

    #endregion

    #region Combat End

    /// <summary>
    /// Ends the combat based on the current state.
    /// </summary>
    private IEnumerator EndCombat()
    {
        if (state == CombatState.WON)
        {
            yield return HandleCombatWin();
        }
        else if (state == CombatState.LOST)
        {
            yield return HandleCombatLose();
        }
    }

    /// <summary>
    /// Handles the victory state.
    /// </summary>
    private IEnumerator HandleCombatWin()
    {
        int level = enemyInstance.GetComponent<EnemyBehaviour>().enemyData.level;
        string enemyName = enemyStats.characterName;
        Item dropItem = enemyStats.enemyData.dropItem;
        int xpGained = enemyStats.enemyData.experiencePoints;

        MusicManager.Instance.PlayMusic("Win");
        CombatUIManager.Instance.ShowDialogueUI(true, enemyStats.enemyData.deathText);
        Destroy(enemyInstance);
        yield return InputManager.Instance.WaitForInput();

        // Grant experience points
        PlayerStatsScriptableObject playerData = playerInstance.GetComponent<PlayerBehaviour>().playerData;
        yield return playerData.AddExperience(xpGained);
        playerData.currentHealth = playerStats.currentHealth;

        if (dropItem != null)
        {
            CombatUIManager.Instance.ShowDialogueUI(true, $"{ enemyName} dropped a {dropItem.itemName}.");
            Inventory.Instance.AddItem(dropItem);
        }
        yield return InputManager.Instance.WaitForInput();

        OnItemsButton();
        yield return InputManager.Instance.WaitForInput();

        CombatUIManager.Instance.ShowDialogueUI(true, "Let's go to the next fight");

        if (cameraFade != null)
        {
            cameraFade.FadeOut();
        }
        else
        {
            Debug.LogError("CameraFade reference is not set!");
        }

        yield return new WaitForSeconds(1.0f);
        playerData.currentHealth = playerStats.currentHealth;

        if (level == 7)
        {
            SceneManager.LoadScene("WinningScene");
        }

        // Reload the scene
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    /// <summary>
    /// Handles the defeat state.
    /// </summary>
    private IEnumerator HandleCombatLose()
    {
        CombatUIManager.Instance.ShowDialogueUI(true, "You were defeated...");
        yield return InputManager.Instance.WaitForInput();

        CombatUIManager.Instance.ShowDialogueUI(false);

        // Fade out camera if applicable
        if (cameraFade != null)
        {
            cameraFade.FadeOut();
        }
        else
        {
            Debug.LogError("CameraFade reference is not set!");
        }

        yield return new WaitForSeconds(1.0f);

        // Load Game Over scene
        SceneManager.LoadScene("GameOverScene");
    }

    #endregion

    #region Utility

    /// <summary>
    /// Checks if combat should end due to player or enemy death.
    /// </summary>
    private void CheckCombatEndConditions()
    {
        if (playerStats.IsDead())
        {
            state = CombatState.LOST;
        }
        else if (enemyStats.IsDead())
        {
            state = CombatState.WON;
        }
    }

    /// <summary>
    /// Marks the current action as completed.
    /// </summary>
    public void OnActionCompleted()
    {
        actionDone = true;
    }

    #endregion
}
