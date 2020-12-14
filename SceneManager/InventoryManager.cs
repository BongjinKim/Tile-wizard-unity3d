using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class InventoryManager : CommonBehaviorInGame {
    public Text characterMoneyText;
    public Text playerName;
	public SoundManager sm;
    //tab contents
    public GameObject[] tabs = new GameObject[3];
    public GameObject[] itemPanel = new GameObject[3];
    public List<GameObject> weaponSlots = new List<GameObject>();
    public List<GameObject> armorSlots = new List<GameObject>();
    public List<GameObject> accessorySlots = new List<GameObject>();
    public List<GameObject> deckSlots = new List<GameObject>(); 
    //prevew
    //weapon
    public Image weaponImage;
    public Text weaponNameText;
    public Text weaponDescText;
    //armor
    public Image armorImage;
    public Text armorNameText;
    public Text armorDescText;
    //accessory
    public Image accessoryImage;
    public Text accessoryNameText;
    public Text accessoryDescText;

    //기본 이미지
    public Sprite weaponEmpty;
    public Sprite armorEmpty;
    public Sprite accessoryEmpty;
    
    //cost
    public Text priceText;

    //status
    //hp
    public Slider HP_Guage_bottom;
    public Slider HP_Guage_middle;
    public Slider HP_Guage_top;
    //mp
    public Slider MP_Guage_bottom;
    public Slider MP_Guage_middle;
    public Slider MP_Guage_top;
    //atk
    public Slider ATK_Guage_bottom;
    public Slider ATK_Guage_middle;
    public Slider ATK_Guage_top;

    public Image Img_ATK_fire;
    public Image Img_ATK_water;
    public Image Img_ATK_earth;
    public Image Img_ATK_wind;

    public Sprite Sprite_fire_active;
    public Sprite Sprite_water_active;
    public Sprite Sprite_earth_active;
    public Sprite Sprite_wind_active;
    public Sprite Sprite_fire_inactive;
    public Sprite Sprite_water_inactive;
    public Sprite Sprite_earth_inactive;
    public Sprite Sprite_wind_inactive;


    //def
    public Slider Def_Guage_bottom;
    public Slider Def_Guage_middle;
    public Slider Def_Guage_top;
    public Text Text_def_fire_10;
    public Text Text_def_water_10;
    public Text Text_def_earth_10;
    public Text Text_def_wind_10;

    public Text hpValueText;
    public Text mpValueText;
    public Text atkValueText;
    public Text defValueText;

    //
    float curHP;
    float curMP;
    float curATK;
    float curDEF;
    bool curATKFire;
    bool curATKWater;
    bool curATKEarth;
    bool curATKWind;
    int curRegFire;
    int curRegWater;
    int curRegEarth;
    int curRegWind;

    //sell mode & normal mode
    int idCnt;
    bool isSellMode;
    Dictionary<int, Item> selectedItemsToSell;
    public GameObject btnSell;
    public GameObject btnAccept;
    public GameObject btnCancel;
    
    //DeckBuyPanel
    public GameObject deckBuyPanel;
    public GameObject hitObj;
    public GameObject deckMaskView;

    //Deck element
    public GameObject amountFireTile;
    public GameObject amountWaterTile;
    public GameObject amountEarthTile;
    public GameObject amountWindTile;

    //player portrait
    public Image playerPortraitImg;


    void Start()
    {
        GameData.sortingTiles(GameData.pc.tilesOwnedByCharacter);
        initVariables();
        UpdateCharacterMoney(characterMoneyText);
        DisplayPlayerName(playerName, playerPortraitImg);
        LoadItemData();
        LoadDeckData();
        SelectTab(0);
        DisplayCurEquipment();
        DisplayCurStatus();
        if (Game.tutorialIng == true)
        {
            GameData.FindTutorial();
            GameData.curTutorialManager.Stage1Tutorial();
        }
    }

    protected void initVariables() {
        isSellMode = false;
        selectedItemsToSell = new Dictionary<int, Item>();
        idCnt = 0;
    }

    protected void LoadItemData()
    {
        //weapon
        for (int i = 0; i < 21; i++) {
            if (i < GameData.pc.weaponsOwnedByCharacter.Count)
            {
                Weapon w = GameData.GetWeaponData(GameData.pc.weaponsOwnedByCharacter[i]);
                weaponSlots[i].SetActive(true);
                weaponSlots[i].GetComponent<Image>().sprite = w.icon;
                weaponSlots[i].GetComponent<ItemSlotInfo>().SetValues("weapon", w.englishName);
                weaponSlots[i].GetComponent<ItemSlotInfo>().id = idCnt++;
                weaponSlots[i].GetComponent<ItemSlotInfo>().checkMark.SetActive(false);
            }
            else {
                weaponSlots[i].SetActive(false);
            }
        }
        //armor
        for (int i = 0; i < 21; i++)
        {
            if (i < GameData.pc.armorsOwnedByCharacter.Count)
            {
                Armor ar = GameData.GetArmorData(GameData.pc.armorsOwnedByCharacter[i]);
                armorSlots[i].SetActive(true);
                armorSlots[i].GetComponent<Image>().sprite = ar.icon;
                armorSlots[i].GetComponent<ItemSlotInfo>().SetValues("armor", ar.englishName);
                armorSlots[i].GetComponent<ItemSlotInfo>().id = idCnt++;
                armorSlots[i].GetComponent<ItemSlotInfo>().checkMark.SetActive(false);
            }
            else {
                armorSlots[i].SetActive(false);
            }
        }
        //accessory
        for (int i = 0; i < 21; i++) {
            if (i < GameData.pc.accessoriesOwnedByCharacter.Count)
            {
                Accessory ac = GameData.GetAccessoryData(GameData.pc.accessoriesOwnedByCharacter[i]);
                accessorySlots[i].SetActive(true);
                accessorySlots[i].GetComponent<Image>().sprite = ac.icon;
                accessorySlots[i].GetComponent<ItemSlotInfo>().SetValues("accessory", ac.englishName);
                accessorySlots[i].GetComponent<ItemSlotInfo>().id = idCnt++;
                accessorySlots[i].GetComponent<ItemSlotInfo>().checkMark.SetActive(false);
            }
            else {
                accessorySlots[i].SetActive(false);
            }
        }
    }

    public void LoadDeckData(){
        for(int i=0; i < 4; i++){
            deckSlots[i].transform.FindChild("Deck_Name").GetComponent<Text>().text = GameData.pc.tileDeckListOwnedByCharacter[i].name;
            if (GameData.pc.tileDeckListOwnedByCharacter[i].isUnlocked == true)
            {
                deckSlots[i].transform.FindChild("Deck_Display/ON").gameObject.SetActive(true);
                deckSlots[i].transform.FindChild("Deck_Display/ON").GetComponent<Image>().color = new Color32(255, 255, 255, 255);
                deckSlots[i].transform.FindChild("Deck_Display/LOCK").gameObject.SetActive(false);
                //Debug.Log(GameData.pc.tileDeckListOwnedByCharacter[i]);
                //Debug.Log(GameData.pc.tileDeckListOwnedByCharacter[i].numOfElement[0]);
                int fire = GameData.pc.tileDeckListOwnedByCharacter[i].numOfElement[0];
                int water = GameData.pc.tileDeckListOwnedByCharacter[i].numOfElement[1];
                int earth = GameData.pc.tileDeckListOwnedByCharacter[i].numOfElement[2];
                int wind = GameData.pc.tileDeckListOwnedByCharacter[i].numOfElement[3];

                deckSlots[i].transform.FindChild("Amount_FireTile/BG/Text").GetComponent<Text>().text = fire.ToString();
                deckSlots[i].transform.FindChild("Amount_WaterTile/BG/Text").GetComponent<Text>().text = water.ToString();
                deckSlots[i].transform.FindChild("Amount_EarthTile/BG/Text").GetComponent<Text>().text = earth.ToString();
                deckSlots[i].transform.FindChild("Amount_WindTile/BG/Text").GetComponent<Text>().text = wind.ToString();
            }
            else
            {
                deckSlots[i].transform.FindChild("Deck_Display/ON").gameObject.SetActive(false);
                deckSlots[i].transform.FindChild("Deck_Display/LOCK").gameObject.SetActive(true);
                deckSlots[i].transform.FindChild("Deck_Display/LOCK").GetComponent<Image>().color = new Color32(255, 255, 255, 128);
                deckSlots[i].transform.FindChild("Amount_FireTile/BG/Text").GetComponent<Text>().text = "0";
                deckSlots[i].transform.FindChild("Amount_WaterTile/BG/Text").GetComponent<Text>().text = "0";
                deckSlots[i].transform.FindChild("Amount_EarthTile/BG/Text").GetComponent<Text>().text = "0";
                deckSlots[i].transform.FindChild("Amount_WindTile/BG/Text").GetComponent<Text>().text = "0";
            }
            if (GameData.pc.selectedTileDeckIndex == i)
            {
                deckSlots[i].transform.GetComponent<Image>().color = new Color(255,255,0,255);
                switch(i){
                    case 0:
                        deckMaskView.GetComponent<RectTransform>().anchoredPosition = new Vector2(1024, 0);
                        break;
                    case 1:
                        deckMaskView.GetComponent<RectTransform>().anchoredPosition = new Vector2(512, 0);
                        break;
                    case 2:
                        deckMaskView.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 0);
                        break;
                    case 3:
                        deckMaskView.GetComponent<RectTransform>().anchoredPosition = new Vector2(-62, 0);
                        break;
                }
            }else
                deckSlots[i].transform.GetComponent<Image>().color = new Color(255, 255, 0, 0);
        }
    }

    protected void DisplayCurEquipment()
    {
        if (GameData.pc.equipedWeapon == null)
        {
            weaponImage.sprite = weaponEmpty;
            DisplayWeaponInfo(null);
        }
        else
        {
            Weapon w = GameData.GetWeaponData(GameData.pc.equipedWeapon);
            weaponImage.sprite = w.icon;
            weaponImage.GetComponent<ItemSlotInfo>().itemName = w.englishName;
            DisplayWeaponInfo(w);
        }

        if (GameData.pc.equipedArmor == null)
        {
            armorImage.sprite = armorEmpty;
            DisplayArmorInfo(null);
        }
        else
        {
            Armor ar = GameData.GetArmorData(GameData.pc.equipedArmor);
            armorImage.sprite = ar.icon;
            armorImage.GetComponent<ItemSlotInfo>().itemName = ar.englishName;
            DisplayArmorInfo(ar);
        }

        if (GameData.pc.equipedAccessory == null)
        {
            accessoryImage.sprite = accessoryEmpty;
            DisplayAccessoryInfo(null);
        }
        else
        {
            Accessory ac = GameData.GetAccessoryData(GameData.pc.equipedAccessory);
            accessoryImage.sprite = ac.icon;
            accessoryImage.GetComponent<ItemSlotInfo>().itemName = ac.englishName;
            DisplayAccessoryInfo(ac);
        }
    }

    protected void DisplayCurStatus()
    {

        curHP = (float)GameData.pc.characterStatus.characterHP;
        curMP = (float)GameData.pc.characterStatus.characterMP;
        curATK = (float)GameData.pc.characterStatus.characterDamage;
        curDEF = (float)GameData.pc.characterStatus.characterDefense;
        curATKFire = GameData.pc.characterStatus.characterAttackEA_FIRE;
        curATKWater = GameData.pc.characterStatus.characterAttackEA_WATER;
        curATKEarth = GameData.pc.characterStatus.characterAttackEA_EARTH;
        curATKWind = GameData.pc.characterStatus.characterAttackEA_WIND;
        curRegFire = GameData.pc.characterStatus.characterElementalReg_FIRE;
        curRegWater = GameData.pc.characterStatus.characterElementalReg_WATER;
        curRegEarth = GameData.pc.characterStatus.characterElementalReg_EARTH;
        curRegWind = GameData.pc.characterStatus.characterElementalReg_WIND;


        HP_Guage_middle.value = curHP / GameData.maxHP;
        HP_Guage_bottom.value = HP_Guage_middle.value;
        HP_Guage_top.value = HP_Guage_middle.value;
        MP_Guage_middle.value = curMP / GameData.maxMP;
        MP_Guage_bottom.value = MP_Guage_middle.value;
        MP_Guage_top.value = MP_Guage_middle.value;
        ATK_Guage_middle.value = curATK / GameData.maxATK;
        ATK_Guage_bottom.value = ATK_Guage_middle.value;
        ATK_Guage_top.value = ATK_Guage_middle.value;
        Def_Guage_middle.value = curDEF / GameData.maxDEF;
        Def_Guage_bottom.value = Def_Guage_middle.value;
        Def_Guage_top.value = Def_Guage_middle.value;
        Text_def_fire_10.text = curRegFire.ToString()+"%";
        Text_def_water_10.text = curRegWater.ToString() + "%";
        Text_def_earth_10.text = curRegEarth.ToString() + "%";
        Text_def_wind_10.text = curRegWind.ToString() + "%";

        if (curATKFire == true)
        {
            Img_ATK_fire.sprite = Sprite_fire_active;
        }
        else
        {
            Img_ATK_fire.sprite = Sprite_fire_inactive;
        }

        if (curATKWater == true)
        {
            Img_ATK_water.sprite = Sprite_water_active;
        }
        else
        {
            Img_ATK_water.sprite = Sprite_water_inactive;
        }

        if (curATKEarth == true)
        {
            Img_ATK_earth.sprite = Sprite_earth_active;
        }
        else
        {
            Img_ATK_earth.sprite = Sprite_earth_inactive;
        }

        if (curATKWind == true)
        {
            Img_ATK_wind.sprite = Sprite_wind_active;
        }
        else
        {
            Img_ATK_wind.sprite = Sprite_wind_inactive;
        }

        hpValueText.text = curHP.ToString();
        mpValueText.text = curMP.ToString();
        atkValueText.text = curATK.ToString();
        defValueText.text = curDEF.ToString();
    }

    protected void ChangeCharacterStatus() {
        float tempHP = GameData.baseHP;
        float tempMP = GameData.baseMP;
        float tempATK = GameData.baseATK;
        float tempDEF = GameData.baseDEF;
        float tempRegFire = GameData.baseREG_FIRE;
        float tempRegWater = GameData.baseREG_WATER;
        float tempRegEarth = GameData.baseREG_EARTH;
        float tempRegWind = GameData.baseREG_WIND;
        bool tempATKFire = false;
        bool tempATKWater = false;
        bool tempATKEarth = false;
        bool tempATKWind = false;

        Weapon tempWeapon;
        Armor tempArmor;
        Accessory tempAccessory;

        //weapon
        tempWeapon = GameData.GetWeaponData(GameData.pc.equipedWeapon);
        

        if (tempWeapon != null)
        {
            switch (tempWeapon.weaponEA1)
            {
                case ElementalAttribute.FIRE:
                    tempATKFire = true;
                    break;
                case ElementalAttribute.WATER:
                    tempATKWater = true;
                    break;
                case ElementalAttribute.WIND:
                    tempATKWind = true;
                    break;
                case ElementalAttribute.EARTH:
                    tempATKEarth = true;
                    break;
            }

            switch (tempWeapon.weaponEA2)
            {
                case ElementalAttribute.FIRE:
                    tempATKFire = true;
                    break;
                case ElementalAttribute.WATER:
                    tempATKWater = true;
                    break;
                case ElementalAttribute.WIND:
                    tempATKWind = true;
                    break;
                case ElementalAttribute.EARTH:
                    tempATKEarth = true;
                    break;
            }

            tempATK += tempWeapon.damage;
        }

        //armor
        tempArmor = GameData.GetArmorData(GameData.pc.equipedArmor);
        

        if (tempArmor != null)
        {
            switch (tempArmor.elementalReg1Attribute)
            {
                case ElementalAttribute.FIRE:
                    tempRegFire += tempArmor.elementalReg1Percent;
                    break;
                case ElementalAttribute.WATER:
                    tempRegWater += tempArmor.elementalReg1Percent;
                    break;
                case ElementalAttribute.WIND:
                    tempRegWind += tempArmor.elementalReg1Percent;
                    break;
                case ElementalAttribute.EARTH:
                    tempRegEarth += tempArmor.elementalReg1Percent;
                    break;
            }

            switch (tempArmor.elementalReg2Attribute)
            {
                case ElementalAttribute.FIRE:
                    tempRegFire += tempArmor.elementalReg2Percent;
                    break;
                case ElementalAttribute.WATER:
                    tempRegWater += tempArmor.elementalReg2Percent;
                    break;
                case ElementalAttribute.WIND:
                    tempRegWind += tempArmor.elementalReg2Percent;
                    break;
                case ElementalAttribute.EARTH:
                    tempRegEarth += tempArmor.elementalReg2Percent;
                    break;
            }

            tempDEF += tempArmor.defense;
            tempHP += tempArmor.HP;
        }

        //accessory
        tempAccessory = GameData.GetAccessoryData(GameData.pc.equipedAccessory);
        

        if (tempAccessory != null)
        {
            switch (tempAccessory.attackElementalAttribute)
            {
                case ElementalAttribute.FIRE:
                    tempATKFire = true;
                    break;
                case ElementalAttribute.WATER:
                    tempATKWater = true;
                    break;
                case ElementalAttribute.WIND:
                    tempATKWind = true;
                    break;
                case ElementalAttribute.EARTH:
                    tempATKEarth = true;
                    break;
            }

            switch (tempAccessory.defenseRegAttribute)
            {
                case ElementalAttribute.FIRE:
                    tempRegFire += tempAccessory.defenseRegPercent;
                    break;
                case ElementalAttribute.WATER:
                    tempRegWater += tempAccessory.defenseRegPercent;
                    break;
                case ElementalAttribute.WIND:
                    tempRegWind += tempAccessory.defenseRegPercent;
                    break;
                case ElementalAttribute.EARTH:
                    tempRegEarth += tempAccessory.defenseRegPercent;
                    break;
            }
        }

        GameData.pc.characterStatus.characterHP = tempHP;
        GameData.pc.characterStatus.characterMP = tempMP;
        GameData.pc.characterStatus.characterDamage = (int)tempATK;
        GameData.pc.characterStatus.characterDefense = (int)tempDEF;
        GameData.pc.characterStatus.characterAttackEA_FIRE = tempATKFire;
        GameData.pc.characterStatus.characterAttackEA_WATER = tempATKWater;
        GameData.pc.characterStatus.characterAttackEA_EARTH = tempATKEarth;
        GameData.pc.characterStatus.characterAttackEA_WIND = tempATKWind;
        GameData.pc.characterStatus.characterElementalReg_FIRE = (int)tempRegFire;
        GameData.pc.characterStatus.characterElementalReg_WATER = (int)tempRegWater;
        GameData.pc.characterStatus.characterElementalReg_EARTH = (int)tempRegEarth;
        GameData.pc.characterStatus.characterElementalReg_WIND = (int)tempRegWind;

        SaveLoad.Save();
    }

    public void SelectTab(int idx)
    {
        for (int i = 0; i < tabs.Length; i++)
        {
            if (i == idx)
            {
                SetTab(i, true);
            }
            else
            {
                SetTab(i, false);
            }
        }
    }

    protected void SetTab(int idx, bool b)
    {
        itemPanel[idx].SetActive(b);
        tabs[idx].transform.FindChild("Active").gameObject.SetActive(b);
    }

    public void ChangeMode() {
        isSellMode = !isSellMode;
        if (isSellMode == true)
        {
            btnSell.SetActive(false);
            btnAccept.SetActive(true);
            btnCancel.SetActive(true);
        }
        else {
            btnSell.SetActive(true);
            btnAccept.SetActive(false);
            btnCancel.SetActive(false);
        }
    }

    public void SelectIcon(GameObject go) {
        if (isSellMode == true)
        {
            CheckItemToSell(go);
        }
        else {
            EquipItem(go);
        }
    }

    public void EquipItem(GameObject go) {
		if (sm == null)
			sm = GameObject.Find ("SoundManager").GetComponent<SoundManager> ();
        ItemSlotInfo isi = go.GetComponent<ItemSlotInfo>();
        string itemType = isi.itemType;
        string itemName = isi.itemName;

        if (itemName == null)
        {
            Debug.Log("return");
            return;
        }

        if (itemType.Equals("weapon"))
        {
            //보유 무기 리스트에 착용하던 아이템 추가
            if(GameData.pc.equipedWeapon != null)
                GameData.pc.weaponsOwnedByCharacter.Add(GameData.pc.equipedWeapon);

            GameData.pc.equipedWeapon = GameData.GetWeaponData(itemName).englishName;

            weaponImage.sprite = GameData.GetWeaponData(itemName).icon;
            weaponImage.GetComponent<ItemSlotInfo>().itemName = GameData.pc.equipedWeapon;
            DisplayWeaponInfo(GameData.GetWeaponData(itemName));
            DeleteItem("weapon", itemName, isi.id);

        }
        else if (itemType.Equals("armor"))
        {
            //보유 아머 리스트에 착용하던 아이템 추가
            if(GameData.pc.equipedArmor != null)
                GameData.pc.armorsOwnedByCharacter.Add(GameData.pc.equipedArmor);

            GameData.pc.equipedArmor = GameData.GetArmorData(itemName).englishName;

            armorImage.sprite = GameData.GetArmorData(itemName).icon;
            armorImage.GetComponent<ItemSlotInfo>().itemName = GameData.pc.equipedWeapon;
            DisplayArmorInfo(GameData.GetArmorData(itemName));

            DeleteItem("armor", itemName, isi.id);
        }
        else if (itemType.Equals("accessory"))
        {
            //보유 악세 리스트에 착용하던 아이템 추가
            if(GameData.pc.equipedAccessory != null)
                GameData.pc.accessoriesOwnedByCharacter.Add(GameData.pc.equipedAccessory);

            GameData.pc.equipedAccessory = GameData.GetAccessoryData(itemName).englishName;
            accessoryImage.sprite = GameData.GetAccessoryData(itemName).icon;
            accessoryImage.GetComponent<ItemSlotInfo>().itemName = GameData.pc.equipedAccessory;
            DisplayAccessoryInfo(GameData.GetAccessoryData(itemName));
            DeleteItem("accessory", itemName, isi.id);
        }

        LoadItemData();
        ChangeCharacterStatus();
        DisplayCurStatus();
		sm.PlaySound ("EquipSound");
    }

    protected void DeleteItem(string type, string name, int id) {
        int itemCount = 0;

        if (type.Equals("weapon"))
        {
            for (int i = 0; i < GameData.pc.weaponsOwnedByCharacter.Count; i++)
            {
                if (name.Equals(weaponSlots[i].GetComponent<ItemSlotInfo>().itemName))
                {
                    itemCount++;
                    if (id == weaponSlots[i].GetComponent<ItemSlotInfo>().id) {
                        break;
                    }
                }
            }

            for (int i = 0; i < GameData.pc.weaponsOwnedByCharacter.Count; i++) {
                if (GameData.pc.weaponsOwnedByCharacter[i].Equals(name)) {
                    itemCount--;
                    if (itemCount == 0) {
                        GameData.pc.weaponsOwnedByCharacter.RemoveAt(i);
                        break;
                    }
                }
            }

        }
        else if (type.Equals("armor"))
        {
            for (int i = 0; i < GameData.pc.armorsOwnedByCharacter.Count; i++)
            {
                if (name.Equals(armorSlots[i].GetComponent<ItemSlotInfo>().itemName))
                {
                    itemCount++;
                    if (id == armorSlots[i].GetComponent<ItemSlotInfo>().id)
                    {
                        break;
                    }
                }
            }

            for (int i = 0; i < GameData.pc.armorsOwnedByCharacter.Count; i++)
            {
                if (GameData.pc.armorsOwnedByCharacter[i].Equals(name))
                {
                    itemCount--;
                    if (itemCount == 0)
                    {
                        GameData.pc.armorsOwnedByCharacter.RemoveAt(i);
                        break;
                    }
                }
            }
        }
        else if (type.Equals("accessory"))
        {
            for (int i = 0; i < GameData.pc.accessoriesOwnedByCharacter.Count; i++)
            {
                if (name.Equals(accessorySlots[i].GetComponent<ItemSlotInfo>().itemName))
                {
                    itemCount++;
                    if (id == accessorySlots[i].GetComponent<ItemSlotInfo>().id)
                    {
                        break;
                    }
                }
            }

            for (int i = 0; i < GameData.pc.accessoriesOwnedByCharacter.Count; i++)
            {
                if (GameData.pc.accessoriesOwnedByCharacter[i].Equals(name))
                {
                    itemCount--;
                    if (itemCount == 0)
                    {
                        GameData.pc.accessoriesOwnedByCharacter.RemoveAt(i);
                        break;
                    }
                }
            }
        }

    }

    public void CheckItemToSell(GameObject go) {
        ItemSlotInfo isi = go.GetComponent<ItemSlotInfo>();
        string itemType = isi.itemType;
        string itemName = isi.itemName;
        bool b = isi.isSelectedToSell;

        if (b == true)
        {
            selectedItemsToSell.Remove(isi.id);
            isi.checkMark.SetActive(false);
        }
        else {
            //새로 선택한 아이템
            if (itemType.Equals("weapon"))
            {
                selectedItemsToSell.Add(isi.id, GameData.GetWeaponData(itemName));
                Debug.Log("added");
            }
            else if (itemType.Equals("armor"))
            {
                selectedItemsToSell.Add(isi.id, GameData.GetArmorData(itemName));
            }
            else if (itemType.Equals("accessory"))
            {
                selectedItemsToSell.Add(isi.id, GameData.GetAccessoryData(itemName));
            }
            isi.checkMark.SetActive(true);
        }
       
        isi.isSelectedToSell = !isi.isSelectedToSell;
        
        PreviewPrice();
    }

    protected void PreviewPrice()
    {
        int price = 0;

        foreach (KeyValuePair<int, Item> item in selectedItemsToSell)
        {
            price += (item.Value.price / 2);
        }
        priceText.text = price.ToString();
    }

    public void UnequipItem(GameObject go)
    {
        if (isSellMode == true)
            return;

        ItemSlotInfo isi = go.GetComponent<ItemSlotInfo>();
        string itemType = isi.itemType;

        if (itemType.Equals("weapon"))
        {
            if (GameData.pc.equipedWeapon != null) {
                GameData.pc.weaponsOwnedByCharacter.Add(GameData.pc.equipedWeapon);
                GameData.pc.equipedWeapon = null;
                weaponImage.sprite = weaponEmpty;
                isi.itemName = null;
                DisplayWeaponInfo(null);
            }
            
        }
        else if (itemType.Equals("armor"))
        {
            if (GameData.pc.equipedArmor != null) {
                GameData.pc.armorsOwnedByCharacter.Add(GameData.pc.equipedArmor);
                GameData.pc.equipedArmor = null;
                armorImage.sprite = armorEmpty;
                isi.itemName = null;
                DisplayArmorInfo(null);
            }
        }
        else if (itemType.Equals("accessory"))
        {
            if (GameData.pc.equipedAccessory != null) {
                GameData.pc.accessoriesOwnedByCharacter.Add(GameData.pc.equipedAccessory);
                GameData.pc.equipedAccessory = null;
                accessoryImage.sprite = accessoryEmpty;
                isi.itemName = null;
                DisplayAccessoryInfo(null);
            }
            
        }
        LoadItemData();
        ChangeCharacterStatus();
        DisplayCurStatus();
    }

    public void ConfirmSell() {
        GameData.pc.characterMoney += int.Parse(priceText.text);
        foreach (KeyValuePair<int, Item> item in selectedItemsToSell)
        {
            GameData.pc.weaponsOwnedByCharacter.Remove(item.Value.englishName);
            GameData.pc.armorsOwnedByCharacter.Remove(item.Value.englishName);
            GameData.pc.accessoriesOwnedByCharacter.Remove(item.Value.englishName);
        }
        LoadItemData();
        selectedItemsToSell.Clear();
        PreviewPrice();
        ChangeMode();
		UpdateCharacterMoney (characterMoneyText);
        SaveLoad.Save();
    }

    public void CancelSell() {
        selectedItemsToSell.Clear();
        PreviewPrice();
        ChangeMode();
    }

    //
	public void DeckClicked(){
        Ray2D ray = new Ray2D(Input.mousePosition, Vector2.zero);
		RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction);

        if(hit)
        {
            hitObj = hit.collider.gameObject;

            if (hitObj.transform.FindChild("Deck_Display/ON").gameObject.activeSelf == true)
            {
                GameData.pc.selectedTileDeckIndex = int.Parse(hitObj.name.Split('_')[1]) - 1; //선택된 타일덱 번호 설정해줌 
                Application.LoadLevel("4-2. TILEDECK");
            }
            else
                deckBuyPanel.SetActive(true);
        }
        
	}

    public void OnDeckSelectedCancel()
    {
        deckBuyPanel.SetActive(false);
    }

    public void OnDeckSelectedBuy()
    {
        int selectedDeckNum = int.Parse(hitObj.name.Split('_')[1]);
        hitObj.transform.FindChild("Deck_Display/ON").gameObject.SetActive(true);
        hitObj.transform.FindChild("Deck_Display/ON").GetComponent<Image>().color = new Color32(255,255,255,255);
        hitObj.transform.FindChild("Deck_Display/LOCK").gameObject.SetActive(false);
        deckBuyPanel.SetActive(false);
        GameData.pc.TileDeckUnLock(selectedDeckNum);
        SaveLoad.Save();
    }

    //
    protected void DisplayWeaponInfo(Weapon w)
    {
        if (w == null)
        {
            weaponNameText.text = null;
            weaponDescText.text = null;
        }
        else
        {
            weaponNameText.text = w.koreanName;
            weaponDescText.text = "Atk +";
            weaponDescText.text += w.damage.ToString();


            switch (w.weaponEA1)
            {
                case ElementalAttribute.FIRE:
                    weaponDescText.text += " / ";
                    weaponDescText.text += ChangeStringColor("Fire", "red");
                    break;
                case ElementalAttribute.WATER:
                    weaponDescText.text += " / ";
                    weaponDescText.text += ChangeStringColor("Water", "#87CEEB");
                    break;
                case ElementalAttribute.WIND:
                    weaponDescText.text += " / ";
                    weaponDescText.text += ChangeStringColor("Wind", "green");
                    break;
                case ElementalAttribute.EARTH:
                    weaponDescText.text += " / ";
                    weaponDescText.text += ChangeStringColor("Earth", "yellow");
                    break;
            }



            switch (w.weaponEA2)
            {
                case ElementalAttribute.FIRE:
                    weaponDescText.text += " / ";
                    weaponDescText.text += ChangeStringColor("Fire", "red");
                    break;
                case ElementalAttribute.WATER:
                    weaponDescText.text += " / ";
                    weaponDescText.text += ChangeStringColor("Water", "#87CEEB");
                    break;
                case ElementalAttribute.WIND:
                    weaponDescText.text += " / ";
                    weaponDescText.text += ChangeStringColor("Wind", "green");
                    break;
                case ElementalAttribute.EARTH:
                    weaponDescText.text += " / ";
                    weaponDescText.text += ChangeStringColor("Earth", "yellow");
                    break;
            }
        }
    }

    protected void DisplayArmorInfo(Armor ar)
    {
        if (ar == null)
        {
            armorNameText.text = null;
            armorDescText.text = null;
        }
        else
        {
            armorNameText.text = ar.koreanName;
            armorDescText.text = "HP +";
            armorDescText.text += ar.HP.ToString();
            armorDescText.text += " / ";
            armorDescText.text += "Def +";
            armorDescText.text += ar.defense.ToString();


            switch (ar.elementalReg1Attribute)
            {
                case ElementalAttribute.FIRE:
                    armorDescText.text += " / ";
                    armorDescText.text += ChangeStringColor(ar.elementalReg1Percent.ToString() + "%", "red");
                    break;
                case ElementalAttribute.WATER:
                    armorDescText.text += " / ";
                    armorDescText.text += ChangeStringColor(ar.elementalReg1Percent.ToString() + "%", "#87CEEB");
                    break;
                case ElementalAttribute.WIND:
                    armorDescText.text += " / ";
                    armorDescText.text += ChangeStringColor(ar.elementalReg1Percent.ToString() + "%", "green");
                    break;
                case ElementalAttribute.EARTH:
                    armorDescText.text += " / ";
                    armorDescText.text += ChangeStringColor(ar.elementalReg1Percent.ToString() + "%", "yellow");
                    break;
            }
            switch (ar.elementalReg2Attribute)
            {
                case ElementalAttribute.FIRE:
                    armorDescText.text += " / ";
                    armorDescText.text += ChangeStringColor(ar.elementalReg2Percent.ToString() + "%", "red");
                    break;
                case ElementalAttribute.WATER:
                    armorDescText.text += " / ";
                    armorDescText.text += ChangeStringColor(ar.elementalReg2Percent.ToString() + "%", "#87CEEB");
                    break;
                case ElementalAttribute.WIND:
                    armorDescText.text += " / ";
                    armorDescText.text += ChangeStringColor(ar.elementalReg2Percent.ToString() + "%", "green");
                    break;
                case ElementalAttribute.EARTH:
                    armorDescText.text += " / ";
                    armorDescText.text += ChangeStringColor(ar.elementalReg2Percent.ToString() + "%", "yellow");
                    break;
            }
        }
    }

    protected void DisplayAccessoryInfo(Accessory ac)
    {
        if (ac == null)
        {
            accessoryNameText.text = null;
            accessoryDescText.text = null;
        }
        else
        {
            accessoryNameText.text = ac.koreanName;

            accessoryDescText.text = "";

            switch (ac.attackElementalAttribute)
            {
                case ElementalAttribute.FIRE:
                    accessoryDescText.text += ChangeStringColor("Fire", "red");
                    break;
                case ElementalAttribute.WATER:
                    accessoryDescText.text += ChangeStringColor("Water", "#87CEEB");
                    break;
                case ElementalAttribute.WIND:
                    accessoryDescText.text += ChangeStringColor("Wind", "green");
                    break;
                case ElementalAttribute.EARTH:
                    accessoryDescText.text += ChangeStringColor("Earth", "yellow");
                    break;
            }

            switch (ac.defenseRegAttribute)
            {
                case ElementalAttribute.FIRE:
                    accessoryDescText.text += " / ";
                    accessoryDescText.text += ChangeStringColor(ac.defenseRegPercent.ToString() + "%", "red");
                    break;
                case ElementalAttribute.WATER:
                    accessoryDescText.text += " / ";
                    accessoryDescText.text += ChangeStringColor(ac.defenseRegPercent.ToString() + "%", "#87CEEB");
                    break;
                case ElementalAttribute.WIND:
                    accessoryDescText.text += " / ";
                    accessoryDescText.text += ChangeStringColor(ac.defenseRegPercent.ToString() + "%", "green");
                    break;
                case ElementalAttribute.EARTH:
                    accessoryDescText.text += " / ";
                    accessoryDescText.text += ChangeStringColor(ac.defenseRegPercent.ToString() + "%", "yellow");
                    break;
            }
        }
    }


}
