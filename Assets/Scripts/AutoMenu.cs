// AutoMenu.cs
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AutoMenu : MonoBehaviour
{
    void OnGUI()
    {
        if (GUI.Button(new Rect(300, 600, 400, 100), "JUGAR"))
        {
            Debug.Log("Cargando GameScene...");
            SceneManager.LoadScene("GameScene");
        }
        if (GUI.Button(new Rect(750, 600, 400, 100), "MENU"))
        {
            Debug.Log("Cargando SoundMenu...");
            SceneManager.LoadScene("SoundMenu");
        }
        if (GUI.Button(new Rect(1200, 600, 400, 100), "SALIR"))
        {
            Debug.Log("Saliendo...");
            Application.Quit();
        }
    }
}
