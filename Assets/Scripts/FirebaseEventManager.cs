using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// using Firebase.Analytics;

public class FirebaseEventManager : MonoBehaviour
{

	static FirebaseEventManager _instance;
	static int instances = 0;

	public bool isReady = false;

	public static FirebaseEventManager Instance
	{
		get
		{
			if (_instance == null)
			{
				_instance = FindObjectOfType(typeof(FirebaseEventManager)) as FirebaseEventManager;
			}

			return _instance;
		}
	}

	void Start()
	{
		instances++;

		if (instances > 1)
		{
			Debug.LogWarning("There are more than one FirebaseEventManager");
		}
		else
		{
			_instance = this;
		}

		StartFirebase();
	}

	public void StartFirebase()
	{
		// 		Firebase.FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task =>
		// 		{
		// 			var dependencyStatus = task.Result;
		// 			if (dependencyStatus == Firebase.DependencyStatus.Available)
		// 			{
		// 				isReady = true;

		// #if (UNITY_IOS)
		// 				SendFirstOpenEvent();
		// 				SendSessionStart();
		// #endif
		// 			}
		// 			else
		// 			{
		// 				UnityEngine.Debug.LogError(System.String.Format(
		// 				  "Could not resolve all Firebase dependencies: {0}", dependencyStatus));
		// 				// Firebase Unity SDK is not safe to use here.
		// 			}
		// 		});
	}

	public void SendFirstOpenEvent()
	{
		if (isReady)
		{
			if (!PreferencesManager.Instance.GetFirtsOpen())
			{
				// FirebaseAnalytics.LogEvent("first_open");
				PreferencesManager.Instance.SetFirtsOpen();
			}
		}
	}

	public void SendSessionStart()
	{
		if (isReady)
		{
			// FirebaseAnalytics.LogEvent("session_start");
		}
	}

	public void SendEarnVirtualCurrency()
	{
		if (isReady)
		{
			// FirebaseAnalytics.LogEvent(FirebaseAnalytics.EventEarnVirtualCurrency);
		}
	}

	public void SendSpendVirtualCurrency()
	{
		if (isReady)
		{
			// FirebaseAnalytics.LogEvent(FirebaseAnalytics.EventSpendVirtualCurrency);
		}
	}

	public void SendLevelUp()
	{
		if (isReady)
		{
			// FirebaseAnalytics.LogEvent(FirebaseAnalytics.EventLevelUp);
		}
	}

	public void SendCustomEvent(string name)
	{
		if (isReady)
		{
			// FirebaseAnalytics.LogEvent(name);
		}
	}
}
