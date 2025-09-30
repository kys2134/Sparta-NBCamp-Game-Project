using System;
using System.Collections;
using Backend.Util.Data.ActionDatas;
using Backend.Util.Debug;

using UnityEngine;

namespace Backend.Object.Character.Enemy.Boss.Skill
{
    public abstract class BossSkillBase : MonoBehaviour
    {
        [field: SerializeField] public ActionBossData SkillData { get; protected set; }

        [field: SerializeField] protected GameObject projectilePrefab;
        [field: SerializeField] protected Transform[] projectileTransform;
    }
}
