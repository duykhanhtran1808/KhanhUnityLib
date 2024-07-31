using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Jobs;
using Unity.VisualScripting;
using UnityEngine;

public class EnemyTypeFollowShootExact : EnemyTypeFollowPunch
{
    [SerializeField]
    float oneShotTime = 0.05f;
    [SerializeField]
    int poolSize = 15; //Kích cỡ pool
    [SerializeField]
    private GameObject bullet;
    [SerializeField]
    GameObject objetPool;
    Queue<GameObject> poolQueue = new Queue<GameObject>();
    List<GameObject> firedBullet = new List<GameObject>();
    [SerializeField]
     float bulletSpeed = 20;
    private float bulletDisableTime = 5f; //Thời gian disable viên đạn
    Coroutine fireBulletCorountine = null;
    Quaternion originalRotation;

    private void Awake()
    {
        PopulatePool(poolSize);
    }

    void PopulatePool(int size)
    {
        for (int i = 0; i < size; i++)
        {
            GameObject newBullet = Instantiate(bullet, objetPool.transform);
            newBullet.SetActive(false);
            poolQueue.Enqueue(newBullet);
            if(i == 0)
            {
                originalRotation = newBullet.transform.rotation;
            }
        }
    }
    void DeactivateItemInPool(bool isDestroy)
    {
        foreach (GameObject newBullet in firedBullet)
        {
            if (!isDestroy && newBullet.activeInHierarchy)
            {
                newBullet.SetActive(false);
                poolQueue.Enqueue(newBullet);
            }
            if (isDestroy)
            {
                Destroy(newBullet);
            }
        }
    }

    protected override void HandleDeath()
    {
        while (poolQueue.Count > 0)
        {
            GameObject newBullet = poolQueue.Dequeue();
            Destroy(newBullet);
        }
        DeactivateItemInPool(true);
        base.HandleDeath();
    }
    public override void MoveToPlayer()
    {
        if (fireBulletCorountine == null)
        {
            base.MoveToPlayer();
        }

    }
    public override void Attacking()
    {
        if (fireBulletCorountine == null)
        {
            base.Attacking();
            fireBulletCorountine = StartCoroutine(FireBulletCorountine(oneShotTime));
        }

    }
    IEnumerator FireBulletCorountine(float delayTime)
    {

        if(poolQueue.Count > 0)
        {
            GameObject newBullet = poolQueue.Dequeue();
            firedBullet.Add(newBullet);
            if (newBullet != null)
            {
                newBullet.transform.position = this.transform.position;
                newBullet.transform.rotation = originalRotation;
                newBullet.SetActive(true);

                float angleToShoot;
             
                if (CalculateAngle(true) != null)
                {
                    angleToShoot = (float)CalculateAngle(true);
                    newBullet.transform.Rotate(Vector3.forward, 270f + angleToShoot);
                }
                else
                {
                    newBullet.transform.Rotate(Vector3.forward, 45);
                }

                newBullet.GetComponent<Rigidbody2D>().velocity = bulletSpeed * newBullet.transform.up;
                StartCoroutine(DisableBullet(bulletDisableTime, newBullet));
            }
        }
        
        yield return new WaitForSeconds(delayTime);
        fireBulletCorountine = null;
    }

    IEnumerator DisableBullet(float bulletDisableTime, GameObject bullet)
    {
        yield return new WaitForSeconds(bulletDisableTime);
        poolQueue.Enqueue(bullet);
        firedBullet.Remove(bullet);
        bullet.SetActive(false);
    }

    float? CalculateAngle(bool low)
    {
        Vector3 targetDir = base.targetPlayer.transform.position - this.transform.position; //enemy là đích hướng đến
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
