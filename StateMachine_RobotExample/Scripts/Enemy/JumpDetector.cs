using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class JumpDetector : MonoBehaviour
{
    //[SerializeField] EnemyTypeFollowPunch enemy;
    private bool allowJump = true;
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player") &&
            !collision.GetComponent<Collider2D>().isTrigger &&
            !collision.CompareTag("Enemy") &&
            allowJump)
        {
            StartCoroutine(AttempJump());
        }
        
    }

    private void OnTriggerExit2D()
    {
        StopAllCoroutines();
        allowJump = true;
    }

    IEnumerator AttempJump()
    {
        //enemy.TriggerJump();
        SendMessageUpwards("TriggerJump");
        allowJump = false;
        yield return new WaitForSeconds(0.2f);
        allowJump = true;
    }
}
