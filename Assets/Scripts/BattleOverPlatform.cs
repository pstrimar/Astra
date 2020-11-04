using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleOverPlatform : MonoBehaviour
{
    [SerializeField] Enemy boss;

    // Start is called before the first frame update
    void Start()
    {
        transform.GetComponent<SpriteRenderer>().enabled = false;
        transform.GetComponent<BoxCollider2D>().enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (boss.isDead)
        {
            transform.GetComponent<SpriteRenderer>().enabled = true;
            transform.GetComponent<BoxCollider2D>().enabled = true;
        }
    }
}
