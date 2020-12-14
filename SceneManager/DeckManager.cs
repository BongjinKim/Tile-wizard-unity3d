using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;
using System.Text;

public class DeckManager : MonoBehaviour {
	
	public GameObject tileInfoPrefab;
	public GameObject presentTileBG;
	public RectTransform center;
	public RectTransform panel;
    public Sprite[] images = new Sprite[6];
    public List<GameObject> tileInfoObjects = new List<GameObject>();

	//private Variables
	private int tileImgCountInDeck = 0;
	private int currentIdxInDeck = 0; 
	private int currentCountInDeck = 0;
	private int layerMask = 1 << 10; // 10을제외한 모든 레이어에대해 raycast
	private int numOfTiles = 0; //number of tiles total
	private int indexOfTiles = 0; //Tiles index total

    

	//private Variables
	float[] distance; //tile의 위치
	float[] tilePosition; // tile의 절대 위치
	float[] absDistance; // tile의 절대 거리
	int tileDistance; //타일간의 거리
	int minTileNum; //center와의 거리가 가장짧은 타일번호
	bool dragging = false; // true일때 drag
	int tileTotal = 0; // 타일 index 개수 


	void Start () {
		
		Init();
		countTileTotal (); // 캐릭터 타일의 총합 구하기
		//Debug.Log (tileTotal);


	}
	
	
	void LerpToTile(float position)
	{
		float positionX = Mathf.Lerp(panel.anchoredPosition.x, panel.anchoredPosition.x+position, Time.deltaTime * 10f);
		Vector2 newPosition = new Vector2 (positionX, panel.anchoredPosition.y);	
		
		panel.anchoredPosition = newPosition;
	}
	
	public void StartDrag()
	{
		dragging = true;
	}
	
	public void EndDrag()
	{
		dragging = false;
	}
	/*
	 * 캐릭터가 가지고 있는 타일 생성
	 */
	public void Init(){
		indexOfTiles = GameData.pc.tilesOwnedByCharacter.Count;

		ExpandPresentTileBG();//타일 배경 확장
		
		for (int i = 0; i < indexOfTiles; i++)
		{
			tileInfoObjects.Add(MakeTileInfoPrefab(i)); //타일을 종류에 맞게 생성
		}

	}

	/*
	 * 타일 배경 확장
	 */
	void ExpandPresentTileBG() {
		if (indexOfTiles <= 3)
		{
			
		}
		else {
            float width = 1635 + 518 * (indexOfTiles - 3);
			float newWidth = width / 2;
			presentTileBG.GetComponent<RectTransform>().sizeDelta = new Vector2(width, 728);
			presentTileBG.GetComponent<RectTransform>().anchoredPosition = new Vector2(newWidth, 0);
		}
	}

	/*
	 * tile의 틀을 만들어줌
	 */
	GameObject MakeTileInfoPrefab(int idx) {
       // GameData.sortingTiles(GameData.pc.tilesOwnedByCharacter);
		GameObject tileObject = Instantiate(tileInfoPrefab) as GameObject; //임시 타일 오브젝트
		tileObject.transform.SetParent(presentTileBG.transform);
		tileObject.name = GameData.pc.tilesOwnedByCharacter[idx].tileName + "_Info";
		//tileObject.transform.FindChild("TileNumImage").GetComponent<Image>().sprite = tileObject.GetComponent<TileInfo>().numImg[ GameData.pc.tilesOwnedByCharacter[idx].num-1]; //타일개수
		tileObject.GetComponent<RectTransform>().localScale = new Vector3(1, 1, 1);
		tileObject.GetComponent<RectTransform>().anchoredPosition = new Vector2(300 + idx * 518, 0);
		tileObject.transform.FindChild("CardImg").GetComponent<Image> ().sprite = GameData.GetTileData(GameData.pc.tilesOwnedByCharacter[idx].tileName).tileCard;// tileIcon데이터 가져오기
		//tileObject.transform.FindChild("CardImg/TileImg").GetComponent<Image> ().sprite = GameData.GetTileData(GameData.pc.tilesOwnedByCharacter[idx].tileName).tileIcon;
        tileObject.transform.FindChild("CardAmount").GetComponent<Text>().text = "x" + GameData.pc.tilesOwnedByCharacter[idx].num.ToString();
        tileObject.GetComponent<CardInfo>().num = GameData.pc.tilesOwnedByCharacter[idx].num;
        tileObject.GetComponent<CardInfo>().name = GameData.pc.tilesOwnedByCharacter[idx].tileName;
		return tileObject;
	}
	/*
	 * deck icon 클릭하면 Tile선택화면으로 넘어감
	 */
	/*
	public void DeckIconClicked() {
		int idx = 1;//선택된 타일덱의 번호 확인
		Debug.Log ("선택된 덱 번호는 : " + idx);
		
		GameData.pc.tileDeckListOwnedByCharacter = characterInfo.characterTileDeck[idx];
		
		tileDeckPanel.SetActive(true);
		displayTilesInDeck(idx);
	}
	*/

	public void countTileTotal(){
		for (int i=0; i<GameData.pc.tilesOwnedByCharacter.Count; i++) {
			this.tileTotal += GameData.pc.tilesOwnedByCharacter[i].num;
		}
	}
	/*
	public void displayTilesInDeck(int index){
		GameData.pc.selectedTileDeckIndex = index;
		for (int i = 0; i < GameData.pc.; i++) {
			tileImgInDeck[i].transform.FindChild("Image").GetComponent<Image>().sprite = GameData.tiles [index].tileIcon;
		}
	}
	*/

	
}
