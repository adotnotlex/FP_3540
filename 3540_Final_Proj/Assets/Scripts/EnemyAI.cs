using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using Random = System.Random;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    public enum FSMStates
    {
        Idle,
        Patrol,
        Chase,
        Attack,
        Dead,
        RangedAttack
    }

    public enum AttackType
    {
        Melee,
        Ranged
    }

    public FSMStates currentState;
    private Animator anim;
    public AttackType enemyType = AttackType.Melee;

    // Enemy Stats
    private EnemyHealth enemyHealth;
    private int health;
    public float enemySpeed = 5;
    public float attackDistance = 2;
    public float chaseDistance = 10;

    // Melee Enemy Variables
    public GameObject weapon;
    // private Collider weaponEdge;
    public int enemyMeleeDamage = 1;
    public AudioClip hitFX;

    // Ranged Enemy Variables
    public AudioClip shootFX;
    public GameObject arrows;
    public float rangedAttackDistance = 15;

    public float rangedChaseDistance = 25;

    // public GameObject wandTip;
    public float shootRate = 2;
    private float elapsedTime = 0;

    // Enemy Navigation
    private Vector3 nextDestination;
    private int currentDestinationIndex = 0;
    private NavMeshAgent agent;
    private GameObject[] wanderPoints;

    // Target Information
    private float distanceToPlayer;
    public GameObject player;

    // Enemy Death
    public GameObject deadVFX;
    private Transform deadTransform;
    private bool isDead;

    // Enemy Vision
    public Transform enemyEyes;
    public float FOV = 45f;


    // Start is called before the first frame update
    void Start()
    {
        isDead = false;
        player = GameObject.FindGameObjectWithTag("Player");
        wanderPoints = GameObject.FindGameObjectsWithTag("WanderPoint");
        anim = GetComponent<Animator>();
        // wandTip = GameObject.FindGameObjectWithTag("WandTip");
        enemyHealth = GetComponent<EnemyHealth>();
        health = enemyHealth.currentHealth;
        agent = GetComponent<NavMeshAgent>();
        // weaponEdge = weapon.GetComponent<Collider>();
        Initialize();
    }

    // Update is called once per frame
    void Update()
    {
        distanceToPlayer = Vector3.Distance(transform.position, player.transform.position);
        health = enemyHealth.currentHealth;


        switch (currentState)
        {
            case FSMStates.Patrol:
                UpdatePatrolState();
                break;
            case FSMStates.Chase:
                UpdateChaseState();
                break;
            case FSMStates.Attack:
                UpdateAttackState();
                break;
            case FSMStates.Dead:
                UpdateDeadState();
                break;
        }


        elapsedTime = -Time.deltaTime;

        if (health <= 0)
        {
            currentState = FSMStates.Dead;
        }
    }

    private void UpdateRangedAttackState()
    {
        agent.stoppingDistance = rangedAttackDistance;
        nextDestination = player.transform.position;

        if (distanceToPlayer <= rangedAttackDistance)
        {
            currentState = FSMStates.RangedAttack;
        }
        else if (distanceToPlayer > rangedAttackDistance && distanceToPlayer <= rangedChaseDistance)
        {
            currentState = FSMStates.Chase;
        }
        else if (distanceToPlayer > rangedChaseDistance)
        {
            currentState = FSMStates.Patrol;
        }

        FaceTarget(nextDestination);
        anim.SetInteger("animState", 4);
        EnemyShoot();
    }

    private void EnemyShoot()
    {
        // Fire projectiles at Player
        throw new NotImplementedException();
    }


    private void Initialize()
    {
        currentState = FSMStates.Patrol;
        FindNextPoint();
    }

    private void UpdateDeadState()
    {
        anim.SetInteger("animState", 5);
        deadTransform = gameObject.transform;
        isDead = true;
        Destroy(gameObject, 3);
    }

    private void UpdateAttackState()
    {
        agent.stoppingDistance = attackDistance;
        nextDestination = player.transform.position;

        if (distanceToPlayer <= attackDistance)
        {
            currentState = FSMStates.Attack;
        }
        else if (distanceToPlayer > attackDistance && distanceToPlayer <= chaseDistance)
        {
            currentState = FSMStates.Chase;
        }
        else if (distanceToPlayer > chaseDistance)
        {
            currentState = FSMStates.Patrol;
        }

        FaceTarget(nextDestination);
        anim.SetInteger("animState", 3);
        EnemyMeleeAttack();
    }

    private void EnemyMeleeAttack()
    {
        weapon.GetComponent<WeaponHit>().DamagePlayer();
    }

    private void UpdateChaseState()
    {
        anim.SetInteger("animState", 2);

        agent.stoppingDistance = attackDistance;

        agent.speed = 5;

        nextDestination = player.transform.position;
        if (distanceToPlayer <= attackDistance)
        {
            currentState = FSMStates.Attack;
        }
        else if (distanceToPlayer > chaseDistance)
        {
            FindNextPoint();
            currentState = FSMStates.Patrol;
        }

        FaceTarget(nextDestination);
        agent.SetDestination(nextDestination);
    }

    private void UpdatePatrolState()
    {
        anim.SetInteger("animState", 1);

        agent.stoppingDistance = 0;

        agent.speed = 3.5f;

        if (Vector3.Distance(transform.position, nextDestination) <= 2)
        {
            FindNextPoint();
        }
        else if (distanceToPlayer <= chaseDistance && IsPlayerInClearFOV())
        {
            currentState = FSMStates.Chase;
        }

        FaceTarget(nextDestination);
        agent.SetDestination(nextDestination);
    }

    private void FaceTarget(Vector3 target)
    {
        Vector3 directionToTarget = (target - transform.position).normalized;
        directionToTarget.y = 0;

        Quaternion lookRotation = Quaternion.LookRotation(directionToTarget);
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, 10 * Time.deltaTime);
    }

    void FindNextPoint()
    {
        nextDestination = wanderPoints[currentDestinationIndex].transform.position;
        currentDestinationIndex = (currentDestinationIndex + 1) % wanderPoints.Length;
        agent.SetDestination(nextDestination);
    }

    private void OnDrawGizmos()
    {
        if (enemyType == AttackType.Melee)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position + new Vector3(0, 0, 0), attackDistance);

            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(transform.position + new Vector3(0, 0, 0), chaseDistance);

            Vector3 frontRayPoint = enemyEyes.position + (enemyEyes.forward * chaseDistance);
            Vector3 leftRayPoint = Quaternion.Euler(0, FOV * 0.5f, 0) * frontRayPoint;
            Vector3 rightRayPoint = Quaternion.Euler(0, -FOV * 0.5f, 0) * frontRayPoint;

            Debug.DrawLine(enemyEyes.position, frontRayPoint, Color.cyan);
            Debug.DrawLine(enemyEyes.position, leftRayPoint, Color.yellow);
            Debug.DrawLine(enemyEyes.position, rightRayPoint, Color.yellow);
        }
        else if (enemyType == AttackType.Ranged)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position + new Vector3(0, 0, 0), rangedAttackDistance);

            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(transform.position + new Vector3(0, 0, 0), rangedChaseDistance);

            Vector3 frontRayPoint = enemyEyes.position + (enemyEyes.forward * rangedChaseDistance);
            Vector3 leftRayPoint = Quaternion.Euler(0, FOV * 0.5f, 0) * frontRayPoint;
            Vector3 rightRayPoint = Quaternion.Euler(0, -FOV * 0.5f, 0) * frontRayPoint;

            Debug.DrawLine(enemyEyes.position, frontRayPoint, Color.cyan);
            Debug.DrawLine(enemyEyes.position, leftRayPoint, Color.yellow);
            Debug.DrawLine(enemyEyes.position, rightRayPoint, Color.yellow);
        }
    }

    // void EnemySpellCast()
    // {
    //     if (!isDead)
    //     {
    //         if (elapsedTime >= shootRate)
    //         { 
    //             var animDuration = anim.GetCurrentAnimatorStateInfo(0).length;
    //             Invoke("SpellCasting", animDuration);
    //             elapsedTime = 0.0f;
    //         }
    //     }
    //     
    // }

    // void SpellCasting()
    // {
    //     GameObject spellProjectile = arrows;
    //     // Instantiate(spellProjectile, wandTip.transform.position, wandTip.transform.rotation);
    //     
    //     var projectile = Instantiate(spellProjectile, wandTip.transform.position 
    //                                                    + wandTip.transform.forward, wandTip.transform.rotation);
    //     Rigidbody p = projectile.GetComponent<Rigidbody>();
    //             
    //     p.AddForce(transform.forward * 100, ForceMode.Impulse);
    //     projectile.transform.SetParent(GameObject.FindGameObjectWithTag("ProjectileParent").transform);
    //     AudioSource.PlayClipAtPoint(shootFX, wandTip.transform.position);
    // }

    private void OnDestroy()
    {
        Instantiate(deadVFX, deadTransform.position, deadTransform.rotation);
    }

    bool IsPlayerInClearFOV()
    {
        RaycastHit hit;
        Vector3 directionToPlayer = player.transform.position - enemyEyes.position;
        if (Vector3.Angle(directionToPlayer, enemyEyes.forward) <= FOV)
        {
            if (Physics.Raycast(enemyEyes.position, directionToPlayer, out hit, chaseDistance))
            {
                if (hit.collider.CompareTag("Player"))
                {
                    return true;
                }
            }

            return false;
        }

        return false;
    }
}