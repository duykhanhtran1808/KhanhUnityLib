using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDamage : MonoBehaviour
{
    [SerializeField]
    float damage = 5f;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            //collision.gameObject.GetComponent<EnemyHealth>().ReduceHealth(damage);
            collision.gameObject.SendMessage("ReduceHealth", damage);
            this.gameObject.SetActive(false);
        }
    }
}
