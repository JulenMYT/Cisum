using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;
public class CharacterStats : MonoBehaviour
{
    public string characterName;
    public int level;
    public int currentHealth;
    public int maxHealth;
    public int offense;
    public int defense;
    public int defenseBonus;

    public float defenseMultiplier = 1f;

    public EnemyScriptableObject enemyData;

    private Renderer characterRenderer;
    private Material characterMaterial;
    private Material flashMaterial;

    [SerializeField]
    private Animator animator;

    [SerializeField]
    private GameObject damageEffectPrefab;
    [SerializeField]
    private GameObject healingEffectPrefab;

    [SerializeField]
    private AudioClip healingEffectAudioClip;

    private Transform effectSpawnPoint;

    private void Start()
    {
        characterRenderer = GetComponentInChildren<Renderer>();
        characterMaterial = characterRenderer.material;
        flashMaterial = new Material(characterMaterial);
        flashMaterial.color = Color.red;

        effectSpawnPoint = transform;
    }
    public int TakeDamage(int amount, float multiplier = 1f, float comboMultiplier = 1f, bool absolute = false)
    {
        StartCoroutine(DamagedAnimation());
        PlayDamageEffect();
        ScreenShake(multiplier);

        if (!absolute)
        {
            float damageAfterDefense = Mathf.Max(1f, multiplier * amount - (defense + defenseBonus));

            float variability = Random.Range(0.75f, 1.25f);
            int damageAfterMultiplier = Mathf.Max(1, Mathf.RoundToInt(damageAfterDefense * variability));
            int finalDamage = Mathf.Max(1, Mathf.RoundToInt(damageAfterMultiplier / defenseMultiplier));

            int comboDamage = Mathf.Max(1, Mathf.RoundToInt(finalDamage * comboMultiplier));

            currentHealth -= comboDamage;

            currentHealth = Mathf.Max(0, currentHealth);

            return comboDamage;
        }
        else
        {
            currentHealth -= amount;

            currentHealth = Mathf.Max(0, currentHealth);

            return amount;
        }
    }

    public void Heal(int amount)
    {
        PlayHealingEffect();

        currentHealth += amount;

        currentHealth = Mathf.Min(maxHealth, currentHealth);
    }

    public bool IsDead()
    {
        return currentHealth <= 0;
    }
    private IEnumerator DamagedAnimation()
    {
        animator.SetTrigger("damaged");

        characterRenderer.material = flashMaterial;

        yield return new WaitForSeconds(0.1f);

        characterRenderer.material = characterMaterial; 
    }

    private void PlayDamageEffect()
    {
        if (damageEffectPrefab != null)
        {
            GameObject effect = Instantiate(damageEffectPrefab, effectSpawnPoint.position, Quaternion.identity);

            Destroy(effect, 2f);
        }
    }
    private void PlayHealingEffect()
    {
        GetComponent<AudioSource>().PlayOneShot(healingEffectAudioClip);

        if (damageEffectPrefab != null)
        {
            GameObject effect = Instantiate(healingEffectPrefab, effectSpawnPoint.position, Quaternion.identity);

            Destroy(effect, 2f);
        }
    }

    private void ScreenShake(float strength)
    {
        CinemachineImpulseSource impulseSource = GetComponent<CinemachineImpulseSource>();
        if (impulseSource != null)
        {
            impulseSource.GenerateImpulseWithForce(strength * 5);
        }
    }
    public void UpdateCharacterData(string _name, int _maxHealth, int _currentHealth, int _offense, int _defense, int _level)
    {
        characterName = _name;
        maxHealth = _maxHealth;
        currentHealth = _currentHealth;
        offense = _offense;
        defense = _defense;
        level = _level;
    }
}
