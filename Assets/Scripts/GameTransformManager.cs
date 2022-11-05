using UnityEngine;
using System.Collections;

public class GameTransformManager : MonoBehaviour
{
	static GameTransformManager _instance;
	static int instances = 0;

	float leftPosition = 0;
	float rightPosition = 0;
	float headerLeftPosition = 0;
	float headerRightPosition = 0;
	float scale = 0;
	float galataPosition = 0;
	float galataEndXPosition = -300.0f;
	float logoPosition = 0;
	float galataScale = 0;
	float playerPosition = 0;
	float playerStartingPosition = 0;
	float abracadabraPosition = 0;

	float cameraHorizontalExtent;
	float cameraVerticalExtent;

	public static GameTransformManager Instance
	{
		get
		{
			if (_instance == null)
				_instance = FindObjectOfType(typeof(GameTransformManager)) as GameTransformManager;

			return _instance;
		}
	}

	public float GalataEndXPosition
	{
		get
		{
			return galataEndXPosition;
		}
	}

	public float AbracadabraPosition
	{
		get
		{
			return abracadabraPosition;
		}
	}

	public float RightPosition
	{
		get
		{
			return rightPosition;
		}
	}

	void Start()
	{
		instances++;

		if (instances > 1)
			Debug.LogWarning("Warning: There are more than one ResulationManager at the level");
		else
			_instance = this;

		cameraHorizontalExtent = Camera.main.orthographicSize * Screen.width / Screen.height;
		cameraVerticalExtent = Camera.main.orthographicSize;
	}

	public void SetGameTransformByAspectRatio(GameObject[] scalable,
					    GameObject[] shopElements,
					    GameObject[] leftElements,
					    GameObject[] leftGUIElements,
					    GameObject[] rightElements,
					    GameObject[] rightGUIElements,
					    GameObject[] backButtons,
					    GameObject logoObject,
					    GameObject touchToPlayObject,
					    GameObject galataObject,
					    GameObject galataTowerObject)
	{
		//Calculate aspect ratio
		float r = (float)Screen.width / (float)Screen.height * 10f;
		int aspect = (int)r;

		cameraHorizontalExtent = Camera.main.orthographicSize * Screen.width / Screen.height;
		cameraVerticalExtent = Camera.main.orthographicSize;

		//Debug.LogWarning("Current Aspect: " + aspect);
		//Debug.LogWarning("cameraHorizontalExtent: " + cameraHorizontalExtent);
		//Debug.LogWarning("cameraVerticalExtent: " + cameraVerticalExtent);

		galataEndXPosition = -(cameraHorizontalExtent * 1.5f + 175.0f);
		abracadabraPosition = cameraHorizontalExtent + 15.0f;
		leftPosition = -(cameraHorizontalExtent - 5f);
		rightPosition = cameraHorizontalExtent - 5f;
		headerLeftPosition = -(cameraHorizontalExtent / 2 - 20f);
		headerRightPosition = (cameraHorizontalExtent / 2) - 15f;
		logoPosition = 10;
		scale = (cameraHorizontalExtent * 2);
		galataPosition = -(cameraHorizontalExtent / 2);
		galataScale = 1f;
		playerPosition = -(cameraHorizontalExtent - 10f);
		playerStartingPosition = -(cameraHorizontalExtent - 10f);

		Vector3 temp;
		Vector2 offset;

		foreach (GameObject element in leftElements)
		{
			temp = element.transform.position;
			temp.x = headerLeftPosition;
			element.transform.position = temp;
		}

		foreach (GameObject element in leftGUIElements)
		{
			temp = element.transform.position;

			temp.x = leftPosition;

			element.transform.position = temp;
		}

		foreach (GameObject element in rightElements)
		{
			temp = element.transform.position;
			temp.x = headerRightPosition;
			element.transform.position = temp;

		}

		foreach (GameObject element in rightGUIElements)
		{
			temp = element.transform.position;
			temp.x = rightPosition;
			element.transform.position = temp;
		}

		foreach (GameObject element in shopElements)
		{
			switch (element.name)
			{
				case "Bar":
				case "Header":
					temp = element.transform.localScale;
					temp.x = scale;
					element.transform.localScale = temp;
					break;
			}
		}

		foreach (GameObject element in scalable)
		{
			temp = element.transform.localScale;
			temp.x = scale;
			element.transform.localScale = temp;

			offset = element.GetComponent<Renderer>().material.mainTextureScale;
			offset.x = scale / 100;
			element.transform.GetComponent<Renderer>().material.mainTextureScale = offset;
		}

		temp = galataObject.transform.position;
		temp.x = galataPosition;
		galataObject.transform.position = temp;

		temp = galataTowerObject.transform.position;
		temp.x = -(cameraHorizontalExtent - 10f);
		galataTowerObject.transform.position = temp;

		LevelSpawnManager.Instance.SetGalataPos(galataPosition);

		LevelSpawnManager.Instance.galata.transform.localScale = new Vector3(galataScale, 1f, 1f);

		temp = logoObject.transform.position;
		temp.x = logoPosition;
		logoObject.transform.position = temp;

		temp = touchToPlayObject.transform.position;
		temp.x = logoPosition;
		touchToPlayObject.transform.position = temp;

		GameObject player = PlayerManager.Instance.gameObject;
		temp = player.transform.position;
		temp.x = playerStartingPosition;
		player.transform.position = temp;

		PlayerManager.Instance.SetPositions(playerStartingPosition, playerPosition);
	}
}
