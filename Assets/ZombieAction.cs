using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class ZombieAction : MonoBehaviour
{
    Animator zombieAnimator;
    FpsControllerLPFP player;
    public enum ZombieState
    {
        None,
        Idle,
        Walk,
        Attack,
        TakeHit,
        Death,
    }
    public ZombieState zombie;

    Func<IEnumerator> m_currentFsm;
    Coroutine fsmHandle;
    [SerializeField]
    Transform attackPosition;
    // 좀비의 Fsm을 만들자
    public static ZombieAction instance;
    public float zombieMaxHP = 20;
    public float zombieHP;
    Vector3 lookAtPosition;

    // 감지하는 범위를 랜덤으로 정하자
    float detecedDistance;
    public float attackDistance = 10;
    private void Awake()
    {
        instance = this;
        zombieHP = zombieMaxHP;
        detecedDistance = Random.Range(30f, 50f);
    }
    private IEnumerator Start()
    {
        player = FpsControllerLPFP.instance;
        zombieAnimator = GetComponentInChildren<Animator>();
        CurrentFsm = IdleFSM;
        while (true)    // 무한 반복
        {
            var previousFSM = CurrentFsm;
            fsmHandle = StartCoroutine(CurrentFsm());
            if (fsmHandle == null && previousFSM == CurrentFsm)
            {
                yield return null;
            }
            while (fsmHandle != null)
            {
                yield return null;
            }
        }
    }

    internal void TakeHit(float bulletPower)
    {
        zombie = ZombieState.TakeHit;
        if (zombieHP <= 0)
        {
            return;
        }
        zombieHP -= bulletPower;
        // 
        if (fsmHandle != null)
        {
            StopCoroutine(fsmHandle);
        }
        if (zombieHP > 0)
        {
            CurrentFsm = IdleFSM;
        }
        else
        {
            CurrentFsm = DeathFSM;
        }
    }
    public float deathTime = 3f;
    private IEnumerator DeathFSM()
    {
        zombie = ZombieState.Death;
        PlayAnimation("Death");
        yield return new WaitForSeconds(deathTime);
        Destroy(gameObject);
    }

    public Func<IEnumerator> CurrentFsm
    {
        get { return m_currentFsm; }
        set
        {
            m_currentFsm = value;
            fsmHandle = null;
        }
    }

    private IEnumerator IdleFSM()
    {
        zombie = ZombieState.Idle;
        PlayAnimation("Idle");
        while (Vector3.Distance(transform.position, lookAtPosition) > detecedDistance)
        {
            yield return null;
        }
        CurrentFsm = ChaseFSM;
    }

    private void Update()
    {
        lookAtPosition = new Vector3(player.transform.position.x, transform.position.y, player.transform.position.z);
    }
    public float speed = 0.7f;
    private IEnumerator ChaseFSM()
    {
        zombie = ZombieState.Walk;
        PlayAnimation("Walk");
        while (true)
        {
            Vector3 toPlayerDirection = lookAtPosition - transform.position;
            toPlayerDirection.Normalize();
            transform.LookAt(lookAtPosition);
            transform.Translate(toPlayerDirection * speed * Time.deltaTime, Space.World);
            if (Vector3.Distance(transform.position, lookAtPosition) < attackDistance)
            {
                CurrentFsm = AttackFSM;
                yield break;
            }
            yield return null;
        }
    }
    public float attackTime = 0.5f;
    public float attackFullTime = 1;
    public float power = 3;
    public float attackArange = 0.3f;
    private IEnumerator AttackFSM()
    {
        zombie = ZombieState.Attack;
        PlayAnimation("Attack");
        yield return new WaitForSeconds(attackTime);
        var colliders = Physics.OverlapSphere(attackPosition.position, attackArange);
        foreach (var item in colliders)
        {
            if (item.CompareTag("Player"))
            {
                player.TakeHit(power);
            }
        }
        yield return new WaitForSeconds(attackFullTime - attackTime);
        CurrentFsm = ChaseFSM;
    }

    public void PlayAnimation(string anim)
    {
        zombieAnimator.Play(anim, 0, 0);
    }

    private void OnDrawGizmos()
    {
        attackPosition = transform.Find("AttackPosition").transform;
        Gizmos.DrawWireSphere(transform.position, detecedDistance);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackDistance);
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(attackPosition.position, attackArange);
    }
}
