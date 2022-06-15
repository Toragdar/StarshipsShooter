using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Monetization;
using UnityEngine.Advertisements;

public class PlayerShipBuild : MonoBehaviour 
{
	[SerializeField]
	GameObject[] visualWeapons;
	GameObject textBoxPanel;
	GameObject bankObject;
	GameObject buyButton;
	GameObject tmpSelection;
	int bank = 600;
	string placementId_rewardedvideo = "rewardedVideo";
	string gameId = "1234567";
	bool purchaseMade = false;
	[SerializeField]
	SOActorModel defaultPlayerShip;
	GameObject playerShip;
	//GameObject target;

	void Start()
	{
		purchaseMade = false;
		bankObject = GameObject.Find("bank");
		bankObject.GetComponentInChildren<TextMesh>().text = bank.ToString();
		textBoxPanel = GameObject.Find("textBoxPanel");
		buyButton = GameObject.Find("BUY?").gameObject;
		buyButton.SetActive(false);
		TurnOffPlayerShipVisuals();
		TurnOffSelectionHighlights();
		CheckPlatform();
		PreparePlayerShipForUpgrade();
	}

	void PreparePlayerShipForUpgrade()
	{
		playerShip = GameObject.Instantiate(Resources.Load("Prefab/Player/player_ship")) as GameObject;
		playerShip.GetComponent<Player>().enabled = false;
		playerShip.transform.position = new Vector3(0,10000,0);
		playerShip.GetComponent<Player>().ActorStats(defaultPlayerShip);
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
	void TurnOffPlayerShipVisuals()
	{
		for (int i = 0; i < visualWeapons.Length; i++)
		{
			visualWeapons[i].gameObject.SetActive(false);
		}
	}
	void TurnOffSelectionHighlights()
	{
		GameObject[] selections = GameObject.FindGameObjectsWithTag("Selection");
		for (int i = 0; i < selections.Length; i++)
		{
			if (selections[i].GetComponentInParent<ShopPiece>())
			{
				if (selections[i].GetComponentInParent<ShopPiece>().ShopSelection.iconName == "sold Out")
				{
					selections[i].SetActive(false);
				}
			}
		}
	}

	void UpdateDescriptionBox()
	{
		textBoxPanel.transform.Find("name").gameObject.GetComponent<TextMesh>().text = tmpSelection.GetComponent<ShopPiece>().ShopSelection.iconName;
		textBoxPanel.transform.Find("desc").gameObject.GetComponent<TextMesh>().text = tmpSelection.GetComponent<ShopPiece>().ShopSelection.description;	
	}

	void LackOfCredits()
	{
		if (bank < System.Int32.Parse(tmpSelection.GetComponentInChildren<Text>().text))
		{
			Debug.Log("CAN'T BUY");
		}
	}
	void Affordable()
	{
		if (bank >= System.Int32.Parse(tmpSelection.GetComponentInChildren<Text>().text))
		{
			Debug.Log("CAN BUY");
			buyButton.SetActive(true);
		}
	}
	void SoldOut()
	{
		Debug.Log("SOLD OUT");
	}

	public void WatchAdvert()
	{
		if (Application.internetReachability != NetworkReachability.NotReachable)
		{
			ShowRewardedAds();	
		}	
	}
	public void BuyItem()
	{
		Debug.Log("PURCHASED");
		purchaseMade = true;
		buyButton.SetActive(false);
		textBoxPanel.transform.Find("desc").gameObject.GetComponent<TextMesh>().text = "";
		textBoxPanel.transform.Find("name").gameObject.GetComponent<TextMesh>().text = "";

		for (int i = 0; i < visualWeapons.Length; i++)
		{
			if (visualWeapons[i].name == tmpSelection.GetComponent<ShopPiece>().ShopSelection.iconName)
			{
				visualWeapons[i].SetActive(true);
			}
		}

		UpgradeToShip(tmpSelection.GetComponent<ShopPiece>().ShopSelection.iconName);

		bank = bank - System.Int16.Parse(tmpSelection.GetComponent<ShopPiece>().ShopSelection.cost);
		bankObject.transform.Find("bankText").GetComponent<TextMesh>().text = bank.ToString();
		tmpSelection.transform.Find("itemText").GetComponentInChildren<Text>().text = "SOLD";
	}

	void UpgradeToShip(string upgrade)
	{
		GameObject shipItem = GameObject.Instantiate(Resources.Load("Prefab/Player/"+upgrade)) as GameObject;
		shipItem.transform.SetParent(playerShip.transform);
		shipItem.transform.localPosition = Vector3.zero;	
	}

	public void StartGame()
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

		GameManager.Instance.GetComponent<ScenesManager>().BeginGame(GameManager.gameLevelScene);
	}

	public void AttemptSelection(GameObject buttonName)
	{
		if(buttonName)
        {
			TurnOffSelectionHighlights();
			tmpSelection=buttonName;
			tmpSelection.transform.GetChild(1).gameObject.SetActive(true);
        }

		UpdateDescriptionBox();

		//not sold
		if (buttonName.GetComponentInChildren<Text>().text != "SOLD")
		{
			//can afford
			Affordable();

			//can not afford
			LackOfCredits();
		}
		else if (buttonName.GetComponentInChildren<Text>().text == "SOLD")
		{
			SoldOut();
		}
    }
}