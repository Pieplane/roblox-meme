using Cinemachine;
using StarterAssets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem.XR;

public class EnemyAI : MonoBehaviour
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

    private enum State { Patrolling, Chasing, Attacking, Searching}
    private State currentState;

    public float patrolSpeed = 2f;
    public float chaseSpeed = 4f;

    public float viewAngle = 90f;
    public float viewDistance = 10f;
    public float rotationSpeed = 15f;

    public string catchCutscene;

    private bool wasChasingPlayer = false;
    private float lostPlayerTimer = 0f;
    public float timeToSearchAfterLost = 2f;

    private Vector3 lastKnownPlayerPosition;
    private bool isGoingToLastKnownPosition = false;

    private float patrolWaitTime = 1f;
    private float patrolTimer = 0f;

    public float delay;

    private Vector3 initialPosition;
    private Quaternion initialRotation;
    public VentInteraction ventInteraction;

    

    //public GameObject timeline;
    //ThirdPersonController controller;

    private void Start()
    {
        initialPosition = transform.localPosition;
        initialRotation = transform.localRotation;
        Debug.Log($"Initial position:{initialPosition}, Initial rotation: {initialRotation}");
        agent.autoBraking = false;
        Debug.Log($"[EnemyAI] viewDistance = {viewDistance}, sightRange = {sightRange}, stopDistance = {stopDistance}");
        //controller = player.GetComponent<ThirdPersonController>();
    }

    private void Update()
    {
        playerInSightRange = IsPlayerVisible();
        playerInStopRange = Physics.CheckSphere(transform.position, stopDistance, whatIsPlayer);

        //Debug.Log($"[EnemyAI] CurrentState: {currentState}, PlayerInSight: {playerInSightRange}, PlayerInStopRange: {playerInStopRange}, IsPlayerVisible: {IsPlayerVisible()}");

        if (playerInSightRange)
        {
            //Debug.Log("[EnemyAI] Player spotted.");
            wasChasingPlayer = true;
            lostPlayerTimer = 0f;
            lastKnownPlayerPosition = player.position;
            currentState = playerInStopRange ? State.Attacking : State.Chasing;
            isGoingToLastKnownPosition = false;
        }
        else if (wasChasingPlayer)
        {
            //Debug.Log("[EnemyAI] Lost sight of player, searching...");
            currentState = State.Searching;
        }
        else
        {
            currentState = State.Patrolling;
        }

        switch (currentState)
        {
            case State.Patrolling:
                //Debug.Log("[EnemyAI] Patrolling...");
                Patrol();
                break;
            case State.Chasing:
                //Debug.Log("[EnemyAI] Chasing player...");
                ChasePlayer();
                break;
            case State.Attacking:
                //Debug.Log("[EnemyAI] Attacking player!");
                StopAndAttack();
                break;
            case State.Searching:
                //Debug.Log("[EnemyAI] Searching for player...");
                SearchForPlayer();
                break;
            default:
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
            float distance = Vector3.Distance(transform.position, patrolPoints[0].position);

            if (distance > 0.5f)
            {
                agent.isStopped = false;
                agent.speed = patrolSpeed;
                agent.SetDestination(patrolPoints[0].position);
                anim.SetBool("walking", true);
            }
            else
            {
                agent.isStopped = true;
                agent.velocity = Vector3.zero;
                anim.SetBool("walking", false);

                // Плавный поворот к направлению
                Quaternion targetRotation = patrolPoints[0].rotation;
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);
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

        RotateTo(agent.velocity);
    }

    private void StopAndAttack()
    {
        //anim.applyRootMotion = false;
        agent.velocity = Vector3.zero;
        agent.isStopped = true;
        agent.updateRotation = false;
        //timeline?.SetActive(true);
        //controller.enabled = false;

        RotateTo(player.position);

        if (!alreadyAttacked)
        {
            // anim.SetTrigger("Attack"); // Раскомментируй при наличии триггера
            anim.SetBool("isAttacking", true);
            alreadyAttacked = true;
            CutsceneManager.Instance.StartCutscene(catchCutscene);
            if (player.childCount > 0)
            {
                Transform mesh = player.GetChild(1);
                //mesh.gameObject.SetActive(false);
                StartCoroutine(CallWithDelay(mesh.gameObject, delay));
            }

            //Invoke(nameof(ResetAttack), timeBetweenAttacks);
        }
    }
    private IEnumerator CallWithDelay(GameObject player, float delay)
    {
        Debug.Log("Ожидаю респавн");
        yield return new WaitForSeconds(delay);

        // Переместить и включить игрока
        ResetEnemy();
        GameManager.Instance.EnterID(CheckpointManager.Instance.GetLastCheckpointID());
        
        // 👇 можно убрать, если включение уже происходит внутри StagePreparation
        player.SetActive(true);
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
        Vector3 eyePosition = transform.position + Vector3.up;
        Vector3 targetPosition = player.position + Vector3.up;
        Vector3 directionToPlayer = targetPosition - eyePosition;
        float distanceToPlayer = directionToPlayer.magnitude;

        float angle = Vector3.Angle(transform.forward, directionToPlayer);

        bool inDistance = distanceToPlayer <= viewDistance;
        bool inAngle = angle <= viewAngle / 2f;

        //Debug.Log($"[IsPlayerVisible] Distance: {distanceToPlayer:F2}, InDistance: {inDistance}, Angle: {angle:F2}, InAngle: {inAngle}");

        if (!inDistance || !inAngle)
        {
            Debug.Log("[IsPlayerVisible] Player is out of view distance or angle.");
            return false;
        }

        if (Physics.Raycast(eyePosition, directionToPlayer.normalized, out RaycastHit hit, viewDistance, whatIsPlayer))
        {
            Debug.DrawRay(eyePosition, directionToPlayer.normalized * viewDistance, Color.green);

            bool isPlayerHit = hit.transform.root == player || hit.transform.CompareTag("Player");
            //Debug.Log($"[IsPlayerVisible] Raycast hit: {hit.transform.name}, IsPlayer: {isPlayerHit}");

            return isPlayerHit;
        }
        else
        {
            Debug.DrawRay(eyePosition, directionToPlayer.normalized * viewDistance, Color.red);
            Debug.Log("[IsPlayerVisible] Raycast hit nothing.");
        }

        return false;
    }

    private bool IsPlayingAnimation(string animationName)
    {
        return anim.GetCurrentAnimatorStateInfo(0).IsName(animationName) &&
               anim.GetCurrentAnimatorStateInfo(0).normalizedTime < 1.0f;
    }

    private void RotateTo(Vector3 targetPosition)
    {
        //Vector3 direction = targetPosition - transform.position;
        targetPosition.y = 0;
        if (targetPosition == Vector3.zero) return;

        Quaternion lookRotation = Quaternion.LookRotation(targetPosition);
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * rotationSpeed);
    }

    //private void ResetAttack()
    //{
    //    alreadyAttacked = false;
    //}
    public void ResetEnemy()
    {
        if(ventInteraction != null)
        {
            ventInteraction.ResetVent();
        }
        // 1. Отключаем агент, чтобы вручную выставить позицию
        agent.enabled = false;

        // 2. Сбрасываем позицию и поворот (лучше local если есть родитель)
        transform.localPosition = initialPosition;
        transform.localRotation = initialRotation * Quaternion.Euler(0, 90f, 0); // если надо довернуть

        // 3. Проверка — стоит ли он на NavMesh? Если нет — переместим вручную
        if (!NavMesh.SamplePosition(transform.position, out NavMeshHit hit, 1.0f, NavMesh.AllAreas))
        {
            Debug.LogWarning("[EnemyAI] Враг вне NavMesh! Принудительно перемещаю на ближайшую точку.");
            if (NavMesh.SamplePosition(initialPosition, out NavMeshHit fixedHit, 10f, NavMesh.AllAreas))
            {
                transform.position = fixedHit.position;
            }
        }

        // 4. Включаем обратно NavMeshAgent
        agent.enabled = true;

        // 5. Сбрасываем поведение и анимации
        alreadyAttacked = false;
        wasChasingPlayer = false;
        isGoingToLastKnownPosition = false;
        walkPointSet = false;
        currentPatrolIndex = 0;
        patrolTimer = 0f;
        lostPlayerTimer = 0f;
        currentState = State.Patrolling;

        anim.SetBool("isAttacking", false);
        anim.SetBool("running", false);
        anim.SetBool("walking", false);
        anim.SetBool("confused", false);

        agent.ResetPath();
        agent.isStopped = false;
        agent.velocity = Vector3.zero;
        agent.updateRotation = true;

        Debug.Log($"[EnemyAI] Сброшен. Позиция: {transform.position}, Поворот: {transform.rotation}");
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

            // Линия до игрока
            if (player != null)
            {
                Gizmos.color = Color.green;
                Gizmos.DrawLine(transform.position + Vector3.up, player.position + Vector3.up);
            }
        }
    }
}
