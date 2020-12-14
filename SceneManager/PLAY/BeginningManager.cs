using UnityEngine;
using UnityEngine.UI;
using System.Collections;

//대사출력 시작
//대사가 없으면 바로 다음 페이즈로 이동
public class BeginningManager : CommonBehaviorInGame {
    public Image background;
    public Image characterImgL;
    public Image characterImgR;
    public Image characterImgC;
    public Text characterName;
    public Text dialogue;
    public static int cnt;

    void Start() {
        background.sprite = Game.curStageInfo.backgroundImg;
        myCoroutine = init();
        StartCoroutine(myCoroutine);
    }

    IEnumerator init()
    {
        
        yield return StartCoroutine(ShowDialogues(characterImgL, characterImgR, characterImgC, background, characterName, dialogue));
        
        
        if (GameData.pc.stageProgress == -1)
        {
            Game.EndGame();
            GameData.pc.stageProgress = 0;
            GameData.pc.isPrologueComplete = true;
            SaveLoad.Save();
            ChangeScene((int)Scenes.WORLDMAP);
        }
        else
        {
            if (WorldMapManager.selectedStageNum == 1)
            {
                yield return StartCoroutine(LoadScene(Scenes.MAIN));
            }
            else if (WorldMapManager.selectedStageNum == 2)
            {
                Game.curPhase = GamePhase.BUILDING;
                yield return StartCoroutine(LoadScene((Scenes)((int)Game.curPhase + 9)));
            }
            else if (Game.curStageInfo.stageNum == 26) {
                Game.EndGame();
                ChangeScene((int)Scenes.WORLDMAP);
            }
            else {
                GoToNextPhase();
            }
        }
    }
}
