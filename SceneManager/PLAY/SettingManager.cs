using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class SettingManager : InventoryManager {
    public Text timerText;

    void Start() {
        initVariables();
        DisplayPlayerName(playerName, playerPortraitImg);
        LoadItemData();
        SelectTab(0);
        DisplayCurEquipment();
        DisplayCurStatus();
        LoadDeckData();
        myCoroutine = init();
        StartCoroutine(myCoroutine);
    }

    IEnumerator init() {
        yield return StartCoroutine(Timer(GameData.settingTime, timerText, true));
    }

    public void SelectDeck(int idx) {
        if (GameData.pc.tileDeckListOwnedByCharacter[idx].isUnlocked == false)
            return;

        GameData.pc.selectedTileDeckIndex = idx;

        for (int i = 0; i < 4; i++) {
            if (GameData.pc.selectedTileDeckIndex == i)
            {
                deckSlots[i].transform.GetComponent<Image>().color = new Color(255, 255, 0, 255);
            }
            else
                deckSlots[i].transform.GetComponent<Image>().color = new Color(255, 255, 0, 0);
        }
        
    }
}
