﻿using UnityEngine;
using UnityEngine.AI;

public class EnemyFlee : MonoBehaviour, IActorTemplate {

    [SerializeField] SOActorModel actorModel;
    int health;
    int travelSpeed;
    int hitPower;
    int score;

    GameObject player;
    bool gameStarts = false;

    [SerializeField] float enemyDistanceRun = 200;
    NavMeshAgent enemyAgent;

    void Start()
    {
        ActorStats(actorModel);
        Invoke("DelayedStart", 0.5f);
    }
    void Update()
    {
        if (gameStarts)
        {
            if (player != null)
            {
                float distance = Vector3.Distance(transform.position, player.transform.position);
                if (distance < enemyDistanceRun)
                {
                    Vector3 dirToPlayer = transform.position - player.transform.position;
                    Vector3 newPos = transform.position + dirToPlayer;
                    enemyAgent.SetDestination(newPos);
                }
            }
        }
    }
    void DelayedStart()
    {
        gameStarts = true;
        player = GameObject.FindGameObjectWithTag("Player");
        enemyAgent = GetComponent<NavMeshAgent>();
    }

    public void ActorStats(SOActorModel actorModel)
    {
        health = actorModel.health;
        hitPower = actorModel.hitPower;
        score = actorModel.scores;
        GetComponent<NavMeshAgent>().speed = actorModel.speed;
    }

	public void TakeDamage(int incomingDamage)
    {
        health -= incomingDamage;
    }
    public int SendDamage()
    {
        return hitPower;
    }
	public void Die()
    {
        Destroy(this.gameObject);
    }

	    void OnTriggerEnter(Collider other)
    {
        // if the player or their bullet hits you....
        if (other.tag == "Player")
        {
            if (health >= 1)
            {
                health -= other.GetComponent<IActorTemplate>().SendDamage();    
            }
            if (health <= 0)
            {
                //died by player, apply score to 
                GameManager.Instance.GetComponent<ScoreManager>().SetScore(score);
                Die();
            }
        }
    }
}
