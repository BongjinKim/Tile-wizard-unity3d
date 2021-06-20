using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Login : MonoBehaviour {

	public static string Email = "";
	public static string Password = "";

	public GameObject popup;

	//private varialess
	private string CreateAccountUrl = "http://52.69.242.82/TileWizard-Login.php";
	private string LoginUrl = "http://52.69.242.82/TileWizard-LoginCheck.php"; 
	private string ConfirmPass = "";
	private string ConfirmEmail = "";
	private string CEmail = "";
	private string CPassword = "";
	private string LogOnCheck = "";

	public GameObject GamePlayPanel;
	public GameObject LoginPanel;
	public GameObject CreatPanel;
	public GameObject LoginCreate;
	public InputField GameID;
	public InputField GamePW;
	public InputField CreateID;
	public InputField CreatePW;
	public InputField ConfirmPW;

	void Awake(){
		if (GameData.isDoubleLogOn == true) {
			popup.SetActive(true);
			GameData.isDoubleLogOn  = false;

		}
		CreatPanel.SetActive (false);
		LoginPanel.SetActive (true);
	}

	public void LogInGUI(){
		Email = GameID.text;
		Password = GamePW.text;
		StartCoroutine ("LoginAccount");
	}
	
	public void CreateAccountGUI(){
		CEmail = CreateID.text;
		CPassword = CreatePW.text;
		ConfirmPass = ConfirmPW.text;
		Debug.Log(CEmail+CPassword+ConfirmPass+ConfirmEmail);
		if (ConfirmPass == CPassword) {
			Debug.Log("OK");
			StartCoroutine ("CreateAccount");
			
		} else {
			//StartCoroutine (0);
		}
	}


	IEnumerator CreateAccount(){
		WWWForm Form = new WWWForm ();
		Form.AddField ("Email",CEmail);
		Form.AddField ("Password", CPassword);
		WWW CreateAccountWWW = new WWW (CreateAccountUrl, Form);
		//php에서 unity 로 정보를 받을때까지 기다림
		yield return CreateAccountWWW;

		if (CreateAccountWWW.error != null)
			Debug.LogError ("계정을 만들수 없어!");
		else {
			string CreateAccountReturn = CreateAccountWWW.text;
			Debug.Log(CreateAccountReturn);
			if (CreateAccountReturn == "Success!!") {
				Debug.Log ("계정이 생성되었어");
				CreatPanel.SetActive (false);
				LoginPanel.SetActive (true);
			}
		}
	}

	IEnumerator LoginAccount(){
		//php스크립트에 입력정보 전달해줌
		WWWForm form = new WWWForm();
		form.AddField("Email",Email);
		form.AddField ("Password", Password);
		int rand = Random.Range(10000000,99999999) ;
		LogOnCheck = rand.ToString();
		form.AddField ("LogOnCheck", LogOnCheck);
		WWW loginAccountWWW = new WWW (LoginUrl, form);
		yield return loginAccountWWW;
		if (loginAccountWWW.error != null) {
			string logText2 = loginAccountWWW.text;
			Debug.Log(loginAccountWWW.error);
		} else {
			string logText = loginAccountWWW.text;
			string[] logTextSplit = logText.Split(':');
			if(logTextSplit[0] == "1"){

				GameData.LogOnCheck = LogOnCheck;
				Debug.Log(GameData.LogOnCheck.ToString());
				
				LoginCreate.SetActive (false);
				GamePlayPanel.SetActive(true);
			}
		}
	}

	public void ClosePanel(){
		CreatPanel.SetActive (false);
		LoginPanel.SetActive (true);
	}

	public void CreatePanel(){
		CreatPanel.SetActive (true);
		LoginPanel.SetActive (false);
	}
}
