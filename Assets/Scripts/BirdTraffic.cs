using UnityEngine;
using System.Collections;

public class BirdTraffic : MonoBehaviour
{
	public BirdTrafficManager parent;

	public GameObject indicator;
	public GameObject birdBody;
	public ParticleSystem hideParticle;

	bool canMove = false;
	bool paused = false;

	float originalSpeed = 0;
	float speed = 0;
	Vector3 originalPos = new Vector3();
	Vector3 originalHideParticlePos = new Vector3();

	bool hideParticlePlaying = false;

	void Start()
	{
		originalPos = birdBody.transform.position;
		originalHideParticlePos = hideParticle.transform.position;
	}

	void Update()
	{
		if (canMove && !paused)
		{
			birdBody.transform.position -= Vector3.right * speed * Time.deltaTime;
		}
	}

	IEnumerator PlaceIndicator()
	{
		int yPos = Random.Range(-5, 23);
		float indPos = GameTransformManager.Instance.RightPosition;
		if (indPos == 0)
		{
			indPos = 41;
		}

		indicator.transform.position = new Vector3(indPos, yPos, -4.9f);

		indicator.SetActive(true);

		if (!paused)
		{
			yield return new WaitForSeconds(3.0f);
		}

		indicator.SetActive(false);
		indicator.transform.position = new Vector3(41, 0, -4.9f);

		Vector3 pos = birdBody.transform.position;
		pos.y = yPos;
		birdBody.transform.position = pos;

		this.speed = originalSpeed * LevelSpawnManager.Instance.SpeedMultiplier();

		birdBody.SetActive(true);

		canMove = true;
	}

	IEnumerator PlaceHideParticle(float x, float y)
	{
		hideParticle.transform.position = new Vector3(x - 6, y, originalHideParticlePos.z);
		hideParticle.Play();

		hideParticlePlaying = true;
		LevelSpawnManager.Instance.AddParticle(hideParticle.gameObject);

		if (!paused)
		{
			yield return new WaitForSeconds(2.0f);
		}

		LevelSpawnManager.Instance.RemoveParticle(hideParticle.gameObject);
		hideParticlePlaying = false;

		hideParticle.transform.position = originalHideParticlePos;
	}

	public void Launch(float s)
	{
		originalSpeed = s;
		StartCoroutine(PlaceIndicator());
	}

	public void ResetThis()
	{
		StopAllCoroutines();

		if (hideParticlePlaying)
		{
			LevelSpawnManager.Instance.RemoveParticle(hideParticle.gameObject);
		}

		canMove = false;
		paused = false;

		birdBody.transform.position = originalPos;
		birdBody.SetActive(false);

		indicator.SetActive(false);
		indicator.transform.position = new Vector3(41, 0, -4.9f);

		parent.ResetBird(this);
	}

	public void TargetHit(bool playEffect)
	{
		canMove = false;
		paused = false;

		if (playEffect)
		{
			StartCoroutine(PlaceHideParticle(birdBody.transform.position.x, birdBody.transform.position.y));
		}

		birdBody.transform.position = originalPos;
		birdBody.SetActive(false);

		parent.ResetBird(this);
	}

	public void Pause()
	{
		paused = true;
	}

	public void Resume()
	{
		paused = false;
	}
}
