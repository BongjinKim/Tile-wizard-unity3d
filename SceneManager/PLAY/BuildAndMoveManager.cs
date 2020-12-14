using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using Vectrosity;
using UnityEngine.EventSystems;

public class BuildAndMoveManager : CommonBehaviorInGame {
    void OnEnable()
    {
        DeleteBtn.OnClicked += DestroyBlock;
        TileEffect.OnTriggerEntered += DisplayCurrentAffectedTile;
    }

    void OnDisable()
    {
        DeleteBtn.OnClicked -= DestroyBlock;
        TileEffect.OnTriggerEntered -= DisplayCurrentAffectedTile;
    }

    public Sprite transparentBG;
    public GameObject tutorialPanel;
    public Image background;
    public Image characterImgL;
    public Image characterImgR;
    public Image characterImgC;
    public Text characterName;
    public Text dialogue;
    public BackgroundMusicManager bm;

    public Text timerText;

    private IEnumerator playerControlCoroutine;
    private IEnumerator timerCoroutine;

    //cameras
    public Camera playerCam;
    public Camera enemyCam;
    public Camera backgroundCam;
    public SpriteRenderer mapBg;
    bool isPlayerCamDisplayed;
    IEnumerator camCoroutine;

    //build
    private Rect playerCamBuildRect = new Rect(0.2f, 0, 0.8f, 0.85f);
    private Vector3 playerCamBuildPos = new Vector3(5, 10, 5);
    private Vector3 playerCamBuildRot = new Vector3(90, 90, 0);
    private float playerCamBuildSize = 6;
    //move
    private Rect playerCamMoveRect = new Rect(0.2f, 0, 0.5f, 0.85f);
    private Vector3 playerCamMovePosBig = new Vector3(1, 8, 9);
    private Vector3 playerCamMovePosSmall = new Vector3(1.1f, 8.39f, 8.9f);
    private Vector3 playerCamMoveRot = new Vector3(53, 135, 0);
   
    
    private float playerCamMoveSize = 8;
    private Rect enemyCamMoveRect = new Rect(0.7f, 0, 0.3f, 0.85f);
    //private Vector3 enemyCamMovePos = new Vector3(13, 31, 17);
    private Vector3 enemyCamMovePosBig = new Vector3(18.15f, 32.04f, 21.85f);
    private Vector3 enemyCamMovePosSmall = new Vector3(18, 33, 22);
    private Vector3 enemyCamMoveRot = new Vector3(53, 135, 0);
    
    private float enemyCamMoveSize = 13;
    //
    private Vector3 enemyMapPos = new Vector3(40, 0, 10);

    //map prefab
    public GameObject mapPrefab;
    public GameObject wallPrefab;
    public GameObject tilePrefab;

    //패널관련
    public GameObject playerStat;
    public GameObject enemyStat;
    public GameObject skipBtn;
    public GameObject timer;
    public GameObject optionPopup;
    public GameObject tileView;

    //detailed
    public GameObject[] tileSlots = new GameObject[5];
    public Sprite frameSelected;
    public Sprite frameUnselected;
    public Sprite baseTileImg;
    public static Tile curSelectedTile;
    public static int curSelectedTileIdx;
    

    //tile info view
    public Image tileInfo_icon;
    public Text tileInfo_name;
    public Text tileInfo_mana;
    public Text tileInfo_dmg;
    public Text tileInfo_slow;
    public Text tileInfo_stun;
    public Text tileInfo_knockback;
    public Image tileInfo_dmg_icon;
    public Image tileInfo_slow_icon;
    public Image tileInfo_stun_icon;
    public Image tileInfo_knockback_icon;
    public Sprite dmg_instant;
    public Sprite dmg_dot;
    public Sprite dmg_instant_dot;
    public Sprite slow_active;
    public Sprite slow_inactive;
    public Sprite stun_active;
    public Sprite stun_inactive;
    public Sprite knockback_active;
    public Sprite knockback_inactive;


    //route
    //Queue<Base> path = new Queue<Base>();
    List<Base> openList = new List<Base>();
    List<Base> closedList = new List<Base>();
    List<Base> adjacentBases = new List<Base>();
    VectorLine vl;
    public Material lineMaterial;
    public Texture2D frontTex;
    public Texture2D backTex;

    //
    public GameObject deleteBtn;
    private GameObject deleteBtnRef;
    public GameObject canvas;
    private GameObject selectedBlock;

    //3d model
    public GameObject wizardModel;
    public GameObject knightModel;
    private GameObject playerModelRef;
    private GameObject enemyModelRef;

    //move
    public static MoveResult playerMoveResult;
    public static MoveResult enemyMoveResult;
    Model pModel;
    Model eModel;

    //Coroutines HP/MP
    IEnumerator mpDisplayCoroutine;
    IEnumerator hpDisplayCoroutine;
    public GameObject pMPPanel;
    public GameObject pHPPanel;
    public Text pMPText;
    public Text pHPText;
    public Text eHPText;

    public Image pHPBar;
    public Image eHPBar;
    public Image pMPBar;
    
    public static float pMaxMP;
    public static float pMaxHP;
    public static float eMaxHP;

    public static int barMaxWidth = 586;

    //stat
    public Image pEffectSlow;
    public Image pEffectStun;
    public Image pEffectKnockback;
    public Image eEffectSlow;
    public Image eEffectStun;
    public Image eEffectKnockback;


    //
    public static ModelType firstAttacker;

    //path
    Queue<Base> playerPath = new Queue<Base>();
    Queue<Base> enemyPath = new Queue<Base>();

    //
    public Text playerNameText;
    public Text enemyNameText;

    //남은 빌드 시간
    public static float remainingT;

    //mana auto recovery
    float manaRecoveryValue;
    IEnumerator restoreManaCoroutine;

    //
    public Image playerPortraitImg;
    public Image enemyPortraitImg;

    //
    public static ModelType focusedType;

    // Use this for initialization
    void Start() {
        playerControlCoroutine = PlayerControl();
        mpDisplayCoroutine = DisplayMP();
        timerCoroutine = Timer(Game.curStageInfo.buildTime, timerText, true);
        restoreManaCoroutine = RestoreMana();
        hpDisplayCoroutine = DisplayHP();
        camCoroutine = ChangeCamByTouch();



        VectorLine.SetCamera3D(playerCam);
        VectorManager.useDraw3D = true;

        if (GameData.arrowIsAdded == false) { 
            VectorLine.SetEndCap("Arrow", EndCap.Both, lineMaterial, frontTex, backTex);
            GameData.arrowIsAdded = true;
        }

        background.sprite = transparentBG;
        mapBg.sprite = Game.curStageInfo.backgroundImg;
        if (Game.tutorialIng == true)
        {
            GameData.FindTutorial();
            tutorialPanel.SetActive(true);
        }
        else {
            tutorialPanel.SetActive(false);

        }
        firstAttacker = ModelType.NONE;

        myCoroutine = StartBuild();
        StartCoroutine(myCoroutine);
    }

    IEnumerator StartBuild() {

        SetCameras(); //카메라 세팅
        PrepareGame(); // 게임 준비
        FindPath(Game.playerMap, 10, 0, enemyPath);
        DrawRoute(ConstuctPath(Game.playerMap, enemyPath));

        if (WorldMapManager.selectedStageNum == 2)
        {
            characterImgL.gameObject.SetActive(false);
            characterImgR.gameObject.SetActive(false);
            characterImgC.gameObject.SetActive(false);
            characterName.transform.parent.gameObject.SetActive(false);
            yield return StartCoroutine(GameData.curTutorialManager.Stage2());
            tutorialPanel.SetActive(false);

            StartCoroutine(playerControlCoroutine);
            StartCoroutine(mpDisplayCoroutine);
            StartCoroutine(restoreManaCoroutine);
            yield return StartCoroutine(timerCoroutine);
        }
        else if (WorldMapManager.selectedStageNum == 3)
        {
            characterImgL.gameObject.SetActive(false);
            characterImgR.gameObject.SetActive(false);
            characterImgC.gameObject.SetActive(false);
            characterName.transform.parent.gameObject.SetActive(false);
            yield return StartCoroutine(GameData.curTutorialManager.Stage3());
            tutorialPanel.SetActive(false);

            StartCoroutine(playerControlCoroutine);
            StartCoroutine(mpDisplayCoroutine);
            StartCoroutine(restoreManaCoroutine);
            yield return StartCoroutine(timerCoroutine);
        }
        else
        {
            yield return StartCoroutine(ShowDialogues(characterImgL, characterImgR, characterImgC, background, characterName, dialogue));
            StartCoroutine(playerControlCoroutine);
            StartCoroutine(mpDisplayCoroutine);
            StartCoroutine(restoreManaCoroutine);
            yield return StartCoroutine(timerCoroutine);
        }

        //페이즈 변경
        
        StartMovePhase();
    }

    void PrepareGame() {
        //플레이어 생성
        Game.playerDummy = new Dummy(GameData.pc.playerName);
        
        //스탯
        Game.playerDummy.stat = Game.playerDummy.stat.CalStatus(GameData.pc.characterStatus, GameData.GetWeaponData(GameData.pc.equipedWeapon), GameData.GetArmorData(GameData.pc.equipedArmor), GameData.GetAccessoryData(GameData.pc.equipedAccessory), false);
        //타일
        List<string> tilesInDeck = GameData.pc.tileDeckListOwnedByCharacter[GameData.pc.selectedTileDeckIndex].tilesInDeck;

        foreach (string tileName in tilesInDeck) {
            Game.playerDummy.tiles.Add(GameData.GetTileData(tileName));
        }

        //적군 생성
        Game.enemyDummy = new Dummy(Game.curStageInfo.enemy.enemyName);
        //스탯
        Game.enemyDummy.stat = Game.enemyDummy.stat.CalStatus(Game.curStageInfo.enemy.enemyStatus, GameData.GetWeaponData(Game.curStageInfo.enemy.equipedWeapon), GameData.GetArmorData(Game.curStageInfo.enemy.equipedArmor), GameData.GetAccessoryData(Game.curStageInfo.enemy.equipedAccessory), true);
        //타일
        List<string> tilesInEnemyDeck = Game.curStageInfo.enemy.enemyTiles;
        foreach (string tileName in tilesInEnemyDeck)
        {
            Game.enemyDummy.tiles.Add(GameData.GetTileData(tileName));
        }

        pMaxHP = (float)GameData.pc.characterStatus.characterHP;
        pMaxMP = (float)GameData.pc.characterStatus.characterMP;
        eMaxHP = (float)Game.curStageInfo.enemy.enemyStatus.characterHP;

        Game.playerDummy.stat.characterMP = 30;

        DisplayCharacterNames(playerNameText,null, playerPortraitImg, null);
        DisplayMP(pMPText, pMPBar, (float)Game.playerDummy.stat.characterMP, pMaxMP);

        //기본 미로 생성
        //플레이어
        if (Game.curStageInfo.stageNum != 3)
        {
            Game.playerMap = new Map(true);
        }
        else
        {
            Game.playerMap = new Map(false);
        }
        Game.playerMap.myRef = MakeMapInScene(Game.playerMap);
        Game.playerMap.myRef.name = "Player Map";
        //적군
        Game.enemyMap = new Map(false);
        Game.enemyMap.myRef = MakeMapInScene(Game.enemyMap);
        Game.enemyMap.myRef.name = "Enemy Map";
        Game.enemyMap.myRef.transform.position = enemyMapPos;

        //타일 세팅
        
        initTileSlots();
    }

    //타일 설치
    IEnumerator PlayerControl() {
        while (true) {
            if (Input.GetMouseButtonDown(0) && GameData.isPaused == false) {
                Ray ray = playerCam.ScreenPointToRay(Input.mousePosition);
                RayCasting(ray);
            }
            yield return null;
        }
    }

    void RayCasting(Ray ray)
    {
        RaycastHit hitObj;
        if (Physics.Raycast(ray, out hitObj, Mathf.Infinity))
        {
            if (deleteBtnRef == null)
            {
                if (hitObj.transform.tag.Equals("Base"))
                {
                    if (curSelectedTile == null) {

                        return;
                    }
                        

                    string str = hitObj.transform.name;
                    int r = int.Parse(str.Split(',')[0]);
                    int c = int.Parse(str.Split(',')[1]);

                    if (Game.playerMap.map[r, c].myBlock != null) {

                        Debug.Log("not null");
                        return;
                    }
                        

                    if (MakeBlock(Game.playerMap, r, c))
                    {
                        if (FindPath(Game.playerMap, 10, 0, enemyPath))
                        {
                            //경로가 있을 때만 설치
                            ConstructWall(Game.playerMap, r, c);
                            DrawRoute(ConstuctPath(Game.playerMap, enemyPath));
                        }
                        else
                        {
                            DeleteBlock(Game.playerMap, r, c);
                        }
                    }
                    else
                    {
                        //선택한 것이 타일일때
                        float preview = (float)Game.playerDummy.stat.characterMP + curSelectedTile.mana;

                        if (preview >= 0)
                        {
                            ConstructTile(Game.playerMap, r, c);
                        }
                        else {
                            DeleteBlock(Game.playerMap, r, c);
                        }
                    }
                }
                else if (hitObj.transform.tag.Equals("Block"))
                {
                    deleteBtnRef = Instantiate(deleteBtn, new Vector3(Input.mousePosition.x + Screen.width * 0.03f, Input.mousePosition.y - Screen.height * 0.01f, Input.mousePosition.z), Quaternion.identity) as GameObject;
                    deleteBtnRef.transform.SetParent(canvas.transform);
                    deleteBtnRef.transform.localScale = new Vector3(1, 1, 1);
                    selectedBlock = hitObj.transform.gameObject;
                    
                }
            }
            else
            {
                PointerEventData pointer = new PointerEventData(EventSystem.current);
                pointer.position = Input.mousePosition;
                List<RaycastResult> raycastResults = new List<RaycastResult>();
                EventSystem.current.RaycastAll(pointer, raycastResults);
                if (raycastResults.Count > 0)
                {
                    if (!raycastResults[0].gameObject.tag.Equals("DeleteBtn"))
                    {
                        Destroy(deleteBtnRef);
                    }
                }
            }
        }
    }


    bool FindPath(Map map, int r, int c, Queue<Base> path) {
        path.Clear();
        openList.Clear();
        closedList.Clear();

        Base cur = map.map[r, c];
        openList.Add(cur);
        cur.SetGScore(cur);
        cur.SetHScore(map.map[0, 10]);
        cur.SetFScore();

        while (openList.Count > 0) {
            cur = openList[indexOfMinimumCost(openList)];

            if (cur.myBlock != null && cur.myBlock.type == BlockType.EXIT) {
                while (true) {
                    path.Enqueue(cur);
                    cur = cur.parent;

                    if (cur.row == r && cur.column == c) {
                        path.Enqueue(cur);
                        break;
                    }
                }

                return true;
            }

            openList.Remove(cur);
            closedList.Add(cur);

            adjacentBases.Clear();

            if (cur.row >= 1 && map.map[cur.row - 1, cur.column].myBlock == null)
            {
                adjacentBases.Add(map.map[cur.row - 1, cur.column]);
            }
            else {
                if (cur.row >= 1 && map.map[cur.row - 1, cur.column].myBlock.type != BlockType.WALL)
                    adjacentBases.Add(map.map[cur.row - 1, cur.column]);
            }

            if (cur.column >= 1 && map.map[cur.row, cur.column - 1].myBlock == null) {
                adjacentBases.Add(map.map[cur.row, cur.column - 1]);
            }
            else {
                if (cur.column >= 1 && map.map[cur.row, cur.column - 1].myBlock.type != BlockType.WALL)
                    adjacentBases.Add(map.map[cur.row, cur.column - 1]);
            }

            if (cur.row <= 9 && map.map[cur.row + 1, cur.column].myBlock == null)
            {
                adjacentBases.Add(map.map[cur.row + 1, cur.column]);
            }
            else {
                if (cur.row <= 9 && map.map[cur.row + 1, cur.column].myBlock.type != BlockType.WALL)
                    adjacentBases.Add(map.map[cur.row + 1, cur.column]);
            }

            if (cur.column <= 9 && map.map[cur.row, cur.column + 1].myBlock == null)
            {
                adjacentBases.Add(map.map[cur.row, cur.column + 1]);
            }
            else {
                if (cur.column <= 9 && map.map[cur.row, cur.column + 1].myBlock.type != BlockType.WALL)
                    adjacentBases.Add(map.map[cur.row, cur.column + 1]);
            }
            
            foreach (Base b in adjacentBases) {
                if (closedList.Contains(b))
                    continue;

                int temp_gScore = cur.gScore + 1;
                b.SetGScore(map.map[r, c]);

                if ((openList.Contains(b) == false) || (temp_gScore < b.gScore)) {
                    b.parent = cur;
                    b.gScore = temp_gScore;
                    b.SetHScore(map.map[0, 10]);
                    b.SetFScore();

                    if (openList.Contains(b) == false)
                        openList.Add(b);
                }
            }
        }
        return false;
    }

    int indexOfMinimumCost(List<Base> list)
    {
        int index = 0;
        int min = list[0].fScore;

        for (int i = 0; i < list.Count; i++) {
            if (list[i].fScore <= min) {
                min = list[i].fScore;
                index = i;
            }
        }

        return index;
    }

    Vector3[] ConstuctPath(Map map, Queue<Base> path) {
        Vector3[] vec3 = new Vector3[path.Count];
        int idx = 0;
        while (path.Count > 0) {
            Base b = path.Dequeue();
            Vector3 pos = map.myRef.transform.GetChild(b.row * 11 + b.column).gameObject.transform.position;
            vec3[idx] = pos;
            idx++;
        }
        return vec3;
    }

    void DrawRoute(Vector3[] points) {
        VectorLine.Destroy(ref vl);
        vl = new VectorLine("route", points, lineMaterial, 10.0f, LineType.Continuous, Joins.Weld);
        vl.capLength = 5.0f;
        vl.endCap = "Arrow";
        vl.SetColor(Color.red);
        vl.rectTransform.anchoredPosition = new Vector2(0, 1f);
        vl.Draw3D();
    }

    //맵 생성
    GameObject MakeMapInScene(Map map) {
        GameObject go = Instantiate(mapPrefab) as GameObject;
        for (int i = 0; i < 11; i++) {
            for (int j = 0; j < 11; j++) {
                map.map[i, j].myRef = go.transform.GetChild(i * 11 + j).gameObject;
                map.map[i, j].myRef.GetComponent<Renderer>().material.mainTexture = map.map[i, j].texture;
                if (map.map[i, j].myBlock != null && map.map[i, j].myBlock.type == BlockType.WALL) {
                    ConstructWall(map, i, j);
                }
            }
        }
        return go;
    }

    bool MakeBlock(Map map, int r, int c) {
        map.map[r, c].myBlock = new Block();
        map.map[r, c].myBlock.tile = curSelectedTile;

        if (curSelectedTile.englishName.Equals("Wall"))
        {
            map.map[r, c].myBlock.type = BlockType.WALL;
            return true;
        }
        
        map.map[r, c].myBlock.type = BlockType.TILE;
        
        
        return false;
    }

    void DeleteBlock(Map map, int r, int c) {
        map.map[r, c].myBlock.tile = null;
        map.map[r, c].myBlock.type = BlockType.NONE;
        map.map[r, c].myBlock = null;
    }

    void DestroyBlock() {
        int r = int.Parse(selectedBlock.name.Split(',')[0]);
        int c = int.Parse(selectedBlock.name.Split(',')[1]);
        Destroy(selectedBlock);

        if (Game.playerMap.map[r, c].myBlock.type == BlockType.WALL)
        {
            DeleteBlock(Game.playerMap, r, c);
            FindPath(Game.playerMap, 10, 0, enemyPath);
            DrawRoute(ConstuctPath(Game.playerMap, enemyPath));
        }
        else {
            DeleteBlock(Game.playerMap, r, c);
        }
    }

    void ConstructWall(Map map, int r, int c) {
        Vector3 pos = map.map[r, c].myRef.transform.position;
        map.map[r, c].myBlock.myRef = Instantiate(wallPrefab, new Vector3(pos.x, pos.y + 1, pos.z), Quaternion.identity) as GameObject;
        map.map[r, c].myBlock.myRef.name = r + "," + c;
        map.map[r, c].myBlock.myRef.transform.SetParent(map.map[r, c].myRef.gameObject.transform);
        map.map[r, c].myBlock.SetTexture(map.mapEA);
        map.map[r, c].myBlock.myRef.GetComponent<Renderer>().material.mainTexture = map.map[r, c].myBlock.texture;
    }

    void ConstructTile(Map map, int r, int c)
    {
        
        Vector3 pos = map.map[r, c].myRef.transform.position;
        map.map[r, c].myBlock.myRef = Instantiate(tilePrefab, new Vector3(pos.x, pos.y + 0.1f, pos.z), Quaternion.identity) as GameObject;
        map.map[r, c].myBlock.myRef.name = r + "," + c;
        map.map[r, c].myBlock.myRef.transform.SetParent(map.map[r, c].myRef.gameObject.transform);
        map.map[r, c].myBlock.SetTexture(map.mapEA);
        //텍스쳐는 속성별로
        map.map[r, c].myBlock.myRef.GetComponent<Renderer>().material.mainTexture = map.map[r, c].myBlock.texture;
        map.map[r, c].myBlock.myRef.GetComponent<TileEffect>().myTile = map.map[r, c].myBlock.tile;

        Game.playerDummy.stat.characterMP = Mathf.Clamp(Game.playerDummy.stat.characterMP + map.map[r, c].myBlock.tile.mana, 0, pMaxMP);
        GetNewTileFromDeck();
        
    }

    //카메라 세팅
    void SetCameras() {
        if (Game.curPhase == GamePhase.BUILDING)
        {
            playerCam.gameObject.SetActive(true);
            enemyCam.gameObject.SetActive(false);
            playerCam.rect = playerCamBuildRect;
            playerCam.transform.position = playerCamBuildPos;
            playerCam.transform.eulerAngles = playerCamBuildRot;
            playerCam.orthographicSize = playerCamBuildSize;
        }
        else if (Game.curPhase == GamePhase.MOVING) {
            isPlayerCamDisplayed = true;

            playerCam.gameObject.SetActive(true);
            enemyCam.gameObject.SetActive(true);
            playerCam.rect = playerCamMoveRect;
            playerCam.transform.position = playerCamMovePosBig;
            playerCam.transform.eulerAngles = playerCamMoveRot;
            playerCam.orthographicSize = playerCamMoveSize;
            enemyCam.rect = enemyCamMoveRect;
            enemyCam.transform.position = enemyCamMovePosBig;
            enemyCam.transform.eulerAngles = enemyCamMoveRot;
            enemyCam.orthographicSize = enemyCamMoveSize;

            focusedType = ModelType.ENEMY;
        }
    }

    //타일 슬롯 초기화
    void initTileSlots() {
        if (Game.curStageInfo.stageNum > 2) { 
            for (int i = 0; i < 4; i++) {
                int rnd = Random.Range(0, Game.playerDummy.tiles.Count);

                tileSlots[i].GetComponent<TileSlotInfo>().myTile = Game.playerDummy.tiles[rnd];
                tileSlots[i].GetComponent<TileSlotInfo>().UpdateTileImg(baseTileImg);
                Game.playerDummy.tiles.RemoveAt(rnd);
            }

            tileSlots[4].GetComponent<TileSlotInfo>().myTile = GameData.GetTileData("Wall");
            tileSlots[4].GetComponent<TileSlotInfo>().UpdateTileImg(baseTileImg);

            curSelectedTileIdx = 0;
        }
        else{

            for (int i = 0; i < 4; i++)
            {
                tileSlots[i].GetComponent<TileSlotInfo>().myTile = null;
                tileSlots[i].GetComponent<TileSlotInfo>().UpdateTileImg(baseTileImg);
            }
            tileSlots[4].GetComponent<TileSlotInfo>().myTile = GameData.GetTileData("Wall");
            tileSlots[4].GetComponent<TileSlotInfo>().UpdateTileImg(baseTileImg);

            curSelectedTileIdx = 4;
        }

        SelectTileSlot(curSelectedTileIdx);
    }

    //타일 슬롯 선택
    public void SelectTileSlot(int idx) {
        curSelectedTileIdx = idx;
        curSelectedTile = tileSlots[idx].GetComponent<TileSlotInfo>().myTile;
        for (int i = 0; i < 5; i++) {
            if (i == idx) {
                tileSlots[i].transform.FindChild("Image").GetComponent<Image>().sprite = frameSelected;
            }
            else {
                tileSlots[i].transform.FindChild("Image").GetComponent<Image>().sprite = frameUnselected;
            }
        }
        UpdateTileInfoView(curSelectedTile);
    }

    void GetNewTileFromDeck() {
        if (Game.playerDummy.tiles.Count > 0)
        {
            int rnd = Random.Range(0, Game.playerDummy.tiles.Count);
            tileSlots[curSelectedTileIdx].GetComponent<TileSlotInfo>().myTile = Game.playerDummy.tiles[rnd];
            tileSlots[curSelectedTileIdx].GetComponent<TileSlotInfo>().UpdateTileImg(baseTileImg);
            Game.playerDummy.tiles.RemoveAt(rnd);
            
        }
        else {
            tileSlots[curSelectedTileIdx].GetComponent<TileSlotInfo>().myTile = null;
            tileSlots[curSelectedTileIdx].GetComponent<TileSlotInfo>().UpdateTileImg(baseTileImg);
        }

        curSelectedTile = tileSlots[curSelectedTileIdx].GetComponent<TileSlotInfo>().myTile;
        if(curSelectedTile != null)
            UpdateTileInfoView(curSelectedTile);
    }

    void UpdateTileInfoView(Tile tile) {
        if (tile == null)
        {
            tileInfo_icon.sprite = baseTileImg;
            tileInfo_name.text = "";
            tileInfo_mana.text = "";
            tileInfo_dmg.text = "";
            tileInfo_slow.text = "";
            tileInfo_stun.text = "";
            tileInfo_knockback.text = "";

            tileInfo_dmg_icon.sprite = dmg_instant;
            tileInfo_stun_icon.sprite = stun_inactive;
            tileInfo_slow_icon.sprite = slow_inactive;
            tileInfo_knockback_icon.sprite = knockback_inactive;
        }
        else {
            tileInfo_icon.sprite = tile.tileIcon;

            switch (tile.tileEA)
            {
                case ElementalAttribute.NONE:
                    tileInfo_name.text = ChangeStringColor(tile.koreanName, "white");
                    break;
                case ElementalAttribute.FIRE:
                    tileInfo_name.text = ChangeStringColor(tile.koreanName, "red");
                    break;
                case ElementalAttribute.WATER:
                    tileInfo_name.text = ChangeStringColor(tile.koreanName, "#87CEEB");
                    break;
                case ElementalAttribute.WIND:
                    tileInfo_name.text = ChangeStringColor(tile.koreanName, "green");
                    break;
                case ElementalAttribute.EARTH:
                    tileInfo_name.text = ChangeStringColor(tile.koreanName, "yellow");
                    break;
            }

            tileInfo_mana.text = tile.mana.ToString();
            tileInfo_dmg.text = "";
            tileInfo_slow.text = "";
            tileInfo_stun.text = "";
            tileInfo_knockback.text = "";

            tileInfo_dmg_icon.sprite = dmg_instant;
            tileInfo_stun_icon.sprite = stun_inactive;
            tileInfo_slow_icon.sprite = slow_inactive;
            tileInfo_knockback_icon.sprite = knockback_inactive;

            if (tile.instantDamage == 0)
            {
                if (tile.dotDamageDuration == 0)
                {

                }
                else
                {
                    //도트
                    tileInfo_dmg.text = "도트 : " + (tile.dotDamagePerTick * tile.dotDamageDuration * 2).ToString() + " / " + tile.dotDamageDuration.ToString() + "초";
                    tileInfo_dmg_icon.sprite = dmg_dot;
                }
            }
            else
            {
                if (tile.dotDamageDuration == 0)
                {
                    //즉시
                    tileInfo_dmg.text = "즉시 : " + tile.instantDamage.ToString();
                    tileInfo_dmg_icon.sprite = dmg_instant;
                }
                else
                {
                    //둘다
                    tileInfo_dmg.text = "즉시 : " + tile.instantDamage.ToString() + "\n" + "도트 : " + (tile.dotDamagePerTick * tile.dotDamageDuration * 2).ToString() + " / " + tile.dotDamageDuration.ToString() + "초";
                    tileInfo_dmg_icon.sprite = dmg_instant_dot;
                }
            }

            if (tile.slownessDuration != 0)
            {
                tileInfo_slow.text = "효과 : 50%\n" + "지속 : " + tile.slownessDuration + "초";
                tileInfo_slow_icon.sprite = slow_active;
            }
            else
            {
                tileInfo_slow_icon.sprite = slow_inactive;
            }

            if (tile.stunTime != 0)
            {
                tileInfo_stun.text = "지속 : " + tile.stunTime + "초";
                tileInfo_stun_icon.sprite = stun_active;
            }
            else
            {
                tileInfo_stun_icon.sprite = stun_inactive;
            }

            if (tile.knockbackDistance != 0)
            {
                tileInfo_knockback.text = "밀어냄 : " + tile.knockbackDistance + "칸";
                tileInfo_knockback_icon.sprite = knockback_active;
            }
            else
            {
                tileInfo_knockback_icon.sprite = knockback_inactive;
            }
        }
    }

	//move phase
	public void StartMovePhase(){
        if (bm == null)
            bm = GameObject.Find("BackgroundMusicManager").GetComponent<BackgroundMusicManager>();
        //bm.BackgroundPlay("background");
        
        PutTilesByAI();
        VectorLine.Destroy(ref vl);
        StopCoroutine(mpDisplayCoroutine);
        StopCoroutine(timerCoroutine);
        StopCoroutine(playerControlCoroutine);
        StopCoroutine(restoreManaCoroutine);
        StopCoroutine(myCoroutine);
        myCoroutine = StartMoving();
        StartCoroutine(myCoroutine);
    }

    IEnumerator StartMoving() {
        UpdateTileInfoView(null);
        Destroy(deleteBtnRef);
        
        pMPPanel.SetActive(false);
        pHPPanel.SetActive(true);

        timer.SetActive(false);
        skipBtn.SetActive(false);
        tileView.GetComponent<RectTransform>().anchoredPosition = new Vector2(50, -648);
        enemyStat.SetActive(true);
        SetCameras();

        DisplayCharacterNames(playerNameText, enemyNameText, playerPortraitImg, enemyPortraitImg);

        FindPath(Game.enemyMap, 10, 0, playerPath);
        playerModelRef = Instantiate(wizardModel) as GameObject;
        pModel = playerModelRef.GetComponent<Model>();
        pModel.Init(Game.playerDummy, playerPath, ModelType.PLAYER);


        FindPath(Game.playerMap, 10, 0, enemyPath);
        enemyModelRef = Instantiate(knightModel) as GameObject;
        eModel = enemyModelRef.GetComponent<Model>();
        eModel.Init(Game.enemyDummy, enemyPath, ModelType.ENEMY);
        

        DisplayCurrentAffectedTile();

        StartCoroutine(hpDisplayCoroutine);

        StartCoroutine(camCoroutine);

        while (true) {
            if (pModel.mr == MoveResult.FAIL || eModel.mr == MoveResult.FAIL) {
                if (pModel.mr == MoveResult.FAIL) {
                    eModel.mr = MoveResult.PASS;
                }

                if (eModel.mr == MoveResult.FAIL) {
                    pModel.mr = MoveResult.PASS;
                }
                break;
            }

            if (pModel.mr != MoveResult.NONE && eModel.mr != MoveResult.NONE)
                break;

            yield return null;
        }

        pModel.StopMoving();
        eModel.StopMoving();


        yield return new WaitForSeconds(2);

        if (pModel.mr == MoveResult.PASS && eModel.mr == MoveResult.PASS)
        {
            //battle로
            if (Game.curStageInfo.stageNum == 3)
            {
                tutorialPanel.SetActive(true);
                yield return StartCoroutine(GameData.curTutorialManager.Stage3());
            }
            Game.gResult = GameResult.NONE;
            ChangePhase();
        }
        else {
            if (pModel.mr == MoveResult.PASS && eModel.mr == MoveResult.FAIL)
            {
                //플레이어 승
                Game.gResult = GameResult.WIN;
                Game.curPhase = GamePhase.RESULT;
                ChangeScene(((int)Game.curPhase + 8));
            }
            else if (pModel.mr == MoveResult.FAIL && eModel.mr == MoveResult.PASS) {
                //플레이어 패
                Game.gResult = GameResult.LOSE;
                Game.curPhase = GamePhase.RESULT;
                ChangeScene(((int)Game.curPhase + 8));
            }
            
        }
    }

    IEnumerator ChangeCamByTouch() {
        while (true) {
            if (Input.GetMouseButtonDown(0) && GameData.isPaused == false) {
                Vector3 input = Input.mousePosition;
                if (input.x >= Screen.width * 0.7f && input.x <= Screen.width &&
                    input.y >= Screen.height * 0.0f && input.y <= Screen.height * 0.85f)
                {
                    ChangeCameraView();
                }
            }
            yield return null;
        }
    }


    void ChangeCameraView() {
        if (isPlayerCamDisplayed == true)
        {
            playerCam.rect = enemyCamMoveRect;
            playerCam.orthographicSize = enemyCamMoveSize;
            playerCam.transform.position = playerCamMovePosSmall;

            enemyCam.rect = playerCamMoveRect;
            enemyCam.orthographicSize = playerCamMoveSize;
            enemyCam.transform.position = enemyCamMovePosBig;
            
            isPlayerCamDisplayed = false;
            focusedType = ModelType.PLAYER;
        }
        else {
            playerCam.rect = playerCamMoveRect;
            playerCam.orthographicSize = playerCamMoveSize;
            playerCam.transform.position = playerCamMovePosBig;

            enemyCam.rect = enemyCamMoveRect;
            enemyCam.orthographicSize = enemyCamMoveSize;
            enemyCam.transform.position = enemyCamMovePosSmall;

            isPlayerCamDisplayed = true;
            focusedType = ModelType.ENEMY;
        }

        DisplayCurrentAffectedTile();
    }

    public void DisplayCurrentAffectedTile() {
        if (isPlayerCamDisplayed == true)
        {
            UpdateTileInfoView(eModel.curAffectedTile);
        }
        else {
            UpdateTileInfoView(pModel.curAffectedTile);
        }
    }

    IEnumerator DisplayMP() {
        while (true) {
            //float ratio = Game.playerDummy.stat.characterMP / pMaxMP;

            DisplayMP(pMPText, pMPBar, (float)Game.playerDummy.stat.characterMP, pMaxMP);
            yield return null;
        }
    }

    IEnumerator DisplayHP() {
        while (true)
        {
            DisplayHP(pHPText, eHPText, pHPBar, eHPBar, (float)Game.playerDummy.stat.characterHP, pMaxHP, (float)Game.enemyDummy.stat.characterHP, eMaxHP);
            DisplayCurEffect();
            yield return null;
        }
    }

    void DisplayCurEffect()
    {
        pEffectSlow.sprite = pModel.isSlowed == true ? slow_active : slow_inactive;
        pEffectStun.sprite = pModel.isStunned == true ? stun_active : stun_inactive;
        pEffectKnockback.sprite = pModel.isKnockedback == true ? knockback_active : knockback_inactive;
        eEffectSlow.sprite = eModel.isSlowed == true ? slow_active : slow_inactive;
        eEffectStun.sprite = eModel.isStunned == true ? stun_active : stun_inactive; ;
        eEffectKnockback.sprite = eModel.isKnockedback == true ? knockback_active : knockback_inactive; ;
    }

    void PutTilesByAI()
    {
        if (Game.curStageInfo.stageNum < 4)
            return;


        FindPath(Game.enemyMap, 10, 0, playerPath);

        //player path
        List<Base> temp = new List<Base>();
        List<Base> bases = new List<Base>();

        while (playerPath.Count > 0)
        {
            Base b = playerPath.Dequeue();
            temp.Add(b);
        }

        for (int i = temp.Count-1; i >= 0; i--) {
            bases.Add(temp[i]);
        }

        
        EnemyAI.GetInstance().Init();
        EnemyAI.GetInstance().CopyData(Game.enemyDummy, Game.enemyMap, bases, Game.curStageInfo.buildTime);
        EnemyAI.GetInstance().PutTiles();
        
        for (int i = 0; i < bases.Count; i++) {
            if (bases[i].myBlock != null && bases[i].myBlock.type == BlockType.TILE) {
                int r = bases[i].row;
                int c = bases[i].column;
                Vector3 pos = Game.enemyMap.map[r, c].myRef.transform.position;
                Game.enemyMap.map[r, c].myBlock.myRef = Instantiate(tilePrefab, new Vector3(pos.x, pos.y + 0.1f, pos.z), Quaternion.identity) as GameObject;
                Game.enemyMap.map[r, c].myBlock.myRef.name = r + "," + c;
                Game.enemyMap.map[r, c].myBlock.myRef.transform.SetParent(Game.enemyMap.map[r, c].myRef.gameObject.transform);
                Game.enemyMap.map[r, c].myBlock.SetTexture(Game.enemyMap.mapEA);
                Game.enemyMap.map[r, c].myBlock.myRef.GetComponent<Renderer>().material.mainTexture = Game.enemyMap.map[r, c].myBlock.texture;
                Game.enemyMap.map[r, c].myBlock.myRef.GetComponent<TileEffect>().myTile = Game.enemyMap.map[r, c].myBlock.tile;
            }
        }

        /*
        for (int i = 0; i < Game.enemyDummy.tiles.Count; i++)
        {
            Base rndB = bases[Random.Range(0, bases.Count)];

            Game.enemyMap.map[rndB.row, rndB.column].myBlock = new Block();
            Game.enemyMap.map[rndB.row, rndB.column].myBlock.tile = Game.enemyDummy.tiles[i];
            Game.enemyMap.map[rndB.row, rndB.column].myBlock.type = BlockType.TILE;

            ConstructTile(Game.enemyMap, rndB.row, rndB.column);

            bases.Remove(rndB);
        }*/
    }

    IEnumerator RestoreMana() {
        while (true) {
            yield return new WaitForSeconds(1);
            Game.playerDummy.stat.characterMP = Mathf.Clamp(Game.playerDummy.stat.characterMP + 2, 0, pMaxMP);
        }
    }

    public void Exit(int num) {
        /*
        if(mpDisplayCoroutine != null)
            StopCoroutine(mpDisplayCoroutine);
        if (timerCoroutine != null)
            StopCoroutine(timerCoroutine);
        if (playerControlCoroutine != null)
            StopCoroutine(playerControlCoroutine);
        if (restoreManaCoroutine != null)
            StopCoroutine(restoreManaCoroutine);
        if (hpDisplayCoroutine != null)
            StopCoroutine(hpDisplayCoroutine);
        if(camCoroutine != null)
            StopCoroutine(camCoroutine);
        if (myCoroutine != null)
            StopCoroutine(myCoroutine);
        */
        Game.EndGame();
        ChangeScene(num);
    }
}
