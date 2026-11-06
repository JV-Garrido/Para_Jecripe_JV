using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityStandardAssets.CrossPlatformInput;

public class InitialCountdown : MonoBehaviour {

	GameObject countdownCanvas;
	public AudioSource cdAudio; //gunshot sound
	float countdownTime = 3;
	
	GameObject startCanvas;
	public playerBehaviourCoop p;
	AthleticsSounds s;

	public GameObject timer;
	
	
	
	Timer t;
	
	public static int c = 0; //counter so that you can only press the space key to start the game once
	
	
	void Start(){

		AthleticsController.gameStarted = false;
		countdownCanvas = GameObject.Find ("Countdown");
		startCanvas = GameObject.Find ("StartCanvas");
		s = GameObject.Find ("Sounds").GetComponent<AthleticsSounds>(); 

		countdownCanvas.SetActive(false);
		
		t = countdownCanvas.GetComponent<Timer>();
		c = 0;
		timer.SetActive (false);
	}
	
	void Update(){

		
		if (Input.GetKeyDown (KeyCode.Space) && c == 0) {
			
			countdownCanvas.SetActive (true);
			startCanvas.SetActive (false);
			t.SetTimer ();
			c++;

		}
#if MOBILE_INPUT
		if (CrossPlatformInputManager.GetButtonDown("Space") && c == 0)
		{

			countdownCanvas.SetActive(true);
			startCanvas.SetActive(false);
			t.SetTimer();
			c++;

		}
#endif
		else if (c == 1) {
			
			if (t.time <= 2.5f) { 
				countdownTime = 3.4f - t.time;
				countdownCanvas.GetComponentInChildren<Text> ().text = countdownTime.ToString ("0");
			} else {

				cdAudio.Play ();
				countdownCanvas.SetActive (false);
				GameObject.Find("terezinha9").GetComponent<playerBehaviourCoop>().pronto = true;
				AthleticsController.gameStarted = true;
				s.PlayAudio (s.background);
				timer.SetActive (true);
				timer.GetComponent<Image> ().fillAmount = 0;
				c++;
			}


		} else if (c == 2) {


			if (Input.GetKeyDown (KeyCode.UpArrow)) {
				p.startRunning ();
				c++;
			}
#if MOBILE_INPUT
			if (CrossPlatformInputManager.GetButtonDown("SetaEsquerda"))
			{
				p.startRunning();
				c++;
			}
			if (CrossPlatformInputManager.GetButtonDown("SetaDireita"))
			{
				p.startRunning();
				c++;
			}
#endif
			if (Input.GetKeyDown(KeyCode.W))
			{
				p.startRunning();
				c++;
			}
			if (Input.GetKeyDown(KeyCode.A))
			{
				p.startRunning();
				c++;
			}
			if (Input.GetKeyDown(KeyCode.D))
			{
				p.startRunning();
				c++;
			}

		} else if (c == 3) {

			this.gameObject.SetActive (false);


		}


		
	}
	
}
