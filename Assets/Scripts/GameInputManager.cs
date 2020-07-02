using UnityEngine;
using System.Collections;

public class GameInputManager : MonoBehaviour
{
	public bool useTouch = false;

	public LayerMask mask = -1;

	Ray ray;
	RaycastHit hit;

	Transform button;

	void Update()
	{
		if (Input.GetKey(KeyCode.Z))
		{
			StartCoroutine(CaptureScreenshot());
		}

		if (useTouch)
			GetTouches();
		else
			GetClicks();
	}

	IEnumerator CaptureScreenshot()
	{
		string filename = GetFileName(Screen.width, Screen.height);
		Debug.LogError("Screenshot saved to " + filename);
		ScreenCapture.CaptureScreenshot(filename);
		yield return new WaitForSeconds(0.1f);
	}

	string GetFileName(int width, int height)
	{
		return string.Format("screenshot_{0}x{1}_{2}.png", width, height, System.DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss"));
	}

	void GetClicks()
	{
		if (Input.GetMouseButtonDown(0))
		{
			ray = Camera.main.ScreenPointToRay(Input.mousePosition);

			if (Physics.Raycast(ray, out hit, Mathf.Infinity, mask))
			{
				button = hit.transform;
				GameMenuManager.Instance.ButtonDown(button);
			}
			else
			{
				button = null;
				PlayerManager.Instance.MoveUp();
			}
		}
		else if (Input.GetMouseButtonUp(0))
		{
			if (button == null)
				PlayerManager.Instance.MoveDown();
			else
				GameMenuManager.Instance.ButtonUp(button);
		}
	}

	void GetTouches()
	{
		foreach (Touch touch in Input.touches)
		{
			if (touch.phase == TouchPhase.Began && touch.phase != TouchPhase.Canceled)
			{
				ray = Camera.main.ScreenPointToRay(touch.position);

				if (Physics.Raycast(ray, out hit, Mathf.Infinity, mask))
				{
					button = hit.transform;
					GameMenuManager.Instance.ButtonDown(button);
				}
				else
				{
					button = null;
					PlayerManager.Instance.MoveUp();
				}
			}
			else if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled)
			{
				if (button == null)
					PlayerManager.Instance.MoveDown();
				else
					GameMenuManager.Instance.ButtonUp(button);
			}
		}
	}
}
