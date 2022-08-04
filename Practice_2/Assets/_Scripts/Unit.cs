using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[RequireComponent(typeof(BoxCollider2D), typeof(Rigidbody2D))]
public class Unit : MonoBehaviour
{
    Vector3 targetPosition;    
    bool isActivated = false;
    int maxHp;
    int curHp;
    int ATKDamage;
    float moveSpeed; 
    float attackDelay = 0.5f;
    bool isAttackReady;
    BoxCollider2D box;
    Rigidbody2D body;
    public Action<Unit> Destroyed;
    public Action<Unit> AttackedBySword;
    public Action<int> AttackPlayer;
    bool isInitialized = false;

    public void Initialize(int maxHp, int ATKDamage, float Speed)
    {
        if(isInitialized)
            return;
        this.maxHp = maxHp;
        this.ATKDamage = ATKDamage;
        this.moveSpeed = Speed;

        curHp = maxHp;
        isInitialized = true;
    }
    void Awake()
    {
        box = GetComponent<BoxCollider2D>();
        body = GetComponent<Rigidbody2D>();
        body.bodyType = RigidbodyType2D.Kinematic;
        box.isTrigger = true; 
        isAttackReady = true;
    }
    public void Activate(Vector3 startPosition, Vector3 targetPosition)
    {
        transform.position = startPosition;
        this.targetPosition = targetPosition;
        if(!isActivated)
            transform.rotation = Quaternion.Euler(0,0,180);
        Vector3 dir = (targetPosition - startPosition).normalized;
        transform.rotation = Quaternion.LookRotation(transform.forward, dir);
        //StartCoroutine(slerpRotation(dir));
        isActivated = true;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if(other.GetComponent<Sword>() != null)
        {
            AttackedBySword?.Invoke(this);
            return;
        }
        if(other.GetComponent<PlayerController>() != null)
        {
            if(!isAttackReady)
                return;
            Attack();
            isAttackReady = false;
            if(this.gameObject.activeSelf)
                StartCoroutine(AttackDelay());
            return;
        }
    }
    IEnumerator AttackDelay()
    {
        yield return new WaitForSeconds(attackDelay);
        isAttackReady = true;
    }
    public void Attack()
    {
        AttackPlayer?.Invoke(ATKDamage);
    }
    public void Attacked(int damage)
    {
        curHp -= damage;
        if(curHp <= 0)
            DestroySelf();
        else
            StartCoroutine(KnockBack());
    }
    public void DestroySelf()
    {
        isActivated = false;
        Destroyed?.Invoke(this);
        curHp = maxHp;
    }
    public void ResetRotation(Vector3 targetPosition)
    {
        this.targetPosition = targetPosition;
        Vector3 dir = (targetPosition - transform.position).normalized;
        transform.rotation = Quaternion.LookRotation(transform.forward, dir);
       // StartCoroutine(slerpRotation(dir));
    }
    IEnumerator KnockBack()
    {   
        float elapsedTime=0f;
        while(elapsedTime < 0.1f)
        {
            this.transform.Translate(this.transform.up * 5 * Time.deltaTime);
            elapsedTime+=Time.deltaTime;
            yield return null;
        }
    }
    IEnumerator slerpRotation(Vector3 dir)
    {
        Quaternion targetRotation = Quaternion.LookRotation(transform.forward, dir);
        while(true)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 0.1f);
            if(transform.rotation == targetRotation)
                yield break;
            yield return null;
        }
    }
    void Update()
    {
        transform.position += transform.up * moveSpeed * Time.deltaTime;
    }
}
