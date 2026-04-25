using System.Collections;
using UnityEngine;

[CreateAssetMenu(fileName = "PlayerStats", menuName = "Stats/PlayerStats")]
public class PlayerStatsScriptableObject : ScriptableObject
{
    [Header("Player Info")]
    public string playerName;

    [Header("Stats")]
    public int level;
    public int currentHealth;
    public int maxHealth;
    public int offense;
    public int defense;
    public int speed;
    public int vitality;

    [Header("Experience")]
    public int experiencePoints;
    public int experienceToNextLevel;

    [Header("Growth Rates")]
    public int offenseGrowthRate;
    public int defenseGrowthRate;
    public int speedGrowthRate;
    public int vitalityGrowthRate;

    public AudioClip levelUpClip;

    private static readonly System.Random random = new System.Random();

    // Tableau des XP nécessaires pour chaque niveau (1 à 20 inclus)
    private static readonly int[] xpTable =
    {
        0,    // Niveau 1
        4,    // Niveau 2
        13,   // Niveau 3
        27,   // Niveau 4
        65,  // Niveau 5
        127,  // Niveau 6
        213,  // Niveau 7
        323,  // Niveau 8
    };

    /// <summary>
    /// Ajoute de l'expérience et gère la montée en niveau si nécessaire.
    /// </summary>
    public IEnumerator AddExperience(int amount)
    {
        CombatUIManager.Instance.ShowDialogueUI(true, $"You gained {amount} experience.");

        yield return InputManager.Instance.WaitForInput();

        experiencePoints += amount;

        while (level < xpTable.Length && experiencePoints >= experienceToNextLevel)
        {
            yield return LevelUp();
        }
    }

    /// <summary>
    /// Gère la logique de montée en niveau.
    /// </summary>
    private IEnumerator LevelUp()
    {
        FindFirstObjectByType<PlayerBehaviour>().GetComponent<AudioSource>().PlayOneShot(levelUpClip);

        level++;
        experiencePoints -= experienceToNextLevel;
        CombatUIManager.Instance.ShowDialogueUI(true, $"{playerName} level up to {level} !");

        yield return InputManager.Instance.WaitForInput();

        CalculateNextLevelXP();
        yield return ApplyStatGains();
    }

    /// <summary>
    /// Calcule l'XP requis pour atteindre le niveau suivant.
    /// </summary>
    private void CalculateNextLevelXP()
    {
        if (level < xpTable.Length)
        {
            experienceToNextLevel = xpTable[level];
        }
        else
        {
            // Si le niveau dépasse les valeurs du tableau, on peut définir une progression par défaut
            experienceToNextLevel = Mathf.RoundToInt(100 * Mathf.Pow(level, 1.5f));
        }
    }

    /// <summary>
    /// Applique les gains de stats en fonction des taux de croissance et des formules.
    /// </summary>
    private IEnumerator ApplyStatGains()
    {
        // Offense
        int offenseGain = CalculateStatGain(offense, offenseGrowthRate);
        offense += offenseGain;
        if (offenseGain > 0)
        {
            yield return ShowStatGainMessage("Offense", offenseGain);
        }

        // Defense
        int defenseGain = CalculateStatGain(defense, defenseGrowthRate);
        defense += defenseGain;
        if (defenseGain > 0)
        {
            yield return ShowStatGainMessage("Defense", defenseGain);
        }

        // Speed
        int speedGain = CalculateStatGain(speed, speedGrowthRate);
        speed += speedGain;
        if (speedGain > 0)
        {
            yield return ShowStatGainMessage("Speed", speedGain);
        }

        // Vitalité (affecte les HP)
        int vitalityGain = CalculateStatGain(vitality, vitalityGrowthRate);
        vitality += vitalityGain;

        // Gains de HP en fonction de la vitalité
        int hpGain = CalculateHPGain(vitalityGain);

        maxHealth += hpGain;
        currentHealth += hpGain; // Optionnel : soigner le joueur en montant de niveau

        yield return ShowStatGainMessage("HP", hpGain);
        yield return InputManager.Instance.WaitForInput();
    }

    /// <summary>
    /// Calcule le gain pour une stat spécifique en fonction des formules.
    /// </summary>
    private int CalculateStatGain(int currentStat, int growthRate)
    {
        int baseIncrease = (growthRate * (level-1) - (currentStat - 2) * 10);

        // Déterminer r
        int r;
        if (level <= 10 && currentStat == vitality)
        {
            r = 5;
        }
        else if (level % 4 == 0)
        {
            r = random.Next(7, 11); // r dans [7, 10]
        }
        else
        {
            r = random.Next(3, 7); // r dans [3, 6]
        }

        return Mathf.Max(0, Mathf.RoundToInt(baseIncrease * r / 50f));
    }

    /// <summary>
    /// Calcule le gain de HP en fonction de la vitalité.
    /// </summary>
    private int CalculateHPGain(int vitalityGain)
    {
        int targetHP = 15 * vitalityGain;
        int gain = targetHP - maxHealth;

        // Gain minimum de 1-3
        if (gain < 2)
        {
            gain = random.Next(1, 4);
        }

        return gain;
    }

    private IEnumerator ShowStatGainMessage(string statName, int gain)
    {
        string message = $"{statName} goes up by {gain}!";
        CombatUIManager.Instance.ShowDialogueUI(true, message);
        yield return InputManager.Instance.WaitForInput();
    }
}
