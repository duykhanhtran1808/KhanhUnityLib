using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateSummonTeam : IState
{
    private BearStateManager stateManager;
    private GameObject minionTypeShoot;
    private GameObject minionTypePunch;
    private Transform myPosition;
    List<GameObject> objectsToClear = new List<GameObject>();
    private int numOfMinionPairs = 2;
    private float waitToExitTime = 2f;

    public StateSummonTeam(BearStateManager stateManager, GameObject minionTypeShoot, GameObject minionTypePunch, Transform position)
    {
        this.stateManager = stateManager;
        this.minionTypeShoot = minionTypeShoot;
        this.minionTypePunch = minionTypePunch;
        this.myPosition = position;
    }

    public void Enter()
    {
        Process();
    }

    public void Process()
    {
        SummonMinions();
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

    private void SummonMinions()
    {
        
        float xDistance = 6;
        for (int i = 0; i < numOfMinionPairs; i++)
        {
            GameObject newMinion = stateManager.InstantiateNew(minionTypeShoot, myPosition.position, Quaternion.identity);
            objectsToClear.Add(newMinion);
            newMinion.transform.Translate(xDistance, 0, 0);
            newMinion.SetActive(true);
            xDistance += 6;
            newMinion = stateManager.InstantiateNew(minionTypePunch, myPosition.position, Quaternion.identity);
            objectsToClear.Add(newMinion);
            newMinion.transform.Translate(xDistance, 0, 0);
            newMinion.SetActive(true);
            xDistance += 6;
        }
    }
}
