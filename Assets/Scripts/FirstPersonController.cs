using System;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;
using UnityStandardAssets.Utility;
using Random = UnityEngine.Random;
using System.Collections;
using System.Collections.Generic;
using FMOD.Studio;


[RequireComponent(typeof (CharacterController))]
[RequireComponent(typeof (AudioSource))]
public class FirstPersonController : MonoBehaviour
{
    [SerializeField] private bool m_IsWalking;
    [SerializeField] private float m_WalkSpeed;
    [SerializeField] private float m_RunSpeed;
    [SerializeField] [Range(0f, 1f)] private float m_RunstepLenghten;
    [SerializeField] private float m_JumpSpeed;
    [SerializeField] private float m_StickToGroundForce;
    [SerializeField] private float m_GravityMultiplier;
    [SerializeField] private MouseLook m_MouseLook;
    [SerializeField] private bool m_UseFovKick;
    [SerializeField] private FOVKick m_FovKick = new FOVKick();
    [SerializeField] private bool m_UseHeadBob;
    [SerializeField] private CurveControlledBob m_HeadBob = new CurveControlledBob();
    [SerializeField] private LerpControlledBob m_JumpBob = new LerpControlledBob();
    [SerializeField] private float m_StepInterval;

    private Camera m_Camera;
    private bool m_Jump;
    private float m_YRotation;
    private Vector2 m_Input;
    private Vector3 m_MoveDir = Vector3.zero;
    private CharacterController m_CharacterController;
    private CollisionFlags m_CollisionFlags;
    private bool m_PreviouslyGrounded;
    private Vector3 m_OriginalCameraPosition;
    private float m_StepCycle;
    private float m_NextStep;
    private bool m_Jumping;

	// audio 
	private EventInstance gunshotAudio;
	private ParameterInstance distanceFromSmallRm;
	private ParameterInstance distanceFromLargeRm;
	private List<Transform> audioZones;
	private Transform currAudioZone;
	private Transform largeRoom;
	private Transform smallRoom;
	private Transform currSmallRoomOpening;
	private Transform currLargeRoomOpening;
	private SFXController sfxController;
	

    // Use this for initialization
    private void Start()
    {
        m_CharacterController = GetComponent<CharacterController>();
        m_Camera = Camera.main;
        m_OriginalCameraPosition = m_Camera.transform.localPosition;
        m_FovKick.Setup(m_Camera);
        m_HeadBob.Setup(m_Camera, m_StepInterval);
        m_StepCycle = 0f;
        m_NextStep = m_StepCycle/2f;
        m_Jumping = false;
		m_MouseLook.Init(transform , m_Camera.transform);

		SetupSFX();
    }


    // Update is called once per frame
    private void Update()
    {
        RotateView();
        // the jump state needs to read here to make sure it is not missed
        if (!m_Jump)
        {
            m_Jump = CrossPlatformInputManager.GetButtonDown("Jump");
        }

        if (!m_PreviouslyGrounded && m_CharacterController.isGrounded)
        {
            StartCoroutine(m_JumpBob.DoBobCycle());
            m_MoveDir.y = 0f;
            m_Jumping = false;
        }
        if (!m_CharacterController.isGrounded && !m_Jumping && m_PreviouslyGrounded)
        {
            m_MoveDir.y = 0f;
        }

        m_PreviouslyGrounded = m_CharacterController.isGrounded;

		float smallRoomDistance;

		if (currAudioZone == smallRoom)
		{
			smallRoomDistance = 0;
		}
		else
		{
			smallRoomDistance = Vector3.Distance(this.transform.position, currSmallRoomOpening.position);
		}

		distanceFromSmallRm.setValue(smallRoomDistance);
		float largeRoomDistance;

		if (currAudioZone == largeRoom)
		{
			largeRoomDistance = 0;

		}
		else 
		{
			largeRoomDistance = Vector3.Distance(this.transform.position, currLargeRoomOpening.position);
		}

		distanceFromLargeRm.setValue(largeRoomDistance);

		var attributes = FMOD.Studio.UnityUtil.to3DAttributes(this.transform.position);
		gunshotAudio.set3DAttributes(attributes);


		if (Input.GetKeyDown(KeyCode.Q))
		{

			StartCoroutine(CallAlpaca());

		}

    }

    private void FixedUpdate()
    {
        float speed;
        GetInput(out speed);
        // always move along the camera forward as it is the direction that it being aimed at
        Vector3 desiredMove = transform.forward*m_Input.y + transform.right*m_Input.x;

        // get a normal for the surface that is being touched to move along it
        RaycastHit hitInfo;
        Physics.SphereCast(transform.position, m_CharacterController.radius, Vector3.down, out hitInfo,
                           m_CharacterController.height/2f);
        desiredMove = Vector3.ProjectOnPlane(desiredMove, hitInfo.normal).normalized;

        m_MoveDir.x = desiredMove.x*speed;
        m_MoveDir.z = desiredMove.z*speed;


        if (m_CharacterController.isGrounded)
        {
            m_MoveDir.y = -m_StickToGroundForce;

            if (m_Jump)
            {
                m_MoveDir.y = m_JumpSpeed;
                m_Jump = false;
                m_Jumping = true;
            }
        }
        else
        {
            m_MoveDir += Physics.gravity*m_GravityMultiplier*Time.fixedDeltaTime;
        }
        m_CollisionFlags = m_CharacterController.Move(m_MoveDir*Time.fixedDeltaTime);

        ProgressStepCycle(speed);
        UpdateCameraPosition(speed);
    }

    private void ProgressStepCycle(float speed)
    {
        if (m_CharacterController.velocity.sqrMagnitude > 0 && (m_Input.x != 0 || m_Input.y != 0))
        {
            m_StepCycle += (m_CharacterController.velocity.magnitude + (speed*(m_IsWalking ? 1f : m_RunstepLenghten)))*
                         Time.fixedDeltaTime;
        }

        if (!(m_StepCycle > m_NextStep))
        {
            return;
        }

        m_NextStep = m_StepCycle + m_StepInterval;

    }

    private void UpdateCameraPosition(float speed)
    {
        Vector3 newCameraPosition;
        if (!m_UseHeadBob)
        {
            return;
        }
        if (m_CharacterController.velocity.magnitude > 0 && m_CharacterController.isGrounded)
        {
            m_Camera.transform.localPosition =
                m_HeadBob.DoHeadBob(m_CharacterController.velocity.magnitude +
                                  (speed*(m_IsWalking ? 1f : m_RunstepLenghten)));
            newCameraPosition = m_Camera.transform.localPosition;
            newCameraPosition.y = m_Camera.transform.localPosition.y - m_JumpBob.Offset();
        }
        else
        {
            newCameraPosition = m_Camera.transform.localPosition;
            newCameraPosition.y = m_OriginalCameraPosition.y - m_JumpBob.Offset();
        }
        m_Camera.transform.localPosition = newCameraPosition;
    }


    private void GetInput(out float speed)
    {
        // Read input
        float horizontal = CrossPlatformInputManager.GetAxis("Horizontal");
        float vertical = CrossPlatformInputManager.GetAxis("Vertical");

        bool waswalking = m_IsWalking;

#if !MOBILE_INPUT
        // On standalone builds, walk/run speed is modified by a key press.
        // keep track of whether or not the character is walking or running
        m_IsWalking = !Input.GetKey(KeyCode.LeftShift);
#endif
        // set the desired speed to be walking or running
        speed = m_IsWalking ? m_WalkSpeed : m_RunSpeed;
        m_Input = new Vector2(horizontal, vertical);

        // normalize input if it exceeds 1 in combined length:
        if (m_Input.sqrMagnitude > 1)
        {
            m_Input.Normalize();
        }

        // handle speed change to give an fov kick
        // only if the player is going to a run, is running and the fovkick is to be used
        if (m_IsWalking != waswalking && m_UseFovKick && m_CharacterController.velocity.sqrMagnitude > 0)
        {
            StopAllCoroutines();
            StartCoroutine(!m_IsWalking ? m_FovKick.FOVKickUp() : m_FovKick.FOVKickDown());
        }
    }


    private void RotateView()
    {
        m_MouseLook.LookRotation (transform, m_Camera.transform);
    }


    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        Rigidbody body = hit.collider.attachedRigidbody;
        //dont move the rigidbody if the character is on top of it
        if (m_CollisionFlags == CollisionFlags.Below)
        {
            return;
        }

        if (body == null || body.isKinematic)
        {
            return;
        }
        body.AddForceAtPosition(m_CharacterController.velocity*0.1f, hit.point, ForceMode.Impulse);
    }

	private void SetupSFX()
	{
		sfxController = gameObject.GetComponent<SFXController>();

		gunshotAudio = FMOD_StudioSystem.instance.GetEvent("event:/flute"); 
		gunshotAudio.getParameter("distanceFromLargeRm", out distanceFromLargeRm);
		gunshotAudio.getParameter("distanceFromSmallRm", out distanceFromSmallRm);

		GameObject s = GameObject.Find("Building Small/Audio Zone");
		GameObject l = GameObject.Find("Building Large/Audio Zone");

		smallRoom = s.GetComponent<Transform>();
		largeRoom = l.GetComponent<Transform>();

		InvokeRepeating("FindNearestAudioOpenings", 0, 1);
	}

	private void FindNearestAudioOpenings()
	{
//		if (currLargeRoomOpening != null)
//		{
//			Debug.Log(currLargeRoomOpening);
//			Debug.Log(Vector3.Distance(currLargeRoomOpening.position, this.transform.position));
//		}

		AudioZoneController smallRoomController = smallRoom.GetComponent<AudioZoneController>();
		AudioZoneController largeRoomController = largeRoom.GetComponent<AudioZoneController>();
		float shortestDistance = -1;
		
		foreach (Transform o in smallRoom)
		{
			float distance = Vector3.Distance(o.position, this.transform.position);
			
			if (shortestDistance == -1 || shortestDistance > distance)
			{
				shortestDistance = distance;
				currSmallRoomOpening = o;
			}
		}

		smallRoomController.SetActiveOpening(currSmallRoomOpening);

		shortestDistance = -1;

		foreach (Transform o in largeRoom)
		{
			float distance = Vector3.Distance(o.position, this.transform.position);
			
			if (shortestDistance == -1 || shortestDistance > distance)
			{
				shortestDistance = distance;
				currLargeRoomOpening = o;
			}
		}
		
		largeRoomController.SetActiveOpening(currLargeRoomOpening);
	}

	public void NotifyEnteredAudioZone(Transform zone)
	{
		currAudioZone = zone;
	}

	IEnumerator CallAlpaca()
	{
		sfxController.PlaySFX(gunshotAudio);
		yield return new WaitForSeconds(2);

		Collider[] hitColliders = Physics.OverlapSphere(this.transform.position, 15);
		
		foreach (Collider c in hitColliders) {
			
			AlpacaMovement alpacaMovement = c.gameObject.GetComponent<AlpacaMovement>();
			
			if (c.tag == "Alpaca") {
				alpacaMovement.Hum();
			}
		}
		
	}

	void FindShortestAudioPath()
	{

	}
}

