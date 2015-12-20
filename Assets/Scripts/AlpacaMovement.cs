using UnityEngine;
using System.Collections;
using FMOD.Studio;

public class AlpacaMovement : MonoBehaviour {

	public Transform listener;

	private EventInstance hum;
	private SFXController sfxController;
	private ParameterInstance occlusion;
//	private ParameterInstance distanceFromLargeRm;

	// Use this for initialization
	void Start () {
		SetupSFX();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	private void SetupSFX()
	{
		sfxController = gameObject.GetComponent<SFXController>();
		
		hum = FMOD_StudioSystem.instance.GetEvent("event:/alpaca_hum"); 
		hum.getParameter("occlusion", out occlusion);
//		hum.getParameter("distanceFromSmallRm", out distanceFromSmallRm);

	}

	public void Hum()
	{
		Ray occlusionRay = new Ray(this.transform.position, listener.position);
		RaycastHit hit;

		if(Physics.Raycast(occlusionRay, out hit))
		{

			if (hit.collider.gameObject){}
			occlusion.setValue(4);
		}



		var attributes = FMOD.Studio.UnityUtil.to3DAttributes(this.transform.position);
		hum.set3DAttributes(attributes);
		sfxController.PlaySFX(hum);
	}
}
