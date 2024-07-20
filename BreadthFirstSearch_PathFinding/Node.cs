using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node : MonoBehaviour
{
    public Vector2Int coordinates;
    public bool isWalkable;
    public bool isExplored;
    public bool isPath;

    public Node connectedTo;

    //public Node(bool isWalkable)
    //{
        
    //    this.isWalkable = isWalkable;
    //}
    SpriteRenderer spriteRenderer;
    private void Awake()
    {
        this.coordinates = new Vector2Int((int)this.transform.position.x,
                                            (int)this.transform.position.y);
        spriteRenderer = this.GetComponent<SpriteRenderer>();
    }

    public void ChangeColor()
    {
        spriteRenderer.color = Color.blue;
    }
}
