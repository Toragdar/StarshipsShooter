using UnityEngine;
using System.Collections;
using UnityEngine.Advertisements;

public class PlayerShipBuild : MonoBehaviour
{
    [SerializeField] GameObject[] shopButtons;
    GameObject target;
    GameObject tmpSelection;
    GameObject textBoxPanel;

    [SerializeField] GameObject[] visualWeapons;
    [SerializeField] SOActorModel defaultPlayerShip;
    GameObject playerShip;
    GameObject buyButton;
    GameObject bankObject;
    int bank = 2000;
    bool purchaseMade = false;

    string placementId_rewardedvideo = "rewardedVideo";
    string gameId = "1234567";

    void Start()
    {
        textBoxPanel = GameObject.Find("textBoxPanel");
        TurnOffSelectionHighlights();

        purchaseMade = false;
        bankObject = GameObject.Find("bank");
        bankObject.GetComponentInChildren<TextMesh>().text = bank.ToString();
        buyButton = textBoxPanel.transform.Find("BUY ?").gameObject;

        TurnOffPlayerShipVisuals();
        PreparePlayerShipForUpgrade();
    }
    void Update()
    {
        AttemptSelection();
    }
    void TurnOffSelectionHighlights()
    {
        for(int i=0; i < shopButtons.Length; i++)
        {
            shopButtons[i].SetActive(false);
        }
    }
    GameObject ReturnClickedObject(out RaycastHit hit)
    {
        GameObject target = null;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if(Physics.Raycast(ray.origin,ray.direction*100, out hit))
        {
            target = hit.collider.gameObject;
        }
        return target;
    }
    void AttemptSelection()
    {
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit hitInfo;
            target = ReturnClickedObject(out hitInfo);

            if (target != null)
            {
                if (target.transform.Find("itemText"))
                {
                    TurnOffSelectionHighlights();
                    Select();
                    UpdateDescriptionBox();

                    //Not already sold
                    if (target.transform.Find("itemText").GetComponent<TextMesh>().text != "SOLD")
                    {
                        //can afford
                        Affordable();

                        //canot afford
                        LackOfCredits();
                    }
                    else if (target.transform.Find("itemText").GetComponent<TextMesh>().text == "SOLD")
                    {
                        SoldOut();
                    }                    
                }
                else if (target.name == "WATCH AD")
                {
                    //watchAd
                }
                else if (target.name == "BUY ?")
                {
                    BuyItem();
                }
                else if (target.name == "START")
                {
                    StartGame();
                }
            }
        }
    }
    void BuyItem()
    {
        Debug.Log("PURCHASED");
        purchaseMade = true;
        buyButton.SetActive(false);
        tmpSelection.SetActive(false);

        for (int i = 0; i < visualWeapons.Length; i++)
        {
            if (visualWeapons[i].name == tmpSelection.transform.parent.gameObject.GetComponent<ShopPiece>().ShopSelection.iconName)
            {
                visualWeapons[i].SetActive(true);
            }
        }

        UpgradeToShip(tmpSelection.transform.parent.gameObject.GetComponent<ShopPiece>().ShopSelection.iconName);
        bank = bank - System.Int32.Parse(tmpSelection.transform.parent.GetComponent<ShopPiece>().ShopSelection.cost);
        bankObject.transform.Find("bankText").GetComponent<TextMesh>().text = bank.ToString();
        tmpSelection.transform.parent.transform.Find("itemText").GetComponent<TextMesh>().text = "SOLD";
    }
    void Affordable()
    {
        if (bank >= System.Int32.Parse(target.transform.GetComponent<ShopPiece>().ShopSelection.cost))
        {
            Debug.Log("CAN BUY");
            buyButton.SetActive(true);
        }
    }
    void LackOfCredits()
    {
        if (bank < System.Int32.Parse(target.transform.Find("itemText").GetComponent<TextMesh>().text))
        {
            Debug.Log("CAN'T BUY");
        }
    }
    void SoldOut()
    {
        Debug.Log("SOLD OUT");
    }
    void TurnOffPlayerShipVisuals()
    {
        for (int i = 0; i < visualWeapons.Length; i++)
        {
            visualWeapons[i].gameObject.SetActive(false);
        }
    }
    void PreparePlayerShipForUpgrade()
    {
        playerShip = GameObject.Instantiate(Resources.Load("Prefab/Player/Player_Ship")) as GameObject;
        playerShip.GetComponent<Player>().enabled = false;
        playerShip.transform.position = new Vector3(0, 10000, 0);
        playerShip.GetComponent<IActorTemplate>().ActorStats(defaultPlayerShip);
    }
    void Select()
    {
        tmpSelection = target.transform.Find("SelectionQuad").gameObject;
        tmpSelection.SetActive(true);
    }
    void UpdateDescriptionBox()
    {
        textBoxPanel.transform.Find("name").gameObject.GetComponent<TextMesh>().text = tmpSelection.GetComponentInParent<ShopPiece>().ShopSelection.iconName;
        textBoxPanel.transform.Find("desc").gameObject.GetComponent<TextMesh>().text = tmpSelection.GetComponentInParent<ShopPiece>().ShopSelection.description;
    }
    void UpgradeToShip(string upgrade)
    {
        GameObject shipItem = GameObject.Instantiate(Resources.Load("Prefab/Player/" + upgrade)) as GameObject;
        shipItem.transform.SetParent(playerShip.transform);
        shipItem.transform.localPosition = Vector3.zero;
    }
    void StartGame()
    {
        if (purchaseMade)
        {
            playerShip.name = "UpgradedShip";
            if (playerShip.transform.Find("energy +1(Clone)"))
            {
                playerShip.GetComponent<Player>().Health = 2;
            }
            DontDestroyOnLoad(playerShip);
        }
        UnityEngine.SceneManagement.SceneManager.LoadScene("testLevel");
    }
    void CheckPlatform()
    {
        if (Application.platform == RuntimePlatform.IPhonePlayer)
        {
            //«¿Ã≈Õ»“‹ ¿…ƒ»
            gameId = "REPLACE-THIS-TEXT-FOR-YOUR-IPHONE-GAMEID";
        }
        else if (Application.platform == RuntimePlatform.Android)
        {
            //«¿Ã≈Õ»“‹ ¿…ƒ»
            gameId = "REPLACE-THIS-TEXT-FOR-YOUR-ANDROID-GAMEID";
        }
        //¬ÍÎ˛˜ÂÌ ÚÂÒÚÓ‚˚È ÂÊËÏ ÂÍÎ‡Ï˚
        Advertisement.Initialize(gameId, true);
    }
}
