using UnityEngine;

public class Player : MonoBehaviour, IActorTemplate
{
    int travelSpeed;
    int health;
    int hitPower;
    GameObject actor;
    GameObject fire;

    public int Health
    {
        get { return health; }
        set { health = value; }
    }
    public GameObject Fire
    {
        get { return fire; }
        set { fire = value; }
    }

    GameObject _Player;

    float width;
    float height;

    void Start()
    {
        height = 1 / (Camera.main.WorldToViewportPoint(new Vector3(1,1,0)).y - .5f);
        width = 1 / (Camera.main.WorldToViewportPoint(new Vector3(1, 1, 0)).x - .5f);
        _Player = GameObject.Find("_Player");
    }
}
