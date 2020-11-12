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
        //if (director != null)
        //{
        //    director.stopped += OnPlayableDirectorStopped;
        //}

        if (GameManager.Instance.playerHasCrashed)
        {
            director.enabled = false;
            if (SFX != null)
                SFX.SetActive(false);
        }
    }

    //private void OnDisable()
    //{
    //    if (director != null)
    //    {
    //        director.stopped -= OnPlayableDirectorStopped;
    //    }
    //}

    //private void OnPlayableDirectorStopped(PlayableDirector obj)
    //{
    //    obj.enabled = false;
    //    GameManager.Instance.playerHasCrashed = true;

    //    if (SFX != null)
    //        SFX.SetActive(false);
    //}

    //public object CaptureState()
    //{
    //    return director.enabled;
    //}

    //public void RestoreState(object state)
    //{
    //    director.enabled = (bool)state;
    //    SFX.SetActive((bool)state);
    //}
}
