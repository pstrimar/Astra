using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpSurprise : MonoBehaviour
{
    private SpriteMask spriteMask;

    // Start is called before the first frame update
    void Start()
    {
        spriteMask = FindObjectOfType<SpriteMask>();
    }

    public void DisableSpriteMask()
    {
        spriteMask.enabled = false;
    }
}
