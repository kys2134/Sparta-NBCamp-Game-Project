using Backend.Util.Data.ActionDatas;
using UnityEngine;

[System.Serializable]
public class Blackboard
{
    public float attackTimeCounter = 0f;

    public bool isHit = false;

    public bool playerOnLeft = false; // false means player is on the right

    public int currentAnimationHash = 0;

    public float currentAnimationDuration = 0f;

    public Vector3 moveDestination = Vector3.zero;

    public RangeCheck RangeCheck = RangeCheck.None; // 현재 체크한 공격 범위

    public bool IsBackstepping;

    public bool IsParry = false;
}
