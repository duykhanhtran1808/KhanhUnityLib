using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateBuildWall : IState
{
    private BearStateManager stateManager;
    private GameObject wall;
    private Transform myPosition;
    List<GameObject> objectsToClear = new List<GameObject>();
    private int numOfWalls = 4;
    private float waitToExitTime = 1f;
    public StateBuildWall(BearStateManager stateManager, GameObject wall, Transform position)
    {
        this.stateManager = stateManager;
        this.wall = wall;
        this.myPosition = position;
    }

    public void Enter()
    {
        Process();
    }

    public void Process()
    {
        BuildWall();
        Exit();
    }
    
    public void Exit()
    {
        stateManager.StateExit(waitToExitTime);
    }

    public List<GameObject> CollectGarbage()
    {
        return objectsToClear;
    }

    private void BuildWall()
    {
        float height = -5;
        float xDistance;
        for (int i = 0; i < numOfWalls; i++)
        {
            GameObject newWall = stateManager.InstantiateNew(wall, myPosition.position, Quaternion.identity);
            objectsToClear.Add(newWall);
            xDistance = Random.Range(5, 7);
            newWall.transform.Translate(xDistance, height, 0);
            height += 2;
        }
    }
}
