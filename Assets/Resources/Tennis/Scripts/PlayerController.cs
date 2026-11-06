﻿using UnityEngine;
using System.Collections;
using UnityStandardAssets.CrossPlatformInput;

public class PlayerController : MonoBehaviour
{
	[Header("Mobile")]
	public VariableJoystick variableJoystick;
	public VibrationController vibrationController;

	[Header("Settings")]
	public float maxSpeed;

	private float moveVertical;
	private float moveHorizontal;

	private float speed;

	public GameObject hitArea;
	private ReturnBall hitController;

	public bool isServing = false;
	public bool canServe = true;
	private float servingTime = Mathf.Infinity;

	private float delay;

	private float turn;

	private Rigidbody r;
	private Animator a;

	public GameObject serveMessage;

	void Start()
	{
		canServe = true;
		r = GetComponent<Rigidbody>();
		a = GetComponent<Animator>();
		hitController = hitArea.GetComponent<ReturnBall>();
		speed = 0;
		delay = 0;
	}

	void Update()
	{

		if (isServing == true)
		{
			r.velocity = Vector3.zero;
			a.SetFloat("speed", 0);
			hitController.isServing = true;
			Sacar();
		}

		else
		{
			Mover();
		}
		//Debug.Log(servingTime);
	}


	void Mover()
	{

		if (moveVertical < maxSpeed)
		{
#if MOBILE_INPUT
		moveVertical = variableJoystick.Vertical;
#else
			moveVertical = Input.GetAxis("Vertical");
#endif
		}
		if (moveHorizontal < maxSpeed)
		{
#if MOBILE_INPUT
			moveHorizontal = variableJoystick.Horizontal;
#else
			moveHorizontal = Input.GetAxis("Horizontal");
#endif
		}

		if (Time.time > 0.4 + delay)
		{
			a.SetTrigger("Move");
			delay = Time.time;
		}
		a.SetFloat("speed", speed);
		Vector3 move = new Vector3(moveHorizontal, 0, moveVertical);


		speed = maxSpeed * Mathf.Sqrt((moveHorizontal * moveHorizontal) + (moveVertical * moveVertical));
		if (speed > maxSpeed)
		{
			speed = maxSpeed;
		}
		r.velocity = transform.forward * -speed;
		if (move != Vector3.zero)
		{
			r.rotation = Quaternion.Slerp(r.rotation, Quaternion.Euler(0f, 180f, 0f) * Quaternion.LookRotation(move), Time.deltaTime * 10f);
		}
	}
	void Sacar()
	{
		a.SetBool("isServing", true);
		hitController.isServing = true;
		if (SimpleInput.GetKeyDown(KeyCode.Space) && canServe == true)
		{
			serveMessage.SetActive(false);
			servingTime = Time.time;
			a.SetTrigger("Serve");
			canServe = false;
			BallController.ThrowBall();
		}
		//if (CrossPlatformInputManager.GetButtonDown("Space") && canServe == true)
		//{
		//	vibrationController.VibrateDevice();
		//	serveMessage.SetActive(false);
		//	servingTime = Time.time;
		//	a.SetTrigger("Serve");
		//	canServe = false;
		//	BallController.ThrowBall();
		//}
		if (Time.time > servingTime + 1.6f)
		{
			hitArea.SetActive(true);
			a.SetBool("isServing", false);
		}
		if (Time.time > servingTime + 1.8f)
		{
			hitController.isServing = false;
			isServing = false;
			servingTime = Mathf.Infinity;
		}
	}
}