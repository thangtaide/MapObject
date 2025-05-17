#if UNITY_EDITOR
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
public enum PlatformTarget
{
    Android,
    IOS
}
public class LoadCutSprite : MonoBehaviour
{
    [SerializeField] private Image _image;
    [SerializeField] private float _updateInterval = 0.1f;
    
    private const string TEXT_ASSET_PATH = "textobjdropres/";
    private const string TEXTURE_2D_ASSET_PATH = "Sprites/";
    private const string RESULT_PATH = "Resources/MapObject/";
    private const string ATLAS_PATH = "Resources/Atlas/";
    private const string BUNDLE_PATH = "Assets/Resources/Atlas";
    private float _lastUpdate;
    private string _spritePath;
    private int _maxObject = 256;

    private int _currentLoaded = 0;

    [Button(ButtonSizes.Medium), GUIColor(0, 1, 0)]
    public void BuildAllBundle(PlatformTarget buildTarget, BuildAssetBundleOptions buildOptions = BuildAssetBundleOptions.ChunkBasedCompression)
    {
        string assetFolderPath = Application.dataPath;
        string projectFolderFullPath = Directory.GetParent(assetFolderPath)!.ToString();
        string projectFolderPath = projectFolderFullPath.Replace("\\", "/");
        
        string buildBundleTempPath = $"{projectFolderPath}/{BUNDLE_PATH}/{buildTarget.ToString()}";

        if (!Directory.Exists(buildBundleTempPath))
        {
            Directory.CreateDirectory(buildBundleTempPath);
        }

        AssetBundleManifest assetBundleManifest = default;

        if (buildTarget == PlatformTarget.Android)
        {
            assetBundleManifest = BuildPipeline.BuildAssetBundles(buildBundleTempPath,
                buildOptions,
                BuildTarget.Android);
        }
        else if (buildTarget == PlatformTarget.IOS)
        {
            assetBundleManifest = BuildPipeline.BuildAssetBundles(
                buildBundleTempPath,
                buildOptions,
                BuildTarget.iOS
            );
        }

        if (assetBundleManifest == default)
        {
            Debug.LogError("assetBundleManifest == default");
            return;
        }

        AssetDatabase.Refresh();
        EditorUtility.ClearProgressBar();
    }
    
    [Button]
    public void LoadSprite()
    {
        _currentLoaded = 0;
        TextAsset[] textAssets = Resources.LoadAll<TextAsset>(TEXT_ASSET_PATH);
        foreach (TextAsset textAsset in textAssets)
        {
            string objDropName = textAsset.name;
            Debug.Log($"<color=green>Start Load {objDropName}</color>");
            
            ST_ObjDropResTextureData textureData = LoadTextAsset(objDropName);
            _maxObject = textureData.aStObjDropResFrameTextureData.Length;
            Debug.Log("<color=green>Load Text Asset completed</color>");

            List<Texture2D> texture2DList = LoadTexture2D(textureData, objDropName);
            if (texture2DList == null || texture2DList.Count == 0)
            {
                Debug.LogError("texture2DList is null or empty");
                continue;
            }

            Debug.Log("<color=green>Load Texture 2D completed</color>");

            ExportSprites(textureData, texture2DList);
            Debug.Log($"<color=green>Export Sprites completed name: {objDropName} to {_currentLoaded - 1}</color>");
        }
    }

    [Button]
    public void LoadSpriteByName(string objDropName, DropState dropState, int editIndex = -1)
    {
        objDropName = objDropName.Split("_")[0];
        if (editIndex < 0)
        {
            string resourcesPath = Path.Combine(Application.dataPath, RESULT_PATH);
            if (Directory.Exists(resourcesPath))
            {
                string[] subFolders = Directory.GetDirectories(resourcesPath);
                _currentLoaded = subFolders.Length;
            }
            else
            {
                _currentLoaded = 0;
            }
        }
        else
        {
            _currentLoaded = editIndex;
        }

        ST_ObjDropResTextureData textureData = LoadTextAsset(objDropName);
        _maxObject = textureData.aStObjDropResFrameTextureData.Length;
        Debug.Log("<color=green>Load Text Asset completed</color>");

        List<Texture2D> texture2DList = LoadTexture2D(textureData, objDropName);
        if (texture2DList == null || texture2DList.Count == 0)
        {
            Debug.LogError("texture2DList is null or empty");
            return;
        }

        if (ExportSprites(textureData, texture2DList))
        {
            _currentLoaded++;
            
            string folderName = (_currentLoaded - 1).ToString();
            Debug.Log($"<color=green>Export Sprites completed name: {objDropName} to {folderName}</color>");
            
           string atlasPath = Path.Combine(Application.dataPath, ATLAS_PATH + folderName);
           string spritePath = Path.Combine(Application.dataPath, RESULT_PATH + folderName);
            SpriteAtlasGenerator.CreateSpriteAtlas(GetAssetPath(atlasPath), GetAssetPath(spritePath));
            MapObjectAnimationData mapObjectAnimationData = WriteTextAssetAnim.WriteToAnimationData(textureData, dropState);
            WriteTextAssetAnim.WriteToJsonFile(mapObjectAnimationData, atlasPath);
            
        }
    }
    
    [Button]
    public void LoadAllAnimation(string fallingObjDropName, string landedObjDropName, int editIndex = -1)
    {
        fallingObjDropName = fallingObjDropName.Split("_")[0];
        landedObjDropName = landedObjDropName.Split("_")[0];
        if (editIndex < 0)
        {
            string resourcesPath = Path.Combine(Application.dataPath, RESULT_PATH);
            if (Directory.Exists(resourcesPath))
            {
                string[] subFolders = Directory.GetDirectories(resourcesPath);
                _currentLoaded = subFolders.Length;
            }
            else
            {
                _currentLoaded = 0;
            }
        }
        else
        {
            _currentLoaded = editIndex;
        }

        ST_ObjDropResTextureData fallingDropTextureData = LoadTextAsset(fallingObjDropName);
        ST_ObjDropResTextureData landedDropTextureData = LoadTextAsset(landedObjDropName);
        Debug.Log("<color=green>Load Text Asset completed</color>");

        List<Texture2D> fallingTexture2DList = LoadTexture2D(fallingDropTextureData, fallingObjDropName);
        List<Texture2D> landedTexture2DList = LoadTexture2D(landedDropTextureData, landedObjDropName);
        if (fallingTexture2DList == null || fallingTexture2DList.Count == 0
            || landedTexture2DList == null || landedTexture2DList.Count == 0)
        {
            Debug.LogError("texture2DList is null or empty");
            return;
        }
        
        int countFallingPerDir = fallingDropTextureData.aStObjDropResFrameTextureData.Length / fallingDropTextureData.nNumDir;
        
        if (ExportSprites(fallingDropTextureData, fallingTexture2DList) &&
            ExportSprites(landedDropTextureData, landedTexture2DList, countFallingPerDir))
        {
            string folderName = _currentLoaded.ToString();
            Debug.Log($"<color=green>Export Sprites completed name: {fallingObjDropName} to {folderName}</color>");
            
           string atlasPath = Path.Combine(Application.dataPath, ATLAS_PATH + folderName);
           string spritePath = Path.Combine(Application.dataPath, RESULT_PATH + folderName);
            SpriteAtlasGenerator.CreateSpriteAtlas(GetAssetPath(atlasPath), GetAssetPath(spritePath));
            
            
            MapObjectAnimationData mapObjectAnimationData = WriteTextAssetAnim.WriteToAnimationData(fallingDropTextureData, landedDropTextureData);
            WriteTextAssetAnim.WriteToJsonFile(mapObjectAnimationData, atlasPath);
            
            AssetDatabase.Refresh();
            _currentLoaded++;
        }
    }

    private string GetAssetPath(string path)
    {
        return "Assets" + path.Replace(Application.dataPath, "").Replace("\\", "/");
    }
    
    private List<Texture2D> LoadTexture2D(ST_ObjDropResTextureData textureData, string objDropName)
    {
        List<Texture2D> result = new ();
        HashSet<int> textureSet = new ();
        for (int index = 0; index < textureData.aStObjDropResFrameTextureData.Length; index++)
        {
            ST_ObjDropResFrameTextureData nti = textureData.aStObjDropResFrameTextureData[index];
            if (textureSet.Add(nti.nTi))
            {
                Texture2D texture2D = Resources.Load<Texture2D>(TEXTURE_2D_ASSET_PATH + objDropName + "_" + nti.nTi);
                if (texture2D == null)
                {
                    return null;
                }

                result.Add(texture2D);
            }
        }

        return result;
    }
    
    private ST_ObjDropResTextureData LoadTextAsset(string objDropName)
    {
        TextAsset textAsset = Resources.Load<TextAsset>(TEXT_ASSET_PATH + objDropName);
        
        return JsonUtility.FromJson<ST_ObjDropResTextureData>(textAsset.text);
    }
    
    private bool ExportSprites(ST_ObjDropResTextureData textureData, List<Texture2D> atlasList, int indexCustom = 0)
    {
        if (textureData.aStObjDropResFrameTextureData == null || atlasList == null)
        {
            Debug.LogError("textureData or atlas is null");
            return false;
        }

        string fullPath = Path.Combine(Application.dataPath, RESULT_PATH + _currentLoaded + "/");
        
        if (!Directory.Exists(fullPath))
        {
            Directory.CreateDirectory(fullPath);
        }

        bool isSuccess = false;
        int countPerDir = textureData.aStObjDropResFrameTextureData.Length / textureData.nNumDir;
        
        textureData.aStObjDropResFrameTextureData
            .Select((frameData, index) => new { frameData, index })
            .ToList()
            .ForEach(item =>
            {
                ST_ObjDropResFrameTextureData frame = item.frameData;
                Rect rect = new (frame.nX, frame.nY, frame.nW, frame.nH);
                
                Texture2D frameTex = new ((int)rect.width, (int)rect.height);
                frameTex.SetPixels(atlasList[item.frameData.nTi].GetPixels((int)rect.x, (int)rect.y, (int)rect.width, (int)rect.height));
                frameTex.Apply();

                string assetRelativePath = Path.Combine(fullPath, $"{item.frameData.nTi}_{item.index % countPerDir + indexCustom}.png");
                File.WriteAllBytes(assetRelativePath, frameTex.EncodeToPNG());
                DestroyImmediate(frameTex);
                
#if UNITY_EDITOR
                AssetDatabase.Refresh();
                AssetDatabase.ImportAsset(assetRelativePath, ImportAssetOptions.ForceUpdate);
                
                string assetPath = "Assets" + assetRelativePath.Replace(Application.dataPath, "").Replace("\\", "/");
                TextureImporter importer = AssetImporter.GetAtPath(assetPath) as TextureImporter;
                if (importer != null)
                {
                    importer.textureType = TextureImporterType.Sprite;
                    importer.spriteImportMode = SpriteImportMode.Single;
                    importer.spritePixelsPerUnit = 100;
                    importer.mipmapEnabled = false;
                    importer.alphaIsTransparency = true;
                    
                    importer.spritePivot = new Vector2(frame.fPx, frame.fPy);
                    TextureImporterSettings textureSetting = new ();
                    importer.ReadTextureSettings(textureSetting);
                    
                    textureSetting.spriteMeshType = SpriteMeshType.FullRect;
                    textureSetting.spriteAlignment = (int)SpriteAlignment.Custom;
                    
                    importer.SetTextureSettings(textureSetting);
                    
                    importer.SaveAndReimport();
                    
                    isSuccess = true;
                }
            });
        AssetDatabase.Refresh();
        
        return isSuccess;
#endif
    }

    private void Update()
    {
        // if (Time.time - _lastUpdate > _updateInterval)
        // {
        //     _lastUpdate = Time.time;
        //     if (_isLoad)
        //     {
        //         _index++;
        //         if(_index >= _maxObject)
        //         {
        //             _index = 0;
        //         }
        //         
        //         _image.sprite = Resources.Load<Sprite>(_spritePath + _index);
        //     }
        // }
    }
}
#endif