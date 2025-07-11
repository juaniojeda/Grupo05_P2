using UnityEngine;

public class LoaderController
{
    private readonly BallPool _ballPool;
    private readonly Vector3 _spawnPosition;
    private readonly Vector3 _initialDirection;
    private Transform paddleTransform;
    private PaddleData paddleData;
    internal PaddleController paddleController;

    public LoaderController(BallPool ballPool, Vector3 spawnPosition, Vector3 initialDirection)
    {
        _ballPool = ballPool;
        _spawnPosition = spawnPosition;
        _initialDirection = initialDirection;

        
        AssetsManager.Instance.SubscribeOnLoadComplete(OnAssetsLoaded);
    }

    private void OnAssetsLoaded()
    {
        
        GameObject ballGO = _ballPool.GetBall(_spawnPosition);
        if (ballGO == null)
        {
            Debug.LogError("LoaderController: no pudo obtener bola del pool");
            return;
        }

        
        var controller = ballGO.GetComponent<BallController>();
        if (controller != null)
            controller.LaunchBall(_initialDirection);
        else
            Debug.LogWarning("LoaderController: la bola no tiene BallController");

        GameObject paddleGO = AssetsManager.Instance.GetInstance("Paleta");
        if (paddleGO == null)
        {
            Debug.LogError("GameManager: no encontró el prefab 'Paleta'");
            return;
        }
       
        paddleTransform = paddleGO.transform;

        
        paddleController = new PaddleController(paddleTransform, paddleData);



    }
}
