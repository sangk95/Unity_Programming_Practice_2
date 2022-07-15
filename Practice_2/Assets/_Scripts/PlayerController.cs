using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
[RequireComponent(typeof(BoxCollider2D), typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    [SerializeField]
    Bullet bulletPrefab;
    [SerializeField]
    int bulletDamage = 1;
    float firedelay = 0.5f;
    float elapsedFireTime;
    bool canShoot = true; 
    bool isGameStarted = false;
    Rigidbody2D body;
    BoxCollider2D box;
    Factory bulletFactory;
    public Action FindEnemy;
    public Action<int, Unit> HitEnemy;

    public Vector3 GetPosition{get{return this.transform.position;}}
    void Awake()
    {
        body = GetComponent<Rigidbody2D>();
        box = GetComponent<BoxCollider2D>();
        body.bodyType = RigidbodyType2D.Kinematic;
        box.isTrigger = true;
    }
    void Start()
    {
        bulletFactory = new Factory(bulletPrefab);
    }
    public void Gamestart()
    {
        isGameStarted = true;
        StartCoroutine(FirstFireDelay());
    }
    IEnumerator FirstFireDelay()
    {
        yield return new WaitForSeconds(0.6f);
        FindEnemy?.Invoke();
    }
    public void FireReady(Vector3 position)
    {
        if(!isGameStarted)
            return;
        if(!canShoot)
            return;
        StartCoroutine(SetFirePosition(position));
    }
    void Fire()
    {
        RecycleObject bullet = bulletFactory.Get();
        Vector3 startPosition = this.transform.position + new Vector3(0, 0.4f, 0);
        bullet.Activate(startPosition);
        bullet.Destroyed += this.BulletDestroy;
        canShoot = false;
    }
    IEnumerator SetFirePosition(Vector3 position)
    {
        while(Math.Abs(this.transform.position.x-position.x) > 0.1f)
        {
            if(this.transform.position.x > position.x)
                this.transform.position += Vector3.left*0.05f;
            else
                this.transform.position += Vector3.right*0.05f;
            yield return null;
        }
        Fire();
    }
    void BulletDestroy(RecycleObject usedBullet, Unit unit)
    {
        HitEnemy?.Invoke(bulletDamage, unit);
        usedBullet.Destroyed -= this.BulletDestroy;
        bulletFactory.Restore(usedBullet);
    }
    void Update()
    {
        if(!isGameStarted)
            return;
        if(!canShoot)
        {
            elapsedFireTime += Time.deltaTime;
            if(elapsedFireTime > firedelay)
            {
                canShoot = true;
                FindEnemy?.Invoke();
                elapsedFireTime = 0f;
            }
        }
    }
}
