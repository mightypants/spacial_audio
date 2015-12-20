using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AudioController : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	private void PlaySoundEffect()
	{
		//		m_AudioSource.clip = gun_burst_dry;
		//		m_AudioSource.Play();
		//
		//		GameObject reverbObj = GameObject.Find("Reverb Manager");
		//		ReverbManager reverbManager = reverbObj.GetComponent<ReverbManager>();
		//		reverbManager.PlayReverbs(gameObject, gun_burst_dry);
		
//		var attributes = FMOD.Studio.UnityUtil.to3DAttributes(transform.position);
//		gunshotAudio.set3DAttributes(attributes);
//		
//		float distance = Vector3.Distance(transform.position, largeRoomPosition);
//		distanceFromLargeRm.setValue(distance);
//		
//		//FMOD_StudioSystem.instance.PlayOneShot("event:gunshot", transform.position);
//		gunshotAudio.start();
		
		
		//		FMOD.RESULT gunshotDesc;
		//		gunshotDesc = (FMOD.Studio.EventDescription) gunshotAudio.getDescription(out gunshotDesc);
		//
		//		float vol;
		//		float maxDistance;
		//		float minDistance;
		//
		//		gunshotAudio.getVolume(out vol);
		//		gunshotDesc.getMaximumDistance(out maxDistance);
		//		gunshotDesc.getMinimumDistance(out minDistance);
		//
		//		Debug.Log("vol is " + vol + ", max distance is " + maxDistance + ", min distance is " + minDistance);
		
		
		
	}

//	private void findAudioTarget()
//	{
//		Collider[] hitColliders = Physics.OverlapSphere(this.transform.position, 100f);
//		List<Collider> hitReverbZones = new List<Collider>();
//		
//		foreach(Collider c in hitColliders){
//			
//			if (c.tag == "ReverbZone")
//			{
//				
//				hitReverbZones.Add(c);
//				//Debug.DrawRay(this.transform.position, c.transform.position - this.transform.position, Color.green);
//				//Ray soundRay = new Ray(this.transform.position, this.transform.position - c.transform.position * 100);
//				//Debug.DrawRay(this.transform.position, c.transform.position - this.transform.position, Color.green);
//			}
//		}
//		
//		foreach(Collider c in hitReverbZones)
//		{
//			Debug.DrawRay(this.transform.position, c.transform.position - this.transform.position , Color.green);
//			
//		}
//		
//	}
}
