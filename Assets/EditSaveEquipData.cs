#if UNITY_EDITOR
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using Sirenix.OdinInspector;
using UnityEngine;

public class EditSaveEquipData : MonoBehaviour
{
    private const string TEXT_ASSET_PATH = "Data/equipment";
    private const string TEXT_ASSET_SAVE_PATH = "Resources/Data/equipment.json";
    private const string UI_VI_LANGUAGE_FILE_PATH = "Resources/Data/vi_equipment_title.yml";
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
    public void GetEquipmentNameByDefineId(int equipmentDefineId)
    {
        if (_dictionary.ContainsKey("equipment_title_" + equipmentDefineId))
        {
            Debug.Log($"<color=green>Equipment ID: {equipmentDefineId} Equipment Name: {_dictionary["equipment_title_" + equipmentDefineId]}</color>");
        }
        else
        {
            Debug.LogError($"Equipment ID: <color=green>{equipmentDefineId}</color> not found.");
        }
    }
    
    [Button]
    public void GetEquipmentDefineIdByExactName(string equipmentName)
    {
        List<string> keys = SearchKeyDictionaryByExactValue(equipmentName);
        if (keys is not {Count: > 0})
        {
            Debug.LogError($"Equipment <color=green>{equipmentName}</color> not found.");
            return;
        }

        foreach (string key in keys)
        {
            int equipmentId = int.Parse(key.Split("_")[^1]);
            Debug.Log($"<color=green>Equipment ID: {equipmentId} Equipment Name: {_dictionary[key]}</color>");
        }
    }
    
    [Button]
    public void GetEquipmentDefineId(string equipmentName)
    {
        List<string> keys = SearchKeyDictionaryByValue(equipmentName);
        if (keys is not {Count: > 0})
        {
            Debug.LogError($"Equipment <color=green>{equipmentName}</color> not found.");
            return;
        }

        foreach (string key in keys)
        {
            int equipmentId = int.Parse(key.Split("_")[^1]);
            Debug.Log($"<color=green>Equipment ID: {equipmentId} Equipment Name: {_dictionary[key]}</color>");
        }
    }
    
    [Button]
    public void EditMapObject(
        int equipmentDefineId,
        int mapObjectId)
    {
        EquipmentDefineData equipment = LoadTextAsset();
        string key = "equipment_title_" + equipmentDefineId;
        if (equipment is { Equipments: { Count: > 0 } } && _dictionary.TryGetValue(key, out string value))
        {
            bool isFound = false;
            foreach (EquipmentDefine item in equipment.Equipments)
            {
                if (item.DefineId == equipmentDefineId)
                {
                    isFound = true;
                    item.MapObjectId = (byte)mapObjectId;
                    break;
                }
            }
            
            string textPath = Path.Combine(Application.dataPath, TEXT_ASSET_SAVE_PATH);
            string json = JsonUtility.ToJson(equipment, true).Replace("    ", "  ");
            File.WriteAllText(textPath, json);
            
            string isDone = isFound ? "Done" : "Fail";
            Debug.Log($"<color=green>Write file {isDone}! Equipment ID: {equipmentDefineId} Name: {value}</color>");
        }
        else
        {
            Debug.LogError("Equipment data not found.");
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
    
    private EquipmentDefineData LoadTextAsset()
    {
        TextAsset textAsset = Resources.Load<TextAsset>(TEXT_ASSET_PATH);
        
        return JsonUtility.FromJson<EquipmentDefineData>(textAsset.text);
    }
}
#endif
