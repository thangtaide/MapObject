#if UNITY_EDITOR
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using Sirenix.OdinInspector;
using UnityEngine;

public class EditSaveItemData : MonoBehaviour
{
    private const string TEXT_ASSET_PATH = "Data/item";
    private const string TEXT_ASSET_SAVE_PATH = "Resources/Data/item.json";
    private const string UI_VI_LANGUAGE_FILE_PATH = "Resources/Data/vi_item_title.yml";
    private static readonly Dictionary<string, string> _dictionary = new ();
    
    private void Start()
    {
        string[] lines = File.ReadAllLines(Path.Combine(Application.dataPath, UI_VI_LANGUAGE_FILE_PATH));
        for (int lineNumber = 0; lineNumber < lines.Length; lineNumber++)
        {
            string[] word = SplitLine(lines[lineNumber]);
            if (word == null || word.Length < 2 || string.IsNullOrEmpty(word[0]) || string.IsNullOrEmpty(word[1]))
            {
            }
            else if (_dictionary.ContainsKey(word[0]))
            {
            }
            else
            {
                _dictionary.Add(word[0], word[1]);
            }
        }
    }

    private static string[] SplitLine(string line)
    {
        string[] words =  { "", "" };
        char separator = ':';

        int index = line.IndexOf(separator);

        if (index != -1)
        {
            words[0] = line[..index];
            words[1] = line[(index + 2)..];
        }

        return words;
    }

    [Button]
    public void GetItemNameByDefineId(int itemDefineId)
    {
        if (_dictionary.ContainsKey("item_title_" + itemDefineId))
        {
            Debug.Log($"<color=green>Item ID: {itemDefineId} Item Name: {_dictionary["item_title_" + itemDefineId]}</color>");
        }
        else
        {
            Debug.LogError($"Item ID: <color=green>{itemDefineId}</color> not found.");
        }
    }
    
    [Button]
    public void GetItemDefineIdByExactName(string itemName)
    {
        List<string> keys = SearchKeyDictionaryByExactValue(itemName);
        if (keys is not {Count: > 0})
        {
            Debug.LogError($"Item <color=green>{itemName}</color> not found.");
            return;
        }

        foreach (string key in keys)
        {
            int equipmentId = int.Parse(key.Split("_")[^1]);
            Debug.Log($"<color=green>Item ID: {equipmentId} Item Name: {_dictionary[key]}</color>");
        }
    }
    
    [Button]
    public void GetItemDefineId(string itemName)
    {
        List<string> keys = SearchKeyDictionaryByValue(itemName);
        if (keys is not {Count: > 0})
        {
            Debug.LogError($"Item <color=green>{itemName}</color> not found.");
            return;
        }

        foreach (string key in keys)
        {
            int itemId = int.Parse(key.Split("_")[^1]);
            Debug.Log($"<color=green>Item ID: {itemId} Item Name: {_dictionary[key]}</color>");
        }
    }
    
    [Button]
    public void EditMapObject(
        int itemDefineId,
        int mapObjectId)
    {
        ItemDefineData itemDefineData = LoadTextAsset();
        string key = "item_title_" + itemDefineId;
        if (itemDefineData is { Items: { Count: > 0 } } && _dictionary.TryGetValue(key, out string value))
        {
            bool isFound = false;
            foreach (ItemDefine item in itemDefineData.Items)
            {
                if (item.DefineId == itemDefineId)
                {
                    isFound = true;
                    item.MapObjectId = (byte)mapObjectId;
                    break;
                }
            }
            
            string textPath = Path.Combine(Application.dataPath, TEXT_ASSET_SAVE_PATH);
            string json = JsonUtility.ToJson(itemDefineData, true).Replace("    ", "  ");
            File.WriteAllText(textPath, json);
            
            string isDone = isFound ? "Done" : "Fail";
            Debug.Log($"<color=green>Write file {isDone}! Item ID: {itemDefineId} Name: {value}</color>");
        }
        else
        {
            Debug.LogError("Item data not found.");
        }

    }
    
    private List<string> SearchKeyDictionaryByValue(string keyword)
    {
        keyword = keyword.ToLowerInvariant();

        return _dictionary
            .Where(pair => pair.Value != null && pair.Value.ToLowerInvariant().Contains(keyword))
            .Select(valuePair => valuePair.Key).ToList();
    }

    private List<string> SearchKeyDictionaryByExactValue(string keyword)
    {
        keyword = keyword.ToLowerInvariant();

        return _dictionary
            .Where(pair => pair.Value != null && pair.Value.ToLowerInvariant().Equals(keyword))
            .Select(keyValue => keyValue.Key).ToList();
    }
    
    private ItemDefineData LoadTextAsset()
    {
        TextAsset textAsset = Resources.Load<TextAsset>(TEXT_ASSET_PATH);
        
        return JsonUtility.FromJson<ItemDefineData>(textAsset.text);
    }
}
#endif
