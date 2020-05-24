using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Warfare
{
    public class BattlefieldManager : MonoBehaviour
    {
        public WarfareManager warfare;
        public GridManager[] gridsFriend;
        public GridManager[] gridsFoe;
        public GridManager wall;

        [HeaderAttribute("Battle")]
        public bool isFighting;
        public bool isFiring;
        public int wave;
        public int timer; // 以秒為單位的計數器
        public float actionTime; // 每次行動判斷的時間
        private float dmgInterval;
        private float fireInterval;
        public Text tTime;

        void Awake()
        {
            warfare.playerData.battlefield[0] = 2;
            warfare.playerData.battlefield[1] = 3;
            for (int i = 0; i < 2; i++)
            {
                Legion.Data legion = warfare.legionDB.legions[warfare.playerData.battlefield[i]];
                for (int j = 0; j < 13; j++)
                {
                    Unit.Type type = legion.m_squadron[j].type;
                    if (type == Unit.Type.None)
                        continue;
                    Unit.Data data = warfare.unitDB.units[type];
                    Unit.Model unit = new Unit.Model();
                    unit.type = type;
                    unit.hp = legion.m_squadron[j].HP;
                    unit.stack = data.GetStackCount(unit.hp);
                    unit.fire = data.m_fire;
                    unit.atk = data.m_atk;
                    if (i == 0)
                    {
                        gridsFriend[j].state = GridState.Friend;
                        gridsFriend[j].Deploy(unit);
                    }
                    else
                    {
                        gridsFoe[j].state = GridState.Foe;
                        gridsFoe[j].Deploy(unit);
                    }
                }
            }


            dmgInterval = 0.2f;
            fireInterval = 1 - dmgInterval;
        }

        void Update()
        {
            if (isFighting)
            {
                if (Time.time > actionTime)
                {
                    if (timer == 20) // 此波戰鬥結束
                    {
                        isFighting = false;
                    }
                    if (isFiring)
                    {
                        timer++;
                        tTime.text = timer.ToString();
                        actionTime += dmgInterval;

                        for (int i = 0; i < 17; i++)
                        {
                            if (gridsFriend[i])
                                gridsFriend[i].Fire(timer);
                            if (gridsFoe[i])
                                gridsFoe[i].Fire(timer);
                        }
                    }
                    else
                    {
                        actionTime += fireInterval;

                        for (int i = 0; i < 17; i++)
                        {
                            if (gridsFriend[i])
                                gridsFriend[i].Hit();
                            if (gridsFoe[i])
                                gridsFoe[i].Hit();
                        }
                    }
                    isFiring = !isFiring;
                }
            }
            if (Input.GetKeyDown(KeyCode.B))
            {
                isFighting = true;
                wave++;
                timer = 0;
                actionTime = Time.time + 1.0f;

                gridsFriend[4].target = gridsFoe[4];
                gridsFoe[4].target = gridsFriend[4];

            }




            if (Input.GetKeyDown(KeyCode.F))
            {
                for (int i = 0; i < 13; i++)
                {
                    gridsFriend[i].Fire(0);
                }
                for (int i = 0; i < 13; i++)
                {
                    gridsFoe[i].Fire(0);
                }
            }
            if (Input.GetKeyDown(KeyCode.H))
            {
                for (int i = 0; i < 13; i++)
                {
                    gridsFriend[i].Hit();
                }
                for (int i = 0; i < 13; i++)
                {
                    gridsFoe[i].Hit();
                }
            }
        }
    }

}