using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class StartMenuManager : CommonBehaviorInGame
{
    public Text characterMoneyText;

    //popup - character img
    public Text characterName;
    public Image characterImg;
    public Image characterImgMain;
    public int curCharacterImgIdx;


    //popup - training
    public Text upgradeHPDesc;
    public Text upgradeHPCost;
    public Text upgradeMPDesc;
    public Text upgradeMPCost;
    public Text upgradeATKDesc;
    public Text upgradeATKCost;
    public Text upgradeDEFDesc;
    public Text upgradeDEFCost;

    //
    public Text tipText;
    public Text playerNameText;
    public Text characterMoneyInTileBox;
    public GameObject moneyShortagePopup;

    // Use this for initialization
    void Start () {
        if (Game.tutorialIng == true)
        {
            GameData.FindTutorial();
            GameData.curTutorialManager.Stage1Tutorial();
        }
        curCharacterImgIdx = GameData.pc.curCharacterImgIdx;
        UpdateCharacterMoney(characterMoneyText);
        UpdateCharacterMainImg();
        DisplayTip();
        DisplayPlayerName(playerNameText, null);
    }

    void Update() {
        if (secondPopup != null) {
            if (Input.GetMouseButton(0)) {
                if (secondPopup.name.Equals("Panel_lackofmoney")) {
                    CloseSecondPopup();
                }
            }
        }
    }

    public void ChangeCharacterImg(string dir) {
        //캐릭터 이미지를 바꾸는 popup창 띄우는 함수
        if (dir.Equals("right"))
        {
            curCharacterImgIdx++;
        }
        else if(dir.Equals("left"))
        {
            curCharacterImgIdx--;
            if (curCharacterImgIdx < 0)
                curCharacterImgIdx += 11;
        }

        curCharacterImgIdx = curCharacterImgIdx % 11;

        UpdateCurrentCharacterImg(curCharacterImgIdx);
    }

    public void UpdateCurrentCharacterImg()
        //현재의 이미지로 업데이트 하는 함수
    {
        curCharacterImgIdx = GameData.pc.curCharacterImgIdx;
        int idx = GameData.pc.curCharacterImgIdx;
        UpdateCurrentCharacterImg(idx);
    }

    public void UpdateCurrentCharacterImg(int idx) {
        //index가 들어오면 index에 해당하는 이미지의 정보를 바꾸어 주는 함수
        if (idx == 0)
        {
            characterName.text = GameData.pc.playerName;
        }
        else
        {
            characterName.text = GameData.thumnails[GameData.pc.myThumnails[idx]].figureNameKr;
        }

        if (GameData.pc.thumnailStates[GameData.pc.myThumnails[idx]] == true)
        {
            characterImg.sprite = GameData.thumnails[GameData.pc.myThumnails[idx]].imgs[0];
        }
        else
        {
            characterImg.sprite = GameData.thumnails[GameData.pc.myThumnails[idx]].silhouette;
        }
    }

    public void UpdateCharacterMainImg() {
        // 메인 캐릭터 이미지 초기화
        int cnt = GameData.thumnails[GameData.pc.myThumnails[curCharacterImgIdx]].imgs.Count;
        characterImgMain.sprite = GameData.thumnails[GameData.pc.myThumnails[curCharacterImgIdx]].imgs[Random.Range(0, cnt)];
    }

    public void UnlockCharacter() {
        // 캐릭터 잠금을 해제 하는 함수
        if (GameData.pc.characterMoney >= 10000) {
            GameData.pc.thumnailStates[GameData.pc.myThumnails[curCharacterImgIdx]] = true;
            UpdateCurrentCharacterImg(curCharacterImgIdx);
            GameData.pc.characterMoney -= 10000;
            UpdateCharacterMoney(characterMoneyText);
            SaveLoad.Save();
        }

        CloseSecondPopup();
    }

    public void EnterStoryMode() {
        // 스토리 모드 입장하는 함수
        if (GameData.pc.isPrologueComplete == false)
        {
            Game.NewGame(GameType.SINGLE, 0);
            ChangeScene(9);
        }
        else {
            ChangeScene(7);
        }
    }

    public void DisplayTip() {
        // Tip 출력하는 함수
        tipText.text = GameData.tips[Random.Range(0, GameData.tips.Count)];
    }

    public void DisplayCharacterMoney() {
        // 캐릭터가 가진 돈을 출력하는 함수
        UpdateCharacterMoney(characterMoneyInTileBox);
    }
}
