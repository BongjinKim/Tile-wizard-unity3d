using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.EventSystems;

public class SoundManager: MonoBehaviour {

	//효과음을 재생하는 스크립트
	//public List<AudioClip> audioClips = new List<AudioClip>();
	public AudioSource audio;
	public string tag;


	void Start()
	{
		//InitClips ();
	}
	
	void Update () 
	{
		/*
		//Vector2 wp = cam.ScreenToWorldPoint(Input.mousePosition);
		Ray2D ray = new Ray2D(Input.mousePosition, Vector2.zero);
		//Debug.Log (ray);
		// Raycast Hit
		RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction);
		
		if (hit.collider != null)
		{
			Debug.Log("HIT!!!"+hit.collider.name);
			// If we click it
			if (Input.GetMouseButtonUp(0))
			{
				// Notify of the event!
				OnClick(hit.collider.gameObject);
			}
		}*/

		if(Input.GetMouseButtonUp(0))
		{
			PointerEventData pointer = new PointerEventData(EventSystem.current);
			pointer.position = Input.mousePosition;
			
			List<RaycastResult> raycastResults = new List<RaycastResult>();
			EventSystem.current.RaycastAll(pointer, raycastResults);
			
			if(raycastResults.Count > 0)
			{
				foreach(RaycastResult ray in raycastResults){
					Debug.Log(ray.gameObject.tag);
					if(ray.gameObject.tag == "ButtonNormal"){
						PlaySound(ray.gameObject.tag);
						break;

					}else if(ray.gameObject.tag == "BuySound"){
						PlaySound(ray.gameObject.tag);
						break;
					}else if(ray.gameObject.tag == "CreateStageSound"){
						PlaySound(ray.gameObject.tag);
						break;
					}else if(ray.gameObject.tag == "ButtonNormal"){
						PlaySound(ray.gameObject.tag);
						break;
					}else if(ray.gameObject.tag == "EquipSound"){
						PlaySound(ray.gameObject.tag);
						break;
					}else if(ray.gameObject.tag == "ResultSound"){
						PlaySound(ray.gameObject.tag);
						break;
                    }
                    else if (ray.gameObject.tag == "NoActiveSound")
                    {
						PlaySound(ray.gameObject.tag);
						break;
                    }
                    else if (ray.gameObject.tag == "PickCardSound")
                    {
                        PlaySound(ray.gameObject.tag);
                        break;
                    }
                    else if (ray.gameObject.tag == "DownCardSound")
                    {
                        PlaySound(ray.gameObject.tag);
                        break;
                    }


				}
			}
		}

	}


	public void PlaySound(string name)
	{
		if (GameData.ms == false) {

			audio.clip = GameData.GetSoundClipData (name).soundClip;
			audio.Play ();
		}
	}


}



