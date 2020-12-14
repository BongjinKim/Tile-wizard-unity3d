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

    //popup - character img
    public void ChangeCharacterImg(string dir) {
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
    {
        curCharacterImgIdx = GameData.pc.curCharacterImgIdx;
        int idx = GameData.pc.curCharacterImgIdx;
        UpdateCurrentCharacterImg(idx);
    }

    public void UpdateCurrentCharacterImg(int idx) {
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

    public void SetCharacterImg() {
        if (GameData.pc.thumnailStates[GameData.pc.myThumnails[curCharacterImgIdx]] == true) {
            GameData.pc.curThumnail = GameData.pc.myThumnails[curCharacterImgIdx];
            GameData.pc.curCharacterImgIdx = curCharacterImgIdx;
            UpdateCharacterMainImg();
        }
    }

    public void UpdateCharacterMainImg() {
        int cnt = GameData.thumnails[GameData.pc.myThumnails[curCharacterImgIdx]].imgs.Count;
        characterImgMain.sprite = GameData.thumnails[GameData.pc.myThumnails[curCharacterImgIdx]].imgs[Random.Range(0, cnt)];
    }

    public void openUnlockPopup(GameObject go) {
        if(GameData.pc.thumnailStates[GameData.pc.myThumnails[curCharacterImgIdx]] == false)
            OpenSecondPopup(go);
    }

    public void UnlockCharacter() {
        if (GameData.pc.characterMoney >= 10000) {
            GameData.pc.thumnailStates[GameData.pc.myThumnails[curCharacterImgIdx]] = true;
            UpdateCurrentCharacterImg(curCharacterImgIdx);
            GameData.pc.characterMoney -= 10000;
            UpdateCharacterMoney(characterMoneyText);
            SaveLoad.Save();
        }

        CloseSecondPopup();
    }

    //popup - training
    public void UpgradeStat(string stat) {
        int cost = 0;

        if (stat.Equals("HP"))
        {
            if (GameData.pc.numUpOfHP < GameData.upgradeMax && GameData.pc.characterMoney >= int.Parse(upgradeHPCost.text))
            {
                GameData.pc.numUpOfHP++;
                GameData.pc.characterStatus.characterHP += GameData.HPIncreasePerUp;
                cost = -int.Parse(upgradeHPCost.text);
            }
        }
        else if (stat.Equals("MP"))
        {
            if (GameData.pc.numUpOfMP < GameData.upgradeMax && GameData.pc.characterMoney >= int.Parse(upgradeMPCost.text))
            {
                GameData.pc.numUpOfMP++;
                GameData.pc.characterStatus.characterMP += GameData.MPIncreasePerUp;
                cost = -int.Parse(upgradeMPCost.text);
            }
        }
        else if (stat.Equals("ATK"))
        {
            if (GameData.pc.numUpOfATK < GameData.upgradeMax && GameData.pc.characterMoney >= int.Parse(upgradeATKCost.text))
            {
                GameData.pc.numUpOfATK++;
                GameData.pc.characterStatus.characterDamage += GameData.ATKIncreasePerUp;
                cost = -int.Parse(upgradeATKCost.text);
            }
        }
        else if (stat.Equals("DEF"))
        {
            if (GameData.pc.numUpOfDEF < GameData.upgradeMax && GameData.pc.characterMoney >= int.Parse(upgradeDEFCost.text))
            {
                GameData.pc.numUpOfDEF++;
                GameData.pc.characterStatus.characterDefense += GameData.DEFIncreasePerUp;
                cost = -int.Parse(upgradeDEFCost.text);
            }
        }

        UpdateCharacterMoney(cost, characterMoneyText);
        RefreshTrainingPopup();
    }

    public void RefreshTrainingPopup() {
        if (GameData.pc.numUpOfHP < GameData.upgradeMax)
        {
            upgradeHPDesc.text = (GameData.pc.numUpOfHP + 1) + "번째 체력 훈련을 실시합니다.";
            upgradeHPCost.text = ((GameData.pc.numUpOfHP + 1) * GameData.upgradeHPCost).ToString();
        }
        else {
            upgradeHPDesc.text = "모든 체력 훈련을 완료하였습니다.";
            upgradeHPCost.text = "-";
        }

        if (GameData.pc.numUpOfMP < GameData.upgradeMax)
        {
            upgradeMPDesc.text = (GameData.pc.numUpOfMP + 1) + "번째 마나 훈련을 실시합니다.";
            upgradeMPCost.text = ((GameData.pc.numUpOfMP + 1) * GameData.upgradeMPCost).ToString();
        }
        else {
            upgradeMPDesc.text = "모든 마나 훈련을 완료하였습니다.";
            upgradeMPCost.text = "-";
        }

        if (GameData.pc.numUpOfATK < GameData.upgradeMax)
        {
            upgradeATKDesc.text = (GameData.pc.numUpOfATK + 1) + "번째 공격력 훈련을 실시합니다.";
            upgradeATKCost.text = ((GameData.pc.numUpOfATK + 1) * GameData.upgradeATKCost).ToString();
        }
        else {
            upgradeATKDesc.text = "모든 공격력 훈련을 완료하였습니다.";
            upgradeATKCost.text = "-";
        }

        if (GameData.pc.numUpOfDEF < GameData.upgradeMax)
        {
            upgradeDEFDesc.text = (GameData.pc.numUpOfDEF + 1) + "번째 방어력 훈련을 실시합니다.";
            upgradeDEFCost.text = ((GameData.pc.numUpOfDEF + 1) * GameData.upgradeDEFCost).ToString();
        }
        else {
            upgradeDEFDesc.text = "모든 방어력 훈련을 완료하였습니다.";
            upgradeDEFCost.text = "-";
        }
    }

    //popup - tilebox
    public void OpenTileBox(string rating) {
        int cost = 0;

        if (rating.Equals("bronze"))
        {
            if (GameData.pc.characterMoney >= GameData.bronzeBoxCost)
            {
                cost = -GameData.bronzeBoxCost;
                OpenTileBoxManager.selectedBox = "bronze";
                ChangeScene((int)Scenes.OPEN_TILE_BOX);
            }
            else {
                OpenSecondPopup(moneyShortagePopup);
            }
        }
        else if (rating.Equals("silver"))
        {
            if (GameData.pc.characterMoney >= GameData.silverBoxCost)
            {
                cost = -GameData.silverBoxCost;
                OpenTileBoxManager.selectedBox = "silver";
                ChangeScene((int)Scenes.OPEN_TILE_BOX);
            }
            else
            {
                OpenSecondPopup(moneyShortagePopup);
            }
        }
        else if (rating.Equals("gold"))
        {
            if (GameData.pc.characterMoney >= GameData.goldBoxCost)
            {
                cost = -GameData.goldBoxCost;
                OpenTileBoxManager.selectedBox = "gold";
                ChangeScene((int)Scenes.OPEN_TILE_BOX);
            }
            else
            {
                OpenSecondPopup(moneyShortagePopup);
            }
        }

        UpdateCharacterMoney(cost, characterMoneyText);
    }

    public void EnterStoryMode() {
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
        tipText.text = GameData.tips[Random.Range(0, GameData.tips.Count)];
    }

    public void DisplayCharacterMoney() {
        UpdateCharacterMoney(characterMoneyInTileBox);
    }
}
