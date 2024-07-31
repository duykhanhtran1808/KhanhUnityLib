using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateExactKnife : IState
{
    private BearStateManager stateManager;
    private GameObject knife;
    private Transform myPosition;
    private Transform targetPlayer;
    List<GameObject> objectsToClear = new List<GameObject>();
    //private int numOfKnives = 10;
    private float waitToExitTime = 5f;
    float bulletSpeed;
    Quaternion originalKnifeRotation;

    int poolSize = 50;
    private static Queue<GameObject> poolQueue = new Queue<GameObject>();
    public StateExactKnife(BearStateManager stateManager, GameObject knife, Transform position, Transform targetPlayer, float bulletSpeed)
    {
        this.stateManager = stateManager;
        this.knife = knife;
        this.myPosition = position;
        this.targetPlayer = targetPlayer;
        this.bulletSpeed = bulletSpeed;
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
        ThrowKnife();
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
    private void ThrowKnife()
    {
        Action<GameObject> disableKnife = (newKnife) =>
        {
            poolQueue.Enqueue(newKnife);
            newKnife.SetActive(false);
        };

        Action throwOneKnife = () =>
        {
            GameObject newKnife = poolQueue.Dequeue();
            if (newKnife != null)
            {
                newKnife.transform.position = myPosition.position;
                newKnife.transform.rotation = originalKnifeRotation;
                newKnife.SetActive(true);

                float angleToShoot;
                if (CalculateAngle(true) != null)
                {
                    angleToShoot = (float)CalculateAngle(true);
                    //Debug.Log(angleToShoot);
                    newKnife.transform.Rotate(Vector3.forward, angleToShoot + 270f);
                }
                else
                {
                    newKnife.transform.Rotate(Vector3.forward, 45);
                }

                newKnife.GetComponent<Rigidbody2D>().velocity = bulletSpeed * newKnife.transform.up;
                stateManager.UseCoroutine(10f, disableKnife, newKnife);
            }
        };
        stateManager.UseCoroutine(0f, throwOneKnife);
        stateManager.UseCoroutine(1f, throwOneKnife);
        stateManager.UseCoroutine(2f, throwOneKnife);
    }

    float? CalculateAngle(bool low)
    {
        Vector3 targetDir = targetPlayer.transform.position - myPosition.position; //enemy là đích hướng đến
        float y = targetDir.y;
        targetDir.y = 0f;
        float x = targetDir.magnitude - 1; // X là cột Z - Cột hướng đến đối tượng
        float gravity = 9.8f;
        float sqr = bulletSpeed * bulletSpeed; //Lấy tốc độ của viên đạn bắn ra
        float underTheSqrRoot = (sqr * sqr) - gravity * (gravity * x * x + 2 * y * sqr);

        if (underTheSqrRoot >= 0f)
        {
            float root = Mathf.Sqrt(underTheSqrRoot);
            float highAngle = sqr + root;
            float lowAngle = sqr - root;

            if (low) return (Mathf.Atan2(lowAngle, gravity * x) * Mathf.Rad2Deg);
            else return (Mathf.Atan2(highAngle, gravity * x) * Mathf.Rad2Deg);
        }
        else return null;
    }
}
