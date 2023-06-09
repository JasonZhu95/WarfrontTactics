using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseButton : MonoBehaviour
{
    public void OnPauseButtonClick()
    {
        Time.timeScale = 0f;
    }

    public void OnResumeButtonClick()
    {
        Time.timeScale = 1f;
    }
}
