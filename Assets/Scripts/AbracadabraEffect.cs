using UnityEngine;
using System.Collections;

public class AbracadabraEffect : MonoBehaviour
{
	bool canDisable = false;

	void OnTriggerEnter(Collider other)
	{
		if (other.transform.name == "BirdBrown" || other.transform.name == "BirdWhite" || other.transform.name == "StorkTall")
		{
			PlayHideEffectParticle(other.transform);
		}
		else if (other.name == "BirdBody")
		{
			if (!canDisable)
			{
				other.transform.parent.GetComponent<BirdTraffic>().TargetHit(true);
			}
		}
		else if (other.name == "ResetTriggerer" && other.tag == "Obstacles" && canDisable)
		{
			ResetThis();
		}
	}

	void PlayHideEffectParticle(Transform parent)
	{
		parent.GetComponent<Renderer>().enabled = false;
		parent.GetComponent<Collider>().enabled = false;

		if (canDisable)
		{
			return;
		}

		ParticleSystem hideParticle = parent.Find("HideParticleEffect").gameObject.GetComponent("ParticleSystem") as ParticleSystem;
		hideParticle.Play();
	}

	void ResetThis()
	{
		canDisable = false;
		this.transform.localPosition = new Vector3(-70, 0, -5);
		this.gameObject.SetActive(false);
	}

	public void Disable()
	{
		canDisable = true;
	}
}
