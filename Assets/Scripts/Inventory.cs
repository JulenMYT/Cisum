using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Inventory : MonoBehaviour
{
    public List<Item> items = new List<Item>();
    public Transform buttonParent;
    public GameObject buttonPrefab;

    [SerializeField] CombatLogic combatLogic;
    [SerializeField] List<Item> startingItems;

    public GameObject inventoryUI;
    public TextMeshProUGUI descriptionText;

    public static Inventory Instance;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    private void Start()
    {
        foreach (var item in startingItems)
        {
            AddItem(item);
        }
        UpdateUI();
    }

    public void AddItem(Item newItem)
    {
        items.Add(newItem);
        UpdateUI();
    }

    public void UpdateUI()
    {
        if (combatLogic == null)
        {
            combatLogic = FindObjectOfType<CombatLogic>(); // Tente de trouver un CombatLogic dans la scène
            if (combatLogic == null)
            {
                Debug.LogError("CombatLogic is not assigned and could not be found in the scene.");
                return;
            }
        }
        foreach (Transform child in buttonParent)
        {
            Destroy(child.gameObject);
        }

        foreach (var item in items)
        {
            if (combatLogic.state == CombatState.PLAYERTURN || combatLogic.state == CombatState.WON && item.type == Item.ItemType.Heal)
            {
                GameObject button = Instantiate(buttonPrefab, buttonParent);
                button.GetComponentInChildren<TMP_Text>().text = $"    {item.itemName}";

                ItemButtonHover hoverScript = button.AddComponent<ItemButtonHover>();
                hoverScript.item = item;

                button.GetComponent<Button>().onClick.AddListener(() => OnItemButtonClicked(item));
            }
        }
    }

    private void OnItemButtonClicked(Item item)
    {
        if (combatLogic != null)
        {
            CharacterStats target = (item.type == Item.ItemType.Heal || item.type == Item.ItemType.Buff)
                ? combatLogic.playerStats
                : combatLogic.enemyStats;

            item.Use(target);

            items.Remove(item);
            
            UpdateUI();
            combatLogic.OnActionCompleted();

            CombatUIManager.Instance.ShowCombatUI(false);
            CombatUIManager.Instance.ShowInventoryUI(false);
            CombatUIManager.Instance.EnableButtons(false);
        }
    }

    public void OnGoBackButton()
    {
        CombatUIManager.Instance.ShowInventoryUI(false);
        CombatUIManager.Instance.EnableButtons(true);
    }
}