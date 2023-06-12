using System;
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
    public static event Action OnChangeScene;

    public void ChangeScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
        OnChangeScene?.Invoke();
    }
}
