using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance;
    private HashSet<string> _items = new HashSet<string>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            LoadInventory();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public bool HasItem(string itemId) => _items.Contains(itemId);

    public void AddItem(string itemId) {
        if (!_items.Add(itemId)) return;
        Debug.Log($"[Inventário] Coletou: {itemId}");
        SaveInventory();
        DialoguePanel.Instance.Show("Item Coletado", $"Você obteve: {itemId}");
    }

    public void RemoveItem(string itemId) {
        if (!_items.Remove(itemId)) return;
        Debug.Log($"[Inventário] Removeu: {itemId}");
        SaveInventory();
    }

    private void SaveInventory()
    {
        PlayerPrefs.SetString("Inventory", string.Join(",", _items));
        PlayerPrefs.Save();
    }

    private void LoadInventory()
    {
        var savedData = PlayerPrefs.GetString("Inventory", "");
        if (string.IsNullOrEmpty(savedData)) return;
        var loadedItems = savedData.Split(',');
        _items = new HashSet<string>(loadedItems);
    }

    public IEnumerable<string> GetAllItems() => _items;
}