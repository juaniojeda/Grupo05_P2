using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceLocations;

[DefaultExecutionOrder(-200)]
public class AssetsManager : MonoBehaviour
{
    public static AssetsManager Instance { get; private set; }

    [Header("Addressable References")]
    [Tooltip("Arrastra aquí todos los AssetReferences que quieras cargar")]
    [SerializeField] private List<AssetReference> assetReferences;

    [Header("Remote Settings (Opcional)")]
    [Tooltip("Si está activado, redirige bundles a cloudURL en lugar de localURL.")]
    [SerializeField] private bool useRemoteAssets = true;
    [SerializeField] private string localURL = "http://localhost:3000/";
    [SerializeField] private string cloudURL = "https://myserver.com/";

    [Header("Levels")]
    [Tooltip("Arrastra aquí los AssetReferences de tus prefabs de nivel")]
    [SerializeField] private List<AssetReference> levelReferences;

   
    public event Action OnLoadComplete;

  
    private Dictionary<string, GameObject> loadedAssets = new Dictionary<string, GameObject>();

    private void Awake()
    {
    
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        if (useRemoteAssets)
            Addressables.ResourceManager.InternalIdTransformFunc += ChangeAssetUrlToPrivateServer;

 
        StartCoroutine(LoadAssetsCoroutine());
    }

    private IEnumerator LoadAssetsCoroutine()
    {
    
        var allRefs = assetReferences.Concat(levelReferences).ToList();          
        int total = allRefs.Count;                                              
        int loaded = 0;

      
        foreach (var reference in allRefs)
        {
            var handle = reference.LoadAssetAsync<GameObject>();
            yield return handle;

            if (handle.Status == AsyncOperationStatus.Succeeded)
            {
       
                string key = handle.Result.name;                                   
                loadedAssets[key] = handle.Result;
                loaded++;
            }
            else
            {
                Debug.LogError($"AssetsManager: Error cargando '{reference.RuntimeKey}': {handle.OperationException}");
            }
        }


        if (loaded == total)                                                     
            OnLoadComplete?.Invoke();
    }

    private string ChangeAssetUrlToPrivateServer(IResourceLocation location)
    {
        string url = location.InternalId;
        if (url.StartsWith(localURL, StringComparison.OrdinalIgnoreCase))
            url = url.Replace(localURL, cloudURL);
        return url;
    }

    public void SubscribeOnLoadComplete(Action callback)
    {
        OnLoadComplete += callback;
    }

    public GameObject GetInstance(string assetName)
    {
        if (loadedAssets.TryGetValue(assetName, out var prefab))
            return Instantiate(prefab);

        Debug.LogError($"AssetsManager: Asset '{assetName}' no encontrado.");
        return null;
    }
}
