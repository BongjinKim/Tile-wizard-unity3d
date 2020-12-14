using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class MainManager : CommonBehavior
{
    public GameObject userNameInputPopup;
    public InputField userNameInputField;
	public GameObject Popup_quit;
    public Tutorial tutorial;



	public void LoadPlayerCharacter() {
        if (SaveLoad.Load() == true)
        {
            PlayerCharacter.curPC = GameData.pc;
            ChangeScene((int)Scenes.MAIN);
        }
        else
        {
            OpenPopup(userNameInputPopup);
        }
    }

    public void MakeNewCharacter() {
        if (userNameInputField.text.Length == 0)
        {
            Debug.Log("이름을 입력해 주세요.");
        }
        else if (userNameInputField.text.Contains(" "))
        {
            Debug.Log("이름에 공백이 포함되어 있습니다.");
        }
        else {
            PlayerCharacter.curPC = new PlayerCharacter();
            GameData.pc = PlayerCharacter.curPC;
            PlayerCharacter.curPC.playerName = userNameInputField.text;
            //SaveLoad.Save();
            ClosePopup();
            
            ChangeScene((int)Scenes.MAIN);

        }
    }
}
