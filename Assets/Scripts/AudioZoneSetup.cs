using UnityEngine;
using System.Collections;

public class AudioZoneSetup : MonoBehaviour 
{
	public string snapshotName;
	public GameObject triggerZone;
	public FMOD_Listener listener;

	private Transform[] openings;

	private FMOD.Studio.EventInstance reverbPositionSnapshot;
	
	void Start() 
	{
		openings = GetComponentsInChildren<Transform>();

		var system = FMOD_StudioSystem.instance.System;
		FMOD.Studio.EventDescription description;
		Debug.Log(snapshotName);
		Debug.Log(this.name);
		
		system.getEvent(snapshotName, out description);
		description.createInstance(out reverbPositionSnapshot);
		var attributes = FMOD.Studio.UnityUtil.to3DAttributes(triggerZone.transform.position);
		reverbPositionSnapshot.set3DAttributes(attributes);

		reverbPositionSnapshot.start();
	}

	void OnTriggerEnter(Collider obj)
	{
		if (obj.GetComponent<FMOD_Listener>() != null)
		{
			reverbPositionSnapshot.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
		}
	}

	void OnTriggerExit(Collider obj)
	{
		if (obj.GetComponent<FMOD_Listener>() != null)
		{
			reverbPositionSnapshot.start();
		}
	}

	void OnDestroy()
	{
		reverbPositionSnapshot.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
		reverbPositionSnapshot.release();
	}
}
