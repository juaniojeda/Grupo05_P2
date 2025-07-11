using System.Collections.Generic;
using UnityEngine;

public class BallPool
{
    private GameObject ballPrefab;
    private readonly List<GameObject> pool;

    public BallPool(GameObject prefab, int initialSize)
    {
        ballPrefab = prefab;
        pool = new List<GameObject>(initialSize);

        for (int i = 0; i < initialSize; i++)
        {
            var ball = Object.Instantiate(ballPrefab);
            ball.SetActive(false);
            pool.Add(ball);
        }
    }

    /// <summary>
    /// Devuelve una bola inactiva repositionada o crea una nueva si todas están en uso.
    /// NO toca al BallController: solo reposiciona y activa el GameObject.
    /// </summary>
    public GameObject GetBall(Vector3 position)
    {
        // Busca una inactiva
        foreach (var ball in pool)
        {
            if (!ball.activeInHierarchy)
            {
                ball.transform.position = position;
                ball.SetActive(true);
                return ball;
            }
        }

        // Si todas las bolas están activas, instanciamos una más
        var newBall = Object.Instantiate(ballPrefab, position, Quaternion.identity);
        newBall.SetActive(true);
        pool.Add(newBall);
        return newBall;
    }

    public void ReturnAllBalls()
    {
        foreach (var ball in pool)
            ball.SetActive(false);
    }
}

