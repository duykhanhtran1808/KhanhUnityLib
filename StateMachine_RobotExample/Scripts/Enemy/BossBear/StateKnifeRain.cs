using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateKnifeRain : IState
{
    private BearStateManager stateManager;
    private GameObject knife;
    private Transform myPosition;
    List<GameObject> objectsToClear = new List<GameObject>();
    private int numOfKnives = 6;
    private float waitToExitTime = 4f;

    int poolSize = 18;
    private static Queue<GameObject> poolQueue = new Queue<GameObject>();
    Quaternion originalKnifeRotation;

    public StateKnifeRain(BearStateManager stateManager, GameObject knife, Transform position)
    {
        this.stateManager = stateManager;
        this.knife = knife;
        this.myPosition = position;
    }

    public void Enter()
    {
        PopulatePool(poolSize);
        
    }

    void PopulatePool(int size)
    {
        if (poolQueue.Count <= 0)
        {
            for (int i = 0; i < size; i++)
            {
                GameObject newKnife = stateManager.InstantiateNew(knife, myPosition.position, Quaternion.identity);
                newKnife.SetActive(false);
                poolQueue.Enqueue(newKnife);
                objectsToClear.Add(newKnife);
                if (i == 0)
                {
                    originalKnifeRotation = newKnife.transform.rotation;
                }
            }
        }
        
        Process();
    }
    public void Process()
    {
        KnifeRain();
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

    private void KnifeRain()
    {
        

        Action makeKnife = () => 
            {
            float height = 10;
            float xDistance = 5;
            for (int i = 0; i < numOfKnives; i++)
            {
                GameObject newKnife = poolQueue.Dequeue();
                if (newKnife != null)
                {
                    newKnife.SetActive(true);
                    newKnife.transform.position = myPosition.position;
                        newKnife.transform.rotation = originalKnifeRotation;

                    newKnife.transform.Translate(xDistance, height, 0);
                    newKnife.transform.Rotate(Vector3.forward, 180f);
                    xDistance += 5;

                        stateManager.UseCoroutine(3f, disableKnife);
                    }
            }

            };

        GameObject newKnife = poolQueue.Dequeue();
        Action<GameObject> disableKnife = (newKnife) =>
        {
            poolQueue.Enqueue(newKnife);
            newKnife.SetActive(false);

        };

        stateManager.UseCoroutine(1f, makeKnife);
        stateManager.UseCoroutine(2f, makeKnife);
        stateManager.UseCoroutine(3f, makeKnife);
    }


}
