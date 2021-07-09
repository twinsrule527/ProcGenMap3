using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//Similar to the Base HallwayMaker, but hallways created have different conditions and will disappear after a certain amount of time
public class SecondaryHallwayMaker : BaseHallwayMaker
{
    private int curHallLength;//How many steps this hallway has taken
    public int maxHallLength;//How many steps this hallway is supposed to take before self-destructing
    void Start()
    {
        curStraightLength = 0;
        curHallLength = 0;
    }

    public override void Update()
    {
        //if it shouldn't self-destruct, it continues moving
        if(curHallLength < maxHallLength) {
            if(curStraightLength < Functions.minStraightLength) {
                curStraightLength++;
                HallForward();
            }
            else {
                float rnd = Random.Range(0f, 1f);
                if(rnd < Functions.baseTurnChance) {
                    HallTurn();
                    curStraightLength = 1;
                    HallForward();
                }
            }
        }
        else {
            //Self-Destruct
            float rnd = Random.Range(0f, 1f);
            TileManager.Instance.Makers.Remove(gameObject);
            TileManager.Instance.RemoveFromMakers();
            if(rnd < Functions.HallDeathRoomChance) {
                int width = Random.Range(Functions.minRoomLength, Functions.maxRoomLength);
                int height = Random.Range(Functions.minRoomLength, Functions.maxRoomLength);
                int offset = Random.Range(0, width);
                CreateRoom(width, height, offset);
            }
            Destroy(gameObject);
        }
        //Increments curHallLength
        curHallLength++;
    }
}
