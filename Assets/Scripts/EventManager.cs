using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using GameAnalyticsSDK;

public class EventManager : MonoBehaviour
{

	static EventManager _instance;
	static int instances = 0;

	public bool isEnabled = false;

	public static EventManager Instance
	{
		get
		{
			if (_instance == null)
			{
				_instance = FindObjectOfType(typeof(EventManager)) as EventManager;
			}

			return _instance;
		}
	}

	void Start()
	{
		instances++;

		if (instances > 1)
		{
			Debug.LogWarning("There are more than one EventManager");
		}
		else
		{
			_instance = this;
		}

#if DEVELOPMENT_BUILD
        GameAnalytics.SetCustomId("ADMINUSER1996");
        Debug.Log("DEVELOPMENT build -> user id ADMINUSER1996");
#else
        if(Debug.isDebugBuild) 
        {
            GameAnalytics.SetCustomId("ADMINUSER1996");
        Debug.Log("DEBUG build -> user id ADMINUSER1996");
        }
#endif
		GameAnalytics.Initialize();
	}

	public void SendFirstOpenEvent()
	{
		if (isEnabled)
		{
			if (!PreferencesManager.Instance.GetFirtsOpen())
			{
				Debug.Log("EVENT: first open");
				PreferencesManager.Instance.SetFirtsOpen();
			}
		}
	}

	public void SendEarnVirtualCurrency(float amount)
	{
		if (isEnabled)
		{
			Debug.Log("EVENT: virtual currency earned");
			GameAnalytics.NewResourceEvent (GAResourceFlowType.Source, "Coins", amount, "coin", "coins"+amount);
		}
	}

	public void SendSpendVirtualCurrency(float amount, string item)
	{
		if (isEnabled)
		{
			Debug.Log("EVENT: virtual currency spent");
			GameAnalytics.NewResourceEvent (GAResourceFlowType.Sink, "Coins", amount, item, "coins"+amount);
		}
	}

	public void SendLevelUp()
	{
		if (isEnabled)
		{
			Debug.Log("EVENT: level up");
		}
	}

	public void SendCustomEvent(string name)
	{
		if (isEnabled)
		{
			Debug.Log("EVENT: " + name);
		}
	}
}
