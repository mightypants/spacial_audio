using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using FMOD.Studio;

public class SFXController : MonoBehaviour {
	

	
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
	
	}

	private void findAudioTarget()
	{
		Collider[] hitColliders = Physics.OverlapSphere(this.transform.position, 100f);
		List<Collider> hitReverbZones = new List<Collider>();
		
		foreach(Collider c in hitColliders){
			
			if (c.tag == "ReverbZone")
			{
				hitReverbZones.Add(c);
			}
		}
		
		foreach(Collider c in hitReverbZones)
		{
			Debug.DrawRay(this.transform.position, c.transform.position - this.transform.position , Color.green);
			
		}
		
	}

	public void PlaySoundEffect(EventInstance soundEffect)
	{
		soundEffect.start();
		//soundEffect.release();
		findAudioTarget();
	}
}
