using UnityEngine;
using System.Collections;

public class TileEffect : MonoBehaviour {
    public delegate void UpdateTileInfo();
    public static event UpdateTileInfo OnTriggerEntered;
	public SoundManager sm;

    public Tile myTile;
    float knockbackSpeed = 5f;
    private Model subject;
    bool visitedTile;

    GameObject effect;

    bool blockedByWall = false;
    float stunTime = 0;

    //경직시간
    float stiffTime = 0.5f;

    public IEnumerator InstantDamage(CStatus stat, Tile tile) {
        // 단일 데미지 생성
        if (tile.instantDamage != 0)
        {
            stat.characterMovementSpeed = 0;
            subject.anim.SetInteger("speed", 0);
            subject.anim.SetBool("isHit", true);
            subject.anim.SetTrigger("hit");
            stat.characterHP -= BattleFormulas.CalInstantTileDamage(stat, Game.curStageInfo.mapEA, tile);
            yield return new WaitForSeconds(stiffTime);
            subject.anim.ResetTrigger("hit");
            subject.anim.SetBool("isHit", false);
            stat.characterMovementSpeed = 1;
            subject.anim.SetInteger("speed", 1);

        }
    }

    public IEnumerator DotDamage(CStatus stat, Tile tile)
    // 도트 데미지 생성
    {
        if (tile.dotDamageDuration != 0)
        {
            subject.hasDotDmg = true;
            float t = tile.dotDamageDuration;
            float elapsedT = 0;

            while (elapsedT < t)
            {
                if (subject.isStunned == true)
                {
                    stat.characterHP -= BattleFormulas.CalDotTileDamage(stat, Game.curStageInfo.mapEA, tile, true);
                }
                else {
                    stat.characterHP -= BattleFormulas.CalDotTileDamage(stat, Game.curStageInfo.mapEA, tile, false);
                }
                                
                elapsedT += 0.5f;
                yield return new WaitForSeconds(0.5f);
            }
            subject.hasDotDmg = false;
        }
    }
    
    public IEnumerator Stun(CStatus stat, Tile tile) {
        // 스턴 데미지 생성
        if (tile.stunTime != 0) {
            subject.isStunned = true;
            stat.characterMovementSpeed = 0;
            subject.anim.SetInteger("speed", 0);
            yield return new WaitForSeconds(tile.stunTime);
            subject.isStunned = false;
            stat.characterMovementSpeed = 1;
            subject.anim.SetInteger("speed", 1);
        }
    }

    public IEnumerator Slow(CStatus stat, Tile tile) {
        // 슬로우 데미지 생성
        if (tile.slownessDuration != 0) {
            
            subject.isSlowed = true;
            stat.characterMovementSpeed *= tile.slownessDegree / 100f;
            subject.anim.speed = tile.slownessDegree / 100f;
            yield return new WaitForSeconds(tile.slownessDuration);
            subject.anim.speed = 1;
            stat.characterMovementSpeed = 1;
            subject.anim.SetInteger("speed", 1);
            subject.isSlowed = false;
            subject.slowEffect = null;
        }
    }

    public IEnumerator Knockback(CStatus stat, Tile tile) {
        // 넉백 데미지 생성
        if(tile.knockbackDistance != 0)
        {
            subject.isKnockedback = true;
            stat.characterMovementSpeed = 0;
            subject.anim.SetInteger("speed", 0);

            Vector3 target;

            if (subject.isSlowed == true)
            {
                target = CalTargetPos(tile.knockbackDistance/2);
            }
            else {
                target = CalTargetPos(tile.knockbackDistance);
            }

            if (GameData.isVibration == true)
            {
                if(BuildAndMoveManager.focusedType == subject.myType)
				{}//Handheld.Vibrate();
            }

            while (Vector3.Distance(subject.transform.position, target) > 0.01f)
            {
                float speed = knockbackSpeed * Time.deltaTime;

                subject.transform.position = Vector3.MoveTowards(subject.transform.position, target, speed);
                
                yield return null;
            }

            subject.transform.position = target;

            if (blockedByWall == true) {
				if (sm == null)
					sm = GameObject.Find ("SoundManager").GetComponent<SoundManager> ();
				sm.PlaySound("Stun");
                yield return new WaitForSeconds(stunTime);
            }
            else
            {
                yield return new WaitForSeconds(0.05f);

            }
            


            stat.characterMovementSpeed = 1;
            subject.anim.SetInteger("speed", 1);

            subject.isKnockedback = false;
        }
        
    }

    Vector3 CalTargetPos(float distance) {
        //이펙트 위치 계산하여 위치를 반환하는 함수
        Vector3 target = Vector3.zero;
        int cur = subject.curIdx;
        Direction dir = Direction.Up;
        Vector3 curPos = subject.myObj.transform.position;
        
        if (subject.myPath[cur].row > subject.myPath[cur+1].row)
        {
            dir = Direction.Down;
        }
        else if (subject.myPath[cur].row == subject.myPath[cur+1].row)
        {
            if (subject.myPath[cur].column > subject.myPath[cur+1].column)
            {
                dir = Direction.RIGHT;
            }
            else if(subject.myPath[cur].column < subject.myPath[cur+1].column)
            {
                dir = Direction.LEFT;
            }
        }
        else {
            dir = Direction.Up;
        }
        

        target = curPos;
        
        int i = 0;
        int j = 0;
        int celling = 0;

        switch (dir) {
            case Direction.Up:
                i = 0;
                j = 0;

                while (true)
                {
                    if ((cur - i) >= 0 && subject.myPath[cur - i].column == subject.myPath[cur + 1].column)
                    {
                        i++;
                    }
                    else
                    {
                        break;
                    }
                }

                while (true)
                {
                    if ((subject.myPath[cur].row - j < 0) || (subject.myMap.map[subject.myPath[cur].row - j, subject.myPath[cur].column].myBlock != null && subject.myMap.map[subject.myPath[cur].row - j, subject.myPath[cur].column].myBlock.type == BlockType.WALL))
                    {
                        break;
                    }

                    j++;
                }

                celling = (int)System.Math.Ceiling(distance + 1);

                if (i >= celling || j >= celling)
                {
                    target = new Vector3(target.x + distance, target.y, target.z);
                }
                else if (j != i)
                {
                    Vector3 pos = subject.myMap.map[subject.myPath[cur].row - j + 1, subject.myPath[cur].column].myRef.transform.position;
                    target = new Vector3(pos.x, target.y, pos.z);
                }
                else {
                    target = subject.myPos[cur - (i - 1)];
                    blockedByWall = true;
                    stunTime = distance + 1 - i;
                }
                break;
            case Direction.Down:
                i = 0;
                j = 0;

                while (true)
                {
                    if ((cur - i) >= 0 && subject.myPath[cur - i].column == subject.myPath[cur + 1].column)
                    {
                        i++;
                    }
                    else
                    {
                        break;
                    }
                }

                while (true)
                {
                    if ((subject.myPath[cur].row + j > 10) || (subject.myMap.map[subject.myPath[cur].row + j, subject.myPath[cur].column].myBlock != null && subject.myMap.map[subject.myPath[cur].row +  j, subject.myPath[cur].column].myBlock.type == BlockType.WALL))
                    {
                        break;
                    }

                    j++;
                }

                celling = (int)System.Math.Ceiling(distance + 1);

                if (i >= celling || j >= celling)
                {
                    target = new Vector3(target.x - distance, target.y, target.z);
                }
                else if (j != i)
                {
                    Vector3 pos = subject.myMap.map[subject.myPath[cur].row + j - 1, subject.myPath[cur].column].myRef.transform.position;

                    target = new Vector3(pos.x, target.y, pos.z);
                }
                else
                {
                    target = subject.myPos[cur - (i - 1)];
                    blockedByWall = true;
                    stunTime = distance + 1 - i;
                }
                break;
            case Direction.LEFT:
                //z--
                i = 0;
                j = 0;

                while (true) {
                    if ((cur - i) >= 0 && (subject.myPath[cur - i].row == subject.myPath[cur + 1].row))
                    {
                        i++;
                    }
                    else {
                        break;
                    }
                }

                while (true) {
                    if ((subject.myPath[cur].column - j < 0) || (subject.myMap.map[subject.myPath[cur].row, subject.myPath[cur].column - j].myBlock != null && subject.myMap.map[subject.myPath[cur].row, subject.myPath[cur].column - j].myBlock.type == BlockType.WALL)) {
                        
                        break;
                    }

                    j++;
                }

                celling = (int)System.Math.Ceiling(distance+1);

                if (i >= celling || j >= celling)
                {
                    Debug.Log("distance");
                    target = new Vector3(target.x, target.y, target.z + distance);
                }
                else if (j != i)
                {
                    Vector3 pos = subject.myMap.map[subject.myPath[cur].row, subject.myPath[cur].column - j + 1].myRef.transform.position;
                    target = new Vector3(pos.x, target.y, pos.z);
                }
                else
                {
                    target = subject.myPos[cur - (i - 1)];
                    blockedByWall = true;
                    stunTime = distance + 1 - i;
                }
                break;
            case Direction.RIGHT:
                //z++

                i = 0;
                j = 0;

                while (true)
                {
                    if ((cur - i) >= 0 && subject.myPath[cur - i].row == subject.myPath[cur + 1].row)
                    {
                        i++;
                    }
                    else
                    {
                        break;
                    }
                }

                while (true)
                {
                    if ((subject.myPath[cur].column + j > 10) || (subject.myMap.map[subject.myPath[cur].row, subject.myPath[cur].column + j].myBlock != null && subject.myMap.map[subject.myPath[cur].row, subject.myPath[cur].column + j].myBlock.type == BlockType.WALL))
                    {
                        break;
                    }

                    j++;
                }

                celling = (int)System.Math.Ceiling(distance + 1);
                Debug.Log(celling);

                if (i >= celling || j >= celling)
                {
                    target = new Vector3(target.x, target.y, target.z - distance);
                }
                else if (j != i)
                {
                    Vector3 pos = subject.myMap.map[subject.myPath[cur].row, subject.myPath[cur].column + j - 1].myRef.transform.position;
                    target = new Vector3(pos.x, target.y, pos.z);
                }
                else
                {
                    target = subject.myPos[cur - (i - 1)];
                    blockedByWall = true;
                    stunTime = distance + 1 - i;
                }
                break;
        }

        return target;
        
    }

    private IEnumerator StartEffect(CStatus status, Tile tile) {
        //이펙트를 재생하는 함수
        yield return StartCoroutine(InstantDamage(status, tile));

        yield return StartCoroutine(Knockback(status, tile));

        yield return StartCoroutine(Stun(status, tile));
        
        if (subject.isSlowed == true && tile.slownessDuration != 0)
        {
            
            if(subject.slowEffect != null)
                StopCoroutine(subject.slowEffect);
            subject.isSlowed = false;
        }
        subject.slowEffect = Slow(status, tile);
        StartCoroutine(subject.slowEffect);
        
        if (subject.hasDotDmg == true && tile.dotDamageDuration != 0)
        {
            if (subject.dotDmgEffect != null) {
                Debug.Log("1");
                StopCoroutine(subject.dotDmgEffect);
            }
                
            subject.hasDotDmg = false;
        }
        subject.dotDmgEffect = DotDamage(status, tile);
        StartCoroutine(subject.dotDmgEffect);
    }

    void OnTriggerEnter(Collider col)
        //Triger에 닿으면 이펙트시작하는 함수
    {
		if (sm == null)
			sm = GameObject.Find ("SoundManager").GetComponent<SoundManager> ();
        //데미지
        //넉백
        //스턴
        //슬로우
        //도트
        if (col.tag.Equals("Model")) {
            if (visitedTile == false) {
                visitedTile = true;
                subject = col.GetComponent<Model>();
                subject.curAffectedTile = myTile;
                OnTriggerEntered();
                StartCoroutine(StartEffect(col.GetComponent<Model>().myStat, myTile));
                if (subject.myType == BuildAndMoveManager.focusedType) {
					//Debug.Log(myTile.englishName);
					sm.PlaySound(myTile.englishName);
                }
				    
                MakeEffect();
            }
        }
    }
    void OnTriggerExit(Collider col) {
 
        if (col.tag.Equals("Model")) {
            
        }
    }

    void MakeEffect() {
        //이펙트 만들어주는 함수
        if (myTile.tileEffect == null)
            return;

        Vector3 pos = subject.transform.position;
        
        effect = Instantiate(myTile.tileEffect, new Vector3(pos.x, pos.y+0.75f, pos.z), Quaternion.identity) as GameObject;
        effect.transform.localScale = new Vector3(effect.transform.localScale.x*2, effect.transform.localScale.y*2, effect.transform.localScale.z*2);
        Destroy(effect, effect.GetComponent<ParticleSystem>().duration);

    }
    
}
