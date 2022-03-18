using UnityEngine;

public class PlayerSpawner : MonoBehaviour
{
    SOActorModel actorModel;
    GameObject playerShip;
    bool upgradedShip = false;

    // Start is called before the first frame update
    void Start()
    {
        CreatePlayer();
        GetComponentInChildren<Player>().enabled = true;
    }
    void CreatePlayer()
    {
        //been shopping
        if (GameObject.Find("UpgradedShip"))
        {
            upgradedShip = true;
        }
        //not shopped or died
        //default ship build
        if (!upgradedShip || GameManager.Instance.Died)
        {
            GameManager.Instance.Died = false;
            actorModel = Object.Instantiate(Resources.Load("Script/ScriptableObject/Player_Default")) as SOActorModel;
            playerShip = GameObject.Instantiate(actorModel.actor, this.transform.position, Quaternion.Euler(270, 180, 0)) as GameObject;

            playerShip.GetComponent<IActorTemplate>().ActorStats(actorModel);
        }
        //apply the shop upgrades
        else
        {
            playerShip = GameObject.Find("UpgradedShip");
        }

        playerShip.transform.rotation = Quaternion.Euler(0, 180, 0);
        playerShip.transform.localScale = new Vector3(60, 60, 60);
        playerShip.GetComponentInChildren<ParticleSystem>().transform.localScale = new Vector3(25, 25, 25);
        playerShip.name = "Player";
        playerShip.transform.SetParent(this.transform);
        playerShip.transform.position = Vector3.zero;

        if (upgradedShip)
        {
            //set correct rotation to cluster bomb
            playerShip.transform.Find("c. Bomb(Clone)").rotation = Quaternion.Euler(-90, 180, 0);
        }        
    }
}
