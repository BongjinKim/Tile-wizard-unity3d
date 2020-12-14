using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class BattleManager : CommonBehaviorInGame {
    //0 - 기본
    //1 - 공격
    //2 - 피격
    //이미지는 원본의 65%로

    public Image backgroundImg;
    Vector2 characterImgOriginalSize = new Vector2(583.05f, 706.55f);
    Vector2 characterImgExceptionSize = new Vector2(728, 746.2f);
    
    //player panel
    public GameObject playerPanel;
    public Image playerCharacterImg;
    //-1534 , 1534
    Vector2 playerInitPos = new Vector2(-2000, 0);
    Vector2 playerCenterTarget = new Vector2(-500, 0);
    Vector2 playerCenterPos = new Vector2(-576, 0);
    
    //enemy panel
    public GameObject enemyPanel;
    public Image enemyCharacterImg;
    Vector2 enemyInitPos = new Vector2(2000, 0);
    Vector2 enemyCenterTarget = new Vector2(500, 0);
    Vector2 enemyCenterPos = new Vector2(576, 0);

    //
    public Text pNameText;
    public Text eNameText;
    public Text pHP;
    public Text eHP;
    Coroutine hpDisplayCoroutine;
    public Image pHPBar;
    public Image eHPBar;

    //
    float coefficient = 50f;

    //
    float atkSpeed = 1500f;
    bool boost = false;
    
    void Start() {
        myCoroutine = StartBattle();
        StartCoroutine(myCoroutine);
    }

    IEnumerator StartBattle() {
        init();
        hpDisplayCoroutine = StartCoroutine(DisplayHP());
        yield return StartCoroutine(AppearCharacters());
        yield return StartCoroutine(Attack());
        
        ChangePhase();
    }

    void init() {
        playerPanel.GetComponent<RectTransform>().anchoredPosition = playerInitPos;
        enemyPanel.GetComponent<RectTransform>().anchoredPosition = enemyInitPos;
        SetNameImg();
    }

    IEnumerator AppearCharacters() {
        float a = -1f;
        float v0 = 2500f;
        float v = 0;
        float t = 0;
        Vector2 playerCurPos = playerPanel.GetComponent<RectTransform>().anchoredPosition;
        Vector2 enemyCurPos = enemyPanel.GetComponent<RectTransform>().anchoredPosition;

        //comming to center
        while (true) {
            v = v0 + a * t;

            playerPanel.GetComponent<RectTransform>().anchoredPosition = Vector2.MoveTowards(playerPanel.GetComponent<RectTransform>().anchoredPosition, playerCenterPos, v * Time.deltaTime);
            playerCurPos = playerPanel.GetComponent<RectTransform>().anchoredPosition;

            enemyPanel.GetComponent<RectTransform>().anchoredPosition = Vector2.MoveTowards(enemyPanel.GetComponent<RectTransform>().anchoredPosition, enemyCenterPos, v * Time.deltaTime);
            enemyCurPos = enemyPanel.GetComponent<RectTransform>().anchoredPosition;


            if (Vector2.Distance(playerCurPos, playerCenterPos) < 5f &&
               Vector2.Distance(enemyCurPos, enemyCenterPos) < 5f)
            {
                break;
            }

            t++;
            yield return null;
        }
    }

    void SetNameImg() {
        pNameText.text = Game.playerDummy.name;
        eNameText.text = Game.enemyDummy.name;

        playerCharacterImg.sprite = Game.curStageInfo.playerBattleImg.basic;
        enemyCharacterImg.sprite = Game.curStageInfo.enemyBattleImg.basic;
    }
    IEnumerator DisplayHP()
    {
        while (true)
        {
            DisplayHP(pHP, eHP, pHPBar, eHPBar, (float)Game.playerDummy.stat.characterHP, BuildAndMoveManager.pMaxHP, (float)Game.enemyDummy.stat.characterHP, BuildAndMoveManager.eMaxHP);
            yield return null;
        }
    }

    IEnumerator Attack() {
        float playerDmg = BattleFormulas.DmgByCharacter(Game.playerDummy.stat, Game.enemyDummy.stat);
        float enemyDmg = BattleFormulas.DmgByCharacter(Game.enemyDummy.stat, Game.playerDummy.stat);
        CStatus[] characterStatus = new CStatus[2];
        float[] characterDmg = new float[2];
        GameObject[] panels = new GameObject[2];
        Vector2[] centerPos = new Vector2[2];
        ModelType[] types = new ModelType[2];
        Image[] imgs = new Image[2];
        BattleImg[] bis = new BattleImg[2];

        if (BuildAndMoveManager.firstAttacker == ModelType.PLAYER)
        {
            characterStatus[0] = Game.playerDummy.stat;
            characterStatus[1] = Game.enemyDummy.stat;
            panels[0] = playerPanel;
            panels[1] = enemyPanel;
            centerPos[0] = playerCenterPos;
            centerPos[1] = enemyCenterPos;
            types[0] = ModelType.PLAYER;
            types[1] = ModelType.ENEMY;
            imgs[0] = playerCharacterImg;
            imgs[1] = enemyCharacterImg;
            bis[0] = Game.curStageInfo.playerBattleImg;
            bis[1] = Game.curStageInfo.enemyBattleImg;
        }
        else
        {
            characterStatus[0] = Game.enemyDummy.stat;
            characterStatus[1] = Game.playerDummy.stat;
            panels[0] = enemyPanel;
            panels[1] = playerPanel;
            centerPos[0] = enemyCenterPos;
            centerPos[1] = playerCenterPos;
            types[0] = ModelType.ENEMY;
            types[1] = ModelType.PLAYER;
            imgs[0] = enemyCharacterImg;
            imgs[1] = playerCharacterImg;
            bis[0] = Game.curStageInfo.enemyBattleImg;
            bis[1] = Game.curStageInfo.playerBattleImg;
        }

        characterDmg[0] = BattleFormulas.DmgByCharacter(characterStatus[0], characterStatus[1]);
        characterDmg[1] = BattleFormulas.DmgByCharacter(characterStatus[1], characterStatus[0]);

        int idx = -1;

        while (true) {
            if ((int)Game.playerDummy.stat.characterHP <= 0 || (int)Game.enemyDummy.stat.characterHP <= 0) {
                if ((int)Game.playerDummy.stat.characterHP <= 0)
                {
                    //death
                    playerCharacterImg.sprite = Game.curStageInfo.playerBattleImg.death;
                    Game.playerDummy.stat.characterHP = 0;
                    Game.gResult = GameResult.LOSE;
                    break;
                }
                else if ((int)Game.enemyDummy.stat.characterHP <= 0) {
                    //death
                    enemyCharacterImg.sprite = Game.curStageInfo.enemyBattleImg.death;
                    Game.enemyDummy.stat.characterHP = 0;
                    Game.gResult = GameResult.WIN;
                    break;
                }
            }
            
            idx++;
            int i = (idx) % 2;
            //basic
            imgs[i].sprite = bis[i].basic;
            yield return StartCoroutine(MovePanel(panels[i], centerPos[i], Mathf.Clamp(characterDmg[i] * coefficient,0,400), types[i]));
            //hit
            imgs[(i + 1) % 2].sprite = bis[(i + 1) % 2].hit;
            characterStatus[(i + 1) % 2].characterHP -= characterDmg[i];

            if ((int)characterStatus[(i + 1) % 2].characterHP <= 0)
                characterStatus[(i + 1) % 2].characterHP = 0;

            yield return new WaitForSeconds(0.1f);
        }

        yield return new WaitForSeconds(2);
    }

    IEnumerator MovePanel(GameObject go, Vector2 target, float dist, ModelType m) {
        
        HorizontalDirection hd = HorizontalDirection.Backwards;

        Vector2 backTarget;

        if (m == ModelType.PLAYER)
        {
            backTarget = new Vector2(target.x - dist, target.y);
        }
        else {
            backTarget = new Vector2(target.x + dist, target.y);
        }

        while (true) {
            //v = v0 + a * t;

            if (Input.GetMouseButton(0) && boost == false) {
                boost = true;
                atkSpeed *= 3;
            }

            if (hd == HorizontalDirection.Backwards)
            {
                go.GetComponent<RectTransform>().anchoredPosition = Vector2.MoveTowards(go.GetComponent<RectTransform>().anchoredPosition, backTarget, atkSpeed * Time.deltaTime);
                if (Vector2.Distance(go.GetComponent<RectTransform>().anchoredPosition, backTarget) < 5f) {
                    hd = HorizontalDirection.Forwards;
                }
            }
            else {
                go.GetComponent<RectTransform>().anchoredPosition = Vector2.MoveTowards(go.GetComponent<RectTransform>().anchoredPosition, target, atkSpeed * Time.deltaTime);

                if (Vector2.Distance(go.GetComponent<RectTransform>().anchoredPosition, target) < 5f)
                {
                    break;
                }
            }
            yield return null;
        }
    }
}
