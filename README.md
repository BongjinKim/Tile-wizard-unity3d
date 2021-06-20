# Tile-wizard-unity3d
## 파일 디렉토리

 * [code](./code)
   * [Deck](./code/Deck)
     * [UserDeckManager.cs](./code/Deck/UserDeckManager.cs)
   * [Lobby](./code/Lobby)
     * [StartMenuManager.cs](./code/Lobby/StartMenuManager.cs)
   * [Lodding](./code/Lodding)
        * [BackgroundMusicManager.cs](./code/Lodding/BackgroundMusicManager.cs)
        * [Login.cs](./code/Lodding/Login.cs)
        * [LogOnCheck.cs](./code/Lodding/LogOnCheck.cs)
        * [SoundManager.cs](./code/Lodding/SoundManager.cs)
        * [XmlParser.cs](./code/Lodding/XmlParser.cs)
   * [Main](./code/Main)
        * [MainManager.cs](./code/Main/MainManager.cs)
   * [Shop](./code/Shop)
        * [SHOPManager.cs](./code/Shop/SHOPManager.cs)
   * [Skill](./code/Skill)
     * [TileEffect.cs](./code/Skill/TileEffect.cs)

## **프로젝트 기간**


15.06.23~16.05.12

## **본인 역할**


UI 개발, 스킬 제작

## 세부사항


### 대기화면 제작

- BackgroundMusicManager : Class - 모든 배경음악을 관리하는 Class
- LogOnCheck : Class - 로그인 가능한지 체크하는 Class
- Login : Class - 로그인 Class
- SoundManager : Class - 모든 효과음을 관리하는 Class
- XmlParser : Class - 모든 데이터를 불러오는 Class

### 메인 화면 제작

- **MainManager : Class**
    - LoadPlayerCharacter - 캐릭터를 로드하는 함수
    - MakeNewCharacter - 새로운 캐릭터를 생성하는 함수

### 로비 제작

- **StartMenuManager : Class**
    - StartMenuManager - 캐릭터 이미지를 바꾸는 popup창 띄우는 함수
    - UpdateCurrentCharacterImg - 현재의 이미지로 업데이트 하는 함수
    - UpdateCurrentCharacterImg(int index)- index가 들어오면 index에 해당하는 이미지의 정보를 바꾸어 주는 함수
    - UpdateCharacterMainImg - 메인 캐릭터 이미지 초기화
    - UnlockCharacter - 캐릭터 잠금을 해제 하는 함수
    - EnterStoryMode - 스토리 모드 입장하는 함수
    - DisplayTip - Tip 출력하는 함수
    - DisplayCharacterMoney - 캐릭터가 가진 돈을 출력하는 함수

### 상점 제작

- **SHOPManager : Class**
    - LoadItemData - 무기, 방어구, 장신구 load
    - DisplayCurEquipment - 장비를 보여주는 함수
    - DisplayCurStatus - 현재 캐릭터의 스테이터스를 보여주는 함수
    - DisplayChangedStatus - Status가 바뀔 때 마다 호출되는 함수
    - PreviewItem - 선택한 아이템을 장착해서 미리 보는 함수
    - SelectTab - Tab을 활성화 시킬 것인지 선택하는 함수
    - SetTab - Tab 활성화시키는 함수
    - PreviewPrice - 내가 고른 아이템들의 가격의 합을 계산하는 함수
    - BuyItem - 여태까지 고른 아이템을 구매하는 함수
    - DisplayWeaponInfo - 장비창에 무기를 착용했을 때 정보가 업데이트되는 함수
    - DisplayArmorInfo - 장비창에 방어구를 착용했을 때 정보가 업데이트 되는 함수
    - DisplayAccessoryInfo - 장비창에 장신구를 착용했을 때 정보가 업데이트 되는 함수
    - ShowConfirmPopup - 구매할 물품을 최종적으로 확인하는 popup창을 띄우는 함수

### 덱 제작

- **UserDeckManager : Class**
    - DownscaleCard - 보유하고 있는 카드를 클릭했을 때 작아짐
    - UpscaleCard - 보유하고 있는 카드를 클릭했을 때 커짐
    - MakeAutoYesBtn - Yes 버튼을 누르면 덱 자동 구성
    - MakeAutoNoBtn - 덱 자동 완성 취소 함수
    - EditText - 덱 이름 패널 활성화 함수
    - EditTextYesBtn - 덱이름 수정 함수
    - EditTextNoBtn - 덱이름 수정 취소함수
    - MakeTileInfoDeck - 타일을 덱에 생성하는 함수
    - DeleteSelectedTile - 선택된 타일을 지우는 함수.
    - MakeSelectedTile - 선택한 타일을 덱에 만들어 주는 함수
    - CancelDeck - 인벤토리로 돌아가는 함수
    - ClearDeck - 자신의 덱 클리어
    - SubmitDeck - 자신의 덱 저장
    - LoadDeck -  저장된 덱 불러오기
    - GetTotalNum : int - 덱에 있는 카드 갯수 반환 함수
    - CountingElementNum - 원소의 개수를 세는 함수
    - ConvertElementNum - 원소의 개수를 출력하는 함수

### 스킬 제작


- **TileEffect : Class**
    - InstantDamage - 즉발 데미지 생성
    - DotDamage - 도트 데미지 생성
    - Stun - 스턴 데미지 생성
    - Slow - 슬로우 데미지 생성
    - Knockback - 넉백 데미지 생성
    - CalTargetPos : Vector - 스킬 이펙트 위치 계산후 반환
    - StartEffect : 이펙트를 재생하는 함수
    - OnTriggerEnter : Trigger에 닿으면 이펙트를 재생
    - MakeEffect : 이펙트를 인스턴스를 만드는 함수