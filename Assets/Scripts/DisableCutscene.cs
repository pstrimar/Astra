using UnityEngine;
using UnityEngine.Playables;

public class DisableCutscene : MonoBehaviour
{
    PlayableDirector director;
    GameObject SFX;

    void Awake()
    {
        director = GetComponent<PlayableDirector>();
        SFX = GameObject.Find("SFX");
    }

    private void OnEnable()
    {
        // If player has already crashed, do not play opening cinematic and turn off sfx
        if (GameManager.Instance.playerHasCrashed)
        {
            director.enabled = false;
            if (SFX != null)
                SFX.SetActive(false);
        }
    }
}
