using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[DefaultExecutionOrder(-100)]
public class UpdateManager : MonoBehaviour
{
    public static UpdateManager Instance { get; private set; }

    private readonly List<ICustomUpdate> updatables = new();

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        // Suscribirse para limpiar al cargar cualquier escena
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Si no es la GameScene, borramos todo (evita referencias muertas)
        if (scene.name != "GameScene")
        {
            updatables.Clear();
        }
    }

    public void Register(ICustomUpdate obj)
    {
        if (obj != null && !updatables.Contains(obj))
            updatables.Add(obj);
    }

    public void Unregister(ICustomUpdate obj)
    {
        if (obj != null)
            updatables.Remove(obj);
    }

    void LateUpdate()
    {
        float deltaTime = Time.deltaTime;
        // Recorremos una copia para permitir unregister desde dentro
        var copy = updatables.ToArray();
        foreach (var u in copy)
        {
            if (u != null)
                u.CustomUpdate(deltaTime);
            else
                updatables.Remove(u);
        }
    }

    void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
}
