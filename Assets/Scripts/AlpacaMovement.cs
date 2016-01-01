using UnityEngine;
using System.Collections;
using FMOD.Studio;

public class AlpacaMovement : MonoBehaviour {

	public Transform listener;
	public Transform audioZone;
	public float distLargeRm;
	public float distSmallRm;

	private EventInstance hum;
	private SFXController sfxController;
	private ParameterInstance occlusion;
	private ParameterInstance distanceFromLargeRm;
	private ParameterInstance distanceFromSmallRm;
	private EventInstance displacedSound; 
	
	void Start () {
		sfxController = gameObject.GetComponent<SFXController>();
		
		hum = FMOD_StudioSystem.instance.GetEvent("event:/alpaca_hum"); 
		hum.getParameter("occlusion", out occlusion);
		hum.getParameter("distanceFromSmallRm", out distanceFromSmallRm);
		hum.getParameter("distanceFromLargeRm", out distanceFromLargeRm);
		
		distanceFromLargeRm.setValue(distLargeRm);
		distanceFromSmallRm.setValue(distSmallRm);

		// set up snap shot for displaced sound
		var system = FMOD_StudioSystem.instance.System;
		FMOD.Studio.EventDescription description;
		system.getEvent("snapshot:/displacedSound", out description);
		description.createInstance(out displacedSound);

	}
	
	// Update is called once per frame
	void Update () {
		//Debug.DrawRay(this.transform.position, listener.transform.position - this.transform.position , Color.green);
		Debug.DrawRay(this.transform.position, listener.transform.position - this.transform.position , Color.green);
		
		Ray occlusionRay = new Ray(this.transform.position, listener.transform.position - this.transform.position);
		
		RaycastHit hit;
		
		if(Physics.Raycast(occlusionRay, out hit, 15))
		{
			
			switch (hit.collider.gameObject.tag)
			{
				case "Small Thing":
					occlusion.setValue(1.5f);
				    displacedSound.stop(STOP_MODE.IMMEDIATE);
					break;
				case "Medium Thing":
					occlusion.setValue(6);
					FindShortestAudioPath();
					break;
				case "Big Thing":
					occlusion.setValue(8);
					FindShortestAudioPath();
					break;
				default:
					occlusion.setValue(0);
                    displacedSound.stop(STOP_MODE.IMMEDIATE);
                    break;
			}	
		}
		else
		{
			occlusion.setValue(0);
            displacedSound.stop(STOP_MODE.IMMEDIATE);
		}
	}



	private void SetupSFX()
	{

	}

	public void Hum()
	{
		float occ;
		occlusion.getValue(out occ);
		Debug.Log ("occlusion: " + occ);
        PLAYBACK_STATE playstate;
        displacedSound.getPlaybackState(out playstate);
        Debug.Log("snapshot playing? " + playstate);
        Debug.Log("al: " + transform.position);
       


		var attributes = FMOD.Studio.UnityUtil.to3DAttributes(this.transform.position);
		hum.set3DAttributes(attributes);
		sfxController.PlaySFX(hum);
        ATTRIBUTES_3D attr;
        displacedSound.get3DAttributes(out attr);
        Debug.Log ("displaced position " + attr.position.x + ", " + attr.position.y + ", " + attr.position.z);
    }

	void FindShortestAudioPath()
	{
		float shortestDistance = -1;
		Transform nearestOpening = null;
		
		foreach (Transform o in audioZone)
		{
            // distance between sound source and opening
			float distanceNear = Vector3.Distance(o.position, this.transform.position);
			// distance between sound listener and opening
			float distanceFar = Vector3.Distance(o.position, listener.transform.position);
			float totalDistance = distanceFar + distanceNear;
			
			if (shortestDistance == -1 || shortestDistance > totalDistance)
			{
				shortestDistance = totalDistance;
				nearestOpening = o;
			}
		}

        Vector3 audioPosition = new Vector3(nearestOpening.position.x, transform.position.y, nearestOpening.position.z);

        GameObject test = GameObject.Find("testobj");
        //var attributes = FMOD.Studio.UnityUtil.to3DAttributes(test.transform.position);
        var attributes = FMOD.Studio.UnityUtil.to3DAttributes(nearestOpening.position);
		displacedSound.set3DAttributes(attributes);
        displacedSound.start();
	}
}
