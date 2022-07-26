using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
[RequireComponent(typeof(BoxCollider2D), typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    [SerializeField]
    Sword swordPrefab;
    [SerializeField]
    int swordDamage = 1;
    int hp=50;
    float moveSpeed = 0.1f;
    float firedelay = 0.5f;
    float elapsedFireTime;
    bool canShoot = true; 
    bool isGameStarted = false;
    Rigidbody2D body;
    BoxCollider2D box;
    Factory swordFactory;
    public Action FindEnemy;
    public Action AllHPDestroyed;
    public Action StopMoved;

    public Vector3 GetPosition{get{return this.transform.position;}}
    public int HPCount => hp;
    public int GetSwordDamage => swordDamage;
    void Awake()
    {
        body = GetComponent<Rigidbody2D>();
        box = GetComponent<BoxCollider2D>();
        body.bodyType = RigidbodyType2D.Kinematic;
        box.isTrigger = true;
    }
    void Start()
    {
        swordFactory = new Factory(swordPrefab);
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
    }/*
    public void FireReady(Vector3 position)
    {
        if(!isGameStarted)
            return;
        if(!canShoot)
            return;
        StartCoroutine(SetFirePosition(position));
    }*/
    void Fire()
    {
        StopMoved?.Invoke();
        RecycleObject sword = swordFactory.Get();
        Vector3 startPosition = this.transform.position + this.transform.up*0.6f;
        sword.Activate(startPosition);
        sword.Destroyed += this.SwordDestroy;
        AudioManager.instance.PlaySound(SoundId.Shoot);
        canShoot = false;
    }/*
    IEnumerator SetFirePosition(Vector3 position)
    {
        while(Vector3.Distance(this.transform.position, position) > 0.1f)
        {
            this.transform.position = Vector3.MoveTowards(this.transform.position, position,moveSpeed*Time.deltaTime);
            yield return null;
        }
        Fire();
    }*/
    public void SetPosition()
    {
        StartCoroutine(SetPosition_());
    }
    public IEnumerator SetPosition_()
    {
        while(Vector3.Distance(this.transform.position, new Vector3(0,-4,0)) > 0.1f)
        {
            this.transform.position = Vector3.MoveTowards(this.transform.position, new Vector3(0,-4,0),Time.deltaTime*2);
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(new Vector3(0,0,0)), Time.deltaTime*2);
            yield return null;
        }
    }
    void SwordDestroy(RecycleObject usedSword)
    {
        usedSword.Destroyed -= this.SwordDestroy;
        swordFactory.Restore(usedSword);
        AudioManager.instance.PlaySound(SoundId.SwordExplosion);
    }
    public void Attacked(int damage)
    {
        hp-=damage;
    }
    public void OnGameEnded(bool isVictory, int HPCount) 
    { 
        isGameStarted = false; 
    }
    void Update()
    {
        if(!isGameStarted)
            return;
        var objects = GameObject.FindGameObjectsWithTag("Enemy").ToList();
        var neareastObject = objects.OrderBy(obj => {return Vector3.Distance(this.transform.position, obj.transform.position);}).FirstOrDefault();
        if(neareastObject == null)
            return;
        if(!canShoot)
        {
            elapsedFireTime += Time.deltaTime;
            if(elapsedFireTime > firedelay)
            {
                canShoot = true;
                //FindEnemy?.Invoke();  -> // setfireposition() & fireready()
                elapsedFireTime = 0f;
            }
        }
        else
        {
            
            if(Vector3.Distance(this.transform.position, neareastObject.transform.position) < 1f)
            {
                Fire();
                canShoot = false;
            }
                
        }
        Vector3 dir = (neareastObject.transform.position - transform.position).normalized;
        this.transform.rotation = Quaternion.LookRotation(transform.forward, dir);
        if(Vector3.Distance(this.transform.position, neareastObject.transform.position) > 1f)
        this.transform.position = Vector3.MoveTowards(this.transform.position, neareastObject.transform.position, moveSpeed*0.1f);
    }
}
