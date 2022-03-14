using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour
{
    public bool isStartMenu = false;
    public bool mainTheme = true;

    private void Start()
    {
        SoundManager.instance.PlayTheme(mainTheme);
    }

    void Update()
    {
        if (isStartMenu)
        {
            if (Input.GetKey(KeyCode.S))
            {
                SceneManager.LoadScene(3, LoadSceneMode.Single);
                SoundManager.instance.Play("Transition");
            }
            if (Input.GetKey(KeyCode.C))
            {
                SceneManager.LoadScene(1, LoadSceneMode.Single);
                SoundManager.instance.Play("Transition");
            }
            if (Input.GetKey(KeyCode.T))
            {
                SceneManager.LoadScene(2, LoadSceneMode.Single);
                SoundManager.instance.Play("Transition");
            }
            if (Input.GetKey(KeyCode.Escape))
                Application.Quit();
        }
        else
        {
            if (Input.GetKey(KeyCode.Escape))
            {
                SceneManager.LoadScene(0, LoadSceneMode.Single);
                SoundManager.instance.Play("Transition");
            }
        }
    }
}
