using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class SHOPManager : CommonBehavior {
    public Text characterMoneyText;
    public Text playerName;

    //tab contents
    public GameObject[] tabs = new GameObject[3];
    public GameObject[] itemPanel = new GameObject[3];
    public List<GameObject> weaponSlots = new List<GameObject>();
    public List<GameObject> armorSlots = new List<GameObject>();
    public List<GameObject> accessorySlots = new List<GameObject>();

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

    //selected item
    public Weapon selectedWeapon;
    public Armor selectedArmor;
    public Accessory selectedAccessory;

    //cost
    public Text priceText;

    //status
    //hp
    public Slider HP_Guage_bottom;
    public Slider HP_Guage_middle;
    public Slider HP_Guage_top;
    public Text hpValueText;
    //mp
    public Slider MP_Guage_bottom;
    public Slider MP_Guage_middle;
    public Slider MP_Guage_top;
    public Text mpValueText;
    //atk
    public Slider ATK_Guage_bottom;
    public Slider ATK_Guage_middle;
    public Slider ATK_Guage_top;
    public Text atkValueText;

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

    //confirm pop-up
    public GameObject confirmPopup;
    public Image confirm_weapon_img;
    public Text confirm_weapon_name;
    public Text confirm_weapon_desc;
    public Image confirm_armor_img;
    public Text confirm_armor_name;
    public Text confirm_armor_desc;
    public Image confirm_accessory_img;
    public Text confirm_accessory_name;
    public Text confirm_accessory_desc;

    //
    public Image playerPortraitImg;


    void Start() {
        UpdateCharacterMoney(characterMoneyText);
        DisplayPlayerName(playerName, playerPortraitImg);
        LoadItemData();
        selectedWeapon = null;
        selectedArmor = null;
        selectedAccessory = null;
        SelectTab(0);
        DisplayCurEquipment();
        DisplayCurStatus();

        if (Game.tutorialIng == true)
        {
            GameData.FindTutorial();
            GameData.curTutorialManager.Stage1Tutorial();
        }
    }

    void LoadItemData() {
        //장신구를 로드하는 함수
        //weapon
        for (int i = 0; i < GameData.weapons.Count; i++) {
            weaponSlots[i].GetComponent<Image>().sprite = GameData.weapons[i].icon;
            weaponSlots[i].GetComponent<ItemSlotInfo>().SetValues("weapon", GameData.weapons[i].englishName);
        }
        //armor
        for (int i = 0; i < GameData.armors.Count; i++) {
            armorSlots[i].GetComponent<Image>().sprite = GameData.armors[i].icon;
            armorSlots[i].GetComponent<ItemSlotInfo>().SetValues("armor", GameData.armors[i].englishName);
        }
        //accessory
        for (int i = 0; i < GameData.accessories.Count; i++) {
            accessorySlots[i].GetComponent<Image>().sprite = GameData.accessories[i].icon;
            accessorySlots[i].GetComponent<ItemSlotInfo>().SetValues("accessory", GameData.accessories[i].englishName);
        }
    }

    void DisplayCurEquipment() {
        //현재 장비하고 있는 장비를 보여주는 함수
        if (GameData.pc.equipedWeapon == null)
        {
            weaponImage.sprite = weaponEmpty;
            DisplayWeaponInfo(null, weaponNameText, weaponDescText);
        }
        else
        {
            Weapon w = GameData.GetWeaponData(GameData.pc.equipedWeapon);
            weaponImage.sprite = w.icon;
            weaponImage.GetComponent<ItemSlotInfo>().itemName = w.englishName;

            DisplayWeaponInfo(w, weaponNameText, weaponDescText);
        }

        if (GameData.pc.equipedArmor == null)
        {
            armorImage.sprite = armorEmpty;
            DisplayArmorInfo(null, armorNameText, armorDescText);
        }
        else
        {
            Armor ar = GameData.GetArmorData(GameData.pc.equipedArmor);
            armorImage.sprite = ar.icon;
            armorImage.GetComponent<ItemSlotInfo>().itemName = ar.englishName;
            DisplayArmorInfo(ar, armorNameText, armorDescText);
        }
          
        if (GameData.pc.equipedAccessory == null)
        {
            accessoryImage.sprite = accessoryEmpty;
            DisplayAccessoryInfo(null, accessoryNameText, accessoryDescText);
        }
        else
        {
            Accessory ac = GameData.GetAccessoryData(GameData.pc.equipedAccessory);
            accessoryImage.sprite = ac.icon;
            accessoryImage.GetComponent<ItemSlotInfo>().itemName = ac.englishName;

            DisplayAccessoryInfo(ac, accessoryNameText, accessoryDescText);
        }
    }

    void DisplayCurStatus() {
        //현재 캐릭터의 스테이터스를 보여주는 함수. 장비를 착용함에 따라 바뀐다.
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

        //value
        hpValueText.text = curHP.ToString();
        mpValueText.text = curMP.ToString();
        atkValueText.text = curATK.ToString();
        defValueText.text = curDEF.ToString();
    }

    void DisplayChangedStatus() {
        //Status가 바뀔 때 마다 호출되는 함수
        DisplayCurStatus();

        //모두 inactive로 만들기
        Img_ATK_fire.sprite = Sprite_fire_inactive;
        Img_ATK_water.sprite = Sprite_water_inactive;
        Img_ATK_earth.sprite = Sprite_earth_inactive;
        Img_ATK_wind.sprite = Sprite_wind_inactive;

        float tempHP = GameData.baseHP;
        float tempMP = GameData.baseMP;
        float tempATK = GameData.baseATK;
        float tempDEF = GameData.baseDEF;
        float tempRegFire = GameData.baseREG_FIRE;
        float tempRegWater = GameData.baseREG_WATER;
        float tempRegEarth = GameData.baseREG_EARTH;
        float tempRegWind = GameData.baseREG_WIND;

        Weapon tempWeapon;
        Armor tempArmor;
        Accessory tempAccessory;

        //weapon
        if (selectedWeapon != null)
        {
            tempWeapon = selectedWeapon;
        }
        else {
            tempWeapon = GameData.GetWeaponData(GameData.pc.equipedWeapon);
        }

        if (tempWeapon != null) {
            switch (tempWeapon.weaponEA1)
            {
                case ElementalAttribute.FIRE:
                    Img_ATK_fire.sprite = Sprite_fire_active;
                    break;
                case ElementalAttribute.WATER:
                    Img_ATK_water.sprite = Sprite_water_active;
                    break;
                case ElementalAttribute.WIND:
                    Img_ATK_wind.sprite = Sprite_wind_active;
                    break;
                case ElementalAttribute.EARTH:
                    Img_ATK_earth.sprite = Sprite_earth_active;
                    break;
            }

            switch (tempWeapon.weaponEA2)
            {
                case ElementalAttribute.FIRE:
                    Img_ATK_fire.sprite = Sprite_fire_active;
                    break;
                case ElementalAttribute.WATER:
                    Img_ATK_water.sprite = Sprite_water_active;
                    break;
                case ElementalAttribute.WIND:
                    Img_ATK_wind.sprite = Sprite_wind_active;
                    break;
                case ElementalAttribute.EARTH:
                    Img_ATK_earth.sprite = Sprite_earth_active;
                    break;
            }

            tempATK += tempWeapon.damage;
        }
        

        //armor
        if (selectedArmor != null)
        {
            tempArmor = selectedArmor;
        }
        else {
            tempArmor = GameData.GetArmorData(GameData.pc.equipedArmor);
        }

        if (tempArmor != null) {
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
        if (selectedAccessory != null)
        {
            tempAccessory = selectedAccessory;
        }
        else {
            tempAccessory = GameData.GetAccessoryData(GameData.pc.equipedAccessory);
        }

        if (tempAccessory != null) {
            switch (tempAccessory.attackElementalAttribute)
            {
                case ElementalAttribute.FIRE:
                    Img_ATK_fire.sprite = Sprite_fire_active;
                    break;
                case ElementalAttribute.WATER:
                    Img_ATK_water.sprite = Sprite_water_active;
                    break;
                case ElementalAttribute.WIND:
                    Img_ATK_wind.sprite = Sprite_wind_active;
                    break;
                case ElementalAttribute.EARTH:
                    Img_ATK_earth.sprite = Sprite_earth_active;
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

        hpValueText.text = tempHP.ToString();
        mpValueText.text = tempMP.ToString();
        atkValueText.text = tempATK.ToString();
        defValueText.text = tempDEF.ToString();

        //hp
        if (tempHP > curHP)
        {
            HP_Guage_bottom.value = tempHP / GameData.maxHP;
            hpValueText.text = ChangeStringColor(hpValueText.text, "#87CEEB");
        }
        else if (tempHP == curHP)
        {

        }
        else
        {
            HP_Guage_bottom.value = tempHP / GameData.maxHP;
            HP_Guage_top.value = HP_Guage_bottom.value;
            hpValueText.text = ChangeStringColor(hpValueText.text, "red");
        }

        //mp
        if (tempMP > curMP)
        {
            MP_Guage_bottom.value = tempMP / GameData.maxMP;
            mpValueText.text = ChangeStringColor(mpValueText.text, "#87CEEB");
        }
        else if (tempMP == curMP)
        {

        }
        else
        {
            MP_Guage_bottom.value = tempMP / GameData.maxMP;
            MP_Guage_top.value = MP_Guage_bottom.value;
            mpValueText.text = ChangeStringColor(mpValueText.text, "red");
        }

        //atk
        if (tempATK > curATK)
        {
            ATK_Guage_bottom.value = tempATK / GameData.maxATK;
            atkValueText.text = ChangeStringColor(atkValueText.text, "#87CEEB");
        }
        else if (tempATK == curATK)
        {

        }
        else
        {
            ATK_Guage_bottom.value = tempATK / GameData.maxATK;
            ATK_Guage_top.value = ATK_Guage_bottom.value;
            atkValueText.text = ChangeStringColor(atkValueText.text, "red");
        }

        //def
        if (tempDEF > curDEF)
        {
            Def_Guage_bottom.value = tempDEF / GameData.maxDEF;
            defValueText.text = ChangeStringColor(defValueText.text, "#87CEEB");
        }
        else if (tempDEF == curDEF)
        {

        }
        else
        {
            Def_Guage_bottom.value = tempDEF / GameData.maxDEF;
            Def_Guage_top.value = Def_Guage_bottom.value;
            defValueText.text = ChangeStringColor(defValueText.text, "red");
        }

        Text_def_fire_10.text = tempRegFire.ToString() + "%";
        Text_def_water_10.text = tempRegWater.ToString() + "%";
        Text_def_earth_10.text = tempRegEarth.ToString() + "%";
        Text_def_wind_10.text = tempRegWind.ToString() + "%";
    }

    public void SelectTab(int idx) {
        //Tab을 활성화 시킬 것인지 선택하는 함수
        for (int i = 0; i < tabs.Length; i++) {
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

    void SetTab(int idx, bool b) {
        //Tab 활성화시키는 함수
        itemPanel[idx].SetActive(b);
        tabs[idx].transform.FindChild("Active").gameObject.SetActive(b);
    }

    public void PreviewItem(GameObject go) {
        //선택한 아이템을 장착해서 미리 볼 수 있음
        string itemType = go.GetComponent<ItemSlotInfo>().itemType;
        string itemName = go.GetComponent<ItemSlotInfo>().itemName;

        if (itemType.Equals("weapon"))
        {
            selectedWeapon = GameData.GetWeaponData(itemName);
            weaponImage.sprite = selectedWeapon.icon;
            DisplayWeaponInfo(selectedWeapon, weaponNameText, weaponDescText);
        }
        else if (itemType.Equals("armor"))
        {
            selectedArmor = GameData.GetArmorData(itemName);
            armorImage.sprite = selectedArmor.icon;
            DisplayArmorInfo(selectedArmor, armorNameText, armorDescText);
        }
        else if (itemType.Equals("accessory"))
        {
            selectedAccessory = GameData.GetAccessoryData(itemName);
            accessoryImage.sprite = selectedAccessory.icon;
            DisplayAccessoryInfo(selectedAccessory, accessoryNameText, accessoryDescText);
        }

        PreviewPrice();
        DisplayChangedStatus();
    }

    void PreviewPrice() {
        // 내가 고른 아이템들의 가격의 합을 계산하는 함수
        int price = 0;
        if(selectedWeapon != null)
            price += selectedWeapon.price;
        if(selectedArmor != null)
            price += selectedArmor.price;
        if(selectedAccessory != null)
            price += selectedAccessory.price;

        priceText.text = price.ToString();
    }

    public void UnequipItem(GameObject go) {
        //아이템 장착 해제하는 함수
        string itemType = go.GetComponent<ItemSlotInfo>().itemType;
        string itemName = go.GetComponent<ItemSlotInfo>().itemName;

        if (itemType.Equals("weapon"))
        {
            selectedWeapon = null;
            if (GameData.pc.equipedWeapon == null)
            {
                weaponImage.sprite = weaponEmpty;
                DisplayWeaponInfo(null, weaponNameText, weaponDescText);
            }
            else {
                weaponImage.sprite = GameData.GetWeaponData(itemName).icon;
                DisplayWeaponInfo(GameData.GetWeaponData(itemName), weaponNameText, weaponDescText);
            }
            
        }
        else if (itemType.Equals("armor"))
        {
            selectedArmor = null;
            
            if (GameData.pc.equipedArmor == null)
            {
                armorImage.sprite = armorEmpty;
                DisplayArmorInfo(null, armorNameText, armorDescText);
            }
            else
            {
                armorImage.sprite = GameData.GetArmorData(itemName).icon;
                DisplayArmorInfo(GameData.GetArmorData(itemName), armorNameText, armorDescText);
            }
        }
        else if (itemType.Equals("accessory"))
        {
            selectedAccessory = null;
      
            if (GameData.pc.equipedAccessory == null)
            {
                accessoryImage.sprite = accessoryEmpty;
                DisplayAccessoryInfo(null, accessoryNameText, accessoryDescText);
            }
            else
            {
                accessoryImage.sprite = GameData.GetAccessoryData(itemName).icon;
                DisplayAccessoryInfo(GameData.GetAccessoryData(itemName), accessoryNameText, accessoryDescText);
            }
        }

        PreviewPrice();
        DisplayChangedStatus();
    }
    

    public void BuyItem() {
        //여태까지 고른 아이템을 구매하는 함수
        int totalCost = int.Parse(priceText.text);

        if (GameData.pc.characterMoney >= totalCost)
        {
            if (selectedWeapon != null)
            {
                GameData.pc.weaponsOwnedByCharacter.Add(selectedWeapon.englishName);
            }

            if (selectedArmor != null)
            {
                GameData.pc.armorsOwnedByCharacter.Add(selectedArmor.englishName);
            }

            if (selectedAccessory != null)
            {
                GameData.pc.accessoriesOwnedByCharacter.Add(selectedAccessory.englishName);
            }

            GameData.pc.characterMoney -= totalCost;
            UpdateCharacterMoney(characterMoneyText);
            SaveLoad.Save();
        }

        selectedWeapon = null;
        selectedArmor = null;
        selectedAccessory = null;
        DisplayCurEquipment();
        DisplayCurStatus();
        PreviewPrice();
    }

    void DisplayWeaponInfo(Weapon w, Text name, Text desc)
        //장비창에 무기를 착용했을 때 정보가 업데이트되는 함수
    {
        if (w == null)
        {
            name.text = null;
            desc.text = null;
        }
        else
        {
            name.text = w.koreanName;
            desc.text = "Atk +";
            desc.text += w.damage.ToString();


            switch (w.weaponEA1)
            {
                case ElementalAttribute.FIRE:
                    desc.text += " / ";
                    desc.text += ChangeStringColor("Fire", "red");
                    break;
                case ElementalAttribute.WATER:
                    desc.text += " / ";
                    desc.text += ChangeStringColor("Water", "#87CEEB");
                    break;
                case ElementalAttribute.WIND:
                    desc.text += " / ";
                    desc.text += ChangeStringColor("Wind", "green");
                    break;
                case ElementalAttribute.EARTH:
                    desc.text += " / ";
                    desc.text += ChangeStringColor("Earth", "yellow");
                    break;
            }



            switch (w.weaponEA2)
            {
                case ElementalAttribute.FIRE:
                    desc.text += " / ";
                    desc.text += ChangeStringColor("Fire", "red");
                    break;
                case ElementalAttribute.WATER:
                    desc.text += " / ";
                    desc.text += ChangeStringColor("Water", "#87CEEB");
                    break;
                case ElementalAttribute.WIND:
                    desc.text += " / ";
                    desc.text += ChangeStringColor("Wind", "green");
                    break;
                case ElementalAttribute.EARTH:
                    desc.text += " / ";
                    desc.text += ChangeStringColor("Earth", "yellow");
                    break;
            }
        }
    }

    void DisplayArmorInfo(Armor ar, Text name, Text desc)
        //장비창에 방어구를 착용했을 때 정보가 업데이트 되는 함수
    {
        if (ar == null)
        {
            name.text = null;
            desc.text = null;
        }
        else
        {
            name.text = ar.koreanName;
            desc.text = "HP +";
            desc.text += ar.HP.ToString();
            desc.text += " / ";
            desc.text += "Def +";
            desc.text += ar.defense.ToString();


            switch (ar.elementalReg1Attribute)
            {
                case ElementalAttribute.FIRE:
                    desc.text += " / ";
                    desc.text += ChangeStringColor(ar.elementalReg1Percent.ToString() + "%", "red");
                    break;
                case ElementalAttribute.WATER:
                    desc.text += " / ";
                    desc.text += ChangeStringColor(ar.elementalReg1Percent.ToString() + "%", "#87CEEB");
                    break;
                case ElementalAttribute.WIND:
                    desc.text += " / ";
                    desc.text += ChangeStringColor(ar.elementalReg1Percent.ToString() + "%", "green");
                    break;
                case ElementalAttribute.EARTH:
                    desc.text += " / ";
                    desc.text += ChangeStringColor(ar.elementalReg1Percent.ToString() + "%", "yellow");
                    break;
            }
            switch (ar.elementalReg2Attribute)
            {
                case ElementalAttribute.FIRE:
                    desc.text += " / ";
                    desc.text += ChangeStringColor(ar.elementalReg2Percent.ToString() + "%", "red");
                    break;
                case ElementalAttribute.WATER:
                    desc.text += " / ";
                    desc.text += ChangeStringColor(ar.elementalReg2Percent.ToString() + "%", "#87CEEB");
                    break;
                case ElementalAttribute.WIND:
                    desc.text += " / ";
                    desc.text += ChangeStringColor(ar.elementalReg2Percent.ToString() + "%", "green");
                    break;
                case ElementalAttribute.EARTH:
                    desc.text += " / ";
                    desc.text += ChangeStringColor(ar.elementalReg2Percent.ToString() + "%", "yellow");
                    break;
            }
        }
    }

    void DisplayAccessoryInfo(Accessory ac, Text name, Text desc)
    //장비창에 장신구를 착용했을 때 정보가 업데이트 되는 함수
    {
        if (ac == null)
        {
            name.text = null;
            desc.text = null;
        }
        else
        {
            name.text = ac.koreanName;

            desc.text = "";

            switch (ac.attackElementalAttribute)
            {
                case ElementalAttribute.FIRE:
                    desc.text += ChangeStringColor("Fire", "red");
                    break;
                case ElementalAttribute.WATER:
                    desc.text += ChangeStringColor("Water", "#87CEEB");
                    break;
                case ElementalAttribute.WIND:
                    desc.text += ChangeStringColor("Wind", "green");
                    break;
                case ElementalAttribute.EARTH:
                    desc.text += ChangeStringColor("Earth", "yellow");
                    break;
            }

            switch (ac.defenseRegAttribute)
            {
                case ElementalAttribute.FIRE:
                    desc.text += " / ";
                    desc.text += ChangeStringColor(ac.defenseRegPercent.ToString() + "%", "red");
                    break;
                case ElementalAttribute.WATER:
                    desc.text += " / ";
                    desc.text += ChangeStringColor(ac.defenseRegPercent.ToString() + "%", "#87CEEB");
                    break;
                case ElementalAttribute.WIND:
                    desc.text += " / ";
                    desc.text += ChangeStringColor(ac.defenseRegPercent.ToString() + "%", "green");
                    break;
                case ElementalAttribute.EARTH:
                    desc.text += " / ";
                    desc.text += ChangeStringColor(ac.defenseRegPercent.ToString() + "%", "yellow");
                    break;
            }
        }
    }

    public void ShowConfirmPopup() {
        // 구매할 물품을 최종적으로 확인하는 popup창을 띄우는 함수
        if (selectedWeapon == null)
        {
            confirm_weapon_img.sprite = weaponEmpty;
        }
        else {
            confirm_weapon_img.sprite = selectedWeapon.icon;
        }

        if (selectedArmor == null) {
            confirm_armor_img.sprite = armorEmpty;
        }
        else
        {
            confirm_armor_img.sprite = selectedArmor.icon; 
        }

        if (selectedAccessory == null) {
            confirm_accessory_img.sprite = accessoryEmpty;
        }
        else
        {
            confirm_accessory_img.sprite = selectedAccessory.icon;
        }
        DisplayWeaponInfo(selectedWeapon, confirm_weapon_name, confirm_weapon_desc);
        DisplayArmorInfo(selectedArmor, confirm_armor_name, confirm_armor_desc);
        DisplayAccessoryInfo(selectedAccessory, confirm_accessory_name, confirm_accessory_desc);

    }
}
