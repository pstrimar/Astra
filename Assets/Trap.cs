using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trap : MonoBehaviour
{
    [SerializeField] Enemy boss;
    [SerializeField] Collider2D[] colliders;

    void Start()
    {
        boss.GetComponent<Animator>().enabled = false;
        boss.GetComponent<Rigidbody2D>().gravityScale = 0f;
        boss.transform.Find("b crawler").gameObject.SetActive(false);

        foreach (Collider2D collider in colliders)
        {
            collider.enabled = false;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            boss.GetComponent<Animator>().enabled = true;
            boss.GetComponent<Rigidbody2D>().gravityScale = 1f;
            boss.transform.Find("b crawler").gameObject.SetActive(true);
            boss.GetComponent<Animator>().Play("JumpSurprise");
            foreach (Collider2D collider in colliders)
            {
                collider.enabled = true;
            }

            Destroy(this.gameObject);
        }
    }
}
