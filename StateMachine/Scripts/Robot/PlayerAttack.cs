using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    [SerializeField]
    private GameObject bullet;
    [SerializeField]
    private GameObject objectPool;
    [SerializeField]
    int poolSize = 30; //Kích cỡ pool

    Queue<GameObject> poolQueue = new Queue<GameObject>();
    private float bulletDisableTime = 5f; //Thời gian disable viên đạn
    public float bulletSpeed = 900; //Tốc độ đạn

    private void Awake()
    {
        PopulatePool(poolSize); //Làm đầy pool theo số lượng
    }
    void PopulatePool(int size)
    {
        for (int i = 0; i < size; i++)
        {
            GameObject newBullet = Instantiate(bullet, objectPool.transform);
            newBullet.SetActive(false);
            poolQueue.Enqueue(newBullet);
        }
    }
    public void FireBullet(int right)
    {
        GameObject newBullet = poolQueue.Dequeue();
        if (newBullet != null)
        {
            newBullet.transform.position = this.transform.position;
            Vector3 bulletDirection = this.transform.right;
            
            bulletDirection = right > 0? Quaternion.Euler(0, 0, 10) * bulletDirection:
                                         Quaternion.Euler(0, 0, -10) * bulletDirection;
            newBullet.SetActive(true);
            newBullet.GetComponent<Rigidbody2D>().AddForce(bulletDirection * right * bulletSpeed);
            StartCoroutine(DisableBullet(bulletDisableTime, newBullet));
        }
    }

    IEnumerator DisableBullet(float bulletDisableTime, GameObject bullet)
    {
        yield return new WaitForSeconds(bulletDisableTime);
        poolQueue.Enqueue(bullet);
        bullet.SetActive(false);
    }
}
