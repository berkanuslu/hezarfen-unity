using UnityEngine;
using System.Collections;
using UnityEngine.Advertisements;

public class GameMenuManager : MonoBehaviour
{
	public Renderer overlay;

	[Header("Scalable Element Settings")]
	public GameObject[] headerLefts;
	public GameObject[] headerRights;
	public GameObject[] mainGUILefts;
	public GameObject[] mainGUIRights;
	public GameObject[] scalables;
	public GameObject[] backButtons;

	public TextMesh bestDist;
	public Texture2D[] menuTextures;
	public GameObject[] mainMenuElements;

	[Header("Shop Menu Settings")]
	public TextMesh[] shopTexts;
	public int[] shopPrices;
	public GameObject[] shopElements;

	[Header("Main GUI Settings")]
	public GameObject[] startPowerUps;
	public TextMesh[] guiTexts;
	public GameObject[] mainGUIElements;

	[Header("Pause Menu Settings")]
	public GameObject[] pauseElements;

	[Header("Finish Menu Settings")]
	public GameObject finishMenu;
	public GameObject finishMenuTop;
	public GameObject finishMenuTopHeader;
	public TextMesh[] finishTexts;

	[Header("Mission Menu Settings")]
	public GameObject[] missionNotification;
	public TextMesh[] missionTexts;
	public TextMesh[] missionStatus;

	static GameMenuManager _instance;
	static int instances = 0;

	bool showMainGUI = false;
	bool showPause = false;
	bool mainMenuTopHidden = true;
	public bool audioEnabled = true;
	bool mainMissionHidden = true;
	bool shopHidden = true;
	bool aboutHidden = true;
	bool areYouSureHidden = true;

	bool starting = false;
	bool reviveActivated = false;

	bool canClick = true;

	bool mNotification1Used = false;
	bool mNotification2Used = false;
	bool mNotification3Used = false;

	public int restartCount = 0;

	public static GameMenuManager Instance
	{
		get
		{
			if (_instance == null)
				_instance = FindObjectOfType(typeof(GameMenuManager)) as GameMenuManager;

			return _instance;
		}
	}

	void Start()
	{
		instances++;

		if (instances > 1)
			Debug.LogWarning("Warning: There are more than one GameMenuManager at the level");
		else
			_instance = this;
	}

	void Update()
	{
		if (showMainGUI && !showPause)
		{
			DisplayStatistics(guiTexts[0], LevelManager.Instance.Coins(), 4);
			DisplayStatistics(guiTexts[1], (int)LevelSpawnManager.Instance.distance, 5);
		}

		if (Input.GetKeyDown(KeyCode.Escape))
		{
			ToggleAreYouSure();
		}
	}

	void DisplayStatistics(TextMesh target, int data, int digitNumbers)
	{
		string dataString = "";
		int remainingDigitCount = digitNumbers - data.ToString().Length;

		while (remainingDigitCount > 0)
		{
			dataString += "0";
			remainingDigitCount--;
		}

		dataString += data;
		target.text = dataString;
	}

	void Pause()
	{
		if (PlayerManager.Instance.IsCrashed())
			return;

		PlayerManager.Instance.Pause();
		LevelManager.Instance.PauseGame();
		showPause = true;

		pauseElements[0].SetActive(true);

		StartCoroutine(FadeScreen(0.4f, 0.7f));
		StartCoroutine(MoveMenu(pauseElements[1].transform, 0, 29, 0.45f, false));
		StartCoroutine(MoveMenu(pauseElements[2].transform, 0, 0, 0.45f, false));

		SoundManager.Instance.PauseMusic();
	}

	void ToggleMainMenuArrow(Material arrow)
	{
		if (mainMenuTopHidden)
		{
			mainMenuTopHidden = false;
			arrow.mainTexture = menuTextures[1];
			StartCoroutine(FadeScreen(0.25f, 0.7f));
			StartCoroutine(MoveMenu(mainMenuElements[1].transform, 0, 22, 0.25f, false));
		}
		else if (!mainMissionHidden)
		{
			mainMissionHidden = true;
			StartCoroutine(MoveMenu(mainMenuElements[3].transform, 0, -31, 0.4f, false));
		}
		else
		{
			mainMenuTopHidden = true;
			arrow.mainTexture = menuTextures[0];
			StartCoroutine(FadeScreen(0.25f, 0));
			StartCoroutine(MoveMenu(mainMenuElements[1].transform, 0, 32, 0.25f, false));
		}
	}

	public void ToggleAudio(Material audioButton)
	{
		if (!shopHidden)
			return;

		if (audioEnabled)
		{
			SoundManager.Instance.SetMusicVolume(0.0f);
			audioEnabled = false;
			audioButton.mainTexture = menuTextures[3];
		}
		else
		{
			SoundManager.Instance.SetMusicVolume(1.0f);
			audioButton.mainTexture = menuTextures[2];
			audioEnabled = true;
		}

		SoundManager.Instance.CheckVolumeButtons();
	}

	void ToggleMainMissionList()
	{
		if (!shopHidden)
			return;

		if (mainMissionHidden)
		{
			mainMissionHidden = false;
			StartCoroutine(MoveMenu(mainMenuElements[3].transform, 0, -75, 0.4f, false));
		}
		else
		{
			mainMissionHidden = true;
			StartCoroutine(MoveMenu(mainMenuElements[3].transform, 0, -31, 0.4f, false));
		}
	}

	void ToggleShopMenu()
	{
		if (shopHidden)
		{
			UpdateShop();
			SoundManager.Instance.PauseMusic();
			shopHidden = false;
			StartCoroutine(MoveMenu(mainMenuElements[2].transform, 0, -21.85f, 0.45f, false));
		}
		else
		{
			shopHidden = true;
			StartCoroutine(MoveMenu(mainMenuElements[2].transform, 0, -94, 0.45f, false));
		}
		mainMenuElements[1].SetActive(shopHidden);
	}

	void ToggleAboutUs()
	{
		if (aboutHidden)
		{
			aboutHidden = false;
			SoundManager.Instance.PauseMusic();
			StartCoroutine(MoveMenu(mainMenuElements[8].transform, 0, -21.85f, 0.45f, false));
		}
		else
		{
			aboutHidden = true;
			StartCoroutine(MoveMenu(mainMenuElements[8].transform, 0, -94, 0.45f, false));
		}
		mainMenuElements[1].SetActive(aboutHidden);
	}

	void ToggleAreYouSure()
	{
		if (areYouSureHidden)
		{
			areYouSureHidden = false;
			SoundManager.Instance.PauseMusic();
			StartCoroutine(MoveMenu(mainMenuElements[9].transform, 0, -21.85f, 0.45f, false));
		}
		else
		{
			areYouSureHidden = true;
			StartCoroutine(MoveMenu(mainMenuElements[9].transform, 0, -94, 0.45f, false));
		}
		mainMenuElements[1].SetActive(areYouSureHidden);
	}

	public void UpdateShop()
	{
		//coin
		shopTexts[0].text = PreferencesManager.Instance.GetCoins().ToString();
		//extra speed
		shopTexts[1].text = "x " + PreferencesManager.Instance.GetExtraSpeed();
		shopTexts[2].text = shopPrices[0].ToString();
		//shield
		shopTexts[3].text = "x " + PreferencesManager.Instance.GetShield();
		shopTexts[4].text = shopPrices[1].ToString();
		//abracadabra
		shopTexts[5].text = "x " + PreferencesManager.Instance.GetAbracadabra();
		shopTexts[6].text = shopPrices[2].ToString();
		//revive
		shopTexts[7].text = "x " + PreferencesManager.Instance.GetRevive();
		shopTexts[8].text = shopPrices[3].ToString();
	}

	public IEnumerator UpdateShopRoutine()
	{
		yield return new WaitForSeconds(2);
		UpdateShop();
	}

	public void RewardCoin(double amount)
	{
		PreferencesManager.Instance.SetCoins(PreferencesManager.Instance.GetCoins() + System.Convert.ToInt32(amount));
	}

	void BuySpeedPowerup()
	{
		if (PreferencesManager.Instance.GetCoins() >= shopPrices[0])
		{
			PreferencesManager.Instance.SetCoins(PreferencesManager.Instance.GetCoins() - shopPrices[0]);
			PreferencesManager.Instance.ModifyExtraSpeedBy(1);
			FirebaseEventManager.Instance.SendSpendVirtualCurrency();

			UpdateShop();
			MissionManager.Instance.ShopEvent("ExtraSpeed");
		}
	}

	void BuyShieldPowerup()
	{
		if (PreferencesManager.Instance.GetCoins() >= shopPrices[1])
		{
			PreferencesManager.Instance.SetCoins(PreferencesManager.Instance.GetCoins() - shopPrices[1]);
			PreferencesManager.Instance.ModifyShieldBy(1);
			FirebaseEventManager.Instance.SendSpendVirtualCurrency();

			UpdateShop();
			MissionManager.Instance.ShopEvent("Shield");
		}
	}

	void BuyAbracadabraPowerup()
	{
		if (PreferencesManager.Instance.GetCoins() >= shopPrices[2])
		{
			PreferencesManager.Instance.SetCoins(PreferencesManager.Instance.GetCoins() - shopPrices[2]);
			PreferencesManager.Instance.ModifyAbracadabraBy(1);
			FirebaseEventManager.Instance.SendSpendVirtualCurrency();

			UpdateShop();
			MissionManager.Instance.ShopEvent("Abracadabra");
		}
	}

	void BuyRevivePowerup()
	{
		if (PreferencesManager.Instance.GetCoins() >= shopPrices[3])
		{
			PreferencesManager.Instance.SetCoins(PreferencesManager.Instance.GetCoins() - shopPrices[3]);
			PreferencesManager.Instance.ModifyReviveBy(1);
			FirebaseEventManager.Instance.SendSpendVirtualCurrency();

			UpdateShop();
			MissionManager.Instance.ShopEvent("Revive");
		}
	}

	//Activate the extra speed powerup at startup
	void ActivateSpeedPowerup()
	{
		if (showPause)
			return;

		StartCoroutine(MoveMenu(startPowerUps[0].transform, startPowerUps[0].transform.localPosition.x, -45, 0.4f, true));

		PlayerManager.Instance.ExtraSpeed();
		PreferencesManager.Instance.ModifyExtraSpeedBy(-1);

		StopCoroutine("MovePowerUpSelection");
	}

	//Activate the shield powerup at startup
	void ActivateShieldPowerup()
	{
		if (showPause)
			return;

		StartCoroutine(MoveMenu(startPowerUps[1].transform, startPowerUps[1].transform.localPosition.x, -45, 0.4f, true));

		PlayerManager.Instance.RaiseShield();
		PreferencesManager.Instance.ModifyShieldBy(-1);

		StopCoroutine("MovePowerUpSelection");
	}

	//Activate the abracadabra powerup at startup
	void ActivateAbracadabraPowerup()
	{
		if (showPause)
			return;

		StartCoroutine(MoveMenu(startPowerUps[2].transform, startPowerUps[2].transform.localPosition.x, -45, 0.4f, true));

		StartCoroutine(PlayerManager.Instance.LaunchAbracadabra());
		PreferencesManager.Instance.ModifyAbracadabraBy(-1);

		StopCoroutine("MovePowerUpSelection");
	}

	//Activate the revive powerup at crash
	void ActivateRevivePowerup()
	{
		reviveActivated = true;
	}

	void StartToPlay()
	{
		mainMenuElements[10].SetActive(false);

		if (!mainMissionHidden || !shopHidden || starting || !aboutHidden || !areYouSureHidden)
			return;

		starting = true;

		if (PreferencesManager.Instance.GetRevive() > 0)
			mainGUIElements[2].SetActive(true);

		if (!mainMenuTopHidden)
		{
			StartCoroutine(FadeScreen(0.25f, 0));
			StartCoroutine(MoveMenu(mainMenuElements[1].transform, 0, 32, 0.25f, true));
		}
		else
		{
			mainMenuElements[1].SetActive(false);
		}

		PlayerManager.Instance.startAnimation.GetComponent<Animator>().enabled = true;
		SoundManager.Instance.StartMusic();
	}

	IEnumerator SpawnObstacles()
	{
		yield return new WaitForSeconds(5);
		PlayerManager.Instance.firstObstacleSpawned = true;
		LevelSpawnManager.Instance.SpawnObstacles();
	}

	IEnumerator Resume()
	{
		StartCoroutine(FadeScreen(0.4f, 0));
		StartCoroutine(MoveMenu(pauseElements[1].transform, 0, 59, 0.45f, false));
		StartCoroutine(MoveMenu(pauseElements[2].transform, 0, -60, 0.45f, false));

		yield return new WaitForSeconds(0.6f);
		showPause = false;

		PlayerManager.Instance.Resume();
		LevelManager.Instance.ResumeGame();
		SoundManager.Instance.ResumeMusic();
	}

	public IEnumerator RestartRoutine()
	{
		StartCoroutine(FadeScreen(0.4f, 1.0f));

		if (showPause)
		{
			showPause = false;
			StartCoroutine(MoveMenu(pauseElements[1].transform, 0, 59, 0.45f, false));
			StartCoroutine(MoveMenu(pauseElements[2].transform, 0, -60, 0.45f, false));
		}
		else
		{
			StartCoroutine(MoveMenu(finishMenu.transform, 0, -60, 0.55f, false));
			StartCoroutine(MoveMenu(finishMenuTop.transform, 0, 118, 0.55f, false));
		}

		yield return new WaitForSeconds(0.5f);
		StartCoroutine(FadeScreen(0.4f, 0.0f));

		mainMenuElements[0].SetActive(false);
		LevelManager.Instance.Restart();
		SoundManager.Instance.StartMusic();
	}

	public void RestartGame()
	{
		StartCoroutine(RestartRoutine());
	}

	IEnumerator ShowAds()
	{
		AdvertisementManager.Instance.ShowAds();
		yield return new WaitForSeconds(0.01f);
	}

	IEnumerator QuitToMain()
	{
		starting = false;
		StartCoroutine(FadeScreen(0.4f, 1.0f));

		if (showPause)
		{
			showPause = false;
			StartCoroutine(MoveMenu(pauseElements[1].transform, 0, 59, 0.45f, false));
			StartCoroutine(MoveMenu(pauseElements[2].transform, 0, -60, 0.45f, false));
		}
		else
		{
			StartCoroutine(MoveMenu(finishMenu.transform, 0, -60, 0.55f, false));
			StartCoroutine(MoveMenu(finishMenuTop.transform, 0, 118, 0.55f, false));
		}

		mainGUIElements[1].SetActive(false);
		mainGUIElements[2].SetActive(false);

		yield return new WaitForSeconds(0.5f);
		StartCoroutine(FadeScreen(0.4f, 0.0f));

		mainMenuElements[0].SetActive(true);
		LevelManager.Instance.QuitToMain();

		PlayerManager.Instance.horizontalAnimation.GetComponent<Animator>().enabled = false;
		PlayerManager.Instance.horizontalAnimation.GetComponent<SpriteRenderer>().enabled = false;
		PlayerManager.Instance.startAnimation.GetComponent<Animator>().enabled = false;
		PlayerManager.Instance.startAnimation.GetComponent<SpriteRenderer>().enabled = true;
		PlayerManager.Instance.startAnimation.GetComponent<SpriteRenderer>().sprite = PlayerManager.Instance.startAnimation.GetComponent<ChangeAvatar>().startSprite;

		SoundManager.Instance.StopMusic();
	}

	IEnumerator MoveMenu(Transform menuTransform, float endPosX, float endPosY, float time, bool hide)
	{
		canClick = false;

		float i = 0.0f;
		float rate = 1.0f / time;

		Vector3 startPos = menuTransform.localPosition;
		Vector3 endPos = new Vector3(endPosX, endPosY, menuTransform.localPosition.z);

		while (i < 1.0)
		{
			i += Time.deltaTime * rate;
			menuTransform.localPosition = Vector3.Lerp(startPos, endPos, i);
			yield return new WaitForEndOfFrame();
		}

		if (hide)
			menuTransform.gameObject.SetActive(false);

		canClick = true;
	}

	IEnumerator MovePowerUpSelection(bool speed, bool shield, bool abracadabra)
	{
		yield return new WaitForSeconds(3.0f);

		if (speed)
			StartCoroutine(MoveMenu(startPowerUps[0].transform, startPowerUps[0].transform.localPosition.x, -28.5f, 0.4f, false));
		if (shield)
			StartCoroutine(MoveMenu(startPowerUps[1].transform, startPowerUps[1].transform.localPosition.x, -28.5f, 0.4f, false));
		if (abracadabra)
			StartCoroutine(MoveMenu(startPowerUps[2].transform, startPowerUps[2].transform.localPosition.x, -28.5f, 0.4f, false));

		if (!showPause)
		{
			yield return new WaitForSeconds(10.0f);
		}

		if (speed)
			StartCoroutine(MoveMenu(startPowerUps[0].transform, startPowerUps[0].transform.localPosition.x, -45, 0.4f, true));
		if (shield)
			StartCoroutine(MoveMenu(startPowerUps[1].transform, startPowerUps[1].transform.localPosition.x, -45, 0.4f, true));
		if (abracadabra)
			StartCoroutine(MoveMenu(startPowerUps[2].transform, startPowerUps[2].transform.localPosition.x, -45, 0.4f, true));

		yield return new WaitForSeconds(0.4f);

		StopCoroutine("MovePowerUpSelection");
	}

	public void SetLevelResolution()
	{
		GameTransformManager.Instance.SetGameTransformByAspectRatio(scalables, shopElements, headerLefts, mainGUILefts, headerRights, mainGUIRights, backButtons, mainMenuElements[0], mainMenuElements[10], LevelSpawnManager.Instance.galata, mainMenuElements[11]);
	}

	public void ButtonDown(Transform button)
	{
		if (!canClick)
			return;

		Vector3 scale = button.transform.localScale;
		button.transform.localScale = scale * 0.8f;
	}

	public void ButtonUp(Transform button)
	{
		if (!canClick)
			return;

		Vector3 scale = button.transform.localScale;
		button.transform.localScale = scale * 1.25f;

		switch (button.name)
		{
			case "PauseShowButton":
				Pause();
				break;

			case "Resume":
				StartCoroutine(Resume());
				break;

			case "Retry":
				StartCoroutine(ShowAds());
				break;

			case "Home":
				StartCoroutine(QuitToMain());
				break;

			case "YesQuit":
				Application.Quit();
				break;

			case "Quit":
			case "BackAreYouSure":
				ToggleAreYouSure();
				break;

			case "MainArrow":
				ToggleMainMenuArrow(button.GetComponent<Renderer>().material);
				break;

			case "AudioEnabler":
				ToggleAudio(button.GetComponent<Renderer>().material);
				break;

			case "Missions":
				ToggleMainMissionList();
				break;

			case "Shop":
			case "Back":
				ToggleShopMenu();
				break;

			case "AboutUs":
			case "BackAbout":
				ToggleAboutUs();
				break;

			case "PlayTriggerer":
				StartToPlay();
				break;

			case "BuySpeed":
				BuySpeedPowerup();
				break;

			case "BuyShield":
				BuyShieldPowerup();
				break;

			case "BuyRevive":
				BuyRevivePowerup();
				break;

			case "BuyAdvertisement":
				AdvertisementManager.Instance.WatchAds();
				break;

			case "BuyAbracadabra":
				BuyAbracadabraPowerup();
				break;

			case "ExtraSpeedActivation":
				ActivateSpeedPowerup();
				break;

			case "ShieldActivation":
				ActivateShieldPowerup();
				break;

			case "AbracadabraActivation":
				ActivateAbracadabraPowerup();
				break;

			case "ReviveActivation":
				ActivateRevivePowerup();
				break;
		}
	}

	public void ShowEnd()
	{
		SoundManager.Instance.StopMusic();

		MissionManager.Instance.Save();
		finishMenu.SetActive(true);
		finishMenuTop.SetActive(true);

		int currentDist = (int)LevelSpawnManager.Instance.distance;
		int currentCoins = LevelManager.Instance.Coins();

		finishTexts[0].text = currentDist + "M";
		finishTexts[1].text = currentCoins.ToString();

		if (currentDist > PreferencesManager.Instance.GetBestDistance())
			PreferencesManager.Instance.SetBestDistance(currentDist);

		PreferencesManager.Instance.SetCoins(PreferencesManager.Instance.GetCoins() + currentCoins);
		FirebaseEventManager.Instance.SendEarnVirtualCurrency();

		StartCoroutine(FadeScreen(0.4f, 0.7f));
		StartCoroutine(MoveMenu(finishMenu.transform, 0, 67, 0.55f, false));
		StartCoroutine(MoveMenu(finishMenuTop.transform, 0, -37, 0.55f, false));

		restartCount++;
	}

	public void ShowStartPowerUps()
	{
		bool hasSpeed = PreferencesManager.Instance.GetExtraSpeed() > 0;
		bool hasShield = PreferencesManager.Instance.GetShield() > 0;
		bool hasAbracadabra = PreferencesManager.Instance.GetAbracadabra() > 0;

		int numberOfPowerUps = 0;

		if (hasSpeed)
			numberOfPowerUps++;
		if (hasShield)
			numberOfPowerUps++;
		if (hasAbracadabra)
			numberOfPowerUps++;

		if (numberOfPowerUps == 1)
		{
			if (hasSpeed)
			{
				startPowerUps[0].transform.localPosition = new Vector3(0, -40, 0);
				startPowerUps[0].SetActive(true);
			}
			else if (hasShield)
			{
				startPowerUps[1].transform.localPosition = new Vector3(0, -40, 0);
				startPowerUps[1].SetActive(true);
			}
			else
			{
				startPowerUps[2].transform.localPosition = new Vector3(0, -40, 0);
				startPowerUps[2].SetActive(true);
			}
		}
		else if (numberOfPowerUps == 2)
		{
			if (hasSpeed)
			{
				startPowerUps[0].transform.localPosition = new Vector3(-7.5f, -40, 0);
				startPowerUps[0].SetActive(true);
			}
			if (hasShield)
			{
				if (hasSpeed)
					startPowerUps[1].transform.localPosition = new Vector3(7.5f, -40, 0);
				else
					startPowerUps[1].transform.localPosition = new Vector3(-7.5f, -40, 0);

				startPowerUps[1].SetActive(true);
			}
			if (hasAbracadabra)
			{
				startPowerUps[2].transform.localPosition = new Vector3(7.5f, -40, 0);
				startPowerUps[2].SetActive(true);
			}
		}
		else if (numberOfPowerUps == 3)
		{
			startPowerUps[0].transform.localPosition = new Vector3(-15, -40, 0);
			startPowerUps[0].SetActive(true);
			startPowerUps[1].transform.localPosition = new Vector3(0, -40, 0);
			startPowerUps[1].SetActive(true);
			startPowerUps[2].transform.localPosition = new Vector3(15, -40, 0);
			startPowerUps[2].SetActive(true);
		}

		StartCoroutine(MovePowerUpSelection(hasSpeed, hasShield, hasAbracadabra));
	}

	public void ActivateMainGUI()
	{
		showMainGUI = true;
		mainGUIElements[0].SetActive(true);
	}

	public void DeactivateMainGUI()
	{
		showMainGUI = false;
		mainGUIElements[0].SetActive(false);
	}

	public void ActivateMainMenu()
	{
		mainMenuElements[4].GetComponent<Renderer>().material.mainTexture = menuTextures[0];
		mainMenuElements[1].SetActive(true);
	}

	public void DeactivateMainMenu()
	{
		mainMenuElements[1].SetActive(false);
	}

	public void RevivePicked()
	{
		mainGUIElements[1].SetActive(true);
	}

	public void DisableReviveGUI(int num)
	{
		if (num == 0 || num == 1)
			mainGUIElements[num + 1].SetActive(false);
	}

	public void UpdateBestDistance()
	{
		bestDist.text = PreferencesManager.Instance.GetBestDistance() + "M";
	}

	public void UpdateMissionTexts(string text1, string text2, string text3)
	{
		if (text1.Length < 26)
		{
			missionTexts[0].fontSize = 20;
			missionTexts[1].fontSize = 20;
			missionTexts[2].fontSize = 20;
		}
		else if (text1.Length < 31)
		{
			missionTexts[0].fontSize = 18;
			missionTexts[1].fontSize = 18;
			missionTexts[2].fontSize = 18;
		}
		else
		{
			missionTexts[0].fontSize = 14;
			missionTexts[1].fontSize = 14;
			missionTexts[2].fontSize = 14;
		}

		missionTexts[0].text = text1;
		missionTexts[1].text = text1;
		missionTexts[2].text = text1;

		if (text2.Length < 26)
		{
			missionTexts[3].fontSize = 20;
			missionTexts[4].fontSize = 20;
			missionTexts[5].fontSize = 20;
		}
		else if (text2.Length < 31)
		{
			missionTexts[3].fontSize = 18;
			missionTexts[4].fontSize = 18;
			missionTexts[5].fontSize = 18;
		}
		else
		{
			missionTexts[3].fontSize = 14;
			missionTexts[4].fontSize = 14;
			missionTexts[5].fontSize = 14;
		}

		missionTexts[3].text = text2;
		missionTexts[4].text = text2;
		missionTexts[5].text = text2;

		if (text3.Length < 26)
		{
			missionTexts[6].fontSize = 20;
			missionTexts[7].fontSize = 20;
			missionTexts[8].fontSize = 20;
		}
		else if (text3.Length < 31)
		{
			missionTexts[6].fontSize = 18;
			missionTexts[7].fontSize = 18;
			missionTexts[8].fontSize = 18;
		}
		else
		{
			missionTexts[6].fontSize = 14;
			missionTexts[7].fontSize = 14;
			missionTexts[8].fontSize = 14;
		}

		missionTexts[6].text = text3;
		missionTexts[7].text = text3;
		missionTexts[8].text = text3;
	}

	public void UpdateMissionStatus(int i, int a, int b)
	{
		switch (i)
		{
			case 0:
				missionStatus[0].text = a + "/" + b;
				missionStatus[1].text = a + "/" + b;
				missionStatus[2].text = a + "/" + b;
				break;
			case 1:
				missionStatus[3].text = a + "/" + b;
				missionStatus[4].text = a + "/" + b;
				missionStatus[5].text = a + "/" + b;
				break;
			case 2:
				missionStatus[6].text = a + "/" + b;
				missionStatus[7].text = a + "/" + b;
				missionStatus[8].text = a + "/" + b;
				break;
		}
	}

	public IEnumerator ShowRevive()
	{
		StartCoroutine(MoveMenu(startPowerUps[3].transform, startPowerUps[3].transform.localPosition.x, -29.5f, 0.4f, false));

		bool activated = false;

		double waited = 0;
		while (waited <= 3)
		{
			waited += Time.deltaTime;

			if (reviveActivated)
			{
				yield return new WaitForSeconds(0.2f);
				StartCoroutine(MoveMenu(startPowerUps[3].transform, startPowerUps[3].transform.localPosition.x, -45, 0.4f, false));

				yield return new WaitForSeconds(0.4f);
				LevelManager.Instance.Revive();
				reviveActivated = false;
				activated = true;
			}

			yield return new WaitForEndOfFrame();
		}

		if (!activated)
		{
			StartCoroutine(MoveMenu(startPowerUps[3].transform, startPowerUps[3].transform.localPosition.x, -45, 0.4f, false));
			yield return new WaitForSeconds(0.5f);

			ShowEnd();
		}
	}

	public IEnumerator ShowMissionComplete(string text)
	{
		GameObject notificationObject = null;
		TextMesh notificationTextMesh = null;

		int notificationIndex = 0;
		float yPosTarget = 0;

		if (!mNotification1Used)
		{
			notificationObject = missionNotification[0];
			notificationTextMesh = notificationObject.transform.Find("Text").GetComponent<TextMesh>() as TextMesh;

			mNotification1Used = true;
			notificationIndex = 1;
			yPosTarget = 32;
		}
		else if (mNotification1Used && !mNotification2Used)
		{
			notificationObject = missionNotification[1];
			notificationTextMesh = notificationObject.transform.Find("Text").GetComponent<TextMesh>() as TextMesh;

			yPosTarget = 26;
			mNotification2Used = true;
			notificationIndex = 2;
		}
		else if (mNotification1Used && mNotification2Used && !mNotification3Used)
		{
			notificationObject = missionNotification[2];
			notificationTextMesh = notificationObject.transform.Find("Text").GetComponent<TextMesh>() as TextMesh;

			yPosTarget = 20;
			mNotification3Used = true;
			notificationIndex = 3;
		}
		else
		{
			StopCoroutine("ShowMissionComplete");
		}

		if (text.Length < 26)
			notificationTextMesh.fontSize = 24;
		else if (text.Length < 31)
			notificationTextMesh.fontSize = 21;
		else if (text.Length < 36)
			notificationTextMesh.fontSize = 19;
		else
			notificationTextMesh.fontSize = 14;

		notificationTextMesh.text = text;

		StartCoroutine(MoveMenu(notificationObject.transform, 0, yPosTarget, 0.4f, false));

		if (!showPause)
		{
			yield return new WaitForSeconds(2.0f);
		}

		StartCoroutine(MoveMenu(notificationObject.transform, 0, 38.5f, 0.4f, false));

		if (!showPause)
		{
			yield return new WaitForSeconds(0.5f);
		}

		if (notificationIndex == 1)
			mNotification1Used = false;
		else if (notificationIndex == 2)
			mNotification2Used = false;
		else if (notificationIndex == 3)
			mNotification3Used = false;
	}

	public IEnumerator FadeScreen(float time, float to)
	{
		float i = 0.0f;
		float rate = 1.0f / time;

		Color start = overlay.material.color;
		Color end = new Color(start.r, start.g, start.b, to);

		while (i < 1.0)
		{
			i += Time.deltaTime * rate;
			overlay.material.color = Color.Lerp(start, end, i);
			yield return new WaitForEndOfFrame();
		}
	}
}
