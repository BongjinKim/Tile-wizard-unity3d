using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ResultManager : CommonBehaviorInGame {
    //dialogue
    public GameObject dialoguePanel;
    public Image background;
    public Image characterImgL;
    public Image characterImgR;
    public Image characterImgC;
    public Text characterName;
    public Text dialogue;
	public SoundManager sm;

    public Text stageName;
    public Text winLoseText;
    public Text timeRecord;
    public Text rewardMoneyText;
    public Image rewardItemImg;
    public Text rewardItemText;
    private IEnumerator timeToMoneyCoroutine;

    private float timeDecreaseVelocity = 0.05f;
    private int timeToMoney = 10;
    private int rewardMoney;

    void Start() {
        StartCoroutine(init());
    }

    IEnumerator init() {
        yield return new WaitForSeconds(0.01f);

        dialoguePanel.SetActive(true);
        yield return StartCoroutine(ShowDialogues(characterImgL, characterImgR, characterImgC, background, characterName, dialogue));
        dialoguePanel.SetActive(false);

        Stage stage = Game.curStageInfo;

        stageName.text = "Stage" + string.Format("{0:00}", stage.stageNum);

        if (sm == null)
            sm = GameObject.Find("SoundManager").GetComponent<SoundManager>();

        sm.PlaySound ("ResultSound");//result 사운드 출력

        if (Game.gResult == GameResult.WIN) {
            winLoseText.text = "Win";

            rewardMoney = stage.reward.money;
            rewardMoneyText.text = rewardMoney.ToString();


            if (stage.reward.item != null)
            {
                rewardItemImg.gameObject.SetActive(true);
                rewardItemImg.sprite = Game.curStageInfo.reward.item.icon;
                rewardItemText.text = Game.curStageInfo.reward.item.koreanName;
                if (Game.curStageInfo.reward.item is Weapon)
                {
                    GameData.pc.weaponsOwnedByCharacter.Add(Game.curStageInfo.reward.item.englishName);
                }
                else if (Game.curStageInfo.reward.item is Armor)
                {
                    GameData.pc.armorsOwnedByCharacter.Add(Game.curStageInfo.reward.item.englishName);
                }
                else if (Game.curStageInfo.reward.item is Accessory) {
                    GameData.pc.accessoriesOwnedByCharacter.Add(Game.curStageInfo.reward.item.englishName);
                }
            }
            else
            {
                rewardItemImg.gameObject.SetActive(false);
                rewardItemText.text = "None";
            }

            if (stage.buildTime == 0)
            {
                timeRecord.text = "0:00";
                rewardMoneyText.text = stage.reward.money.ToString();
            }
            else
            {
                timeToMoneyCoroutine = ConvertRemainingTimeToMoney();
                StartCoroutine(timeToMoneyCoroutine);
                yield return StartCoroutine(WaitClick());
                StopCoroutine(timeToMoneyCoroutine);

                timeRecord.text = "0:00";
                rewardMoneyText.text = ((int)CalRewardMoney(Game.curStageInfo.reward.money, BuildAndMoveManager.remainingT)).ToString();
            }

            UnlockNextStage();

        } else if (Game.gResult == GameResult.LOSE) {
            

            rewardMoney = (int)(stage.reward.money / 10f);
            rewardMoneyText.text = rewardMoney.ToString();

            //아이템 안줌
            rewardItemImg.gameObject.SetActive(false);
            rewardItemText.text = "None";

            timeRecord.text = "0:00";

            winLoseText.text = "Lose";
        }

        
        GameData.pc.characterMoney += int.Parse(rewardMoneyText.text);
        
        SaveLoad.Save();
        yield return new WaitForSeconds(0.1f);

        yield return StartCoroutine(WaitClick());
        yield return new WaitForSeconds(0.5f);

        Game.EndGame();

        if (Game.curStageInfo.stageNum == 25)
        {
            Game.NewGame(GameType.SINGLE, 26);
            ChangeScene((int)Scenes.PLAY_BEGINNING);
        }
        else {
            ChangeScene((int)Scenes.WORLDMAP);
        }
    }

    IEnumerator ConvertRemainingTimeToMoney()
    {
        float remainingTime = BuildAndMoveManager.remainingT;
        
        while (remainingTime > 0) {
            float minutes = (int)(remainingTime / 60);
            float seconds = remainingTime - (minutes * 60);
            timeRecord.text = minutes.ToString() + ":" + string.Format("{0:00}", Mathf.Round(seconds));

            remainingTime -= 1;
            rewardMoney += timeToMoney;
            rewardMoneyText.text = rewardMoney.ToString();

            yield return new WaitForSeconds(timeDecreaseVelocity);
        }


    }

    float CalRewardMoney(int baseMoney, float remainingT) {
        return baseMoney + (remainingT * timeToMoney);
    }

    private void UnlockNextStage()
    {
        if (WorldMapManager.selectedStageNum <= GameData.pc.stageProgress)
        {

        }
        else
        {
            GameData.pc.stageProgress++;
            WorldMapManager.needDrawing = true;
        }

        if (Game.tutorialIng == true)
            Game.tutorialIng = false;
        
    }
}
