using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class PathFind : MonoBehaviour
{
    [SerializeField] Vector2Int startCoord; //Vi tri bat dau
    public Vector2Int StartCoord { get { return startCoord; } }
    [SerializeField] Vector2Int endCoord; //Vi tri ket thuc
    public Vector2Int EndCoord { get { return endCoord; } }

    Node startNode; //Node bat dau
    Node endNode; //Node ket thuc
    [SerializeField] Transform target;

    Node currentSearchNode; //Node hien tai

    Queue<Node> frontier = new Queue<Node>(); //Queue Xep Duong Di
    Dictionary<Vector2Int, Node> reachedGrid = new Dictionary<Vector2Int, Node>(); //Nhung O da Xem den
    Vector2Int[] directions = { Vector2Int.right, Vector2Int.left, Vector2Int.up, Vector2Int.down }; //Huong tim kiem hang xom

    [SerializeField] Node[] allNodes;
    Dictionary<Vector2Int, Node> grid = new Dictionary<Vector2Int, Node>();//Ban do grid

    private void Awake()
    {
        grid = LoadAllNodes();
        startCoord = new Vector2Int((int)this.transform.position.x,
                                            (int)this.transform.position.y);
        startNode = grid[startCoord];
        endCoord = new Vector2Int((int)target.position.x,
                                            (int)target.position.y);
        endNode = grid[endCoord];
    }

    private void Start()
    {
        StartCoroutine(GoToTarget());
    }
    IEnumerator GoToTarget()
    {
        List<Node> pathToTarget = GetNewPath();
        foreach (Node node in pathToTarget)
        {
            yield return new WaitForSeconds(0.5f);
            Debug.Log(node.coordinates.x + ", " + node.coordinates.y);
            this.transform.position = new Vector2(node.coordinates.x, node.coordinates.y);
        }
    }
    private Dictionary<Vector2Int, Node> LoadAllNodes()
    {
        Dictionary<Vector2Int, Node> returnNodes = new Dictionary<Vector2Int, Node>();
        foreach(Node node in allNodes)
        {
            returnNodes.Add(node.coordinates, node);
        }
        return returnNodes;
    }
    public List<Node> GetNewPath()
    {
        BreadthFirstSearch();
        return BuildPath();
    }

    void ExploreNeighbors()
    {
        List<Node> neighbors = new List<Node>();
        foreach (Vector2Int direction in directions)
        {
            Vector2Int neighborCoordinate = currentSearchNode.coordinates + direction;
            if (grid.ContainsKey(neighborCoordinate))
            {
                neighbors.Add(grid[neighborCoordinate]);
            }
        }

        foreach (Node neighbor in neighbors)
        {
            if (!reachedGrid.ContainsKey(neighbor.coordinates) && neighbor.isWalkable)
            {
                neighbor.connectedTo = currentSearchNode;
                reachedGrid.Add(neighbor.coordinates, neighbor);
                frontier.Enqueue(neighbor);
            }
        }
    }

    void BreadthFirstSearch()
    {
        startNode.isWalkable = true;
        endNode.isWalkable = true;

        frontier.Clear();
        reachedGrid.Clear();

        bool isRunning = true;
        frontier.Enqueue(grid[startCoord]);
        reachedGrid.Add(startCoord, grid[startCoord]);

        while (frontier.Count > 0 && isRunning)
        {
            currentSearchNode = frontier.Dequeue();
            currentSearchNode.isExplored = true;
            ExploreNeighbors();
            if (currentSearchNode.coordinates == endCoord) isRunning = false;
        }
    }

    List<Node> BuildPath()
    {
        List<Node> path = new List<Node>();
        Node currentNode = endNode;

        path.Add(currentNode);
        currentNode.isPath = true;

        while (currentNode.connectedTo != null)
        {
            currentNode = currentNode.connectedTo;
            path.Add(currentNode);
            currentNode.isPath = true;
            currentNode.ChangeColor();
        }

        path.Reverse();

        return path;
    }
}
