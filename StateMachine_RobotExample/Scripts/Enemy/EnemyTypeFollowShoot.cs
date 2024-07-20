using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class EnemyTypeFollowShoot : EnemyTypeFollowPunch
{
    [SerializeField]
    float oneShotTime = 1f;
    [SerializeField]
    int poolSize = 15; //Kích cỡ pool
    [SerializeField]
    private GameObject bullet;
    [SerializeField]
    GameObject objetPool;
    Queue<GameObject> poolQueue = new Queue<GameObject>();
    List<GameObject> firedBullet = new List<GameObject>();
    float bulletSpeed = 800;
    private float bulletDisableTime = 1f; //Thời gian disable viên đạn
    Coroutine fireBulletCorountine = null;

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
        }
    }
    void DeactivateItemInPool(bool isDestroy)
    {
        foreach(GameObject newBullet in firedBullet)
        {
            if(!isDestroy && newBullet.activeInHierarchy)
            {
                newBullet.SetActive(false);
                poolQueue.Enqueue(newBullet);
            }
            if(isDestroy)
            {
                Destroy(newBullet);
            }
        }
    }

    protected override void HandleDeath()
    {
        while(poolQueue.Count > 0)
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
        
        GameObject newBullet = poolQueue.Dequeue();
        firedBullet.Add(newBullet);
        if (newBullet != null)
        {
            newBullet.SetActive(true);
            newBullet.transform.position = this.transform.position;
            
            
            newBullet.GetComponent<Rigidbody2D>().AddForce(this.transform.right * this.transform.localScale.x * bulletSpeed);
            StartCoroutine(DisableBullet(bulletDisableTime, newBullet));
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
}
