using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
[RequireComponent(typeof(BoxCollider2D), typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    public enum PlayMode{Manual, Automatic}
    PlayMode playmode;
    [SerializeField]
    Sword swordPrefab;
    [SerializeField]
    Sword skillPrefab;
    [SerializeField]
    int swordDamage = 1;
    int hp=50;
    float moveSpeed = 0.01f;
    float firedelay = 0.5f;
    float skilldelay = 3.0f;
    float elapsedFireTime;
    float elapsedSkillTime;
    bool canShoot = true; 
    bool canSkill = true;
    bool isGameStarted = false;
    Rigidbody2D body;
    BoxCollider2D box;
    Factory swordFactory;
    Factory skillFactory;
    public Action FindEnemy;
    public Action AllHPDestroyed;
    public Action StopMoved;
    public Action SkillUsed;
    public Action<int> PlayerAttacked;

    public Vector3 GetPosition{get{return this.transform.position;}}
    public int HPCount => hp;
    public int GetSwordDamage => swordDamage;
    void Awake()
    {
        playmode = PlayMode.Manual;
        body = GetComponent<Rigidbody2D>();
        box = GetComponent<BoxCollider2D>();
        body.bodyType = RigidbodyType2D.Kinematic;
        box.isTrigger = true;
    }
    void Start()
    {
        swordFactory = new Factory(swordPrefab);
        skillFactory = new Factory(skillPrefab);
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
        sword.Activate(startPosition, this.transform.up);
        sword.Destroyed += this.SwordDestroy;
        AudioManager.instance.PlaySound(SoundId.Shoot);
        canShoot = false;
    }
    void FireSkill()
    {
        StopMoved?.Invoke();
        RecycleObject skill = skillFactory.Get();
        Vector3 startPosition = this.transform.position + this.transform.up*0.6f;
        skill.Activate(startPosition, this.transform.up);
        skill.Destroyed += this.SkillDestroy;
        AudioManager.instance.PlaySound(SoundId.Shoot);
        canSkill = false;
    }
    /*
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
        AudioManager.instance.PlaySound(SoundId.Shoot);
    }
    void SkillDestroy(RecycleObject usedSkill)
    {
        usedSkill.Destroyed -= this.SkillDestroy;
        skillFactory.Restore(usedSkill);
        AudioManager.instance.PlaySound(SoundId.SwordExplosion);
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
    void PlayerMoveAuto(GameObject obj)
    {
        Vector3 dir = (obj.transform.position - transform.position).normalized;
        this.transform.rotation = Quaternion.LookRotation(transform.forward, dir);
        if(Vector3.Distance(this.transform.position, obj.transform.position) > 1f)
            this.transform.position = Vector3.MoveTowards(this.transform.position, obj.transform.position, moveSpeed);
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
    void Update()
    {
        if(!isGameStarted)
            return;
        var objects = GameObject.FindGameObjectsWithTag("Enemy").ToList();
        var neareastObject = objects.OrderBy(obj => {return Vector3.Distance(this.transform.position, obj.transform.position);}).FirstOrDefault();
        if(neareastObject == null)
            return;
        if(!canSkill)
        {
            elapsedSkillTime += Time.deltaTime;
            if(elapsedSkillTime > skilldelay)
            {
                canSkill = true;
                elapsedSkillTime = 0f;
            }
        }
        else if(canSkill && playmode == PlayMode.Automatic)
        {
            if(Vector3.Distance(this.transform.position, neareastObject.transform.position) < 1.5f)
            {
                FireSkill();
                SkillUsed?.Invoke();
            }
        }
        else if(canSkill && playmode == PlayMode.Manual)
        {
            if(Input.GetKeyDown(KeyCode.Space))
            {
                FireSkill();
                SkillUsed?.Invoke();
            }
        }
        if(!canShoot)
        {
            elapsedFireTime += Time.deltaTime;
            if(elapsedFireTime > firedelay)
            {
                canShoot = true;
                elapsedFireTime = 0f;
            }
        }
        else if(canShoot && playmode == PlayMode.Automatic)
        {
            if(Vector3.Distance(this.transform.position, neareastObject.transform.position) < 1f)
            {
                Fire();
            }
        }
        else if(canShoot && playmode == PlayMode.Manual)
        {
            if(Input.GetMouseButtonDown(0))
                Fire();
        }
        
        if(playmode == PlayMode.Automatic)
            PlayerMoveAuto(neareastObject);
        else if(playmode == PlayMode.Manual)
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
        }
    }
}
