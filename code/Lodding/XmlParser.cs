using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System;

public class XmlParser : CommonBehavior
{
    float progress = 0;
    float length = 1430;
    public GameObject loadingBar;
    public Text loadingText;
    bool isDataloaded = false;
    

	public GameObject sm;
	public GameObject bm;
	public GameObject lc;
	
	
	void Awake(){
		DontDestroyOnLoad (bm);
		DontDestroyOnLoad (sm);
		DontDestroyOnLoad (lc);
	}

    void Start() {
        StartCoroutine(LoadAllData());
    }
    
    public IEnumerator TileInfoLoad()
    {
        TextAsset asset = Resources.Load("XmlFiles/tileInfo") as TextAsset;
        XmlDocument doc = new XmlDocument();
        doc.LoadXml(asset.text);

        XmlNodeList nodes = doc.SelectNodes("/tile/content");

        foreach (XmlNode node in nodes)
        {
            Tile tile = new Tile();
            tile.koreanName = node.SelectSingleNode("name/koreanName").InnerText;
            tile.englishName = node.SelectSingleNode("name/englishName").InnerText;

            tile.tier = int.Parse(node.SelectSingleNode("tier").InnerText);

            string str = node.SelectSingleNode("elementalAttribute").InnerText;
            switch (str)
            {
                case "0":
                    tile.tileEA = ElementalAttribute.NONE;
                    break;
                case "fire":
                    tile.tileEA = ElementalAttribute.FIRE;
                    break;
                case "water":
                    tile.tileEA = ElementalAttribute.WATER;
                    break;
                case "earth":
                    tile.tileEA = ElementalAttribute.EARTH;
                    break;
                case "wind":
                    tile.tileEA = ElementalAttribute.WIND;
                    break;
            }

            //effect
            tile.instantDamage = float.Parse(node.SelectSingleNode("effect/damage/instantDamage").InnerText);

            tile.dotDamageDuration = float.Parse(node.SelectSingleNode("effect/damage/dotDamage/dotDamageduration").InnerText);
            tile.dotDamagePerTick = float.Parse(node.SelectSingleNode("effect/damage/dotDamage/dotDamagePerTick").InnerText);

            tile.slownessDuration = float.Parse(node.SelectSingleNode("effect/slowness/slownessDuration").InnerText);
            tile.slownessDegree = float.Parse(node.SelectSingleNode("effect/slowness/slownessDegree").InnerText);

            tile.stunTime = float.Parse(node.SelectSingleNode("effect/stunTime").InnerText);

            tile.knockbackDistance = int.Parse(node.SelectSingleNode("effect/knockbackDistance").InnerText);

            tile.koreanExplanation = node.SelectSingleNode("explanation/koreanExplanation").InnerText;
            tile.englishExplanation = node.SelectSingleNode("explanation/englishExplanation").InnerText;

            tile.tileCard = Resources.Load<Sprite>("Card_Imgs/" + tile.englishName);
            tile.tileIcon = Resources.Load<Sprite>("Tile_Imgs/" + tile.englishName);
            tile.tileEffect = Resources.Load<GameObject>("Tile_Effects/"+tile.englishName);
            tile.tex = Resources.Load<Texture>("Tile_Textures/" + tile.englishName);
            tile.mana = int.Parse(node.SelectSingleNode("mana").InnerText);

            GameData.tiles.Add(tile);
            GameData.tilesByTier[tile.tier - 1].Add(tile);

        }
        yield return null;
    }

    public IEnumerator WeaponInfoLoad()
    {
        TextAsset asset = Resources.Load("XmlFiles/weaponInfo") as TextAsset;
        XmlDocument doc = new XmlDocument();
        doc.LoadXml(asset.text);

        XmlNodeList nodes = doc.SelectNodes("/weapon/content");

        foreach (XmlNode node in nodes)
        {
            Weapon weapon = new Weapon();

            weapon.koreanName = node.SelectSingleNode("name/koreanName").InnerText;
            weapon.englishName = node.SelectSingleNode("name/englishName").InnerText;

            string str1 = node.SelectSingleNode("elementalAttribute1").InnerText;
            switch (str1)
            {
                case "0":
                    weapon.weaponEA1 = ElementalAttribute.NONE;
                    break;
                case "fire":
                    weapon.weaponEA1 = ElementalAttribute.FIRE;
                    break;
                case "water":
                    weapon.weaponEA1 = ElementalAttribute.WATER;
                    break;
                case "earth":
                    weapon.weaponEA1 = ElementalAttribute.EARTH;
                    break;
                case "wind":
                    weapon.weaponEA1 = ElementalAttribute.WIND;
                    break;
            }

            string str2 = node.SelectSingleNode("elementalAttribute2").InnerText;
            switch (str2)
            {
                case "0":
                    weapon.weaponEA2 = ElementalAttribute.NONE;
                    break;
                case "fire":
                    weapon.weaponEA2 = ElementalAttribute.FIRE;
                    break;
                case "water":
                    weapon.weaponEA2 = ElementalAttribute.WATER;
                    break;
                case "earth":
                    weapon.weaponEA2 = ElementalAttribute.EARTH;
                    break;
                case "wind":
                    weapon.weaponEA2 = ElementalAttribute.WIND;
                    break;
            }

            weapon.damage = int.Parse(node.SelectSingleNode("damage").InnerText);
            weapon.price = int.Parse(node.SelectSingleNode("price").InnerText);
            //weapon.koreanExplanation = node.SelectSingleNode("explanation/koreanExplanation").InnerText;
            //weapon.englishExplanation = node.SelectSingleNode("explanation/englishExplanation").InnerText;
            weapon.icon = Resources.Load<Sprite>("Item_Imgs/weapon/" + weapon.englishName);
            //Debug.Log(weapon.weaponIcon);

            GameData.weapons.Add(weapon);
            yield return null;
        }

        
    }

    public IEnumerator ArmorInfoLoad()
    {
        TextAsset asset = Resources.Load("XmlFiles/armorInfo") as TextAsset;
        XmlDocument doc = new XmlDocument();
        doc.LoadXml(asset.text);

        XmlNodeList nodes = doc.SelectNodes("/armor/content");

        foreach (XmlNode node in nodes)
        {
            Armor armor = new Armor();

            armor.koreanName = node.SelectSingleNode("name/koreanName").InnerText;
            armor.englishName = node.SelectSingleNode("name/englishName").InnerText;
            armor.defense = int.Parse(node.SelectSingleNode("defense").InnerText);
            armor.HP = int.Parse(node.SelectSingleNode("HP").InnerText);

            string str1 = node.SelectSingleNode("elementalReg1/elementalReg1Attribute").InnerText;
            switch (str1)
            {
                case "0":
                    armor.elementalReg1Attribute = ElementalAttribute.NONE;
                    break;
                case "fire":
                    armor.elementalReg1Attribute = ElementalAttribute.FIRE;
                    break;
                case "water":
                    armor.elementalReg1Attribute = ElementalAttribute.WATER;
                    break;
                case "earth":
                    armor.elementalReg1Attribute = ElementalAttribute.EARTH;
                    break;
                case "wind":
                    armor.elementalReg1Attribute = ElementalAttribute.WIND;
                    break;
            }
            armor.elementalReg1Percent = int.Parse(node.SelectSingleNode("elementalReg1/elementalReg1Percent").InnerText);

            string str2 = node.SelectSingleNode("elementalReg2/elementalReg2Attribute").InnerText;
            switch (str2)
            {
                case "0":
                    armor.elementalReg2Attribute = ElementalAttribute.NONE;
                    break;
                case "fire":
                    armor.elementalReg2Attribute = ElementalAttribute.FIRE;
                    break;
                case "water":
                    armor.elementalReg2Attribute = ElementalAttribute.WATER;
                    break;
                case "earth":
                    armor.elementalReg2Attribute = ElementalAttribute.EARTH;
                    break;
                case "wind":
                    armor.elementalReg2Attribute = ElementalAttribute.WIND;
                    break;
            }

            armor.elementalReg2Percent = int.Parse(node.SelectSingleNode("elementalReg2/elementalReg2Percent").InnerText);

            armor.price = int.Parse(node.SelectSingleNode("price").InnerText);
            //armor.koreanExplanation = node.SelectSingleNode("explanation/koreanExplanation").InnerText;
            //armor.englishExplanation = node.SelectSingleNode("explanation/englishExplanation").InnerText;
            armor.icon = Resources.Load<Sprite>("Item_Imgs/armor/" + armor.englishName);
            //Debug.Log(armor.armorIcon);

            GameData.armors.Add(armor);
            yield return null;
        }

        
    }

    public IEnumerator AccessoryInfoLoad()
    {
        TextAsset asset = Resources.Load("XmlFiles/accessoryInfo") as TextAsset;
        XmlDocument doc = new XmlDocument();
        doc.LoadXml(asset.text);

        XmlNodeList nodes = doc.SelectNodes("/accessory/content");

        foreach (XmlNode node in nodes)
        {
            Accessory accessory = new Accessory();

            accessory.koreanName = node.SelectSingleNode("name/koreanName").InnerText;
            accessory.englishName = node.SelectSingleNode("name/englishName").InnerText;
            accessory.movementSpeed = float.Parse(node.SelectSingleNode("movementSpeed").InnerText);

            string str1 = node.SelectSingleNode("attackElementalAttribute").InnerText;
            switch (str1)
            {
                case "0":
                    accessory.attackElementalAttribute = ElementalAttribute.NONE;
                    break;
                case "fire":
                    accessory.attackElementalAttribute = ElementalAttribute.FIRE;
                    break;
                case "water":
                    accessory.attackElementalAttribute = ElementalAttribute.WATER;
                    break;
                case "earth":
                    accessory.attackElementalAttribute = ElementalAttribute.EARTH;
                    break;
                case "wind":
                    accessory.attackElementalAttribute = ElementalAttribute.WIND;
                    break;
            }

            string str2 = node.SelectSingleNode("defenseReg/defenseRegAttribute").InnerText;
            switch (str2)
            {
                case "0":
                    accessory.defenseRegAttribute = ElementalAttribute.NONE;
                    break;
                case "fire":
                    accessory.defenseRegAttribute = ElementalAttribute.FIRE;
                    break;
                case "water":
                    accessory.defenseRegAttribute = ElementalAttribute.WATER;
                    break;
                case "earth":
                    accessory.defenseRegAttribute = ElementalAttribute.EARTH;
                    break;
                case "wind":
                    accessory.defenseRegAttribute = ElementalAttribute.WIND;
                    break;
            }

            accessory.defenseRegPercent = int.Parse(node.SelectSingleNode("defenseReg/defenseRegPercent").InnerText);
            accessory.price = int.Parse(node.SelectSingleNode("price").InnerText);
            //accessory.koreanExplanation = node.SelectSingleNode("explanation/koreanExplanation").InnerText;
            //accessory.englishExplanation = node.SelectSingleNode("explanation/englishExplanation").InnerText;
            accessory.icon = Resources.Load<Sprite>("Item_Imgs/accessory/" + accessory.englishName);
            //Debug.Log(accessory.accessoryIcon);

            GameData.accessories.Add(accessory);
            yield return null;
        }
    }

    public IEnumerator ItemLoad()
    {
        foreach (Weapon w in GameData.weapons)
        {
            GameData.items.Add(w);
            yield return null;
        }

        foreach (Armor a in GameData.armors)
        {
            GameData.items.Add(a);
            yield return null;
        }

        foreach (Accessory ac in GameData.accessories)
        {
            GameData.items.Add(ac);
            yield return null;
        }
    }

    public IEnumerator VarialbleInfoLoad()
    {
        TextAsset asset = Resources.Load("XmlFiles/variables") as TextAsset;
        XmlDocument doc = new XmlDocument();
        doc.LoadXml(asset.text);

        XmlNodeList nodes = doc.SelectNodes("/variables");

        foreach (XmlNode node in nodes)
        {

            GameData.baseMoney = int.Parse(node.SelectSingleNode("CharacterBaseMoney").InnerText);
            GameData.baseHP = float.Parse(node.SelectSingleNode("CharacterBaseHP").InnerText);
            GameData.baseMP = float.Parse(node.SelectSingleNode("CharacterBaseMP").InnerText);
            GameData.baseATK = float.Parse(node.SelectSingleNode("CharacterBaseATK").InnerText);
            GameData.baseATK_FIRE = bool.Parse(node.SelectSingleNode("CharacterBaseATK_FIRE").InnerText);
            GameData.baseATK_WATER = bool.Parse(node.SelectSingleNode("CharacterBaseATK_WATER").InnerText);
            GameData.baseATK_EARTH = bool.Parse(node.SelectSingleNode("CharacterBaseATK_EARTH").InnerText);
            GameData.baseATK_WIND = bool.Parse(node.SelectSingleNode("CharacterBaseATK_WIND").InnerText);
            GameData.baseDEF = float.Parse(node.SelectSingleNode("CharacterBaseDEF").InnerText);
            GameData.baseREG_FIRE = int.Parse(node.SelectSingleNode("CharacterBaseDEF_FIRE").InnerText);
            GameData.baseREG_WATER = int.Parse(node.SelectSingleNode("CharacterBaseDEF_WATER").InnerText);
            GameData.baseREG_EARTH = int.Parse(node.SelectSingleNode("CharacterBaseDEF_EARTH").InnerText);
            GameData.baseREG_WIND = int.Parse(node.SelectSingleNode("CharacterBaseDEF_WIND").InnerText);
            GameData.baseSpeed = float.Parse(node.SelectSingleNode("CharacterBaseSpeed").InnerText);
            GameData.maxHP = float.Parse(node.SelectSingleNode("MaxHP").InnerText);
            GameData.maxMP = float.Parse(node.SelectSingleNode("MaxMP").InnerText);
            GameData.maxATK = int.Parse(node.SelectSingleNode("MaxATK").InnerText);
            GameData.maxDEF = int.Parse(node.SelectSingleNode("MaxDEF").InnerText);
            GameData.bronzeBoxCost = int.Parse(node.SelectSingleNode("BronzeBox").InnerText);
            GameData.silverBoxCost = int.Parse(node.SelectSingleNode("SilverBox").InnerText);
            GameData.goldBoxCost = int.Parse(node.SelectSingleNode("GoldBox").InnerText);
            GameData.upgradeMax = int.Parse(node.SelectSingleNode("UpgradeMax").InnerText);
            GameData.upgradeHPCost = int.Parse(node.SelectSingleNode("UpgradeHP").InnerText);
            GameData.upgradeMPCost = int.Parse(node.SelectSingleNode("UpgradeMP").InnerText);
            GameData.upgradeATKCost = int.Parse(node.SelectSingleNode("UpgradeATK").InnerText);
            GameData.upgradeDEFCost = int.Parse(node.SelectSingleNode("UpgradeDEF").InnerText);

            GameData.HPIncreasePerUp = int.Parse(node.SelectSingleNode("HPIncreasePerUp").InnerText);
            GameData.MPIncreasePerUp = int.Parse(node.SelectSingleNode("MPIncreasePerUp").InnerText);
            GameData.ATKIncreasePerUp = float.Parse(node.SelectSingleNode("ATKIncreasePerUp").InnerText);
            GameData.DEFIncreasePerUp = float.Parse(node.SelectSingleNode("DEFIncreasePerUp").InnerText);
            
            GameData.initTileList = new List<TileInfo>();

            XmlNodeList initTiles = node.SelectNodes("InitTiles/Tile");
            foreach (XmlNode tile in initTiles)
            {
                GameData.initTileList.Add(new TileInfo(tile.SelectSingleNode("Name").InnerText, int.Parse(tile.SelectSingleNode("Quantity").InnerText)));
            }

            GameData.settingTime = float.Parse(node.SelectSingleNode("SettingTime").InnerText);
            yield return null;
        }
    }

    public IEnumerator ProbLoad()
    {
        TextAsset asset = Resources.Load("XmlFiles/tileboxProbs") as TextAsset;
        XmlDocument doc = new XmlDocument();
        doc.LoadXml(asset.text);

        XmlNodeList nodes = doc.SelectNodes("/probs");

        foreach (XmlNode node in nodes)
        {
            for (int i = 0; i < 5; i++)
            {
                GameData.probBronzeFix[i] = int.Parse(node.SelectSingleNode("bronze/fix/t" + (i + 1)).InnerText);
                GameData.probBronzeRnd[i] = int.Parse(node.SelectSingleNode("bronze/random/t" + (i + 1)).InnerText);

            }

            for (int i = 0; i < 5; i++)
            {
                GameData.probSilverFix[i] = int.Parse(node.SelectSingleNode("silver/fix/t" + (i + 1)).InnerText);
                GameData.probSilverRnd[i] = int.Parse(node.SelectSingleNode("silver/random/t" + (i + 1)).InnerText);

            }

            for (int i = 0; i < 5; i++)
            {
                GameData.probGoldFix[i] = int.Parse(node.SelectSingleNode("gold/fix/t" + (i + 1)).InnerText);
                GameData.probGoldRnd[i] = int.Parse(node.SelectSingleNode("gold/random/t" + (i + 1)).InnerText);
            }

            yield return null;
        }
    }

    public IEnumerator LoadFigureInfo()
    {
        TextAsset asset = Resources.Load("XmlFiles/figureInfo") as TextAsset;
        XmlDocument doc = new XmlDocument();
        doc.LoadXml(asset.text);

        XmlNodeList nodes = doc.SelectNodes("/figures/figure");

        foreach (XmlNode node in nodes)
        {
            Figure figure = new Figure();

            figure.koreanName = node.SelectSingleNode("koreanName").InnerText;
            figure.englishName = node.SelectSingleNode("englishName").InnerText;

            string[] actions = node.SelectSingleNode("actions").InnerText.Split(',');
            for (int i = 0; i < actions.Length; i++)
            {
                string fileName = figure.englishName + "_" + actions[i];
                figure.imgs.Add(fileName, Resources.Load<Sprite>("Figure_Imgs/" + fileName));
            }

            GameData.figures.Add(figure);

            yield return null;
        }
    }

    public IEnumerator LoadEnemyInfo()
    {
        TextAsset asset = Resources.Load("XmlFiles/enemyInfo") as TextAsset;
        XmlDocument doc = new XmlDocument();
        doc.LoadXml(asset.text);

        XmlNodeList nodes = doc.SelectNodes("/enemies/enemy");

        foreach (XmlNode node in nodes)
        {
            Enemy enemy = new Enemy();

            enemy.enemyID = node.SelectSingleNode("id").InnerText;
            enemy.enemyName = node.SelectSingleNode("name").InnerText;

            enemy.equipedWeapon = node.SelectSingleNode("equipment/weapon").InnerText;
            enemy.equipedArmor = node.SelectSingleNode("equipment/armor").InnerText;
            enemy.equipedAccessory = node.SelectSingleNode("equipment/accessory").InnerText;

            enemy.enemyStatus = new CStatus();

            XmlNodeList initTiles = node.SelectNodes("InitTiles/Tile");
            foreach (XmlNode tile in initTiles)
            {

                int quantity = int.Parse(tile.SelectSingleNode("Quantity").InnerText);

                for (int i = 0; i < quantity; i++)
                {
                    enemy.enemyTiles.Add(tile.SelectSingleNode("Name").InnerText);
                }

            }

            enemy.enemyStatus.characterHP = float.Parse(node.SelectSingleNode("stat/baseHP").InnerText);
            enemy.enemyStatus.characterMP = float.Parse(node.SelectSingleNode("stat/baseMP").InnerText);
            enemy.enemyStatus.characterDamage = int.Parse(node.SelectSingleNode("stat/baseATK").InnerText);
            enemy.enemyStatus.characterAttackEA_FIRE = bool.Parse(node.SelectSingleNode("stat/baseATK_FIRE").InnerText);
            enemy.enemyStatus.characterAttackEA_WATER = bool.Parse(node.SelectSingleNode("stat/baseATK_WATER").InnerText);
            enemy.enemyStatus.characterAttackEA_EARTH = bool.Parse(node.SelectSingleNode("stat/baseATK_EARTH").InnerText);
            enemy.enemyStatus.characterAttackEA_WIND = bool.Parse(node.SelectSingleNode("stat/baseATK_WIND").InnerText);
            enemy.enemyStatus.characterDefense = int.Parse(node.SelectSingleNode("stat/baseDEF").InnerText);
            enemy.enemyStatus.characterElementalReg_FIRE = int.Parse(node.SelectSingleNode("stat/baseDEF_FIRE").InnerText);
            enemy.enemyStatus.characterElementalReg_WATER = int.Parse(node.SelectSingleNode("stat/baseDEF_WATER").InnerText);
            enemy.enemyStatus.characterElementalReg_EARTH = int.Parse(node.SelectSingleNode("stat/baseDEF_EARTH").InnerText);
            enemy.enemyStatus.characterElementalReg_WIND = int.Parse(node.SelectSingleNode("stat/baseDEF_WIND").InnerText);
            enemy.enemyStatus.characterMovementSpeed = float.Parse(node.SelectSingleNode("stat/baseSpeed").InnerText);

            GameData.enemies.Add(enemy);
            yield return null;
        }
    }

    public IEnumerator LoadDiaglogueInfo()
    {
        TextAsset asset = Resources.Load("XmlFiles/dialogueInfo") as TextAsset;
        XmlDocument doc = new XmlDocument();
        doc.LoadXml(asset.text);

        XmlNodeList nodes = doc.SelectNodes("/dialogues/dialogue");
        foreach (XmlNode node in nodes)
        {
            Dialogue dialogue = new Dialogue();

            dialogue.stage = int.Parse(node.SelectSingleNode("stage").InnerText);
            dialogue.figureName = node.SelectSingleNode("character").InnerText;
            dialogue.figureState = node.SelectSingleNode("characterState").InnerText;

            string str = node.SelectSingleNode("timing").InnerText;
            if (str.Equals("BEGINNING"))
            {
                dialogue.phase = GamePhase.BEGINNING;
            }
            else if (str.Equals("SETTING"))
            {
                dialogue.phase = GamePhase.SETTING;
            }
            else if (str.Equals("BUILD"))
            {
                dialogue.phase = GamePhase.BUILDING;
            }
            else if (str.Equals("MOVING"))
            {
                dialogue.phase = GamePhase.MOVING;
            }
            else if (str.Equals("BATTLE"))
            {
                dialogue.phase = GamePhase.BATTLE;
            }
            else if (str.Equals("RESULT"))
            {
                dialogue.phase = GamePhase.RESULT;
            }

            str = node.SelectSingleNode("condition").InnerText;
            if (str.Equals("NONE"))
            {
                dialogue.condition = Condition.NONE;
            }
            else if (str.Equals("WIN"))
            {
                dialogue.condition = Condition.WIN;
            }
            else if (str.Equals("LOSE"))
            {
                dialogue.condition = Condition.LOSE;
            }

            dialogue.content = node.SelectSingleNode("content").InnerText;

            str = node.SelectSingleNode("imgLocation").InnerText;
            if (str.Equals("BG"))
            {
                dialogue.imgAttribute = DialogueImgAttribute.BG;
            }
            else if (str.Equals("NONE"))
            {
                dialogue.imgAttribute = DialogueImgAttribute.NONE;
            }
            else if (str.Equals("CENTER"))
            {
                dialogue.imgAttribute = DialogueImgAttribute.CENTER;
            }
            else if (str.Equals("LEFT"))
            {
                dialogue.imgAttribute = DialogueImgAttribute.LEFT;
            }
            else if (str.Equals("RIGHT"))
            {
                dialogue.imgAttribute = DialogueImgAttribute.RIGHT;
            }

            if (node.SelectSingleNode("anonymous").InnerText.Equals("Y"))
            {
                dialogue.isAnonymous = true;
            }
            else
            {
                dialogue.isAnonymous = false;
            }

            if (node.SelectSingleNode("continued").InnerText.Equals("Y"))
            {
                dialogue.isContinued = true;
            }
            else
            {
                dialogue.isContinued = false;
            }

            GameData.dialogues.Add(dialogue);
            yield return null;
        }
        //Debug.Log(GameData.dialogues.Count);
    }

    public IEnumerator LoadStageInfo()
    {
        TextAsset asset = Resources.Load("XmlFiles/stageInfo") as TextAsset;
        XmlDocument doc = new XmlDocument();
        doc.LoadXml(asset.text);

        XmlNodeList nodes = doc.SelectNodes("/stages/stage");

        foreach (XmlNode node in nodes)
        {
            Stage stage = new Stage();
            stage.stageNum = int.Parse(node.SelectSingleNode("num").InnerText);
            stage.stageTitle = node.SelectSingleNode("title").InnerText;

            stage.reward = new Reward(node.SelectSingleNode("reward/gold").InnerText.Equals("NONE") ? int.Parse("0") : int.Parse(node.SelectSingleNode("reward/gold").InnerText), node.SelectSingleNode("reward/item").InnerText.Equals("NONE") ? null : GameData.GetItemData(node.SelectSingleNode("reward/item").InnerText));

            stage.stageDescription = node.SelectSingleNode("desc").InnerText;
            stage.backgroundImg = Resources.Load<Sprite>("StoryBackgrounds_Imgs/" + node.SelectSingleNode("background").InnerText);

            string attributeStr = node.SelectSingleNode("attribute").InnerText;
            if (attributeStr.Equals("NONE"))
            {
                stage.mapEA = ElementalAttribute.NONE;
            }
            else if (attributeStr.Equals("FIRE"))
            {
                stage.mapEA = ElementalAttribute.FIRE;
            }
            else if (attributeStr.Equals("WATER"))
            {
                stage.mapEA = ElementalAttribute.WATER;
            }
            else if (attributeStr.Equals("EARTH"))
            {
                stage.mapEA = ElementalAttribute.EARTH;
            }
            else if (attributeStr.Equals("WIND"))
            {
                stage.mapEA = ElementalAttribute.WIND;
            }

            stage.buildTime = node.SelectSingleNode("buildTime").InnerText.Equals("NONE") ? 0 : int.Parse(node.SelectSingleNode("buildTime").InnerText);

            if (!node.SelectSingleNode("baseWall").InnerText.Equals("NONE"))
            {
                string[] strArr1 = node.SelectSingleNode("baseWall").InnerText.Split(' ');
                for (int i = 0; i < strArr1.Length; i++)
                {
                    string[] str = strArr1[i].Split(',');
                    stage.baseWallCoordinate.Add(new Vector2(int.Parse(str[0]), int.Parse(str[1])));
                }
            }

            if (!node.SelectSingleNode("aiWall").InnerText.Equals("NONE"))
            {
                string[] strArr2 = node.SelectSingleNode("aiWall").InnerText.Split(' ');
                for (int i = 0; i < strArr2.Length; i++)
                {
                    string[] str = strArr2[i].Split(',');
                    stage.aiWallCoordinate.Add(new Vector2(int.Parse(str[0]), int.Parse(str[1])));
                }
            }

            foreach (Dialogue dialogue in GameData.dialogues)
            {
                if (dialogue.stage == stage.stageNum)
                {
                    stage.dialogues.Add(dialogue);
                }
            }

            for (int i = 0; i < 6; i++)
            {
                stage.dialoguesByPhase[i] = new List<Dialogue>();

            }

            foreach (Dialogue dialogue in stage.dialogues)
            {
                stage.dialoguesByPhase[(int)dialogue.phase].Add(dialogue);
            }

            string enemyID = node.SelectSingleNode("enemyID").InnerText;

            foreach (Enemy enemy in GameData.enemies)
            {
                if (enemy.enemyID.Equals(enemyID))
                {
                    stage.enemy = enemy;
                }
            }

            if (node.SelectSingleNode("playerBattleImg").InnerText.Equals("NONE")) {
                stage.playerBattleImg = null;
            }
            else {
                stage.playerBattleImg = GameData.battleImgs[int.Parse(node.SelectSingleNode("playerBattleImg").InnerText)];
            }

            if (node.SelectSingleNode("enemyBattleImg").InnerText.Equals("NONE"))
            {
                stage.enemyBattleImg = null;
            }
            else
            {
                stage.enemyBattleImg = GameData.battleImgs[int.Parse(node.SelectSingleNode("enemyBattleImg").InnerText)];
            }

            GameData.stages.Add(stage);
            yield return null;
        }
    }

    public IEnumerator LoadTextures()
    {
        GameData.textures.Add(ElementalAttribute.FIRE, new List<Texture>());
        GameData.textures.Add(ElementalAttribute.WATER, new List<Texture>());
        GameData.textures.Add(ElementalAttribute.EARTH, new List<Texture>());
        GameData.textures.Add(ElementalAttribute.WIND, new List<Texture>());

        GameData.wallTextures.Add(ElementalAttribute.FIRE, new List<Texture>());
        GameData.wallTextures.Add(ElementalAttribute.WATER, new List<Texture>());
        GameData.wallTextures.Add(ElementalAttribute.EARTH, new List<Texture>());
        GameData.wallTextures.Add(ElementalAttribute.WIND, new List<Texture>());


        for (int i = 0; i < 16; i++)
        {
            GameData.textures[ElementalAttribute.FIRE].Add(Resources.Load<Texture>("Textures/fire/volcano" + i));
            GameData.textures[ElementalAttribute.WATER].Add(Resources.Load<Texture>("Textures/water/ice" + i));
            GameData.textures[ElementalAttribute.EARTH].Add(Resources.Load<Texture>("Textures/earth/sand" + i));
            GameData.textures[ElementalAttribute.WIND].Add(Resources.Load<Texture>("Textures/wind/grass" + i));

            GameData.wallTextures[ElementalAttribute.FIRE].Add(Resources.Load<Texture>("Wall_Textures/fire/volcano" + i));
            GameData.wallTextures[ElementalAttribute.WATER].Add(Resources.Load<Texture>("Wall_Textures/water/ice" + i));
            GameData.wallTextures[ElementalAttribute.EARTH].Add(Resources.Load<Texture>("Wall_Textures/earth/sand" + i));
            GameData.wallTextures[ElementalAttribute.WIND].Add(Resources.Load<Texture>("Wall_Textures/wind/grass" + i));

            yield return null;
        }

    }

    public IEnumerator LoadBattleImgs()
    {
        TextAsset asset = Resources.Load("XmlFiles/battleImgsInfo") as TextAsset;
        XmlDocument doc = new XmlDocument();
        doc.LoadXml(asset.text);

        XmlNodeList nodes = doc.SelectNodes("/BattleImgs/BattleImg");

        foreach (XmlNode node in nodes)
        {
            BattleImg bi = new BattleImg();
            bi.id = node.SelectSingleNode("id").InnerText;
            bi.figure = node.SelectSingleNode("figure").InnerText;

            foreach (Figure f in GameData.figures) {
                if (f.englishName.Equals(bi.figure)) {
                    bi.basic = f.imgs[f.englishName + "_" + node.SelectSingleNode("default").InnerText];
                    bi.hit = f.imgs[f.englishName + "_" + node.SelectSingleNode("hit").InnerText];
                    bi.death = f.imgs[f.englishName + "_" + node.SelectSingleNode("death").InnerText];
                }
            }
            GameData.battleImgs.Add(bi);

            yield return null;
        }
    }

    public IEnumerator LoadThumnailInfo() {
        TextAsset asset = Resources.Load("XmlFiles/figureThumnailInfo") as TextAsset;
        XmlDocument doc = new XmlDocument();
        doc.LoadXml(asset.text);

        XmlNodeList nodes = doc.SelectNodes("/figurethumnails/figurethumnail");

        foreach (XmlNode node in nodes)
        {
            FigureThumnail thumnail = new FigureThumnail();
            thumnail.figureNameKr = node.SelectSingleNode("name_kr").InnerText;
            thumnail.figureNameEn = node.SelectSingleNode("name_en").InnerText;
            thumnail.figurePortrait = Resources.Load<Sprite>("Figure_Portraits/" + thumnail.figureNameEn+"_"+"crop");
            thumnail.battleImg = GameData.battleImgs[int.Parse(node.SelectSingleNode("battleImgID").InnerText)];
            thumnail.silhouette = Resources.Load<Sprite>("Figure_Thumnails/" + thumnail.figureNameEn + "/" + thumnail.figureNameEn + "_black");

            string[] strArr = node.SelectSingleNode("imgs").InnerText.Split(',');
            for (int i = 0; i < strArr.Length; i++) {
                thumnail.imgs.Add(Resources.Load<Sprite>("Figure_Thumnails/" + thumnail.figureNameEn + "/" + thumnail.figureNameEn + strArr[i]));
                
            }
            
            GameData.thumnails.Add(thumnail.figureNameEn, thumnail);

            yield return null;
        }
    }

    public IEnumerator SoundInfoLoad()
	{
		TextAsset asset = Resources.Load("XmlFiles/soundInfo") as TextAsset;
		XmlDocument doc = new XmlDocument();
		doc.LoadXml(asset.text);
		
		XmlNodeList nodes = doc.SelectNodes("/sound/music");
		
		foreach (XmlNode node in nodes)
		{
			GameSound sound = new GameSound();
			sound.soundName = node.SelectSingleNode("name/soundName").InnerText;
			sound.soundType = node.SelectSingleNode("soundType").InnerText;
			sound.soundClip = Resources.Load<AudioClip>("Sounds/" + sound.soundType + "/" + sound.soundName);
			
			GameData.gameSounds.Add(sound);

            yield return null;
        }
	}
    /*
	public static void BackgoundSoundInfoLoad()
	{
		TextAsset asset = Resources.Load("XmlFiles/soundInfo") as TextAsset;
		XmlDocument doc = new XmlDocument();
		doc.LoadXml(asset.text);
		
		XmlNodeList nodes = doc.SelectNodes("/sound/music");
		
		foreach (XmlNode node in nodes)
		{
			BackgroundMusic sound = new BackgroundMusic();
			sound.musicName = node.SelectSingleNode("name/soundName").InnerText;
			sound.musicType = node.SelectSingleNode("soundType").InnerText;
			sound.backgroundSoundClip = Resources.Load<AudioClip>("Sounds/" + sound.musicType + "/" + sound.musicName);
			
			GameData.backgoundMusics.Add(sound);
		}
	}*/

    public IEnumerator LoadTips() {
        TextAsset asset = Resources.Load("XmlFiles/tipInfo") as TextAsset;
        XmlDocument doc = new XmlDocument();
        doc.LoadXml(asset.text);

        XmlNodeList nodes = doc.SelectNodes("/tips/tip");

        foreach (XmlNode node in nodes)
        {
            GameData.tips.Add(node.SelectSingleNode("contents").InnerText);
            yield return null;
        }
    }

    private IEnumerator LoadAllData() {
        StartCoroutine(DisplayLoadingBar());
        GameData.initData();
        progress = 5;
        yield return StartCoroutine(TileInfoLoad());
        progress = 10;
        yield return StartCoroutine(WeaponInfoLoad());
        progress = 15;
        yield return StartCoroutine(ArmorInfoLoad());
        progress = 20;
        yield return StartCoroutine(AccessoryInfoLoad());
        progress = 25;
        yield return StartCoroutine(ItemLoad());
        progress = 30;
        yield return StartCoroutine(VarialbleInfoLoad());
        progress = 35;
        yield return StartCoroutine(ProbLoad());
        progress = 40;
        yield return StartCoroutine(LoadFigureInfo());
        progress = 45;
        yield return StartCoroutine(LoadEnemyInfo());
        progress = 50;
        yield return StartCoroutine(LoadDiaglogueInfo());
        progress = 55;
        yield return StartCoroutine(LoadBattleImgs());
        progress = 60;
        yield return StartCoroutine(LoadStageInfo());
        progress = 70;
        yield return StartCoroutine(LoadTextures());
        progress = 75;
        yield return StartCoroutine(LoadThumnailInfo());
        progress = 80;
        yield return StartCoroutine(SoundInfoLoad());
        progress = 85;
        yield return StartCoroutine(LoadTips());
        progress = 90;
        yield return new WaitForSeconds(0.5f);
        progress = 100;
        yield return new WaitForSeconds(0.5f);
        isDataloaded = true;
        ChangeScene(1);
    }

    private IEnumerator DisplayLoadingBar() {
        while (isDataloaded == false) {
            loadingText.text = progress.ToString() + "% LOADING...";
            loadingBar.GetComponent<RectTransform>().sizeDelta = new Vector2(progress / 100 * length, 60);
            yield return null;
        }
    }


}
