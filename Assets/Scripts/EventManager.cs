using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using GameAnalyticsSDK;

public class EventManager : MonoBehaviour
{

	static EventManager _instance;
	static int instances = 0;
	public bool isEnabled = false;
	public bool debug = true;

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
        if(debug) Debug.Log("DEVELOPMENT build -> user id ADMINUSER1996");
#else
        if(Debug.isDebugBuild) 
        {
            GameAnalytics.SetCustomId("ADMINUSER1996");
        	if(debug) Debug.Log("DEBUG build -> user id ADMINUSER1996");
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
				PreferencesManager.Instance.SetFirtsOpen();
			}
		}
		if(debug) Debug.Log("EVENT: first open");
	}

	// Resource events
	public void SendEarnVirtualCurrency(float amount)
	{
		if (isEnabled)
		{
			GameAnalytics.NewResourceEvent (GAResourceFlowType.Source, "Coins", amount, "coin", "coins"+amount);
		}
		if(debug) Debug.Log("EVENT: virtual currency earned: " + amount);
	}

	public void SendSpendVirtualCurrency(float amount, string item)
	{
		if (isEnabled)
		{
			GameAnalytics.NewResourceEvent (GAResourceFlowType.Sink, "Coins", amount, item, "coins"+amount);
		}
		if(debug) Debug.Log("EVENT: virtual currency spent for: " + amount + " of " + item);
	}

	// Progression events
	public void SendMission(int mission)
	{
		if (isEnabled)
		{
			int status = PreferencesManager.Instance.GetLevelStatus() + mission;
			PreferencesManager.Instance.SetLevelStatus(status);
			GameAnalytics.NewProgressionEvent (GAProgressionStatus.Complete, "level" + PreferencesManager.Instance.GetLevel(), "mission" + mission);
			if(status >= 6) SendLevelUp(status);
		}
		if(debug) Debug.Log("EVENT: completed mission " + mission + " -> status: " + status);
	}
	private void SendLevelUp(int status)
	{
		if (isEnabled)
		{
			status -= 6;
			PreferencesManager.Instance.SetLevelStatus(status);
			int level = PreferencesManager.Instance.GetLevel();
			GameAnalytics.NewProgressionEvent (GAProgressionStatus.Complete, "level" + level);
			PreferencesManager.Instance.SetLevel(++level);
			GameAnalytics.NewProgressionEvent (GAProgressionStatus.Start, "level" + level);
		}
		if(debug) Debug.Log("EVENT: completed level " + (level-1) + " -> new status: " + status);
	}

	// Design events
	public void SendCustomEvent(string name)
	{
		if (isEnabled)
		{
		}
		if(debug) Debug.Log("EVENT: " + name);
	}
}
