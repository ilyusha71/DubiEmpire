using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Warfare
{
    public class BattleModel : MonoBehaviour
    {
        public Text tTime;
        public WarfareManager warfare;
        public Legion.BattleModel[] legions = new Legion.BattleModel[2];
        public GridManager[] grids;

        bool quickBattle = false;
        bool finish = false;
        bool isFighting = false;
        int wave = 0, maxWave = 5, action = 0, maxAction = 30;
        float nextActionRearrangeTime;
        float nextActionFireTime;
        float nextActionResultTime;

        void Awake()
        {
            warfare.SynchronizeLegionsToPlayerData();
            warfare.SynchronizeUnitsToPlayerData();
        }

        void Start()
        {

        }

        public void Initialize(int rightSide, int leftSide, bool quickBattle)
        {
            legions[0] = new Legion.BattleModel(warfare.units, warfare.playerData.legions[rightSide].squadron);
            legions[1] = new Legion.BattleModel(warfare.units, warfare.playerData.legions[leftSide].squadron);
            this.quickBattle = quickBattle;
            if (quickBattle)
                QuickBattle();
            else
                Deploy();
        }

        void QuickBattle()
        {
            for (wave = 1; wave <= maxWave; wave++)
            {
                for (action = 0; action < maxAction; action += 0)
                {
                    Rearrange();
                    Fire();
                    ActionResult();
                    if (finish)
                    {
                        Deploy();
                        return;
                    }
                }
            }
            Deploy();
            quickBattle = false;
            finish = false;
            wave = 0;
            action = 0;
        }

        void Deploy()
        {
            for (int side = 0; side < 2; side++)
            {
                for (int i = 0; i < 17; i++)
                {
                    int index = side * 17 + i;
                    grids[index].Disarmament();
                    grids[index].Order = i;
                    if (legions[side].squadron.ContainsKey(i))
                    {
                        grids[index].Ready();
                        if (side == 0)
                            grids[index].state = GridState.Friend;
                        else
                            grids[index].state = GridState.Foe;
                        grids[index].Deploy(legions[side].squadron[i].data, warfare.units[legions[side].squadron[i].data.Type]);

                    }
                    else
                    {
                        grids[index].Disable();
                        grids[index].state = GridState.Disable;
                    }

                }
            }
        }

        // Update is called once per frame
        void Update()
        {
            if (Input.GetKeyDown(KeyCode.B) && !isFighting)
                Fight();
            if (Input.GetKeyDown(KeyCode.F5))
                Initialize(2, 3, true);
            if (Input.GetKeyDown(KeyCode.F6))
                Initialize(2, 3, false);
            if (isFighting)
            {
                tTime.text = action.ToString();
                if (action > maxAction)
                {
                    isFighting = false;
                    for (int side = 0; side < 2; side++)
                    {
                        for (int order = 0; order < 17; order++)
                        {
                            if (legions[side].squadron.ContainsKey(order))
                                grids[side * 17 + order].Ready();
                            else
                                grids[side * 17 + order].Disable();
                        }
                    }
                    return;
                }
                if (Time.time > nextActionRearrangeTime)
                    Rearrange();
                if (Time.time > nextActionFireTime)
                    Fire();
                if (Time.time > nextActionResultTime)
                    ActionResult();
            }
        }

        void Fight()
        {
            isFighting = true;
            wave++;
            action = 0;
            nextActionRearrangeTime = Time.time + 0.9f;
            nextActionFireTime = Time.time + 1f;
            nextActionResultTime = Time.time + 1.2f;
            for (int side = 0; side < 2; side++)
            {
                for (int order = 0; order < 17; order++)
                {
                    if (legions[side].squadron.ContainsKey(order))
                        grids[side * 17 + order].Battle();
                    else
                        grids[side * 17 + order].Disable();
                }
            }
        }
        void Rearrange()
        {
            nextActionRearrangeTime = Time.time + 1f;
            for (int side = 0; side < 2; side++)
            {
                legions[side].Rearrange(wave);
            }
            action++;
        }
        void Fire()
        {
            nextActionFireTime = Time.time + 1f;
            for (int side = 0; side < 2; side++)
            {
                for (int order = 0; order < 13; order++)
                {
                    if (legions[side].squadron.ContainsKey(order))
                        if (legions[side].squadron[order].Fire(action, legions[1 - side].rangeList) && !quickBattle)
                            grids[side * 17 + order].Fire();
                }
            }
        }
        void ActionResult()
        {
            nextActionResultTime = Time.time + 1f;
            for (int side = 0; side < 2; side++)
            {
                for (int order = 0; order < 13; order++)
                {
                    if (legions[side].squadron.ContainsKey(order))
                    {
                        int countDestroy = 0;
                        int countHit = 0;
                        Unit.Range maxRange = Unit.Range.Near;
                        Unit.BattleModel unit = legions[side].squadron[order];
                        // Debug.LogWarning(wave + " / " + action + " / " + side + " / " + order);
                        if (unit.ActionResult(out maxRange, out countDestroy, out countHit))
                        {
                            if (!quickBattle)
                                grids[side * 17 + order].Hit(maxRange, countDestroy, countHit);
                            if (unit.data.HP == 0)
                            {
                                legions[side].squadron.Remove(order);
                                if (!quickBattle)
                                    grids[side * 17 + order].Disable();
                            }
                        }
                    }
                }
                if (legions[side].squadron.Count == 0)
                {
                    finish = true;
                    isFighting = false;
                    return;
                }
            }
        }
    }
}