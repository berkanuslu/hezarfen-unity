using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using GameAnalyticsSDK;

public class EventManager : MonoBehaviour
{

	static EventManager _instance;
	static int instances = 0;
	static int levelStatus = 0;
	static int missionStatus = 0;

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
		GameAnalytics.NewProgressionEvent (GAProgressionStatus.Start, "level"+levelStatus);
		GameAnalytics.NewProgressionEvent (GAProgressionStatus.Complete, "level"+levelStatus, "mission"+1);
		GameAnalytics.NewProgressionEvent (GAProgressionStatus.Complete, "level"+levelStatus, "mission"+2);
		GameAnalytics.NewProgressionEvent (GAProgressionStatus.Complete, "level"+levelStatus, "mission"+3);
	}

	public void SendFirstOpenEvent()
	{
		if (isEnabled)
		{
			if (!PreferencesManager.Instance.GetFirtsOpen())
			{
				if(debug) Debug.Log("EVENT: first open");
				PreferencesManager.Instance.SetFirtsOpen();
			}
		}
	}

	// Resource events
	public void SendEarnVirtualCurrency(float amount)
	{
		if (isEnabled)
		{
			if(debug) Debug.Log("EVENT: virtual currency earned: " + amount);
			GameAnalytics.NewResourceEvent (GAResourceFlowType.Source, "Coins", amount, "coin", "coins"+amount);
		}
	}

	public void SendSpendVirtualCurrency(float amount, string item)
	{
		if (isEnabled)
		{
			if(debug) Debug.Log("EVENT: virtual currency spent for: " + amount + " of " + item);
			GameAnalytics.NewResourceEvent (GAResourceFlowType.Sink, "Coins", amount, item, "coins"+amount);
		}
	}

	// Progression events
	public void SendLevelUp(int mission)
	{
		if (isEnabled)
		{
			missionStatus += mission;
			if(debug) Debug.Log("EVENT: completed mission " + mission + " of level " + levelStatus + " -> new status: " + missionStatus);
			GameAnalytics.NewProgressionEvent (GAProgressionStatus.Complete, "level" + levelStatus, "mission" + mission);

			if(missionStatus >= 6)
			{
				GameAnalytics.NewProgressionEvent (GAProgressionStatus.Complete, "level" + levelStatus);
				levelStatus++;
				missionStatus = 0;
				GameAnalytics.NewProgressionEvent (GAProgressionStatus.Start, "level" + levelStatus);
			}
		}
	}

	// Design events
	public void SendCustomEvent(string name)
	{
		if (isEnabled)
		{
			if(debug) Debug.Log("EVENT: " + name);
		}
	}
}
