using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SearchRange : MonoBehaviour
{
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            SendMessageUpwards("FoundPlayer", collision.gameObject);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            SendMessageUpwards("NotFoundPlayer", SendMessageOptions.DontRequireReceiver);
        }

    }
}
