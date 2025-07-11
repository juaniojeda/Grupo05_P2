using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SplashScreenManager : MonoBehaviour
{
    [SerializeField] private float splashDuration = 3f;
    [SerializeField] private string nextScene = "MainMenu";

    private float timer;

    void Update()
    {
        timer += Time.deltaTime;

        if (timer >= splashDuration)
        {
            SceneManager.LoadScene(nextScene);
        }
    }
}


