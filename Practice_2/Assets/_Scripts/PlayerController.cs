using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
[RequireComponent(typeof(BoxCollider2D), typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    public enum PlayMode{Manual, Automatic}
    enum State{CanShoot, CanSkill, MovetoAttack, MovetoAvoid}
    PlayMode playmode;
    State curState;
    [SerializeField]
    int swordDamage = 1;
    int hp=50;
    float moveSpeed = 0.1f;
    float firedelay = 0.5f;
    float skilldelay = 3.0f;
    bool reloadingFire = false;
    bool reloadingSkill = false;
    bool canShoot = true; 
    bool canSkill = true;
    bool isGameStarted = false;
    bool isInitialized = false;
    Rigidbody2D body;
    BoxCollider2D box;
    GameObject targetObject;
    Factory factory;
    public Action FindEnemy;
    public Action AllHPDestroyed;
    public Action StopMoved;
    public Action SkillUsed;
    public Action<int> PlayerAttacked;

    public Vector3 GetPosition{get{return this.transform.position;}}
    public int HPCount => hp;
    public int GetSwordDamage => swordDamage;


    public void Initialize(Factory factory)
    {
        if(isInitialized)
            return;
        this.factory = factory;

        isInitialized = true;
    }
    void Awake()
    {
        playmode = PlayMode.Manual;
        curState = State.MovetoAttack;
        body = GetComponent<Rigidbody2D>();
        box = GetComponent<BoxCollider2D>();
        body.bodyType = RigidbodyType2D.Kinematic;
        box.isTrigger = true;
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
    void Fire(string attackMode)
    {
        StopMoved?.Invoke();
        RecycleObject obj = null;
        switch(attackMode)
        {
            case "Normal_1":
                obj = factory.GetObject(attackMode);
                canShoot = false;
                break;
            case "Skill_1":
                obj = factory.GetObject(attackMode);
                canSkill = false;
                break;
            default:
                break;                
        }
        Vector3 startPosition = this.transform.position + this.transform.up*1.0f;
        obj.Activate(startPosition, this.transform.up);
        obj.Destroyed += this.Destroy;
        AudioManager.instance.PlaySound(SoundId.Shoot);
    }
    public void SetDefaultPosition()
    {
        StartCoroutine(SetDefaultPosition_());
    }
    public IEnumerator SetDefaultPosition_()
    {
        while(Vector3.Distance(this.transform.position, new Vector3(0,-4,0)) > 0.1f)
        {
            this.transform.position = Vector3.MoveTowards(this.transform.position, new Vector3(0,-4,0),Time.deltaTime*2);
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(new Vector3(0,0,0)), Time.deltaTime*10);
            yield return null;
        }
        curState = State.MovetoAttack;
    }
    void Destroy(RecycleObject usedObject)
    {
        usedObject.Destroyed -= this.Destroy;
        factory.Restore(usedObject, usedObject.name);
        AudioManager.instance.PlaySound(SoundId.Shoot);
    }
    public void Attacked(int damage)
    {
        hp-=damage;
        PlayerAttacked?.Invoke(hp);
    }
    public void OnGameEnded(bool isVictory, int HPCount) 
    { 
        isGameStarted = false; 
    }
    void PlayerMoveToAttackAuto(GameObject obj)
    {
        Vector3 dir = (obj.transform.position - transform.position).normalized;
        this.transform.rotation = Quaternion.LookRotation(transform.forward, dir);
        if(Vector3.Distance(this.transform.position, obj.transform.position) > 1f)
            this.transform.position = Vector3.MoveTowards(this.transform.position, obj.transform.position, moveSpeed);
    }
    void PlayerMoveToAvoidAuto(GameObject obj)
    {
        Vector3 dir = (obj.transform.position - transform.position).normalized;
        this.transform.rotation = Quaternion.LookRotation(transform.forward, dir);
        if(Vector3.Distance(this.transform.position, obj.transform.position) > 1f)
            this.transform.position = Vector3.MoveTowards(this.transform.position, obj.transform.position*-1, moveSpeed*0.5f);
    }
    void PlayerMoveManual(string keyCode, Vector3 mousePosition)
    {
        switch(keyCode)
        {
            case "W":
                this.transform.position += Vector3.up * moveSpeed;
                break;
            case "A":
                this.transform.position += Vector3.left * moveSpeed;
                break;
            case "S":
                this.transform.position += Vector3.down * moveSpeed;
                break;
            case "D":
                this.transform.position += Vector3.right * moveSpeed;
                break;
            default:
                break;
        }
        Vector3 point = Camera.main.ScreenToWorldPoint(mousePosition);
        point.z = 0f;
        Vector3 dir = (point - transform.position).normalized;
        this.transform.rotation = Quaternion.LookRotation(transform.forward, dir);
    }
    public void PlayModeChange()
    {
        if(playmode == PlayMode.Manual)
            playmode = PlayMode.Automatic;
        else
            playmode = PlayMode.Manual;
    }
    IEnumerator SkillDelay()
    {
        reloadingSkill = true;
        yield return new WaitForSeconds(skilldelay);
        canSkill = true;
        reloadingSkill = false;
    }
    IEnumerator FireDelay()
    {
        reloadingFire = true;
        yield return new WaitForSeconds(firedelay);
        canShoot = true;
        reloadingFire = false;
    }
    void FixedUpdate()
    {
        if(!isGameStarted)
            return;
        // 가장 가까운적 탐색(LinQ) -> 처치 시 재탐색
        if(targetObject == null || !targetObject.activeSelf)
        {
            var objects = GameObject.FindGameObjectsWithTag("Enemy").ToList();
            var neareastObject = objects.OrderBy(obj => {return Vector3.Distance(this.transform.position, obj.transform.position);}).FirstOrDefault();
            if(neareastObject == null)
                return;
            targetObject = neareastObject;
        }

        // 적이 존재할 때 자동모드에서의 상태 체크
        else if(Vector3.Distance(this.transform.position, targetObject.transform.position) > 2.0f && (canShoot || canSkill))
            curState = State.MovetoAttack;
        else
        {
            if(canSkill)
                curState = State.CanSkill;
            else if(canShoot)
                curState = State.CanShoot;
            else if(!canShoot && !canSkill)
                curState = State.MovetoAvoid;
        }
        
        if(!canSkill && !reloadingSkill)
            StartCoroutine(SkillDelay());
        if(!canShoot && !reloadingFire)
            StartCoroutine(FireDelay());
        
        // 자동모드에서 현재 상태에 따른 행동
        else if(playmode == PlayMode.Automatic)
        {
            switch(curState)
            {
                case State.CanSkill:
                    Fire("Skill_1");
                    SkillUsed?.Invoke();
                    break;
                
                case State.CanShoot:
                    Fire("Normal_1");
                    break;
                
                case State.MovetoAttack:
                    PlayerMoveToAttackAuto(targetObject);
                    break;
                    
                case State.MovetoAvoid:
                    PlayerMoveToAvoidAuto(targetObject);
                    break;

                default:
                    break;
            }
        }
        //수동 조작
        if(playmode == PlayMode.Manual)
        {
            if(!Input.anyKey)
                PlayerMoveManual("", Input.mousePosition);
            if(Input.GetKey(KeyCode.W))
                PlayerMoveManual("W", Input.mousePosition);
            else if(Input.GetKey(KeyCode.A))
                PlayerMoveManual("A", Input.mousePosition);
            else if(Input.GetKey(KeyCode.S))
                PlayerMoveManual("S", Input.mousePosition);
            else if(Input.GetKey(KeyCode.D))
                PlayerMoveManual("D", Input.mousePosition);
            if(canSkill && Input.GetKeyDown(KeyCode.Space))
            {
                Fire("Skill_1");
                SkillUsed?.Invoke();
            }
            else if(canShoot && Input.GetMouseButtonDown(0))
                Fire("Normal_1");
        }


    }
}
