using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayUIClickSound : MonoBehaviour
{
    /* ------------------------------------------------------------------------
    * Function: PlayUIClick
    * Description: Added a function for easy access to attach to buttons.
    * ---------------------------------------------------------------------- */
    public void PlayUIClick()
    {
        SoundManager.instance.Play("UIClick");
    }
}
