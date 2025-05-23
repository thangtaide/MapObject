
#if UNITY_EDITOR
using System.IO;
using Newtonsoft.Json;
using UnityEditor;
using UnityEngine;
using Formatting = Newtonsoft.Json.Formatting;

public enum DropState
{
    Falling,
    Landed,
}

public static class WriteTextAssetAnim
{
    public static MapObjectAnimationData WriteToAnimationData(ST_ObjDropResTextureData stObjDropResTextureData, ST_ObjDropResTextureData stObjDropResTextureData2)
    {
        int numDir = stObjDropResTextureData.nNumDir > stObjDropResTextureData2.nNumDir ? stObjDropResTextureData.nNumDir : stObjDropResTextureData2.nNumDir;
        
        MapObjectAnimationData basicAnimationData = new()
        {
            Directions = new BasicAnimationDirectionData[numDir],
            HasFallingAnimation = true,
            HasLandedAnimation = true
        };
        float duration = (float)stObjDropResTextureData.aStObjDropResFrameTextureData.Length / stObjDropResTextureData.nNumDir/ 9;
        float duration2 = (float)stObjDropResTextureData2.aStObjDropResFrameTextureData.Length / stObjDropResTextureData.nNumDir/ 9;
        
        int spriteCount = stObjDropResTextureData.aStObjDropResFrameTextureData.Length / stObjDropResTextureData.nNumDir;
        int spriteCount2 = stObjDropResTextureData2.aStObjDropResFrameTextureData.Length / stObjDropResTextureData.nNumDir;
        
        for (int i = 0; i < stObjDropResTextureData.nNumDir; i++)
        {
            basicAnimationData.Directions[i] = new BasicAnimationDirectionData
            {
                States = new BasicAnimationStateData[2],
            };
            basicAnimationData.Directions[i].States[0] = new BasicAnimationStateData
            {
                Duration = duration
            };
            basicAnimationData.Directions[i].States[1] = new BasicAnimationStateData
            {
                Duration = duration2
            };

            
            basicAnimationData.Directions[i].States[0].Frames = new BasicAnimationFrameData[spriteCount];
            basicAnimationData.Directions[i].States[1].Frames = new BasicAnimationFrameData[spriteCount2];
            
            for (int j = 0; j < spriteCount; j++)
            {
                basicAnimationData.Directions[i].States[0].Frames[j] = new BasicAnimationFrameData()
                {
                    SpriteName = $"{i}_{j}"
                };
            }
            
            for (int j = 0; j < spriteCount2; j++)
            {
                basicAnimationData.Directions[i].States[1].Frames[j] = new BasicAnimationFrameData()
                {
                    SpriteName = $"{i}_{j+spriteCount}"
                };
            }
        }
        return basicAnimationData;
    }
    
    public static MapObjectAnimationData WriteToAnimationData(ST_ObjDropResTextureData stObjDropResTextureData, DropState dropState)
    {
        MapObjectAnimationData basicAnimationData = new()
        {
            Directions = new BasicAnimationDirectionData[stObjDropResTextureData.nNumDir],
            HasLandedAnimation = dropState == DropState.Landed,
            HasFallingAnimation = dropState == DropState.Falling
        };
        float duration = (float)stObjDropResTextureData.aStObjDropResFrameTextureData.Length / stObjDropResTextureData.nNumDir/ 18;
        
        int spriteCount = stObjDropResTextureData.aStObjDropResFrameTextureData.Length / stObjDropResTextureData.nNumDir;
        
        for (int i = 0; i < stObjDropResTextureData.nNumDir; i++)
        {
            basicAnimationData.Directions[i] = new BasicAnimationDirectionData
            {
                States = new BasicAnimationStateData[2],
            };
            basicAnimationData.Directions[i].States[(int)dropState] = new BasicAnimationStateData
            {
                Duration = duration,
                Frames = new BasicAnimationFrameData[spriteCount]
            };

            for (int j = 0; j < spriteCount; j++)
            {
                basicAnimationData.Directions[i].States[(int)dropState].Frames[j] = new BasicAnimationFrameData()
                {
                    SpriteName = $"{i}_{j}"
                };
            }
        }
        return basicAnimationData;
    }

    public static void WriteToJsonFile<T>(
        T data,
        string filePath,
        bool needAddBundle = true)
    {
        string json = JsonConvert.SerializeObject(data, Formatting.Indented);

        string finalPath = filePath;
        if (needAddBundle)
        {
            finalPath = Path.Combine(filePath, "animation.json");
        }

        File.WriteAllText(finalPath, json);

        AssetDatabase.Refresh();
        if (needAddBundle)
        {
            string folderName = new DirectoryInfo(filePath).Name;
            string unityRelativePath = finalPath.Replace(Application.dataPath, "Assets").Replace("\\", "/");

            AssetImporter importer = AssetImporter.GetAtPath(unityRelativePath);
            if (importer != null)
            {
                importer.assetBundleName = $"mapobject_{folderName}";
                Debug.Log($"Assigned Text asset bundle name: mapobject_{folderName}");
            }
        }

        AssetDatabase.SaveAssets();
    }
}
#endif
