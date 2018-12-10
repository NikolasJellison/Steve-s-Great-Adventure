using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour {
    public Transform player;
    public Transform home;
    public Transform hand;
    public float attackCoolDown;
    private float coolDownCount = 3;
    private NavMeshAgent me;
    public float wanderRange;
    private Vector3 wanderDestination;
    private bool wandering;
    private Animator anim;
    [HideInInspector] public bool frozen;
    private bool dead;
    private int attackType;


	// Use this for initialization
	void Start () {
        me = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();
    }
	
	// Update is called once per frame
	void Update () {

        if(wandering && Vector3.Distance(transform.position, wanderDestination) < .5)
        {
            wandering = false;
        }
        //sloppy but i need to get this working
        if (!wandering && Vector3.Distance(transform.position, player.position) > wanderRange)
        {
            wandering = true;
            wanderDestination = new Vector3(Random.Range(home.position.x, home.position.x + wanderRange), home.position.y, Random.Range(home.position.z, home.position.z + wanderRange));
            me.destination = wanderDestination;
        }

        if (Vector3.Distance(transform.position, player.position) < wanderRange && frozen == false)
        {
            //Debug.Log("3");
            wandering = false;
            me.destination = player.position;
            coolDownCount += Time.deltaTime;
            if(coolDownCount >= attackCoolDown)
            {
                coolDownCount = 0;
                me.isStopped = true;
                StartCoroutine(magicAttack());
            }
            //Debug.Log(coolDownCount);
        }

        //Debug.Log(wandering);
        //Debug.Log(this.name + Vector3.Distance(transform.position, player.position));
    }

    private IEnumerator magicAttack()
    {
        //Magic attack animiation is like 1.7 seconds, so we just pause the character so they can attack then resume
        anim.SetTrigger("Attack");
        yield return new WaitForSeconds(1);
        GameObject enemyProjectile;
        if(attackType == 0)
        {
            enemyProjectile = Instantiate(Resources.Load("Enemy-Ice-Projectile"), hand.position, hand.rotation) as GameObject;
            attackType++;
        }
        else
        {
            enemyProjectile = Instantiate(Resources.Load("Enemy-Fire-Projectile"), hand.position, hand.rotation) as GameObject;
            attackType--;
        }
        yield return new WaitForSeconds(1);
        me.isStopped = false;
    }
    public void freezeEnemy()
    {
        //Idk why pulling the coroutine from projile script doesnt work so this happens
        StartCoroutine(frozenEnemy());
    }
    private IEnumerator frozenEnemy()
    {
        Debug.Log("freezing");
        frozen = true;
        me.isStopped = true;
        anim.SetTrigger("Freeze");
        Debug.Log(frozen);
        yield return new WaitForSeconds(3);
        me.isStopped = false;
        frozen = false;
        Debug.Log(frozen);
    }
    public void defeatEnemy()
    {
        if (!dead)
        {
            dead = true;
            StartCoroutine(enemyDeath());
        }
    }
    private IEnumerator enemyDeath()
    {
        frozen = true;
        me.isStopped = true;
        anim.SetTrigger("Death");
        yield return new WaitForSeconds(4);
        Destroy(gameObject);
    }
}
