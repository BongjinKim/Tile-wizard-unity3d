using UnityEngine;
using System.Collections;

public class BackgroundMusicManager : MonoBehaviour
{

    public AudioSource audio;

    int isNewIndex = 10; // 0은 타이틀 음악, 1은 메인



    void Awake()
    {
        Debug.Log (Application.loadedLevelName);
        if (Application.loadedLevelName.Equals("0. TITLE") )
        {
            BackgroundPlay("Background1");
        }

    }

    void OnLevelWasLoaded(int level)
    {
        switch (level)
        {

            case 1: //틀인메
			Debug.Log(isNewIndex);
                if (isNewIndex != 9)
                {
                    BackgroundPlay("Background1");
                    isNewIndex = 9;
                }
                break;

            case 2: // 메인, 인포, 샵, 타일박스
                if (isNewIndex != 1)
                {
                    BackgroundPlay("Background2");
                    isNewIndex = 1;
                }
                break;

            case 10: // 대화, 타일배치, 플레이
                if (isNewIndex != 4)
                {
                    BackgroundPlay("Background5");
                    isNewIndex = 4;
                }
                break;

            case 7: // 월드맵 인포, 샵, 타일박스
                if (isNewIndex != 2)
                {
                    BackgroundPlay("Background4");
                    isNewIndex = 2;
                }
                break;

            case 9: //처음시작
                BackgroundPlay("Background3");
                break;

            case 13: //결과
                if (isNewIndex != 3)
                {
                    if (Game.gResult == GameResult.WIN)
                    {
                        BackgroundPlay("Background9");
                        DontLoopSound();

                    }
                    else if (Game.gResult == GameResult.LOSE)
                    {
                        BackgroundPlay("Background8");
                        DontLoopSound();
                    }
                    isNewIndex = 3;
                }
                break;

            case 11: //결과
                BackgroundPlay("Background7");
                break;

            default:
                break;
        }

    }

    public void BackgroundPlay(string ex)
    {
        //Debug.Log (GameData.backgoundMusics [0].musicName);
        audio.Stop();
        audio.clip = GameData.GetSoundClipData(ex).soundClip;
		audio.loop = true;
        audio.Play();
    }
    public void DontLoopSound()
    {
        audio.loop = false;

    }
}
