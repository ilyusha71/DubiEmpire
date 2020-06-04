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

        [HeaderAttribute ("Ready")]
        private int orderSelected;
        private List<Unit.BattleModel> targetList;

        bool quickBattle = false;
        bool finish = false;
        int wave = 0, maxWave = 5, action = 0, maxAction = 30;
        float nextActionRearrangeTime;
        float nextActionFireTime;
        float nextActionResultTime;

        public enum State
        {
            Deploy = 0,
            Ready = 10,
            Aim = 11,
            Fighting = 20,
            Finish = 30,
        }
        State state = State.Deploy;

        void Awake ()
        {
            warfare.MasterModelCollector ();
            warfare.SynchronizeLegionsToPlayerData ();
            warfare.SynchronizeUnitsToPlayerData ();
        }

        public void Initialize (int[] index, bool quickBattle)
        {
            for (int side = 0; side < 2; side++)
            {
                Dictionary<int, Unit.Data> data = warfare.playerData.legions[index[side]].squadron;;
                Dictionary<int, Unit.BattleModel> squadron = new Dictionary<int, Unit.BattleModel> ();
                for (int order = 0; order < 13; order++)
                {
                    if (data.ContainsKey (order))
                    {
                        Unit.BattleModel unit = new Unit.BattleModel (order, warfare.unitModels[data[order].Type], data[order]);
                        squadron.Add (order, unit);
                    }
                }
                legions[side] = new Legion.BattleModel (squadron);
            }
            this.quickBattle = quickBattle;
            if (quickBattle)
                QuickBattle ();
            else
                FormUp ();
        }

        void QuickBattle ()
        {
            for (wave = 1; wave <= maxWave; wave++)
            {
                for (action = 0; action < maxAction; action += 0)
                {
                    Rearrange ();
                    Fire ();
                    ActionResult ();
                    if (finish)
                    {
                        FormUp ();
                        return;
                    }
                }
            }
            if (FormUp ()) state = State.Ready;
            quickBattle = false;
            finish = false;
            wave = 0;
            action = 0;
        }

        bool FormUp ()
        {
            for (int side = 0; side < 2; side++)
            {
                for (int order = 0; order < 17; order++)
                {
                    int index = side * 17 + order;
                    if (state == State.Deploy)
                        grids[index].Disarmament ();
                    if (legions[side].squadron.ContainsKey (order))
                    {
                        grids[index].Ready (side + 100, order);
                        if (state == State.Deploy)
                            grids[index].Deploy (legions[side].squadron[order]);
                    }
                    else
                        grids[index].Disable (order);
                }
                legions[side].Rearrange (wave);
            }
            state = State.Ready;
            wave++;
            action = 0;
            return true;
        }

        void Update ()
        {
            if (Input.GetKeyDown (KeyCode.B) && state == State.Ready)
                Fight ();
            if (Input.GetKeyDown (KeyCode.F5))
                Initialize (new int[] { 2, 3 }, true);
            if (Input.GetKeyDown (KeyCode.F6))
                Initialize (new int[] { 2, 3 }, false);

            if (Input.GetMouseButtonDown (0))
            {
                Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
                RaycastHit hit;
                if (Physics.Raycast (ray, out hit))
                {
                    Warfare.GridManager grid = hit.transform.GetComponent<Warfare.GridManager> ();
                    if (grid)
                    {
                        if (state == State.Ready && grid.state == GridState.Friend)
                        {
                            orderSelected = grid.Order;
                            if (grid.Order > 10)
                                targetList = legions[1].rangeList[3];
                            else if (grid.Order > 8)
                                targetList = legions[1].rangeList[4];
                            else
                                targetList = legions[1].rangeList[(int) grid.unit.model.Range];

                            int count = targetList.Count;
                            for (int i = 0; i < count; i++)
                            {
                                grids[17 + targetList[i].order].Aim ();
                            }
                            state = State.Aim;
                        }
                        else if (state == State.Aim && grid.isTarget)
                        {
                            legions[0].squadron[orderSelected].target = legions[1].squadron[grid.Order];
                            int count = targetList.Count;
                            for (int i = 0; i < count; i++)
                            {
                                grids[17 + targetList[i].order].Ready ();
                            }
                            state = State.Ready;
                        }
                    }
                }
            }

            if (state == State.Fighting)
            {
                tTime.text = action.ToString ();
                if (Time.time > nextActionRearrangeTime)
                    Rearrange ();
                if (Time.time > nextActionFireTime)
                    Fire ();
                if (Time.time > nextActionResultTime)
                    ActionResult ();
            }
        }

        void Fight ()
        {
            state = State.Fighting;
            nextActionRearrangeTime = Time.time + 0.9f;
            nextActionFireTime = Time.time + 1f;
            nextActionResultTime = Time.time + 1.2f;
            for (int side = 0; side < 2; side++)
            {
                for (int order = 0; order < 17; order++)
                {
                    if (legions[side].squadron.ContainsKey (order))
                        grids[side * 17 + order].Battle ();
                }
            }
        }
        void Rearrange ()
        {
            nextActionRearrangeTime = Time.time + 1f;
            for (int side = 0; side < 2; side++)
            {
                legions[side].Rearrange (wave);
            }
            action++;
        }
        void Fire ()
        {
            nextActionFireTime = Time.time + 1f;
            for (int side = 0; side < 2; side++)
            {
                for (int order = 0; order < 13; order++)
                {
                    if (legions[side].squadron.ContainsKey (order))
                        if (legions[side].squadron[order].Fire (action, legions[1 - side].rangeList) && !quickBattle)
                            grids[side * 17 + order].Fire ();
                }
            }
        }
        void ActionResult ()
        {
            nextActionResultTime = Time.time + 1f;
            for (int side = 0; side < 2; side++)
            {
                for (int order = 0; order < 13; order++)
                {
                    if (legions[side].squadron.ContainsKey (order))
                    {
                        int countDestroy = 0;
                        int countHit = 0;
                        Unit.Range maxRange = Unit.Range.Near;
                        Unit.BattleModel unit = legions[side].squadron[order];
                        // Debug.LogWarning(wave + " / " + action + " / " + side + " / " + order);
                        if (unit.ActionResult (out maxRange, out countDestroy, out countHit))
                        {
                            if (!quickBattle)
                                grids[side * 17 + order].Hit (maxRange, countDestroy, countHit);
                            if (unit.data.HP == 0)
                            {
                                legions[side].squadron.Remove (order);
                                if (!quickBattle)
                                    grids[side * 17 + order].Disable (order);
                            }
                        }
                    }
                }
                if (legions[side].squadron.Count == 0)
                {
                    finish = true;
                    state = State.Finish;
                    return;
                }
            }
            if (action == maxAction)
                FormUp ();
        }
    }
}