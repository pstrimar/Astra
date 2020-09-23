using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flicker : MonoBehaviour
{
    public float spriteBlinkingTimer = 0f;
    public float spriteBlinkingMiniDuration = 0.1f;
    public float spriteBlinkingTotalTimer = 0f;
    public float spriteBlinkingTotalDuration = 2f;
    public bool startBlinking = false;

    void Update()
    {
        if (startBlinking == true)
        {
            SpriteBlinkingEffect();
        }
    }

    private void SpriteBlinkingEffect()
    {
        spriteBlinkingTotalTimer += Time.deltaTime;
        if (spriteBlinkingTotalTimer >= spriteBlinkingTotalDuration)
        {
            startBlinking = false;
            spriteBlinkingTotalTimer = 0f;
            GetComponentInChildren<SpriteRenderer>().enabled = true;

            return;
        }

        spriteBlinkingTimer += Time.deltaTime;
        if (spriteBlinkingTimer >= spriteBlinkingMiniDuration)
        {
            spriteBlinkingTimer = 0.0f;
            if (GetComponentInChildren<SpriteRenderer>().enabled == true)
            {
                GetComponentInChildren<SpriteRenderer>().enabled = false;
            }
            else
            {
                GetComponentInChildren<SpriteRenderer>().enabled = true;
            }
        }
    }
}
