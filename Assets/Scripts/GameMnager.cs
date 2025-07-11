 // GameManager.cs
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using YourGame.Utilities;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using System.Linq;

[DefaultExecutionOrder(-100)]
public class GameManager : MonoBehaviour, ICustomUpdate, ITimeProvider
{
    public static GameManager Instance { get; private set; }
    private readonly List<ICustomUpdate> updatables = new();
    private bool _hasInitializedOnce = false;
    public PaddleController PaddleController => paddleController;

    #region Audio
    [Header("Audio Mixer")]
    [SerializeField] private AudioMixer mixer;
    [Header("Sliders")]
    [SerializeField] private Slider[] musicSliders;
    [SerializeField] private Slider[] sfxSliders;
    [Header("Audio Sources")]
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioSource sfxSource;
    [Header("Clips de SFX")]
    [SerializeField] private AudioClip paddleBounceClip;
    [SerializeField] private AudioClip wallBounceClip;
    [SerializeField] private AudioClip powerUpClip;
    [SerializeField] private AudioClip brickBreakClip;

    public bool TryGetMixerVolume(string parameterName, out float dB)
        => mixer.GetFloat(parameterName, out dB);

    public void SetMusicVolumeFromSlider(float v)
        => mixer.SetFloat("MUSICVolume", Mathf.Lerp(-80f, 0f, v));

    public void SetSFXVolumeFromSlider(float v)
        => mixer.SetFloat("SFXVolume", Mathf.Lerp(-80f, 0f, v));

    public void PlayPaddleBounce() { if (paddleBounceClip != null) sfxSource.PlayOneShot(paddleBounceClip); }
    public void PlayWallBounce() { if (wallBounceClip != null) sfxSource.PlayOneShot(wallBounceClip); }
    public void PlayPowerUp() { if (powerUpClip != null) sfxSource.PlayOneShot(powerUpClip); }
    public void PlayBrickBreak() { if (brickBreakClip != null) sfxSource.PlayOneShot(brickBreakClip); }
    #endregion

    #region Parallax Settings (Addressables)
    [Header("Parallax Layers")]
    [Tooltip("Addressable references for each parallax layer, in drawing order")]
    [SerializeField] private AssetReferenceGameObject[] parallaxReferences;
    [Tooltip("Movement factor per layer (0–1), matching parallaxReferences")]
    [SerializeField] private float[] parallaxFactors;
    private readonly List<Transform> parallaxLayers = new();
    private Vector3 prevPaddlePosition;
    #endregion

    #region Game Flow
    [Header("Levels")]
    [SerializeField] private Transform levelRoot;
    [Header("Containers")]
    [SerializeField] private Transform bricksContainer;
    [Header("Level Flow")]
    [SerializeField] private int totalLevels = 10;
    [SerializeField] private int startingLevel = 1;
    private int currentLevel;
    private LevelController levelController;
    #endregion

    #region Ball
    [Header("Ball Settings")]
    [SerializeField] private BallData ballData;
    private BallPool ballPool;
    private BallController _mainBallController;
    #endregion

    #region Bricks
    [Header("Bricks")]
    //[SerializeField] private GameObject brickPrefab;
    [SerializeField] private BrickGeneratorConfig generatorConfig;
    [SerializeField] private Transform brickStartPosition;
    private BrickPool brickPool;
    private BrickGenerator brickGenerator;
    private Dictionary<GameObject, int> _brickHitCounts;
    #endregion

    #region Atlas
    [Header("Atlas Settings")]
    [SerializeField] private float atlasColumns = 4f;
    [SerializeField] private float atlasRows = 4f;
    [SerializeField] private bool randomizeIndex = true;
    [SerializeField] private float fixedIndex = 0f;
    #endregion

    #region Paddle
    [Header("Paddle")]
    [SerializeField] private Transform paddleTransform;
    [SerializeField] private PaddleData paddleData;
    private PaddleController paddleController;
    #endregion

    #region UI
    [Header("UI")]
    [SerializeField] private UIManager uiManager;
    [SerializeField] private TextMeshProUGUI timerText;
    #endregion

    #region PowerUps
    [Header("PowerUps (Scriptable)")]
    [SerializeField] private PowerUpData multiballData;
    [SerializeField] private PowerUpData speedUpData;
    [SerializeField] private PowerUpData timeExtensionData;
    #endregion

    private bool gameEnded;
    private bool loadingScene;
    private float timer, timeLimit = 120f, endDelay = 2f, endTimer;
    private string targetScene;

    private void Awake()
    {
        // Si esto NO es la GameScene, te destruyes
        if (SceneManager.GetActiveScene().name != "GameScene")
        {
            Destroy(gameObject);
            return;
        }

        // En GameScene te conviertes en singleton y persistes mientras siga en esa escena
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        InitAudio();
        AssetsManager.Instance.SubscribeOnLoadComplete(OnAssetsLoaded);
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name != "GameScene")
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
            Destroy(gameObject);
            return;
        }

        // 1) Reset global y limpiar sistemas previos
        Time.timeScale = 1f;
        updatables.Clear();
        foreach (var layer in parallaxLayers)
            if (layer != null)
                Destroy(layer.gameObject);
        parallaxLayers.Clear();

        // 2) Re-asignar referencias de escena
        paddleTransform = GameObject.FindWithTag("Paddle")?.transform
            ?? throw new System.Exception("GameManager: no encontró la pala en la nueva escena.");
        bricksContainer = GameObject.Find("BricksContainer")?.transform
            ?? throw new System.Exception("GameManager: no encontró 'BricksContainer' en la nueva escena.");

        // Reactivar el nivel actualmente cargado
        if (levelController == null)
            levelController = new LevelController(levelRoot);
        levelController.LoadLevel(currentLevel);

        // 3) (Re)crear y registrar PaddleController
        paddleController = new PaddleController(paddleTransform, paddleData);
        //Register(paddleController);

        // 4) Inicializar la UI ya con la bola disponible
        uiManager = FindObjectOfType<UIManager>();
        if (uiManager != null)
        {
            uiManager.Initialize(this, _mainBallController);
        }
        else
        {
            Debug.LogError("GameManager: faltó UIManager.");
        }

        // 5) Registrar GameManager para seguir recibiendo updates
        Register(this);
    }


    private void InitAudio()
    {
        if (TryGetMixerVolume("MUSICVolume", out float mDb))
        {
            float v = Mathf.InverseLerp(-80f, 0f, mDb);
            foreach (var s in musicSliders) s.value = v;
        }
        if (TryGetMixerVolume("SFXVolume", out float sDb))
        {
            float v = Mathf.InverseLerp(-80f, 0f, sDb);
            foreach (var s in sfxSliders) s.value = v;
        }
        foreach (var s in musicSliders) s.onValueChanged.AddListener(SetMusicVolumeFromSlider);
        foreach (var s in sfxSliders) s.onValueChanged.AddListener(SetSFXVolumeFromSlider);
    }

    private void OnAssetsLoaded()
    {
        if (_hasInitializedOnce) return;
        _hasInitializedOnce = true;
        // 1) Nivel inicial
        levelController = new LevelController(levelRoot);
        levelController.LoadLevel(startingLevel);
        currentLevel = startingLevel;

        // 2) Pool y generación de ladrillos
        brickPool = new BrickPool(generatorConfig.brickPrefabs, generatorConfig.rows * generatorConfig.columns);
        brickGenerator = new BrickGenerator(brickPool, generatorConfig, brickStartPosition, bricksContainer);
        brickGenerator.GenerateBricks();
        _brickHitCounts = new Dictionary<GameObject, int>();
        InitializeBricks();

        // 3) Bola principal
        ballPool = new BallPool(ballData.ballPrefab, ballData.poolSize);
        Vector3 spawnPos = paddleTransform.position + Vector3.up * 0.6f;
        _mainBallController = SpawnBall(true, spawnPos);

        // 4) Atlas en ladrillos
        ApplyAtlasToBricks();
         // Atlas en pala
        ApplyAtlasToPaddle();
        // 5) Configurar pala y UI
        paddleController = new PaddleController(paddleTransform, paddleData);
        Register(paddleController);
        uiManager.Initialize(this, _mainBallController);

        // 6) Parallax: carga e instancia Addressables
        prevPaddlePosition = paddleTransform.position;
        int count = Mathf.Min(parallaxReferences.Length, parallaxFactors.Length);
        for (int i = 0; i < count; i++)
        {
            int idx = i;  // captura local para el callback
            parallaxReferences[idx]
                .InstantiateAsync(transform)
                .Completed += handle =>
                {
                    if (handle.Status == AsyncOperationStatus.Succeeded)
                    {
                        var layerGO = handle.Result;
                        layerGO.name = $"ParallaxLayer_{idx}";
                        parallaxLayers.Add(layerGO.transform);
                    }
                };
        }

        // 7) Loop principal
        Register(this);
    }

    private void InitParallax()
    {
        prevPaddlePosition = paddleTransform.position;
        if (parallaxReferences == null || parallaxFactors == null) return;
        int count = Mathf.Min(parallaxReferences.Length, parallaxFactors.Length);
        for (int i = 0; i < count; i++)
        {
            int idx = i;
            AssetReferenceGameObject reference = parallaxReferences[idx];
            reference.InstantiateAsync(transform).Completed += (AsyncOperationHandle<GameObject> handle) =>
            {
                if (handle.Status == AsyncOperationStatus.Succeeded)
                {
                    GameObject layerGO = handle.Result;
                    layerGO.name = $"ParallaxLayer_{idx}";
                    parallaxLayers.Add(layerGO.transform);
                }
            };
        }
    }

    private BallController SpawnBall(bool isMainBall, Vector3 atPosition)
    {
        GameObject go = Instantiate(ballData.ballPrefab, atPosition, Quaternion.identity);
        go.SetActive(true);
        SphereCollider sphere = go.GetComponent<SphereCollider>();
        float radius = sphere != null
            ? sphere.radius * Mathf.Max(go.transform.localScale.x, go.transform.localScale.y, go.transform.localScale.z)
            : 0.5f;
        return new BallController(go.transform, paddleTransform, radius,
                                  ballData.speed, ballData.leftLimit, ballData.rightLimit,
                                  ballData.topLimit, ballData.bottomLimit, this, isMainBall);
    }

    public BallController SpawnExtraBall(Vector3 atPosition, Vector3 direction)
    {
        BallController extra = SpawnBall(false, atPosition);
        extra.LaunchBall(direction);
        return extra;
    }

    public void SpawnPowerUp(PowerUpData data, Vector3 pos)
        => new PowerUpController(this, data, pos,
                                 paddleTransform,
                                 atlasColumns, atlasRows,
                                 randomizeIndex, fixedIndex);

    public void Register(ICustomUpdate obj) => updatables.Add(obj);
    public void Unregister(ICustomUpdate obj) => updatables.Remove(obj);

    private void LateUpdate()
    {
        float dt = Time.deltaTime;

        
        updatables.RemoveAll(u => u == null);

        for (int i = 0; i < updatables.Count; i++)
            updatables[i]?.CustomUpdate(dt);
    }

    public void CustomUpdate(float deltaTime)
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            
            StartNextLevel();
            return;
        }

        // O: instant lose
        if (Input.GetKeyDown(KeyCode.O))
        {
            EndGame("GameOverScreen");
            return;
        }

        if (loadingScene)
        {
            endTimer += deltaTime;
            if (endTimer >= endDelay)
            {
                SceneManager.LoadScene(targetScene);
            }
            return;
        }

        // 2) Si ya estamos en gameEnded (sin delay), no hacemos nada
        if (gameEnded)
            return;


        UpdateParallax();

        if (loadingScene)
        {
            endTimer += deltaTime;
            if (endTimer >= endDelay)
            {
                Unregister(this);
                SceneManager.LoadScene(targetScene);
            }
            return;
        }
        if (gameEnded) return;

        timer += deltaTime;
        timerText.text = $"Time: {GetTimeRemaining():0}";

        if (AllBricksDestroyed())
        {
            if (currentLevel < totalLevels) StartNextLevel();
            else EndGame("VictoryScreen");
        }
        else if (timer >= timeLimit)
        {
            EndGame("GameOverScreen");
        }
    }

    private void UpdateParallax()
    {
        // Filtrar Transform destruidos
        parallaxLayers.RemoveAll(t => t == null);
        if (parallaxLayers.Count == 0) return;
       
        var currentPos = paddleTransform.position;
        var delta = currentPos - prevPaddlePosition;
        for (int i = 0; i < parallaxLayers.Count; i++)
        {
            Transform layer = parallaxLayers[i];
            float factor = parallaxFactors[i];
            Vector3 pos = layer.localPosition;
            pos.x += delta.x * factor;
            pos.y += delta.y * factor;
            layer.localPosition = pos;
        }
        prevPaddlePosition = currentPos;
    }

    private void InitializeBricks()
    {
        foreach (Transform t in bricksContainer)
            if (t.gameObject.activeInHierarchy)
                _brickHitCounts[t.gameObject] = 0;
    }

    private void StartNextLevel()
    {
        currentLevel++;
        levelController.LoadLevel(currentLevel);
        brickPool.ReturnAllBricks();
        brickGenerator.GenerateBricks();
        ApplyAtlasToBricks();
        _brickHitCounts.Clear();
        InitializeBricks();

        foreach (var pu in GameObject.FindGameObjectsWithTag("PowerUp")) Destroy(pu);
        foreach (var b in GameObject.FindGameObjectsWithTag("Ball")) Destroy(b);

        Vector3 spawnPos = paddleTransform.position + Vector3.up * 0.6f;
        _mainBallController = SpawnBall(true, spawnPos);
        paddleController.ResetWithData(paddleData);
        ApplyAtlasToPaddle();
        
        timer = 0f;
        gameEnded = false;
        uiManager.Initialize(this, _mainBallController);
        Register(this);
    }

    public float GetTimeRemaining() => Mathf.Max(0f, timeLimit - timer);
    public void AddTime(float seconds) => timeLimit += seconds;

    public void HandleBrickHit(GameObject brickGO)
    {
        int hits = _brickHitCounts.TryGetValue(brickGO, out var h) ? h + 1 : 1;
        _brickHitCounts[brickGO] = hits;

        int required = currentLevel <= 5 ? 1 : 2;
        if (hits >= required)
        {
            PlayBrickBreak();
            brickGO.SetActive(false);
            _brickHitCounts.Remove(brickGO);

            if (Random.value < 0.05f) SpawnPowerUp(multiballData, brickGO.transform.position);
            if (Random.value < 0.10f) SpawnPowerUp(speedUpData, brickGO.transform.position);
            if (Random.value < 0.15f) SpawnPowerUp(timeExtensionData, brickGO.transform.position);
        }
    }

    private void ApplyAtlasToBricks()
    {
        var renderers = bricksContainer.GetComponentsInChildren<Renderer>(false);
        float count = atlasColumns * atlasRows;
        foreach (var r in renderers)
        {
            float idx = randomizeIndex
                ? Random.Range(0f, count)
                : fixedIndex;

            
            var logic = new BrickAtlasLogic(atlasColumns, atlasRows, idx, r);
            logic.Apply(r);
        }
    }


    private void ApplyAtlasToPaddle()
    {
        var r = paddleTransform.GetComponent<Renderer>();
        if (r == null) return;

        float count = atlasColumns * atlasRows;
        float idx = randomizeIndex
                    ? Random.Range(0f, count)
                    : fixedIndex;

        
        new BrickAtlasLogic(atlasColumns, atlasRows, idx, r)
            .Apply(r);
    }



    private bool AllBricksDestroyed()
    {
        foreach (var b in GameObject.FindGameObjectsWithTag("Brick"))
            if (b.activeInHierarchy) return false;
        return true;
    }

    private void EndGame(string scene)
    {
        gameEnded = true;
        LoadSceneWithDelay(scene);
    }

    private void LoadSceneWithDelay(string scene)
    {
        loadingScene = true;
        targetScene = scene;
        endTimer = 0f;

        // Te bajas de todos los updatables para que no sigan corriendo tras la destrucción de escena
        //Unregister(this);
        //updatables.Clear();
    }


#if UNITY_EDITOR
    private void OnValidate()
    {
        // Sólo en Editor y sin afectar datos en ejecución
        if (!Application.isPlaying && bricksContainer != null)
            ApplyAtlasToBricks();
    }
#endif

}
