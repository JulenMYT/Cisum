using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewEnemy", menuName = "Stats/Enemy")]
public class EnemyScriptableObject : ScriptableObject
{
    public string enemyName;
    public int level;
    public int hp;
    public int offense;
    public int defense;
    public int speed;
    public int experiencePoints;
    [TextArea]
    public string encounterText;
    [TextArea]
    public string deathText;
    public string trackName;
    public AudioClip deathSound;
    public Item dropItem;

    public ActionOrder actionOrder;

    public enum ActionOrder
    {
        RANDOM = 0,
        WEIGHTED_RANDOM = 1,
        CYCLE = 2,
        STAGGERED = 3
    }

    [System.Serializable]
    public class EnemyAction
    {
        public int actionID;
        public string actionName;
        public int argument;
        [TextArea]
        public string attackMessage;
        public float hitTiming;
        public string attackTrigger;
        public AudioClip audioClip;
    }

    public List<EnemyAction> actions;
}
