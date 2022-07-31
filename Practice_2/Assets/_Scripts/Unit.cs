using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[RequireComponent(typeof(BoxCollider2D), typeof(Rigidbody2D))]
public class Unit : MonoBehaviour
{
    protected Vector3 targetPosition;    
    protected bool isActivated = false;
    protected int Hp;
    protected int ATKDamage;
    protected float moveSpeed; 
    protected float attackDelay;
    bool isAttackReady;
    BoxCollider2D box;
    Rigidbody2D body;
    public Action<Unit> Destroyed;
    public Action<Unit> AttackedBySword;
    public Action<int> AttackPlayer;
    void Awake()
    {
        box = GetComponent<BoxCollider2D>();
        body = GetComponent<Rigidbody2D>();
        body.bodyType = RigidbodyType2D.Kinematic;
        box.isTrigger = true; 
        isAttackReady = true;
    }
    public virtual void Activate(Vector3 startPosition, Vector3 targetPosition)
    {
        transform.position = startPosition;
        this.targetPosition = targetPosition;
        if(!isActivated)
            transform.rotation = Quaternion.Euler(0,0,180);
        Vector3 dir = (targetPosition - startPosition).normalized;
        StartCoroutine(slerpRotation(dir));
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
        Hp -= damage;
        if(Hp <= 0)
            DestroySelf();
        else
            StartCoroutine(KnockBack());
    }
    public virtual void DestroySelf()
    {
        isActivated = false;
        Destroyed?.Invoke(this);
    }
    public void ResetRotation(Vector3 targetPosition)
    {
        this.targetPosition = targetPosition;
        Vector3 dir = (targetPosition - transform.position).normalized;
        StartCoroutine(slerpRotation(dir));
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
}
