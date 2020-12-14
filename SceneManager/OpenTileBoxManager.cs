using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class OpenTileBoxManager : CommonBehavior
{
    public GameObject[] cards = new GameObject[3];
    
    private Tile[] chosenTiles = new Tile[3];

    //cards
    private Vector3 initSize = new Vector3(0, 0, 0);
    private Vector3 middleSize = new Vector3(0.6f, 0.6f, 0.6f);
    private Vector3 lastSize = new Vector3(0.5f, 0.5f, 0.5f);
    private Vector3[] iniRot = new Vector3[3];
    private Vector3 lastRot = new Vector3(0, 0, 0);

    private Vector3[] initPos = new Vector3[3];
    private Vector3[] middlePos = new Vector3[3];
    private Vector3[] lastPos = new Vector3[3];

    //
    private bool isEnded = false;

    //chest model
    GameObject chest;
    public GameObject[] chestPrefabs = new GameObject[3];
    private Vector3 chestPos = new Vector3(0, -2.85f, 1.1f);
    private Vector3 chestRot = new Vector3(7.5f, -180, 0);
    private Vector3 chestScale = new Vector3(50, 50, 50);

    Animation anim;

    //
    Coroutine curCoroutine;
    bool isPlayEnded;

    public static string selectedBox;

    //
    public Sprite cardBack;

    //
    public GameObject fonfirmBtn;
    public GameObject cardImgPanel;
    public Image[] cardImgs = new Image[3];
    public GameObject centerImgPanel;
    public Image centerImg;

    // Use this for initialization
    void Start()
    {
        isPlayEnded = false;

        initPos[0] = new Vector3(-2, -1.5f, 0);
        initPos[1] = new Vector3(0, -1.5f, 0);
        initPos[2] = new Vector3(2, -1.5f, 0);

        middlePos[0] = new Vector3(-3.75f, 1, 0);
        middlePos[1] = new Vector3(0, 2, 0);
        middlePos[2] = new Vector3(3.75f, 1, 0);
        lastPos[0] = new Vector3(-3.8f, 1, 0);
        lastPos[1] = new Vector3(0, 1, 0);
        lastPos[2] = new Vector3(3.8f, 1, 0);
        iniRot[0] = new Vector3(0, 0, 35);
        iniRot[1] = new Vector3(0, 0, 0);
        iniRot[2] = new Vector3(0, 0, -35);

        
        calProb(selectedBox);

        StartCoroutine(playOpenAnimation());
    }

    void calProb(string str)
    {
        int firstCardIdx;
        int secondCardIdx;
        int thirdCardIdx;
        List<int> ints = new List<int>();
        ints.Add(0);
        ints.Add(1);
        ints.Add(2);
        int rndTier = 0;

        if (str.Equals("bronze"))
        {
            //확정카드
            rndTier = Random.Range(0, 1000) >= GameData.probBronzeFix[0] ? 1 : 0;
            firstCardIdx = Random.Range(0, 3);
            ints.Remove(firstCardIdx);
            chosenTiles[firstCardIdx] = GameData.tilesByTier[rndTier][Random.Range(0, GameData.tilesByTier[rndTier].Count)];

            rndTier = setCardTier("bronze") - 1;
            secondCardIdx = ints[Random.Range(0, 2)];
            ints.Remove(secondCardIdx);
            chosenTiles[secondCardIdx] = GameData.tilesByTier[rndTier][Random.Range(0, GameData.tilesByTier[rndTier].Count)];

            rndTier = setCardTier("bronze") - 1;
            thirdCardIdx = ints[0];
            chosenTiles[thirdCardIdx] = GameData.tilesByTier[rndTier][Random.Range(0, GameData.tilesByTier[rndTier].Count)];

        }
        else if (str.Equals("silver"))
        {
            rndTier = Random.Range(0, 1000) >= GameData.probSilverFix[1] ? 2 : 1;
            firstCardIdx = Random.Range(0, 3);
            ints.Remove(firstCardIdx);
            chosenTiles[firstCardIdx] = GameData.tilesByTier[rndTier][Random.Range(0, GameData.tilesByTier[rndTier].Count)];

            rndTier = setCardTier("silver") - 1;
            secondCardIdx = ints[Random.Range(0, 2)];
            ints.Remove(secondCardIdx);
            chosenTiles[secondCardIdx] = GameData.tilesByTier[rndTier][Random.Range(0, GameData.tilesByTier[rndTier].Count)];

            rndTier = setCardTier("silver") - 1;
            thirdCardIdx = ints[0];
            chosenTiles[thirdCardIdx] = GameData.tilesByTier[rndTier][Random.Range(0, GameData.tilesByTier[rndTier].Count)];

        }
        else if (str.Equals("gold"))
        {
            rndTier = Random.Range(0, 1000) >= GameData.probGoldFix[2] ? 3 : 2;
            firstCardIdx = Random.Range(0, 3);
            ints.Remove(firstCardIdx);
            chosenTiles[firstCardIdx] = GameData.tilesByTier[rndTier][Random.Range(0, GameData.tilesByTier[rndTier].Count)];

            rndTier = setCardTier("gold") - 1;
            secondCardIdx = ints[Random.Range(0, 2)];
            ints.Remove(secondCardIdx);
            chosenTiles[secondCardIdx] = GameData.tilesByTier[rndTier][Random.Range(0, GameData.tilesByTier[rndTier].Count)];

            rndTier = setCardTier("gold") - 1;
            thirdCardIdx = ints[0];
            chosenTiles[thirdCardIdx] = GameData.tilesByTier[rndTier][Random.Range(0, GameData.tilesByTier[rndTier].Count)];
        }

        for (int i = 0; i < 3; i++) {
            //chosenTiles[i].tileCard
            cards[i].GetComponent<SpriteRenderer>().sprite = cardBack;
            GameData.pc.IncreaseTileNum(chosenTiles[i].englishName);
        }
        
        SaveLoad.Save();
    }

    //브론즈, 실버, 골드 카드별 티어 결정
    int setCardTier(string cardRating)
    {
        int rnd = 0;
        int tier = 0;

        if (cardRating.Equals("bronze"))
        {
            rnd = Random.Range(0, 1000);

            if (rnd < GameData.probBronzeRnd[0])
            {
                //tier 1
                tier = 1;
            }
            else if (GameData.probBronzeRnd[0] <= rnd && rnd < (GameData.probBronzeRnd[0] + GameData.probBronzeRnd[1]))
            {
                //tier 2
                tier = 2;

            }
            else
            {
                //tier 3
                tier = 3;
            }

        }
        else if (cardRating.Equals("silver"))
        {
            rnd = Random.Range(0, 1000);

            if (rnd < GameData.probSilverRnd[0])
            {
                //tier 1
                tier = 1;
            }
            else if (GameData.probSilverRnd[0] <= rnd && rnd < (GameData.probSilverRnd[0] + GameData.probSilverRnd[1]))
            {
                //tier 2
                tier = 2;

            }
            else if ((GameData.probSilverRnd[0] + GameData.probSilverRnd[1]) <= rnd && rnd < (GameData.probSilverRnd[0] + GameData.probSilverRnd[1] + GameData.probSilverRnd[2]))
            {
                //tier 3
                tier = 3;
            }
            else
            {
                //tier 4
                tier = 4;
            }

        }
        else if (cardRating.Equals("gold"))
        {
            rnd = Random.Range(0, 1000);

            if (rnd < GameData.probGoldRnd[1])
            {
                //tier 2
                tier = 2;
            }
            else if (GameData.probGoldRnd[1] <= rnd && rnd < (GameData.probGoldRnd[1] + GameData.probGoldRnd[2]))
            {
                //tier 3
                tier = 3;

            }
            else if ((GameData.probGoldRnd[1] + GameData.probGoldRnd[2]) <= rnd && rnd < (GameData.probGoldRnd[1] + GameData.probGoldRnd[2] + GameData.probGoldRnd[3]))
            {
                //tier 4
                tier = 4;

            }
            else
            {
                //tier 5
                tier = 5;
            }
        }

        return tier;
    }

    IEnumerator playOpenAnimation()
    {
        //chest = GameObject.FindGameObjectWithTag("Chest");
        if (selectedBox.Equals("bronze"))
        {
            chest = Instantiate(chestPrefabs[0]);
        }
        else if (selectedBox.Equals("silver"))
        {
            chest = Instantiate(chestPrefabs[1]);
        }
        else if (selectedBox.Equals("gold"))
        {
            chest = Instantiate(chestPrefabs[2]);
        }

        anim = chest.GetComponent<Animation>();

        chest.transform.position = chestPos;
        chest.transform.eulerAngles = chestRot;
        chest.transform.localScale = chestScale;


        for (int i = 0; i < 3; i++)
        {
            cards[i].transform.localScale = initSize;
            cards[i].transform.position = initPos[i];
            cards[i].transform.eulerAngles = iniRot[i];
        }

        curCoroutine = StartCoroutine(playAnim());

        while (isPlayEnded == false)
        {
            if (Input.GetMouseButton(0))
            {
                break;
            }

            yield return null;
        }

        if (isPlayEnded == false)
        {
            StopCoroutine(curCoroutine);
            anim.GetComponent<Animation>()["Open"].time = 1;
        }

        for (int i = 0; i < 3; i++)
        {
            cards[i].transform.eulerAngles = iniRot[i];
            cards[i].transform.localScale = middleSize;
            cards[i].transform.position = middlePos[i];
        }


        yield return new WaitForSeconds(0.2f);

        while (!Input.GetMouseButton(0))
        {
            yield return null;
        }

        yield return StartCoroutine(flipCard());
    }

    IEnumerator playAnim()
    {
        anim.Play();

        while (anim.isPlaying == true)
        {
            yield return null;
        }

        yield return new WaitForSeconds(0.1f);

        for (int i = 0; i < 3; i++)
        {
            while ((Mathf.Abs(cards[i].transform.localScale.x - middleSize.x) >= 0.005f || Mathf.Abs(cards[i].transform.localScale.y - middleSize.y) >= 0.005f) &&
                   (Mathf.Abs(cards[i].transform.position.x - middlePos[i].x) >= 0.005f || Mathf.Abs(cards[i].transform.position.y - middlePos[i].y) >= 0.005f))
            {

                //scale
                cards[i].transform.localScale = Vector3.Lerp(cards[i].transform.localScale, middleSize, 0.1f);


                //change location
                cards[i].transform.position = Vector2.Lerp(cards[i].transform.position, middlePos[i], 0.1f);


                yield return null;
            }

            cards[i].transform.localScale = middleSize;
            cards[i].transform.position = middlePos[i];
        }

        isPlayEnded = true;
    }

    IEnumerator flipCard()
    {
        cardImgPanel.SetActive(true);

        for (int i = 0; i < 3; i++)
        {
            Destroy(cards[i]);
            cardImgs[i].gameObject.SetActive(true);
            cardImgs[i].sprite = chosenTiles[i].tileCard;
        }

        yield return null;

        isEnded = true;
    }

    public void exit()
    {
        if (isEnded == true)
        {
            GoToPriorScene();
        }
    }

    public void MaginifyCardImage(int idx) {
        for (int i = 0; i < 3; i++) {
            if(i == idx)
                cardImgs[i].gameObject.SetActive(false);
            else
                cardImgs[i].gameObject.SetActive(true);
        }
        

        centerImgPanel.SetActive(true);
        centerImg.sprite = chosenTiles[idx].tileCard;
    }

    public void MinimizeCardImage() {
        centerImgPanel.SetActive(false);

        for (int i = 0; i < 3; i++)
        {
            cardImgs[i].gameObject.SetActive(true);
        }
    }

}
