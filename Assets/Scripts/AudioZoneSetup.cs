﻿using UnityEngine;
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
		// all the openings between this audio zone and adjacent spaces
		openings = GetComponentsInChildren<Transform>();

		// set up reverb snapshot
		var system = FMOD_StudioSystem.instance.System;
		FMOD.Studio.EventDescription description;
		system.getEvent(snapshotName, out description);
		description.createInstance(out reverbPositionSnapshot);

		// set the default opening for audio to leave the zone
		this.SetAudioExit(openings[0]);
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

	public void SetAudioExit(Transform opening)
	{
		var attributes = FMOD.Studio.UnityUtil.to3DAttributes(opening.position);
		reverbPositionSnapshot.set3DAttributes(attributes);
	}
}
