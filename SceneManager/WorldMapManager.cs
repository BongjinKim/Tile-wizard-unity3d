using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using Vectrosity;

//[ExecuteInEditMode]
public class WorldMapManager : CommonBehaviorInGame {
    public GameObject mapPanel;
    public Material lineMaterial;
    public static Coroutine curCouroutine;
    private float delay = 0.05f;
    private float dotVelocity = 20;

    //route
    public GameObject[] stageIndicators = new GameObject[25];
    public Image[] stageIndicatorImgs = new Image[25];

    //indicator
    public Sprite nonFlagImg;
    public Sprite flagImg;
    public Vector2[] indicatorCoordinates = new Vector2[25];
    private VectorLine[] routes = new VectorLine[24];

    //stage information
    public Text stageNum;
    public Image stageBackground;
    public Image stagePreview;
    public Text stageTitle;
    public Text stageDesc;

    //
    public static bool needDrawing = false;


    public static int selectedStageNum;

    //stage info
    public Text stageEAText;
    public Image stageEAImg;
    public Sprite eaFire;
    public Sprite eaWater;
    public Sprite eaWind;
    public Sprite eaEarth;
    public Sprite eaNone;

    //
    public Text characterMoneyInTileBox;
    public GameObject moneyShortagePopup;

    void Start() {
        init();

        if (needDrawing == true)
        {
            GameData.pc.stageProgress--;
            DisplayCurrentProgress();
            stageIndicatorImgs[GameData.pc.stageProgress].sprite = flagImg;
            GameData.pc.stageProgress++;
            StartCoroutine(DrawRoute());
        }
        else {
            DisplayCurrentProgress();
        }
        
    }

    void Update()
    {
        if (secondPopup != null)
        {
            if (Input.GetMouseButton(0))
            {
                CloseSecondPopup();
            }
        }
    }

    //popup - tilebox
    public static string selectedTileBox;

    //popup - tilebox
    public void OpenTileBox(string rating)
    {
        int cost = 0;

        if (rating.Equals("bronze"))
        {
            if (GameData.pc.characterMoney >= GameData.bronzeBoxCost)
            {
                cost = -GameData.bronzeBoxCost;
                OpenTileBoxManager.selectedBox = "bronze";
                ChangeScene((int)Scenes.OPEN_TILE_BOX);
            }
            else
            {
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
        GameData.pc.characterMoney += cost;
        DisplayCharacterMoney();
    }

    void init() {
        for (int i = 0; i < 25; i++)
        {
            indicatorCoordinates[i] = stageIndicators[i].GetComponent<RectTransform>().anchoredPosition;
            stageIndicatorImgs[i] = stageIndicators[i].transform.FindChild("Image").GetComponent<Image>();
        }
            for (int i = 0; i < routes.Length; i++)
        {
            Vector2[] points = {indicatorCoordinates[i], indicatorCoordinates[i+1]};
            routes[i] = new VectorLine("Route"+i, points, lineMaterial, 40.0f);
            ModifyVectorLine(routes[i]);
            routes[i].Draw();
        }

        selectedStageNum = -1;
    }

    void DisplayCurrentProgress() {

        for (int i = 0; i < stageIndicators.Length; i++) {
            if (i < GameData.pc.stageProgress)
            {
                stageIndicators[i].SetActive(true);
                stageIndicatorImgs[i].sprite = flagImg;
            } else if (i == GameData.pc.stageProgress) {
                stageIndicators[i].SetActive(true);
                stageIndicatorImgs[i].sprite = nonFlagImg;
            }
            else
            {
                stageIndicators[i].SetActive(false);
            }
        }

        for (int i = 0; i < routes.Length; i++) {
            if (i <= GameData.pc.stageProgress-1)
            {
                routes[i].active = true;
            }
            else
            {
                routes[i].active = false;
            }
        }
    }

    public void setStageNum(int num) {
        selectedStageNum = num;
        stageNum.text = "STAGE "+ string.Format("{0:00}", GameData.stages[selectedStageNum].stageNum);
        stageBackground.sprite = GameData.stages[selectedStageNum].backgroundImg;
        //stagePreview;
        stageTitle.text = GameData.stages[selectedStageNum].stageTitle;
        stageDesc.text = GameData.stages[selectedStageNum].stageDescription;
        switch (GameData.stages[selectedStageNum].mapEA) {
            case ElementalAttribute.NONE:
                stageEAText.text = ChangeStringColor("NONE", "white");
                stageEAImg.sprite = eaNone;
                break;
            case ElementalAttribute.FIRE:
                stageEAText.text = ChangeStringColor("FIRE", "red");
                stageEAImg.sprite = eaFire;
                break;
            case ElementalAttribute.WATER:
                stageEAText.text = ChangeStringColor("WATER", "#87CEEB");
                stageEAImg.sprite = eaWater;
                break;
            case ElementalAttribute.WIND:
                stageEAText.text = ChangeStringColor("WIND", "green");
                stageEAImg.sprite = eaWind;
                break;
            case ElementalAttribute.EARTH:
                stageEAText.text = ChangeStringColor("EARTH", "yellow");
                stageEAImg.sprite = eaEarth;
                break;
        }
    }

    public void UnlockNextStage() {
        if (selectedStageNum < GameData.pc.stageProgress || GameData.pc.stageProgress == 25)
        {

        }
        else {
            stageIndicatorImgs[GameData.pc.stageProgress].sprite = flagImg;

            GameData.pc.stageProgress++;

            SaveLoad.Save();

            curCouroutine = StartCoroutine(DrawRoute());
        }
    }

    IEnumerator DrawRoute() {
        int k1 = 0;
        int k2 = 0;

        if (GameData.pc.stageProgress == 26)
        {
            k1 = 2;
            k2 = 1;
        }
        else {
            k1 = 1;
            k2 = 0;
        }
        Vector2 init = indicatorCoordinates[GameData.pc.stageProgress - k1];
        Vector2 goal = indicatorCoordinates[GameData.pc.stageProgress - k1];
        Vector2[] points = { init, goal };
        VectorLine tempRoute = new VectorLine("TempRoute", points, lineMaterial, 40.0f);

        ModifyVectorLine(tempRoute);

        float distance = Vector2.Distance(indicatorCoordinates[GameData.pc.stageProgress - k1], indicatorCoordinates[GameData.pc.stageProgress - k2]);
        float xDiff = indicatorCoordinates[GameData.pc.stageProgress - k2].x - indicatorCoordinates[GameData.pc.stageProgress - k1].x;
        float yDiff = indicatorCoordinates[GameData.pc.stageProgress - k2].y - indicatorCoordinates[GameData.pc.stageProgress - k1].y;

        int k = (int)(distance / dotVelocity);
        int idx = 0;

        while (idx<=k)
        {
            goal = new Vector2(init.x + (idx/(float)k)* xDiff, init.y + (idx / (float)k) * yDiff);
            tempRoute.points2[1] = goal;
            tempRoute.Draw();
            idx++;
            yield return new WaitForSeconds(delay);
        }

        VectorLine.Destroy(ref tempRoute);

        DisplayCurrentProgress();
    }

    void ModifyVectorLine(VectorLine vl) {
        vl.rectTransform.SetParent(mapPanel.transform);
        vl.rectTransform.SetAsFirstSibling();
        vl.textureScale = 1.0f;
        vl.rectTransform.localScale = new Vector3(1, 1, 1);
        vl.rectTransform.anchoredPosition = new Vector2(0, 972);
    }

    public void LoadStage() {
        Game.NewGame(GameType.SINGLE, selectedStageNum);
        ChangeScene((int)Scenes.PLAY_BEGINNING);
    }

    public void DisplayCharacterMoney()
    {
        UpdateCharacterMoney(characterMoneyInTileBox);
    }
}
