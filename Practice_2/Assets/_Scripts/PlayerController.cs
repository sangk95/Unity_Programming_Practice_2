using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField]
    Bullet bulletPrefab;
    float firedelay = 0.5f;
    float elapsedFireTime;
    bool canShoot = true; 
    bool isGameStarted = false;
    Factory bulletFactory;

    void Start()
    {
        bulletFactory = new Factory(bulletPrefab);
    }
    public void Gamestart()
    {
        isGameStarted = true;
    }
    public void Fire(Vector3 position)
    {
        if(!isGameStarted)
            return;
        if(!canShoot)
            return;
        RecycleObject bullet = bulletFactory.Get();
        Vector3 startPosition = this.transform.position + new Vector3(0, 1f, 0);
        bullet.Activate(startPosition, position);
        bullet.Destroyed += this.BulletDestroy;
        canShoot = false;
    }
    void BulletDestroy(RecycleObject usedBullet)
    {
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
                elapsedFireTime = 0f;
            }
        }
    }
}
