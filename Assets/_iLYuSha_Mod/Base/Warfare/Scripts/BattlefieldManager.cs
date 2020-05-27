﻿using System.Collections;
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
        [HeaderAttribute("Quick Battle")]
        public int action;
        public GridManager[] grids;
        public GridManager[] grids1;
        public GridManager[] grids2;

        public void QuickBattle()
        {
            Unit.Squadron[] legionA = new Unit.Squadron[17], legionB = new Unit.Squadron[17];
            // 讀資料

            // 開戰
            for (int wave = 0; wave < 3; wave++)
            {
                for (action = 0; action <= 20; action++)
                {
                    // 定義位置
                    
                    for (int index = 0; index < 2; index++)
                    {
                        Unit.Squadron[] legion = index == 0 ? legionA : legionB;
                        for (int range = 0; range < 3; range++)
                        {
                            for (int order = 0; order < 3; order++)
                            {
                                if (legion[range * 3 + order].model != null)
                                {
                                    
                                    break;
                                }
                            }
                        }
                    }
                    //開火
                    for (int i = 0; i < 17; i++)
                    {

                    }
                    // 結算
                    for (int i = 0; i < 17; i++)
                    {

                    }
                    // 是否提前結束
                }

                // 第一波或第二波結束
                if (wave != 2)
                {

                }
            }
        }

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
                    Unit.Squadron unit = new Unit.Squadron();
                    unit.model = data.model;
                    unit.hp = legion.m_squadron[j].HP;

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

            dmgInterval = 0.01f;
            fireInterval = 0.05f - dmgInterval;
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