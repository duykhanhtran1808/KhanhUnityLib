using System.Collections;
using UnityEngine;

namespace Assets.Scripts.Robot
{
    public class FollowPlayerCam : MonoBehaviour
    {
        [SerializeField]
        private Camera cam;

        [SerializeField]
        private Transform player;

        // Use this for initialization
        void Start()
        {

        }


        // Update is called once per frame
        void Update()
        {
            if (cam != null && player != null)
            {
            cam.transform.position = player.transform.position + new Vector3(0, 1, -5);
            }
        }
    }
}