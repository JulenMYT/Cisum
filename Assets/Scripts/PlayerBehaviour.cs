using System.Collections;
using UnityEngine;

public class PlayerBehaviour : MonoBehaviour
{
    public PlayerStatsScriptableObject playerData;
    public CharacterStats characterStats;

    private void Start()
    {
        characterStats.UpdateCharacterData(playerData.name, playerData.maxHealth, playerData.currentHealth, playerData.offense, playerData.defense, playerData.level);
    }
}