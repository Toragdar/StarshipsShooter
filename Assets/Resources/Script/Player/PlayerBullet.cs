using UnityEngine;

public class PlayerBullet : MonoBehaviour, IActorTemplate
{
    GameObject actor;
    int hitPower;
    int health;
    int travelSpeed;

    [SerializeField] SOActorModel bulletModel;

    void Awake()
    {
        ActorStats(bulletModel);
    }
    void Update()
    {
        transform.position += new Vector3(travelSpeed, 0, 0) * Time.deltaTime;
    }
    public void ActorStats(SOActorModel actorModel)
    {
        hitPower = actorModel.hitPower;
        health = actorModel.health;
        travelSpeed = actorModel.speed;
        actor = actorModel.actor;
    }

    public void Die()
    {
        Destroy(gameObject);
    }

    public int SendDamage()
    {
        return hitPower;
    }

    public void TakeDamage(int incomingDamage)
    {
        health -= incomingDamage;
    }
    void OnBecameInvisible()
    {
        Destroy(gameObject);
    }
    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Enemy")
        {
            if (other.GetComponent<IActorTemplate>() != null)
            {
                if (health >= 1)
                {
                    health -= other.GetComponent<IActorTemplate>().SendDamage();
                }
                if (health <= 0)
                {
                    Die();
                }
            }
        }
    }
}
