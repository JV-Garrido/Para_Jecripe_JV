using UnityEngine;
using UnityEngine.UI;
using PlayFab;
using PlayFab.ClientModels;
using System.Collections;

public class gameController : MonoBehaviour {

	
	public CoinManager coinManager;
	public int primeiro, segundo, terceiro;
	public Text result;
	public Text getText;

	public string breakRecord = "";

	private StoreDataContainer sD;
	private enemyBehaviour adversaryScript;
	private enemyBehaviour2 adversary2Script;
	private enemyBehaviour3 adversary3Script;
	private float time1,time2,time3, time4, aux; 
	private string first, second, third, fourth, saux,medal;
	private bool end, save,pF,pS,pT;
	int coin;
	GameObject gameOverCanvas;
	GameObject canvas;

	Text place1;
	Text place2;
	Text place3;
	Text place4;

	public int prizecoins,p,f;

	public GameObject waitCanvas;
	
	
	
	void Start () {
		getText.text = coinManager.GetCoins().ToString();
		adversaryScript = GameObject.Find ("adversary").GetComponent<enemyBehaviour>();
		adversary2Script = GameObject.Find ("adversary2").GetComponent<enemyBehaviour2>();
		adversary3Script = GameObject.Find ("adversary3").GetComponent<enemyBehaviour3>();

		prizecoins = 0;

		end = false;
		save= false;
		pF=false;
		pS=false;
		pT=false;
		p=0;

		gameOverCanvas = GameObject.Find ("GameOver");
		canvas = GameObject.Find("Canvas");

		if (PlayerPrefs.GetFloat ("bestTime") == 0) {
			PlayerPrefs.SetFloat ("bestTime", 90f);
		}
		
		//Sounds = GameObject.Find ("Sounds").GetComponent<AthleticsSounds>();
		
		place1 = GameObject.Find ("FirstPlace").GetComponent<Text>();
		place2 = GameObject.Find ("SecondPlace").GetComponent<Text>();	
		place3 = GameObject.Find ("ThirdPlace").GetComponent<Text>();
		place4 = GameObject.Find ("FourthPlace").GetComponent<Text>();

		result = GameObject.Find ("Result").GetComponentInChildren<Text>();
		gameOverCanvas.SetActive(false);
		
		
	}

	void StoreHighscore(float newHighscore)
	{
		float oldHighscore = PlayerPrefs.GetFloat ("bestTime"); 
		if(newHighscore < oldHighscore)
		{
			PlayerPrefs.SetFloat("bestTime", newHighscore);
			breakRecord = " Você quebrou o Record!";
		}

	}
	
	public void Reload(){
		
		Application.LoadLevel(Application.loadedLevel);
		
	}
	
	public void BackToMenu(){
		Application.LoadLevel("PlayAthletics");
	}
	
	void scoreBuilder(){
		gameOverCanvas.SetActive(true);
		
		place1.text = first;
		place2.text = second;
		place3.text = third;
		place4.text = fourth;

		result.text = "Parabéns, você ganhou "+ coin.ToString() +" moedas!";


		end = true;
		
	}

	void sortedTimes(){

		if (playerBehaviourCoop.termina ==true && adversaryScript.termina == false && adversary2Script.termina == false && adversary3Script.termina == false){
			pF=true;
			pS= false;
			pT=false;
			if (p==0){
			first = "Terezinha Guilhermina e \nRafael Lazarini";
			second = adversary2Script.adversary.name;
			third = adversaryScript.adversary.name;
			fourth = adversary3Script.adversary.name;
				p=1;
				
			}
		}
		else if (playerBehaviourCoop.termina ==true && pF==false && pT==false){
			pF=pT=false;
			pS=true;
			if (p==0){
			first = adversary3Script.adversary.name;
			second = "Terezinha Guilhermina e \nRafael Lazarini";
			third = adversaryScript.adversary.name;
			fourth = adversary2Script.adversary.name;
				p=2;
			}
			
		}

		else if (pF=false && pS==false){
			pT=true;
			pS=pF=false;
			if (p==0){
			first = adversary3Script.adversary.name;
			second = adversary2Script.adversary.name;
			third = "Terezinha Guilhermina e \nRafael Lazarini";
			fourth = adversaryScript.adversary.name;
				p=3;
			}
			
		}
		else if (playerBehaviourCoop.termina ==false && adversaryScript.termina == true && adversary2Script.termina == true && adversary3Script.termina == true)
		{
			pF=pS=pT=false;
		
			first = adversary3Script.adversary.name;
			second = adversary2Script.adversary.name;
			third = adversaryScript.adversary.name;
			fourth = "Terezinha Guilhermina e \nRafael Lazarini";
			p=4;
			
		}	
		else if (playerBehaviourCoop.termina ==false && adversaryScript.termina == false && adversary2Script.termina == false && adversary3Script.termina == false)
		{
			pF=pS=pT=false;
		}
		//Debug.Log(p);
		//Debug.Log("first"+first);
		//Debug.Log("second"+second);
		//Debug.Log("third"+third);
		//Debug.Log("four"+fourth);
	}
	
	public void showPrize(){
		
		if ((p==1)) {
			prizecoins = 100;
			first = "Terezinha Guilhermina e \nRafael Lazarini";
			second = adversary2Script.adversary.name;
			third = adversaryScript.adversary.name;
			fourth = adversary3Script.adversary.name;
			coinManager.AddCoins(primeiro);
			coin = primeiro;
			//medal = "Parabéns você ganhou medalha de ouro e "+prizecoins +" moedas!";
		}
		else if ((p==2)) {
			prizecoins = 50;
			first = adversary3Script.adversary.name;
			second = "Terezinha Guilhermina e \nRafael Lazarini";
			third = adversaryScript.adversary.name;
			fourth = adversary2Script.adversary.name;
			coinManager.AddCoins(segundo);
			coin = segundo;
			//medal = "Parabéns você ganhou medalha de prata e "+prizecoins +" moedas!";
		}
		else if ((p==3)) {
			prizecoins = 25;
			first = adversary3Script.adversary.name;
			second = adversary2Script.adversary.name;
			third = "Terezinha Guilhermina e \nRafael Lazarini";
			fourth = adversaryScript.adversary.name;
			coinManager.AddCoins(terceiro);
			coin = terceiro;
			//medal = "Parabéns você ganhou medalha de bronze e "+prizecoins +" moedas!";;
		}
		else if ((p==4)) {
			//medal = "Não foi dessa vez! Tente mais vezes e conquiste medalhas!";
			first = adversary3Script.adversary.name;
			second = adversary2Script.adversary.name;
			third = adversaryScript.adversary.name;
			fourth = "Terezinha Guilhermina e \nRafael Lazarini";
			coin = 0;
		}

		if( end == true && playerBehaviourCoop.termina==true && save == false){
			//sD = StoreDataContainer.Load();
			//sD.storeObjects[0].coin += prizecoins;
			//sD.Save();
			//save = true;
		}

		result.text = "Parabéns você ganhou "+(playerBehaviourCoop.bonusnumber+prizecoins) +" moedas!";

	}

	public void GameOverAthletics()
	{
		waitCanvas.SetActive (false);

		showPrize();
		scoreBuilder ();
		gameOverCanvas.SetActive (true);

	}
	void Update(){
		
	
		sortedTimes ();

		if (playerBehaviourCoop.termina==true) {
			StoreHighscore (playerBehaviourCoop.playertime);



			if (adversaryScript.termina == false || adversary2Script.termina == false || adversary3Script.termina == false) {
				waitCanvas.SetActive (true);

			} else {
				
				GameOverAthletics();

			}
		}
		
		while (adversaryScript.adversary.id == adversary2Script.adversary.id || 
		       adversary3Script.adversary.id == adversary2Script.adversary.id ||
		       adversaryScript.adversary.id == adversary3Script.adversary.id) {
			
			adversary2Script.adversary.featuresAdversary (Random.Range (1,9));
			adversary3Script.adversary.featuresAdversary (Random.Range (1,9));
			
		}
		
		
	}

	
}