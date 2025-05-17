
#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using UnityEditor.U2D;
using UnityEngine.U2D;
using System.IO;

public static class SpriteAtlasGenerator
{
    public static void CreateSpriteAtlas(string atlasSaveFolderPath, string folderAPath)
    {
        string atlasName = "atlas.spriteatlas";
        string fullPath = Path.Combine(Application.dataPath, atlasSaveFolderPath.Replace("Assets/",""));
        
        if (!Directory.Exists(fullPath))
        {
            Directory.CreateDirectory(fullPath);
        }

        AssetDatabase.Refresh();
        if (!AssetDatabase.IsValidFolder(folderAPath))
        {
            Debug.LogError("Invalid folder A path.");
            return;
        }

        SpriteAtlas atlas = new ();

        SpriteAtlasPackingSettings packingSettings = new ()
        {
            enableRotation = false,
            enableTightPacking = false,
            padding = 2
        };
        atlas.SetPackingSettings(packingSettings);

        SpriteAtlasTextureSettings textureSettings = new ()
        {
            readable = false,
            generateMipMaps = false,
            sRGB = true,
            filterMode = FilterMode.Bilinear
        };
        atlas.SetTextureSettings(textureSettings);

        Object folderObj = AssetDatabase.LoadAssetAtPath<Object>(folderAPath);
        atlas.Add(new[] { folderObj });

        string fullSavePath = Path.Combine(atlasSaveFolderPath, atlasName);
        AssetDatabase.CreateAsset(atlas, fullSavePath);

        string folderName = new DirectoryInfo(folderAPath).Name;
        AssetImporter importer = AssetImporter.GetAtPath(fullSavePath);
        if (importer != null)
        {
            importer.assetBundleName = $"mapobject_{folderName}";
            Debug.Log($"Assigned bundle name: mapobject_{folderName}");
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Debug.Log($"SpriteAtlas created at: {fullSavePath}");
    }
}
#endif