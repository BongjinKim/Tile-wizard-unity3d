using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;

public class UserDeckManager : MonoBehaviour {

	public GameObject TileDeckWindowPanel;
	public GameObject AutoMakePanel;
	public GameObject TileInDeckPrefab; //Tile in deck Prefab
	public GameObject DeckPanel; // Tile in deck Parent
	public GameObject SelectedObj; // Clicked Object
    public GameObject TileInfoPanel;
    public GameObject editTextPanel; // editText panel
    public GameObject showTextBtn;
    public GameObject amountFireTile;
    public GameObject amountWaterTile;
    public GameObject amountEarthTile;
    public GameObject amountWindTile;
	public SoundManager sm;
	public Vector3 point1, point2;
	public ScrollRect tileScrollRect;
	public ScrollRect deckScrollRect;
	public Camera cam;
	public EventSystem es;
    public DeckManager dm;
	public Text inputField;
	public Text text;

	int pixelDragThreshold;
    int fireNum = 0;
    int waterNum = 0;
    int earthNum = 0;
    int windNum = 0;
	int Ecount = 0;
	public bool isClicked;
	public bool tileScrollEnabled;
	public bool deckScrollEnabled;
	public bool deckAreaSelected;
	public bool isPressed;
	public bool isChecked;
	public bool isRaycast;
    public bool isCardEmpty; // 카드가 없는지 확인
	public bool myCardAreaSelected; // 내카드 영역이 선택됬는지
	//private Variables
	private int tileImgCountInDeck = 0;
	private int currentIdxInDeck = 0; 
	private int currentCountInDeck = 0;
	private int layerMask = 1 << 10; // 10을제외한 모든 레이어에대해 raycast
	private int numOfTiles = 0; //number of tiles total
	private int indexOfTiles = 0; //Tiles index total
	
	//private Variables
	public List<GameObject> tileInfoObjects = new List<GameObject> (); //오토할때 게임오브젝트 저장한 곳
	public List<GameObject> InstantPresetObject = new List<GameObject>(); //임시 게임오브젝트
	//public List<GameObject> tileImgInDeck = new List<GameObject>(); //저장하려는 게임오브젝트
	public List<TileInfo> InstantTilesInDeck = new List<TileInfo>(); // 임시 게임오브젝트 데이터
	
	void Start () {
		isClicked = false;
		deckAreaSelected = false;
		myCardAreaSelected = false;
		isRaycast = false;
		pixelDragThreshold = es.pixelDragThreshold;
        LoadDeck(GameData.pc.selectedTileDeckIndex);

        if (sm == null)
            sm = GameObject.Find("SoundManager").GetComponent<SoundManager>();
    }
	
	void Update()
	{   
		if (Input.GetMouseButtonDown(0)) {
			isPressed = true;
			point1 = Input.mousePosition;
			isClicked = true;
			//layerMask = ~layerMask;
			
			if (point1.y >= Screen.height * 0.25f) {
				if(point1.x >= Screen.width * 0.095f && point1.x <= Screen.width * 0.905f)
					myCardAreaSelected = true;
			}else{
				if(point1.x >= Screen.width * 0.179f && point1.x <= Screen.width * 0.821f )
                	deckAreaSelected = true;
			}
			
			//Debug.Log(deckAreaSelected); 
			//Debug.Log(myCardAreaSelected);
			//레이어가 1~9이면 isRaycast = true 가 됨
			if(GameObject.FindGameObjectWithTag("Popup"))
			{
				isRaycast = false;
			}else
				isRaycast = true;
			
			if(isRaycast == true)
			{
				if (deckAreaSelected == true)
				{   //deck area
					deckScrollEnabled = false;
					
					//Vector2 wp = cam.ScreenToWorldPoint(Input.mousePosition);
                    Ray2D ray = new Ray2D(Input.mousePosition, Vector2.zero);
					RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction);
					
					if (hit.collider != null)
					{
						Debug.Log("HIT!!!"+hit.collider.name);
						GameObject go = hit.collider.gameObject;
						
						if (go.tag == "TileInDeck")
						{
                            SelectedObj = go;
						}
					}
				}
				else { 
					//tile info area
					if(myCardAreaSelected == true){
						tileScrollEnabled = false;
						//myCardAreaSelected = false;
						//Vector2 wp = cam.ScreenToWorldPoint(Input.mousePosition);
	                    Ray2D ray = new Ray2D(Input.mousePosition, Vector2.zero);
						RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction);
	                    //Debug.Log("ray : " + ray);
	                    //Debug.Log("wp : " + wp);
	                    //Debug.Log("hit : " + hit); //눌린카드
	                    //Debug.Log(Input.mousePosition); //마우스위치 
						if (hit.collider != null)
						{
							GameObject go = hit.collider.gameObject;
							
							if (go.tag == "TileInfoCard" )
							{
	                            SelectedObj = go;
							}
						}
					}
				}
			}
		}
		
		if (Input.GetMouseButton(0)) {
			
			point2 = Input.mousePosition;
			
			if (deckAreaSelected == true)
			{   //deck area
				if (Mathf.Abs(point2.x - point1.x) >= pixelDragThreshold && deckScrollEnabled == false)
				{
					deckScrollRect.horizontal = true;
					isClicked = false;
					deckScrollEnabled = true;
					//selectedObj = null;
				}
				else
				{
					if (deckScrollEnabled == false)
					{
						deckScrollRect.horizontal = false;
					}
				}
			}
			else
			{
				if(myCardAreaSelected == true){
					//tile info area
					if (SelectedObj != null && SelectedObj.GetComponent<CardInfo>().num!=0)
	                {
	                    StartCoroutine(DownscaleCard(SelectedObj));
					}
					
					if (Mathf.Abs(point2.x - point1.x) >= pixelDragThreshold && tileScrollEnabled == false)
					{
						tileScrollRect.horizontal = true;
						isClicked = false;
						tileScrollEnabled = true;
						//selectedObj = null;
					}
					else
					{
						if (tileScrollEnabled == false)
						{
							tileScrollRect.horizontal = false;
						}
					}
				}
			}
		}
		
		if (Input.GetMouseButtonUp(0))
		{
			if (isPressed == false)
			{
                SelectedObj = null;
				return;
			}
			else {
				isPressed = false;
			}
			
			if (deckAreaSelected == true)
			{
				if (isClicked)
				{
                    if (SelectedObj != null)
					{
						sm.PlaySound("DownCardSound");
                        DeleteSelectedTile(SelectedObj);
		
					}
				}
				else
				{
					//Debug.Log("dragged");
				}
				
				deckScrollRect.horizontal = true;
				deckAreaSelected = false;
			}
			else {
				if(myCardAreaSelected == true)
				{
					if (isClicked)
					{
	                    if (SelectedObj != null)
						{   //add
							if(SelectedObj.GetComponent<CardInfo>().num!=0){
								sm.PlaySound("PickCardSound");//sound
	                        	MakeSelectedTile(SelectedObj);
							}else
								sm.PlaySound("NoActiveSound");
						}
					}
					else
					{
						//Debug.Log("dragged");
					}
					tileScrollRect.horizontal = true;
					StopAllCoroutines();

	                if (SelectedObj != null)
	                    StartCoroutine(UpscaleCard(SelectedObj));
					myCardAreaSelected = false;
				}
			}

            SelectedObj = null;
		}
	}

	IEnumerator DownscaleCard(GameObject go)
        //보유하고 있는 카드를 클릭했을 때 작아짐
    {
		Vector3 presentObjScale = go.GetComponent<RectTransform>().localScale;
		Vector3 newObjScale = new Vector3(0.75f, 0.75f, 0.75f);
		
		while(isClicked == true){
			if (Mathf.Abs(presentObjScale.x - newObjScale.x) >= 0.01f || Mathf.Abs(presentObjScale.y - newObjScale.y) >= 0.01f)
			{
				go.GetComponent<RectTransform>().localScale = Vector3.Lerp(presentObjScale, newObjScale, 0.2f);
				presentObjScale = go.GetComponent<RectTransform>().localScale;
			}
			else {
				go.GetComponent<RectTransform>().localScale = newObjScale;
			}
			
			yield return null;
		}
		
	}

	IEnumerator UpscaleCard(GameObject go) {
        //보유하고 있는 카드를 클릭했을 때 커짐
        Vector3 presentObjScale = go.GetComponent<RectTransform>().localScale;
		Vector3 newObjScale = new Vector3(1, 1, 1);
		
		while (Mathf.Abs(presentObjScale.x - newObjScale.x) >= 0.01f || Mathf.Abs(presentObjScale.y - newObjScale.y) >= 0.01f)
		{ 
			go.GetComponent<RectTransform>().localScale = Vector3.Lerp(presentObjScale, newObjScale, 0.1f);
			presentObjScale = go.GetComponent<RectTransform>().localScale;
			yield return null;
		}
		
		go.GetComponent<RectTransform>().localScale = newObjScale;
	}
	
	public void MakeAutoYesBtn(){
        // 덱 자동 구성 함수
		int count = dm.tileInfoObjects.Count;
		int[] autoset = new int[count];
		for (int i=0; i<25; i++) {
			int auto = Random.Range (0,count);
			if(dm.tileInfoObjects [auto].GetComponent<CardInfo>().num != 0){
				MakeSelectedTile (dm.tileInfoObjects [auto]);
			}else
			{
				i--;
			}
		}
       
        AutoMakePanel.SetActive(false);
        CountingElementNum();

        GameData.pc.tileDeckListOwnedByCharacter[GameData.pc.selectedTileDeckIndex].numOfElement = new int[] { fireNum, waterNum, earthNum, windNum };
        GameData.pc.tileDeckListOwnedByCharacter[GameData.pc.selectedTileDeckIndex].inDeckData = InstantTilesInDeck;
        GameData.pc.tileDeckListOwnedByCharacter[GameData.pc.selectedTileDeckIndex].name = showTextBtn.transform.FindChild("Text").GetComponent<Text>().text;

        GameData.pc.tileDeckListOwnedByCharacter[GameData.pc.selectedTileDeckIndex].tilesInDeck.Clear();
        int cnt = 0;
        for (int i = 0; i < InstantTilesInDeck.Count; i++)
        {
            for (int j = 0; j < InstantTilesInDeck[i].num; j++)
            {
                GameData.pc.tileDeckListOwnedByCharacter[GameData.pc.selectedTileDeckIndex].tilesInDeck.Add(InstantTilesInDeck[i].tileName); // TilesInDeck에 타일 네임 저장
                cnt++;
            }

        }


        SaveLoad.Save();
        Application.LoadLevel("4-1. INVENTORY");
	}
	
	public void MakeAutoNoBtn(){
        //덱 자동 완성 취소 함수
        AutoMakePanel.SetActive(false);
	}
  
    public void EditText()
         //덱 이름 패널 활성화 함수
    {
        editTextPanel.SetActive(true);
    }

    public void EditTextYesBtn()
        //덱이름 수정 함수
    {
		if (editTextPanel.transform.FindChild("NameEditPanel/InputField").GetComponent<InputField>().text == "")
        {
			editTextPanel.transform.FindChild("NameEditPanel/Text").GetComponent<Text>().text = "다시 입력하세요";
            Ecount++;
            if (Ecount > 3)
            {
				editTextPanel.transform.FindChild("NameEditPanel/Text").GetComponent<Text>().text = "1~8글자로 입력하세요";

            }
        }
        else
        {
			showTextBtn.transform.FindChild("Text").GetComponent<Text>().text = editTextPanel.transform.FindChild("NameEditPanel/InputField").GetComponent<InputField>().text;
			Ecount = 0;
            //GameData.pc.tileDeckListOwnedByCharacter[GameData.pc.selectedTileDeckIndex].name = showTextBtn.transform.FindChild("Text").GetComponent<Text>().text;
            editTextPanel.SetActive(false);
        }
    }

    public void EditTextNoBtn()
        //덱이름 수정 취소함수
    {
        editTextPanel.SetActive(false);
    }

	GameObject MakeTileInfoDeck(int idx) {
        //타일을 덱에 생성하는 함수
		if (currentIdxInDeck < 6) {
            DeckPanel.GetComponent<RectTransform>().sizeDelta = new Vector2(1230, 210);
		} else {
            float width = 1360 + 180 * (InstantTilesInDeck.Count - 6);
            DeckPanel.GetComponent<RectTransform>().sizeDelta = new Vector2(width, 210);
		} 
		GameObject go = Instantiate (TileInDeckPrefab) as GameObject;
        go.transform.SetParent(DeckPanel.transform);
		go.name = (idx+1) + "_Info";
		go.GetComponent<RectTransform>().localScale = new Vector3(1, 1, 1);
		return go;
	}
	
	public void DeleteSelectedTile(GameObject sObj){
		// 선택된 타일을 지우는 함수
		int tileIndex = int.Parse(sObj.name.Split('_')[0]);
		string tileName = InstantTilesInDeck[tileIndex-1].tileName;
		
		bool number=false; //카드들이 여러개인지 파악하는 지역변수
		tileImgCountInDeck = InstantPresetObject.Count; //지우기전 tile의 종류의 개수
		
		int countInDeck = 0;//deck에 있는 같은 tile의 개수 지역변수
		for(int i=0; i<tileImgCountInDeck; i++){
			if(InstantPresetObject[i]!=null)
				countInDeck += InstantTilesInDeck[i].num;
		}
		
		currentCountInDeck = countInDeck;// deck에 있는 같은 tile의 개수 전역변수
		
		//tile의 개수가 1이면 false, 그게 아니면 true
		if(InstantTilesInDeck[tileIndex-1].num == 1){
			number = false;
		}else 
			number = true;

        GameObject oBj = FindSObj(tileName); // 보유하고 있는 카드를 찾음

		//겹쳐있는 카드들 지우기
		if(InstantTilesInDeck[tileIndex-1].num == 2)
		{
			InstantTilesInDeck[tileIndex-1].num-=1; // character의 데이터
            oBj.GetComponent<CardInfo>().num += 1;
            oBj.transform.FindChild("CardAmount").GetComponent<Text>().text = "x" + oBj.GetComponent<CardInfo>().num.ToString();
			//InstantPresetObject[tileIndex-1].GetComponent<TileInfo>().num = InstantTilesInDeck[tileIndex-1].num; // 데이터를 오브잭트에 적용
            CountingElementNum();
            ConvertElementNum();
			InstantPresetObject[tileIndex-1].transform.FindChild("Text").GetComponent<Text>().text = "";
		}
		else if(InstantTilesInDeck[tileIndex-1].num == 3)
		{
			InstantTilesInDeck[tileIndex-1].num -=1;
            oBj.GetComponent<CardInfo>().num += 1;
			oBj.transform.FindChild("CardAmount").GetComponent<Text>().text = "x" + oBj.GetComponent<CardInfo>().num.ToString();
			//tileImgInDeck[tileIndex-1].GetComponent<TileInfo>().num = InstantTilesInDeck[tileIndex-1].num;
            CountingElementNum();
            ConvertElementNum();
			InstantPresetObject[tileIndex-1].transform.FindChild("Text").GetComponent<Text>().text = "2";
		}
		else if(InstantTilesInDeck[tileIndex-1].num == 4)
		{
			InstantTilesInDeck[tileIndex-1].num -=1;
            oBj.GetComponent<CardInfo>().num += 1;
			oBj.transform.FindChild("CardAmount").GetComponent<Text>().text = "x" + oBj.GetComponent<CardInfo>().num.ToString();
			//InstantPresetObject[tileIndex-1].GetComponent<TileInfo>().num = InstantTilesInDeck[tileIndex-1].num;
            CountingElementNum();
            ConvertElementNum();
			InstantPresetObject[tileIndex-1].transform.FindChild("Text").GetComponent<Text>().text = "3";
		}
		else if(InstantTilesInDeck[tileIndex-1].num == 5)
		{
			InstantTilesInDeck[tileIndex-1].num -=1;
            oBj.GetComponent<CardInfo>().num += 1;
			oBj.transform.FindChild("CardAmount").GetComponent<Text>().text = "x" + oBj.GetComponent<CardInfo>().num.ToString();
			//InstantPresetObject[tileIndex-1].GetComponent<TileInfo>().num = InstantTilesInDeck[tileIndex-1].num;
            CountingElementNum();
            ConvertElementNum();
			InstantPresetObject[tileIndex-1].transform.FindChild("Text").GetComponent<Text>().text = "4";
		}
		
		if(sObj.name.Equals(tileIndex + "_Info") && number==false){
            oBj.GetComponent<CardInfo>().num += 1;
			oBj.transform.FindChild("CardAmount").GetComponent<Text>().text = "x" + oBj.GetComponent<CardInfo>().num.ToString();
			InstantTilesInDeck.RemoveAt(tileIndex-1); ; // 데이터 삭제
			InstantPresetObject.RemoveAt(tileIndex-1); //오브젝트 껍데기 삭제
			//InstantPresetObject.Insert(19,null); // 빈 오브젝트 생성
			Destroy(sObj); //오브젝트 삭제
            CountingElementNum();
            ConvertElementNum();

            if (InstantPresetObject.Count < 6)
            {
                DeckPanel.GetComponent<RectTransform>().sizeDelta = new Vector2(1230, 210);
            }
            else
            {
                float width = 1360 + 180 * (InstantTilesInDeck.Count - 6);
                DeckPanel.GetComponent<RectTransform>().sizeDelta = new Vector2(width, 210);
            } 

			if(tileIndex != tileImgCountInDeck){
				for (int i = tileIndex+1; i < tileImgCountInDeck+1; i++)//i는 tile의 이름이기 때문에 이렇게 설정
				{
					int j = i-1;
					GameObject.Find(i + "_Info").name = j+"_Info";
				}
			}


			Debug.Log (tileIndex+"_Info가지워졌어!");
		}
		//카드 비활성화
		if (oBj.GetComponent<CardInfo> ().num == 1) {
			oBj.transform.FindChild("CardImg").GetComponent<Image>().color = new Color32(255,255,255,255);
			//sObj.GetComponent<Image> ().color = new Color (0, 0, 0, 0);
		}
	}
	
	public void MakeSelectedTile(GameObject sObj){
        //선택한 타일을 덱에 만들어 주는 함수
        if (sObj.GetComponent<CardInfo>().num == 0) //선택된 오브젝트의 개수가 0이면 isCardEmpty 가 false가 된다.
            isCardEmpty = true;
        else
            isCardEmpty = false;

		string sObjName = sObj.name.Split('_')[0];
        //Debug.Log(GameData.GetTileData(sObjName).tileEA.ToString()+"!!!!!!");
        currentIdxInDeck = InstantTilesInDeck.Count; // Deck에 있는 카드 index개수
		int countInDeck = 0;//deck에 있는 같은 tile의 개수 지역변수
		for(int i=0; i<currentIdxInDeck; i++){
			countInDeck += InstantTilesInDeck[i].num; //Deck에 있는 카드 개수
		}
		
		currentCountInDeck = countInDeck;// deck에 있는 같은 tile의 개수 전역변수
        if (isCardEmpty == false)
        {
            if (currentCountInDeck < 25)
            { // 데이터상의 개수와 진짜 개수가 같지 않으면
                bool number = false;

                if (InstantPresetObject != null)
                {
                    for (int i = 0; i < currentIdxInDeck; i++)
                    {
                        if (sObjName.Equals(InstantTilesInDeck[i].tileName))
                        {
                            sObj.GetComponent<CardInfo>().num -= 1;
							sObj.transform.FindChild("CardAmount").GetComponent<Text>().text = "x" + sObj.GetComponent<CardInfo>().num.ToString();
							InstantTilesInDeck[i].num += 1;
                            //Debug.Log(presetManager.tileImgInDeck[i].GetComponent<TileInDeck>().presentTileName);
                            //Debug.Log("oneTileNumber:" + presetManager.tileImgInDeck[i].GetComponent<TileInDeck>().oneTileNumber);

                            if (InstantTilesInDeck[i].num == 2)
                            {
								InstantPresetObject[i].transform.FindChild("Text").GetComponent<Text>().text = "2";
                                CountingElementNum();
                                ConvertElementNum();
                                number = true;
                            }
                            else if (InstantTilesInDeck[i].num == 3)
                            {
								InstantPresetObject[i].transform.FindChild("Text").GetComponent<Text>().text = "3";
                                CountingElementNum();
                                ConvertElementNum();
                                number = true;
                            }
                            else if (InstantTilesInDeck[i].num == 4)
                            {
								InstantPresetObject[i].transform.FindChild("Text").GetComponent<Text>().text = "4";
                                CountingElementNum();
                                ConvertElementNum();
                                number = true;
                            }
                            else if (InstantTilesInDeck[i].num == 5)
                            {
								InstantPresetObject[i].transform.FindChild("Text").GetComponent<Text>().text = "5";
                                CountingElementNum();
                                ConvertElementNum();
                                number = true;
                            }
                            else if (InstantTilesInDeck[i].num == 6)
                            {
                                sObj.GetComponent<CardInfo>().num += 1;
								sObj.transform.FindChild("CardAmount").GetComponent<Text>().text = "x" + sObj.GetComponent<CardInfo>().num.ToString();
								InstantTilesInDeck[i].num -= 1;
                                CountingElementNum();
                                ConvertElementNum();
                                Debug.Log("카드한도초과임");
                                number = true;
                            }
                        }
                    }
                }
                if (number == false)
                {
                    sObj.GetComponent<CardInfo>().num -= 1;
					sObj.transform.FindChild("CardAmount").GetComponent<Text>().text = "x" + sObj.GetComponent<CardInfo>().num.ToString();
					InstantTilesInDeck.Add(new TileInfo(sObjName)); //임시 덱에 캐릭터 정보 삽입
                    InstantPresetObject.Add(MakeTileInfoDeck(currentIdxInDeck));// 덱으로 타일 생성
                    InstantPresetObject[currentIdxInDeck].transform.FindChild("Image").GetComponent<Image>().sprite = GameData.GetTileData(sObjName).tileIcon;//선택 이미지 삽입
                    CountingElementNum();
                    ConvertElementNum();
                }
            }
            else
                Debug.Log("덱이꽉참");
        }
        else
            Debug.Log("카드가 없음");
		  

		Debug.Log(sObj.GetComponent<CardInfo> ().num);
        if (sObj.GetComponent<CardInfo> ().num == 0) {
			sObj.transform.FindChild("CardImg").GetComponent<Image>().color = new Color32(200,200,200,200);
			//sObj.GetComponent<Image> ().color = new Color (0, 0, 0, 0);
			Debug.Log("색이바뀌어야해");
		}

    }

	public void CancelDeck(){
        // 인벤토리로 돌아가는 함수
        Application.LoadLevel("4-1. INVENTORY");
	}

	public void ClearDeck(){
        //자신의 덱 클리어
        int currentIdxInDeck = InstantTilesInDeck.Count; // Deck에 있는 카드 index개수
        int countInDeck = 0;//deck에 있는 같은 tile의 개수 지역변수
        for (int i = 0; i < currentIdxInDeck; i++)
        {
            countInDeck += InstantTilesInDeck[i].num; //Dexk에 있는 카드 개수
        }

        for (int j = 0; j < countInDeck; j++)
        {
            DeleteSelectedTile(InstantPresetObject[0]);
        }
        CountingElementNum();
        ConvertElementNum();
	}

    public void SubmitDeck()
     //자신의 덱 저장
    {
        if (GetTotalNum() == 25)
        {


            GameData.pc.tileDeckListOwnedByCharacter[GameData.pc.selectedTileDeckIndex].numOfElement = new int[] { fireNum, waterNum, earthNum, windNum };
            GameData.pc.tileDeckListOwnedByCharacter[GameData.pc.selectedTileDeckIndex].inDeckData = InstantTilesInDeck;
            GameData.pc.tileDeckListOwnedByCharacter[GameData.pc.selectedTileDeckIndex].name = showTextBtn.transform.FindChild("Text").GetComponent<Text>().text;

            GameData.pc.tileDeckListOwnedByCharacter[GameData.pc.selectedTileDeckIndex].tilesInDeck.Clear();
            int cnt = 0;
            for (int i = 0; i < InstantTilesInDeck.Count; i++)
            {
                for (int j = 0; j < InstantTilesInDeck[i].num; j++)
                {
                    GameData.pc.tileDeckListOwnedByCharacter[GameData.pc.selectedTileDeckIndex].tilesInDeck.Add(InstantTilesInDeck[i].tileName); // TilesInDeck에 타일 네임 저장
                    cnt++;
                }

            }
            Debug.Log(cnt);
            SaveLoad.Save();
            Application.LoadLevel("4-1. INVENTORY");
        }
        else {
            AutoMakePanel.SetActive(true);
        }
            
    }

	void LoadDeck(int num){
        //저장된 덱데이터 불러오기
        showTextBtn.transform.FindChild("Text").GetComponent<Text>().text = GameData.pc.tileDeckListOwnedByCharacter[GameData.pc.selectedTileDeckIndex].name; // 덱이름 로드
        ConvertElementNum();
        if (GameData.pc.tileDeckListOwnedByCharacter[num].inDeckData == null) 
        {
        }
        else
        {
            for (int i = 0; i < GameData.pc.tileDeckListOwnedByCharacter[num].inDeckData.Count; i++)
            {
                switch (GameData.pc.tileDeckListOwnedByCharacter[num].inDeckData[i].tileElement)
                {
                    case "FIRE":
                        fireNum++;
                        break;
                    case "WATER":
                        waterNum++;
                        break;
                    case "EARTH":
                        earthNum++;
                        break;
                    case "WIND":
                        windNum++;
                        break;
                }
                ConvertElementNum();
                for (int j = 0; j < GameData.pc.tileDeckListOwnedByCharacter[num].inDeckData[i].num; j++)
                {
                    string tName = GameData.pc.tileDeckListOwnedByCharacter[num].inDeckData[i].tileName;
                    MakeSelectedTile(FindSObj(tName));
                }
            }
        }

	}

    int GetTotalNum()
    {
        int countInDeck = 0;//deck에 있는 같은 tile의 개수 지역변수
        int currentIdxInDeck = InstantTilesInDeck.Count;
        for (int i = 0; i < currentIdxInDeck; i++)
        {
            countInDeck += InstantTilesInDeck[i].num; //Deck에 있는 카드 개수
        }
        return countInDeck;
    }

    void CountingElementNum()
    {
        fireNum = 0;
        waterNum = 0;
        earthNum = 0;
        windNum = 0;
        for (int i = 0; i < InstantTilesInDeck.Count; i++)
        {
            for (int j = 0; j < InstantTilesInDeck[i].num; j++)
            {
                switch (InstantTilesInDeck[i].tileElement)
                {
                    case "FIRE":
                        fireNum++;
                        break;
                    case "WATER":
                        waterNum++;
                        break;
                    case "EARTH":
                        earthNum++;
                        break;
                    case "WIND":
                        windNum++;
                        break;
                }
            }
        }
    }

    void ConvertElementNum()
    {
        amountFireTile.transform.FindChild("BG/Text").GetComponent<Text>().text = fireNum.ToString();
        amountWaterTile.transform.FindChild("BG/Text").GetComponent<Text>().text = waterNum.ToString();
        amountEarthTile.transform.FindChild("BG/Text").GetComponent<Text>().text = earthNum.ToString();
        amountWindTile.transform.FindChild("BG/Text").GetComponent<Text>().text = windNum.ToString();
    }

    public GameObject FindSObj(string name)
    {
        GameObject fObj = TileInfoPanel.transform.FindChild(name).gameObject;
        return fObj;
    }
}
