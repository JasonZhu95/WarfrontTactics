using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/* ----------------------------------------------------------------------------
 * Class: SceneLoader
 * Description: Singleton Class responsible for Managing scene loads
 * ---------------------------------------------------------------------------- */
public class SceneLoader : MonoBehaviour
{
    private static SceneLoader instance;

    public static SceneLoader Instance { get { return instance; } }

    private void Awake()
    {
        //Check if instance already exists in the scene
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;

        DontDestroyOnLoad(gameObject);
    }

    public void ChangeScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }
}
