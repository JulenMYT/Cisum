using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CombatUIManager : MonoBehaviour
{
    public static CombatUIManager Instance { get; private set; }

    public TextMeshProUGUI healthText;
    public TextMeshProUGUI dialogueText;
    public GameObject combatUI;
    public GameObject dialogueUI;

    [SerializeField]
    private HealthBasedVignette vignetteEffect;

    [SerializeField]
    private Button[] combatButtons;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void ShowCombatUI(bool show)
    {
        string trigger = show ? "pop" : "unpop";
        combatUI.GetComponent<Animator>().SetTrigger(trigger);
    }

    public void ShowDialogueUI(bool show, string message = "")
    {
        dialogueUI.GetComponent<Animator>().SetBool("pop", show);
        if (show && !string.IsNullOrEmpty(message))
        {
            UpdateDialogue(message);
        }
    }
    public void ShowInventoryUI(bool show)
    {
        Inventory.Instance.UpdateUI();
        Inventory.Instance.inventoryUI.SetActive(show);
    }
    public void ShowItemDescription(string description)
    {
        Inventory.Instance.descriptionText.text = description;
        Inventory.Instance.descriptionText.gameObject.SetActive(true);
    }

    public void HideItemDescription()
    {
        Inventory.Instance.descriptionText.gameObject.SetActive(false);
    }
    public void UpdateDialogue(string message)
    {
        dialogueText.text = message;
    }

    public void UpdateHealth(CharacterStats playerStats)
    {
        healthText.text = $"HP: {playerStats.currentHealth}/{playerStats.maxHealth}";
        vignetteEffect.UpdateHealth(playerStats);
    }

    public void EnableButtons(bool enable)
    {
        foreach (Button button in combatButtons)
        {
            button.interactable = enable;
        }
    }
}
