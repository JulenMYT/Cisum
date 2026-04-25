using System.Collections.Generic;
using UnityEngine;

public class CombatManager : MonoBehaviour
{
    [Header("Combat Logic")]
    public CombatLogic combatLogic;

    [Header("Prefabs")]
    public GameObject playerPrefab;
    public List<GameObject> enemyPrefabs;

    [Header("Enemy Selection")]
    [Tooltip("Specify an enemy prefab to use, or leave empty for a random enemy.")]
    public GameObject selectedEnemyPrefab; // Permet de choisir un ennemi depuis l'Inspecteur

    private GameObject SelectEnemyBasedOnLevel(int playerLevel)
    {
        if (playerLevel == 7)
        {
            // Chercher le boss pour le niveau 8
            GameObject boss = enemyPrefabs.Find(enemy =>
            {
                var enemyData = enemy.GetComponent<EnemyBehaviour>().enemyData;
                return enemyData != null && enemyData.level == 7;
            });

            if (boss != null)
            {
                return boss;
            }
            else
            {
                Debug.LogWarning("No boss found for level 7!");
                return null;
            }
        }
        else
        {
            // Créer une liste pondérée
            List<GameObject> weightedEnemies = new List<GameObject>();

            foreach (var enemy in enemyPrefabs)
            {
                var enemyData = enemy.GetComponent<EnemyBehaviour>().enemyData;
                if (enemyData == null)
                {
                    Debug.LogWarning($"Enemy prefab {enemy.name} is missing an EnemyData component!");
                    continue;
                }

                int enemyLevel = enemyData.level;

                // Ajouter des pondérations : plus de chances pour le même niveau, moins pour ±1
                if (enemyLevel == playerLevel)
                {
                    // Ajouter plusieurs occurrences pour augmenter la probabilité
                    for (int i = 0; i < 1; i++) // Par exemple, 5 occurrences pour le même niveau
                    {
                        weightedEnemies.Add(enemy);
                    }
                }
                else if (enemyLevel == playerLevel - 1 || enemyLevel == playerLevel + 1 && enemyLevel !=7)
                {
                    // Ajouter moins d'occurrences pour les niveaux voisins
                    weightedEnemies.Add(enemy);
                }
            }

            if (weightedEnemies.Count > 0)
            {
                // Sélectionner un ennemi aléatoire dans la liste pondérée
                int randomIndex = Random.Range(0, weightedEnemies.Count);
                return weightedEnemies[randomIndex];
            }
            else
            {
                Debug.LogWarning("No enemies found within the valid level range!");
                return null;
            }
        }
    }

    public void Start()
    {
        if (combatLogic != null)
        {
            int playerLevel = playerPrefab.GetComponent<PlayerBehaviour>().playerData.level;

            GameObject enemyToUse = selectedEnemyPrefab != null
                ? selectedEnemyPrefab
                : SelectEnemyBasedOnLevel(playerLevel);

            if (enemyToUse != null)
            {
                combatLogic.InitCombat(playerPrefab, enemyToUse);
            }
            else
            {
                Debug.LogError("Failed to start combat: no valid enemy prefab.");
            }
        }
        else
        {
            Debug.LogError("CombatLogic is not assigned to CombatManager!");
        }
    }
}
