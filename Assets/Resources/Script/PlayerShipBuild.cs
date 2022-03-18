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
        CheckPlatform();
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
                    WatchAdvert();
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
    void WatchAdvert()
    {
        if (Application.internetReachability != NetworkReachability.NotReachable)
        {
            ShowRewardedAds();
        }
    }
    void ShowRewardedAds()
    {        
        Advertisement.Show(placementId_rewardedvideo);
        AdFinished();
    }
    void AdFinished ()
    {
        bank += 300;
        bankObject.GetComponentInChildren<TextMesh>().text = bank.ToString();
        TurnOffSelectionHighlights();
    }
}

//–≈¿À»«¿÷»ﬂ REWARDEDVIDEO - UNITY DOCUMENTATION

//using UnityEngine;
//using UnityEngine.UI;
//using UnityEngine.Advertisements;
// 
//public class RewardedAdsButton : MonoBehaviour, IUnityAdsLoadListener, IUnityAdsShowListener
//{
//    [SerializeField] Button _showAdButton;
//    [SerializeField] string _androidAdUnitId = "Rewarded_Android";
//    [SerializeField] string _iOSAdUnitId = "Rewarded_iOS";
//    string _adUnitId = null; // This will remain null for unsupported platforms
//
//    void Awake()
//    {
//        // Get the Ad Unit ID for the current platform:
//#if UNITY_IOS
//        _adUnitId = _iOSAdUnitId;
//#elif UNITY_ANDROID
//        _adUnitId = _androidAdUnitId;
//#endif
//
//        //Disable the button until the ad is ready to show:
//        _showAdButton.interactable = false;
//    }
//
//    // Load content to the Ad Unit:
//    public void LoadAd()
//    {
//        // IMPORTANT! Only load content AFTER initialization (in this example, initialization is handled in a different script).
//        Debug.Log("Loading Ad: " + _adUnitId);
//        Advertisement.Load(_adUnitId, this);
//    }
//
//    // If the ad successfully loads, add a listener to the button and enable it:
//    public void OnUnityAdsAdLoaded(string adUnitId)
//    {
//        Debug.Log("Ad Loaded: " + adUnitId);
//
//        if (adUnitId.Equals(_adUnitId))
//        {
//            // Configure the button to call the ShowAd() method when clicked:
//            _showAdButton.onClick.AddListener(ShowAd);
//            // Enable the button for users to click:
//            _showAdButton.interactable = true;
//        }
//    }
//
//    // Implement a method to execute when the user clicks the button:
//    public void ShowAd()
//    {
//        // Disable the button:
//        _showAdButton.interactable = false;
//        // Then show the ad:
//        Advertisement.Show(_adUnitId, this);
//    }
//
//    // Implement the Show Listener's OnUnityAdsShowComplete callback method to determine if the user gets a reward:
//    public void OnUnityAdsShowComplete(string adUnitId, UnityAdsShowCompletionState showCompletionState)
//    {
//        if (adUnitId.Equals(_adUnitId) && showCompletionState.Equals(UnityAdsShowCompletionState.COMPLETED))
//        {
//            Debug.Log("Unity Ads Rewarded Ad Completed");
//            // Grant a reward.
//
//            // Load another ad:
//            Advertisement.Load(_adUnitId, this);
//        }
//    }
//
//    // Implement Load and Show Listener error callbacks:
//    public void OnUnityAdsFailedToLoad(string adUnitId, UnityAdsLoadError error, string message)
//    {
//        Debug.Log($"Error loading Ad Unit {adUnitId}: {error.ToString()} - {message}");
//        // Use the error details to determine whether to try to load another ad.
//    }
//
//    public void OnUnityAdsShowFailure(string adUnitId, UnityAdsShowError error, string message)
//    {
//        Debug.Log($"Error showing Ad Unit {adUnitId}: {error.ToString()} - {message}");
//        // Use the error details to determine whether to try to load another ad.
//    }
//
//    public void OnUnityAdsShowStart(string adUnitId) { }
//    public void OnUnityAdsShowClick(string adUnitId) { }
//
//    void OnDestroy()
//    {
//        // Clean up the button listeners:
//        _showAdButton.onClick.RemoveAllListeners();
//    }
//}
