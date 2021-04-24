using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using Random = System.Random;
using UnityEngine.AI;
using UnityEngine.ProBuilder;

public class EnemyAI : MonoBehaviour
{
    public enum FSMStates
    {
        Loot,
        Patrol,
        Chase,
        Attack,
        Dead,
        RangedAttack,
        Store,
        RangedChase
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
    public float attackDistance = 7;
    public float chaseDistance = 10;
    public float strikeDistance = 2;
    private Boolean beenHit = false;
    private Boolean weaponThrown = false;

    // Melee Enemy Variables
    public GameObject weapon;
    public int enemyDamage = 20;
    public AudioClip hitFX;

    // Ranged Enemy Variables
    public AudioClip shootFX;
    // public GameObject arrows;
    public float rangedAttackDistance = 20;
    public float rangedChaseDistance = 30;

    public GameObject hand;
    public GameObject rShoulder;
    public float shootRate = 4.5f;
    private float elapsedTime = 0;

    // Enemy Navigation
    private Vector3 nextDestination;
    private Vector3 nextLootDestination;
    private int currentDestinationIndex = 0;
    private NavMeshAgent agent;
    private GameObject[] wanderPoints;

    // Target Information
    private float distanceToPlayer;
    public GameObject player;

    // Enemy Death
    // public GameObject deadVFX;
    // private Transform deadTransform;
    // private bool isDead;

    // Enemy Vision
    public Transform enemyEyes;
    public float FOV = 45f;


    // Start is called before the first frame update
    void Start()
    {
        // isDead = false;
        player = GameObject.FindGameObjectWithTag("Player");
        wanderPoints = GameObject.FindGameObjectsWithTag("WanderPoint");
        anim = GetComponent<Animator>();
        enemyHealth = GetComponent<EnemyHealth>();
        health = enemyHealth.currentHealth;
        agent = GetComponent<NavMeshAgent>();
        enemyDamage = 20;
        Initialize();
    }

    // Update is called once per frame
    void Update()
    {
        distanceToPlayer = Vector3.Distance(transform.position, player.transform.position);
        health = enemyHealth.currentHealth;


        switch (currentState)
        {
            case FSMStates.Loot:
                UpdateLootState();
                break;
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
            case FSMStates.Store:
                UpdateStoreState();
                break;
            case FSMStates.RangedAttack:
                UpdateRangedAttackState();
                break;
            case FSMStates.RangedChase:
                UpdateRangedChaseState();
                break;
        }


        elapsedTime += Time.deltaTime;

        if (health <= 0)
        {
            currentState = FSMStates.Dead;
        }
    }

    private void UpdateRangedChaseState()
    {
        anim.SetInteger("animState", 2);

        agent.stoppingDistance = rangedAttackDistance;

        agent.speed = 5;

        nextDestination = player.transform.position;
        if (distanceToPlayer <= rangedAttackDistance)
        {
            currentState = FSMStates.RangedAttack;
        }
        else if (distanceToPlayer > rangedChaseDistance)
        {
            currentState = FSMStates.Patrol;
        }

        FaceTarget(nextDestination);
        agent.SetDestination(nextDestination);
    }

    private void UpdateStoreState()
    {
        anim.SetInteger("animState", 1);

        agent.stoppingDistance = 0;

        agent.speed = 3.5f;

        if (Vector3.Distance(transform.position, nextDestination) <= 2)
        {
            currentState = FSMStates.Loot;
        }
        else if (distanceToPlayer <= chaseDistance && IsPlayerInClearFOV())
        {
            currentState = FSMStates.Chase;
        }
        else if (beenHit)
        {
            nextDestination = player.transform.position;
            FaceTarget(nextDestination);
            agent.SetDestination(nextDestination);
            agent.speed = 5.0f;
            anim.SetInteger("animState", 2);
        }

        FaceTarget(nextDestination);
        agent.SetDestination(nextDestination);
    }

    // ReSharper disable Unity.PerformanceAnalysis
    private void UpdateLootState()
    {
        anim.SetInteger("animState", 1);

        agent.stoppingDistance = 0;

        agent.speed = 3.5f;

        if (Vector3.Distance(transform.position, nextLootDestination) <= 2)
        {
            currentState = FSMStates.Store;
        }
        else if (distanceToPlayer <= chaseDistance && IsPlayerInClearFOV())
        {
            currentState = FSMStates.Chase;
        }
        else if (beenHit)
        {
            nextDestination = player.transform.position;
            FaceTarget(nextDestination);
            agent.SetDestination(nextDestination);
            agent.speed = 5.0f;
            anim.SetInteger("animState", 2);
        }

        FaceTarget(nextLootDestination);
        agent.SetDestination(nextLootDestination);
    }

    // ReSharper disable Unity.PerformanceAnalysis
    private void UpdateRangedAttackState()
    {
        anim.SetInteger("animState", 4);
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
        
        EnemyShoot();
    }

    private void EnemyShoot()
    {
        // Fire projectiles at Player
        if (elapsedTime >= shootRate)
        {
            if (!weaponThrown && Vector3.Distance(hand.transform.position, weapon.transform.position) < 1)
            {
                Rigidbody rb = weapon.GetComponent<Rigidbody>();
                BoxCollider c = weapon.GetComponent<BoxCollider>();
                c.size = Vector3.one;
                rb.freezeRotation = false;
                var aimTarget = nextDestination;
                aimTarget.y = 0;
                rb.velocity = (aimTarget - transform.position).normalized * 5f;
                rb.useGravity = true;
                FaceTarget(nextDestination);
                // weapon.transform.SetParent(GameObject.FindGameObjectWithTag("EnemyProjectileParent").transform);
                AudioSource.PlayClipAtPoint(shootFX, weapon.transform.position);
                elapsedTime = 0;
                weaponThrown = true;
                weapon.transform.position = new Vector3(1000, 1000, 1000);
                rb.useGravity = false;
            }
            else
            {
                var throwableAxe = Instantiate(weapon, rShoulder.transform.position + transform.forward, rShoulder.transform.rotation) as GameObject;
                Rigidbody rb = throwableAxe.GetComponent<Rigidbody>();
                BoxCollider c = throwableAxe.GetComponent<BoxCollider>();
                c.size = Vector3.one;
                rb.freezeRotation = false;
                rb.useGravity = false;
                var aimTarget = nextDestination;
                aimTarget.y = 0;
                rb.velocity = (aimTarget - transform.position).normalized * 5f;
                FaceTarget(nextDestination);
                throwableAxe.transform.SetParent(GameObject.FindGameObjectWithTag("EnemyProjectileParent").transform);
                AudioSource.PlayClipAtPoint(shootFX, transform.position);
                elapsedTime = 0;  
            }
            
        }
        
    }


    private void Initialize()
    {
        if (enemyType == AttackType.Melee)
        {
            currentState = FSMStates.Loot;
            Loot();
        }

        if (enemyType == AttackType.Ranged)
        {
            currentState = FSMStates.Patrol;
            FindNextPoint();
        }
        
    }

    private void UpdateDeadState()
    {
        anim.SetInteger("animState", 5);
        // deadTransform = gameObject.transform;
        // isDead = true;
        Destroy(gameObject, 2);
    }

    private void UpdateAttackState()
    {
        

        if (distanceToPlayer <= attackDistance)
        {
            currentState = FSMStates.Attack;
            agent.stoppingDistance = strikeDistance;
            nextDestination = player.transform.position;
            FaceTarget(nextDestination);
            agent.SetDestination(nextDestination);
            if (distanceToPlayer < strikeDistance)
            {
                anim.SetInteger("animState", 3);
            }
        }
        else if (distanceToPlayer > attackDistance && distanceToPlayer <= chaseDistance)
        {
            currentState = FSMStates.Chase;
        }
        else if (distanceToPlayer > chaseDistance)
        {
            currentState = FSMStates.Patrol;
        }

        
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
        
        if (enemyType == AttackType.Ranged)
        {
            if (Vector3.Distance(transform.position, nextDestination) <= 2)
            {
                FindNextPoint();
            }
            else if (distanceToPlayer <= chaseDistance && IsPlayerInClearFOV())
            {
                currentState = FSMStates.RangedChase;
            }
            else if (beenHit)
            {
                nextDestination = player.transform.position;
                FaceTarget(nextDestination);
                agent.SetDestination(nextDestination);
                agent.speed = 5.0f;
                anim.SetInteger("animState", 2);
            }

            FaceTarget(nextDestination);
            agent.SetDestination(nextDestination);
        }

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
        nextDestination = wanderPoints[UnityEngine.Random.Range(currentDestinationIndex, wanderPoints.Length)].transform.position;
        currentDestinationIndex = (currentDestinationIndex + 1) % wanderPoints.Length;
        agent.SetDestination(nextDestination);
    }

    // ReSharper disable Unity.PerformanceAnalysis
    void Loot()
    {
        var allPots = GameObject.FindGameObjectsWithTag("Pot");
        nextLootDestination = allPots[UnityEngine.Random.Range(currentDestinationIndex, allPots.Length)].transform.position;
        currentDestinationIndex = (currentDestinationIndex + 1) % allPots.Length;
        agent.SetDestination(nextLootDestination);
    }

    void StoreLoot()
    {
        var allStores = GameObject.FindGameObjectsWithTag("Respawn");
        nextDestination = allStores[UnityEngine.Random.Range(currentDestinationIndex, allStores.Length)].transform.position;
        currentDestinationIndex = (currentDestinationIndex + 1) % allStores.Length;
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

    // private void OnDestroy()
    // {
    //     Instantiate(deadVFX, deadTransform.position, deadTransform.rotation);
    // }

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

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Projectile"))
        {
            beenHit = true;
        }
    }
}