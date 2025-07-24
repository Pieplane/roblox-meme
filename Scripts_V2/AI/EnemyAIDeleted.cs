using StarterAssets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem.XR;

public class EnemyAIDeleted : MonoBehaviour
{
    public NavMeshAgent agent;
    public Transform player;
    public LayerMask whatIsGround, whatIsPlayer;
    public Animator anim;

    public float health;

    // Патрулирование
    public Transform[] patrolPoints;
    private int currentPatrolIndex;
    private bool walkPointSet;
    private Vector3 walkPoint;
    public float walkPointRange;

    // Обнаружение
    public float sightRange;
    public float stopDistance;
    private bool playerInSightRange;
    private bool playerInStopRange;

    // Атака
    public float timeBetweenAttacks = 1.5f;
    private bool alreadyAttacked;

    private enum State { Patrolling, Chasing, Attacking, Searching }
    private State currentState;

    public float patrolSpeed = 2f;
    public float chaseSpeed = 4f;

    public float viewAngle = 90f;
    public float viewDistance = 10f;
    public float rotationSpeed = 15f;

    private bool wasChasingPlayer = false;
    private float lostPlayerTimer = 0f;
    public float timeToSearchAfterLost = 2f;

    private Vector3 lastKnownPlayerPosition;
    private bool isGoingToLastKnownPosition = false;

    //private float patrolWaitTime = 1f;
    //private float patrolTimer = 0f;

    public GameObject timeline;
    ThirdPersonController controller;

    private void Start()
    {
        agent.autoBraking = false;
        controller = player.GetComponent<ThirdPersonController>();
    }

    private void Update()
    {
        playerInSightRange = IsPlayerVisible();
        playerInStopRange = Physics.CheckSphere(transform.position, stopDistance, whatIsPlayer);
        Debug.Log(currentState);
        Debug.Log("IsPlaying" + IsPlayingAnimation("confused"));

        if (playerInSightRange)
        {
            wasChasingPlayer = true;
            lostPlayerTimer = 0f;
            lastKnownPlayerPosition = player.position;
            currentState = playerInStopRange ? State.Attacking : State.Chasing;
            isGoingToLastKnownPosition = false;
        }
        else if (wasChasingPlayer)
        {
            currentState = State.Searching;
        }
        else
        {
            currentState = State.Patrolling;
        }

        switch (currentState)
        {
            case State.Patrolling:
                Patrol();
                break;
            case State.Chasing:
                ChasePlayer();
                break;
            case State.Attacking:
                StopAndAttack();
                break;
            case State.Searching:
                SearchForPlayer();
                break;
        }
    }

    private void Patrol()
    {
        anim.SetBool("walking", true);
        if (patrolPoints.Length == 0)
        {
            // Если нет точек — используем случайные
            if (!walkPointSet) SearchWalkPoint();
            if (walkPointSet)
            {
                agent.isStopped = false;
                agent.speed = patrolSpeed;
                agent.SetDestination(walkPoint);

                if (Vector3.Distance(transform.position, walkPoint) < 1f)
                    walkPointSet = false;
            }
            return;
        }

        // Если только одна точка
        if (patrolPoints.Length == 1)
        {
            agent.SetDestination(patrolPoints[0].position);

            if (!agent.pathPending && agent.remainingDistance < 0.5f)
            {
                agent.isStopped = true;
                agent.velocity = Vector3.zero;

                // Плавно поворачиваемся в ту же сторону, что и точка
                Quaternion targetRotation = patrolPoints[0].rotation;
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);
            }
            else
            {
                agent.isStopped = false;
                agent.speed = patrolSpeed;
            }

            return;
        }

        // Если точек больше одной
        agent.isStopped = false;
        agent.speed = patrolSpeed;

        if (!agent.hasPath || agent.remainingDistance < 0.5f)
        {
            currentPatrolIndex = (currentPatrolIndex + 1) % patrolPoints.Length;
            agent.SetDestination(patrolPoints[currentPatrolIndex].position);
        }
    }

    private void SearchWalkPoint()
    {
        for (int i = 0; i < 10; i++)
        {
            float randomX = Random.Range(-walkPointRange, walkPointRange);
            float randomZ = Random.Range(-walkPointRange, walkPointRange);
            Vector3 randomPoint = transform.position + new Vector3(randomX, 0, randomZ);

            if (NavMesh.SamplePosition(randomPoint, out NavMeshHit hit, 1f, NavMesh.AllAreas))
            {
                NavMeshPath path = new NavMeshPath();
                if (agent.CalculatePath(hit.position, path) && path.status == NavMeshPathStatus.PathComplete)
                {
                    walkPoint = hit.position;
                    walkPointSet = true;
                    return;
                }
            }
        }

        walkPointSet = false;
    }

    private void ChasePlayer()
    {
        anim.applyRootMotion = false;
        anim.SetBool("confused", false);
        anim.SetBool("running", true);
        agent.isStopped = false;
        agent.updateRotation = true;
        agent.speed = chaseSpeed;
        agent.SetDestination(player.position);

        RotateTo(player.position);
    }

    private void StopAndAttack()
    {
        anim.applyRootMotion = false;
        agent.velocity = Vector3.zero;
        agent.isStopped = true;
        agent.updateRotation = false;
        anim.SetTrigger("attack");
        timeline.SetActive(true);
        controller.enabled = false;

        RotateTo(player.position);

        if (!alreadyAttacked)
        {
            // anim.SetTrigger("Attack"); // Раскомментируй при наличии триггера
            alreadyAttacked = true;
            Invoke(nameof(ResetAttack), timeBetweenAttacks);
        }
    }

    private void SearchForPlayer()
    {
        if (!isGoingToLastKnownPosition)
        {
            agent.isStopped = false;
            agent.speed = patrolSpeed;
            agent.SetDestination(lastKnownPlayerPosition);
            //anim.SetBool("run", true);
            isGoingToLastKnownPosition = true;
        }

        float distance = Vector3.Distance(transform.position, lastKnownPlayerPosition);

        if (distance <= 1f)
        {
            agent.isStopped = true;
            agent.velocity = Vector3.zero;
            agent.ResetPath();
            //anim.SetBool("run", false);

            lostPlayerTimer += Time.deltaTime;

            if (lostPlayerTimer <= 0.01f)
            {
                anim.applyRootMotion = false;
                anim.SetBool("confused", true);
            }

            if (lostPlayerTimer >= timeToSearchAfterLost && !IsPlayingAnimation("confused"))
            {
                lostPlayerTimer = 0f;
                wasChasingPlayer = false;
                isGoingToLastKnownPosition = false;
                anim.SetBool("confused", false);
                anim.SetBool("running", false);
                //Debug.Log("Запускаю патрулинг");
                currentState = State.Patrolling;
            }
        }
    }

    private bool IsPlayerVisible()
    {
        Vector3 directionToPlayer = player.position - transform.position;
        float distanceToPlayer = directionToPlayer.magnitude;

        if (distanceToPlayer > viewDistance) return false;

        float angle = Vector3.Angle(transform.forward, directionToPlayer);
        if (angle > viewAngle / 2f) return false;

        if (Physics.Raycast(transform.position + Vector3.up, directionToPlayer.normalized, out RaycastHit hit, viewDistance))
            return hit.transform == player;

        return false;
    }

    private bool IsPlayingAnimation(string animationName)
    {
        return anim.GetCurrentAnimatorStateInfo(0).IsName(animationName) &&
               anim.GetCurrentAnimatorStateInfo(0).normalizedTime < 1.0f;
    }

    private void RotateTo(Vector3 targetPosition)
    {
        Vector3 direction = targetPosition - transform.position;
        direction.y = 0;
        if (direction == Vector3.zero) return;

        Quaternion lookRotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * rotationSpeed);
    }

    private void ResetAttack()
    {
        alreadyAttacked = false;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, sightRange);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, stopDistance);

        if (Application.isPlaying)
        {
            Vector3 forward = transform.forward;
            Vector3 leftLimit = Quaternion.Euler(0, -viewAngle / 2f, 0) * forward;
            Vector3 rightLimit = Quaternion.Euler(0, viewAngle / 2f, 0) * forward;

            Gizmos.color = Color.blue;
            Gizmos.DrawRay(transform.position + Vector3.up, leftLimit * viewDistance);
            Gizmos.DrawRay(transform.position + Vector3.up, rightLimit * viewDistance);
        }
    }
}
