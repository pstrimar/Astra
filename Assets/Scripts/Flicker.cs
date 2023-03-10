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
        GameObject graphics = transform.Find("Graphics").gameObject;

        // Once timer has elapsed, stop blinking and turn player graphics on
        spriteBlinkingTotalTimer += Time.deltaTime;
        if (spriteBlinkingTotalTimer >= spriteBlinkingTotalDuration)
        {
            startBlinking = false;
            spriteBlinkingTotalTimer = 0f;
            graphics.SetActive(true);

            return;
        }

        // Flicker player graphics on and off
        spriteBlinkingTimer += Time.deltaTime;
        if (spriteBlinkingTimer >= spriteBlinkingMiniDuration)
        {
            spriteBlinkingTimer = 0.0f;
            if (graphics.activeSelf)
                graphics.SetActive(false);
            else
                graphics.SetActive(true);
        }
    }    
}
