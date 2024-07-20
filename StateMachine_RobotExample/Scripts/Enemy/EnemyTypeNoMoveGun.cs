using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyTypeNoMoveGun : MonoBehaviour, IDamageable, IObserver
{
    //Cách 2: coroutine
    [SerializeField]
    private GameObject bullet; //Chứa prefab viên đạn
    [SerializeField]
    private GameObject gunHeadPosition; //Vị trí của 1 gameobject trống đặt ngay đầu súng
    [SerializeField]
    Transform gunTransform; //Transform của súng
    [SerializeField] 
    int poolSize = 15; //Kích cỡ pool

    //Health==
    private float maxHealth = 15f;
    [SerializeField]
    float currentHealth;
    [SerializeField]
    Slider healthSlide;
    //Health===


    Queue<GameObject> poolQueue = new Queue<GameObject>();
    private float bulletDisableTime = 5f; //Thời gian disable viên đạn
    private bool gunRotating = false; //Có thực hiện quay súng ko
    public float bulletSpeed = 400; //Tốc độ đạn

    private void Awake()
    {
        PopulatePool(poolSize); //Làm đầy pool theo số lượng
    }
    void Start()
    {
        ObserverManager.Instance.AddObserver(this);
        currentHealth = maxHealth;
        healthSlide.value = this.currentHealth / this.maxHealth;

        StartCoroutine(RotateGunCoroutine(1f));
    }

    void Update()
    {
        if(gunRotating)
        {
            rotateGun(); //Quay súng
        }

    }
    public void ReduceHealth(float amount)
    {
        this.currentHealth -= amount;
        healthSlide.value = this.currentHealth / this.maxHealth;
        if (this.currentHealth <= 0)
        {
            ObserverManager.Instance.RemoveObserver(this);
            Destroy(this.gameObject);
        }
    }
    void PopulatePool(int size)
    {
        for (int i = 0; i < size; i++)
        {
            GameObject newBullet = Instantiate(bullet, this.transform);
            newBullet.SetActive(false);
            poolQueue.Enqueue(newBullet);
        }
    }

    private void rotateGun()
    {
        gunTransform.Rotate(Vector3.forward, 30 * Time.deltaTime);
    }

    IEnumerator FireBulletCorountine(float delayTime)
    {
        yield return new WaitForSeconds(delayTime);
        GameObject newBullet = poolQueue.Dequeue();
        if(newBullet != null)
        {
            newBullet.SetActive(true);
            newBullet.transform.position = gunHeadPosition.transform.position;
            newBullet.GetComponent<Rigidbody2D>().AddForce(-gunHeadPosition.transform.up * bulletSpeed);
            StartCoroutine(DisableBullet(bulletDisableTime, newBullet));
        }
    }

    IEnumerator RotateGunCoroutine(float delayTime)
    {
        gunRotating = true;
        yield return new WaitForSeconds(delayTime);

        gunRotating = false;
        StartCoroutine(FireBulletCorountine(0.3f));
        StartCoroutine(FireBulletCorountine(0.6f));
        StartCoroutine(FireBulletCorountine(0.9f));
        yield return new WaitForSeconds(delayTime);

        StartCoroutine(RotateGunCoroutine(1f));
    }

    IEnumerator DisableBullet(float bulletDisableTime, GameObject bullet)
    {
        yield return new WaitForSeconds(bulletDisableTime);
        poolQueue.Enqueue(bullet);
        bullet.SetActive(false);
    }
    public void OnPlayerDeath()
    {
        gunRotating = false;
        StopAllCoroutines();
    }
}
