using UnityEngine;
using System.Collections;

public class LogOnCheck : MonoBehaviour {

	private string LogOnCheckUrl = "http://52.69.242.82/TileWizard-LogOnCheck.php";
	private string LogOn = "";
	private Time time;

	public void StartLogOn(){

		StartCoroutine ("LogOnCheckAccount");
	}


	IEnumerator LogOnCheckAccount(){
		//php스크립트에 입력정보 전달해줌
		WWWForm form = new WWWForm();
		LogOn = GameData.LogOnCheck;
		form.AddField ("LogOn", LogOn);
		WWW logOnCheckWWW = new WWW (LogOnCheckUrl, form);
		yield return logOnCheckWWW;
		string logText = logOnCheckWWW.text;
		Debug.Log(logText);
		if (logText.Equals("Fail")) {
			//Debug.LogError("AlreadyLogOn");
			Application.LoadLevel(1);
			GameData.isDoubleLogOn = true;
			//popup.SetActive(true);
		} else if(logText.Equals("Success")) {
			Debug.Log("That's OK");
		}
	}
}
