using FPSControllerLPFP;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ZomebieAction : MonoBehaviour
{
    Animator zombieAnimator;
    FpsControllerLPFP player;
    public enum ZombieState
    {
        None,
        Walk,
        Attack,
        Death,
    }
    public ZombieState zombie;

    Func<IEnumerator> currentFsm;
    Coroutine fsmHandle;
    [SerializeField]
    Transform attackPosition;
    // 좀비의 Fsm을 만들자
    private IEnumerator Start()
    {
        player = FpsControllerLPFP.instance;
        zombieAnimator = GetComponentInChildren<Animator>();
        CurrentFsm = IdleFSM;
        while (true)    // 무한 반복
        {
            var previousFSM = CurrentFsm;
            fsmHandle = StartCoroutine(CurrentFsm());
            while (fsmHandle != null)
            {
                yield return null;
            }
        }
    }

    public Func<IEnumerator> CurrentFsm
    {
        get { return currentFsm; }
        set
        {
            currentFsm = value;
            fsmHandle = null;
        }
    }

    public float detecedDistance = 50;
    public float attackDistance = 10;
    private IEnumerator IdleFSM()
    {
        PlayAnimation("Idle");
        while (Vector3.Distance(transform.position, player.transform.position) > detecedDistance)
        {
            yield return null;
        }
        currentFsm = ChaseFSM;
    }
    public float speed = 0.7f;
    private IEnumerator ChaseFSM()
    {
        PlayAnimation("Walk");
        while (true)
        {
            Vector3 toPlayerDirection = player.transform.position - transform.position;
            toPlayerDirection.Normalize();
            transform.LookAt(player.transform);
            transform.Translate(toPlayerDirection * speed * Time.deltaTime, Space.World);
            if (Vector3.Distance(transform.position, player.transform.position) < attackDistance)
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
