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

    private Animator anim;

    private void Start()
    {
        anim = GetComponent<Animator>();
        anim.SetBool("startfade", true);
    }


    public void ChangeScene(string sceneName)
    {
        StartCoroutine(LoadSceneAfterFade(sceneName));
    }

    private IEnumerator LoadSceneAfterFade(string sceneName)
    {
        Time.timeScale = 1f;
        anim.SetBool("fade", true);
        yield return new WaitForSeconds(1f);
        OnChangeScene?.Invoke();
        SceneManager.LoadScene(sceneName);
    }
}
