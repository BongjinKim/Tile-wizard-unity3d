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
        //ĳ���͸� �ε��ϴ� �Լ�
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
        //���ο� ĳ���͸� �����ϴ� �Լ�
        if (userNameInputField.text.Length == 0)
        {
            Debug.Log("�̸��� �Է��� �ּ���.");
        }
        else if (userNameInputField.text.Contains(" "))
        {
            Debug.Log("�̸��� ������ ���ԵǾ� �ֽ��ϴ�.");
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
