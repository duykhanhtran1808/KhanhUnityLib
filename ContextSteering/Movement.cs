using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{
    [SerializeField] float speed = 5f;
    float xPos;
    float yPos;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        xPos = Input.GetAxis("Horizontal");
        yPos = Input.GetAxis("Vertical");

        this.transform.Translate(new Vector2(xPos * speed * Time.deltaTime, 
                                            yPos * speed * Time.deltaTime));
    }

    
}
