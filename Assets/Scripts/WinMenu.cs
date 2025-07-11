using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class WinMenu : MonoBehaviour
{
    void OnGUI()
    {
        if (GUI.Button(new Rect(300, 600, 400, 100), "VOLVER A JUGAR"))
        {
            Debug.Log("Cargando GameScene...");
            SceneManager.LoadScene("GameScene");
        }
        if (GUI.Button(new Rect(1200, 600, 400, 100), "MENU"))
        {
            Debug.Log("Cargando MainMenu PAPU...");
            SceneManager.LoadScene("MainMenu");
        }
        if (GUI.Button(new Rect(900, 900, 400, 100), "SALIR"))
        {
            Debug.Log("Saliendo...");
            Application.Quit();
        }
    }
}
