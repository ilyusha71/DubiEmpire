using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Warfare
{
    public class GridManager : MonoBehaviour
    {
        public GridState state;
        [HeaderAttribute("Battle")]
        public GridManager target;
        public List<Unit.Squadron> attackers = new List<Unit.Squadron>();
        [HeaderAttribute("Unit")]
        public Unit.Squadron unit;
        public Dictionary<int, GameObject> stacks = new Dictionary<int, GameObject>();
        public List<int> index = new List<int>(); // 無序
        public List<int> order = new List<int>(); // 有序

        [HeaderAttribute("Base")]
        public Unit.Database database;
        public Color32 orange = new Color32(227, 79, 0, 255);
        public Color32 gray97 = new Color32(97, 97, 97, 255);

        private Color32 targetColor;
        private Color32 enterColor;
        private Color32 exitColor;

        private SpriteRenderer gridSprite;
        public int Index { get; set; }

        void Awake()
        {
            gridSprite = GetComponentInChildren<SpriteRenderer>();
            enterColor = orange;
            if (state == GridState.Deploy)
                exitColor = gray97;
            else
            {
                GetComponent<MeshRenderer>().enabled = false;
                exitColor = Color.clear;
                targetColor = Color.red;
            }
            gridSprite.color = exitColor;
        }

        public bool Deploy(Unit.Squadron unit)
        {
            if (state != GridState.Deploy)
            {
                GetComponent<MeshRenderer>().enabled = true;
                exitColor = gray97;
                gridSprite.color = exitColor;
            }
            this.unit = unit;
            if (unit.model.m_type == Unit.Type.None) return false;
            int[] array = new int[unit.stack];
            int[] array2 = new int[unit.model.m_formation.Length];
            for (int i = 0; i < array2.Length; i++)
            {
                array2[i] = 1; // 取得權重
            }
            for (int j = 0; j < array.Length; j++)
            {
                int lotteryIndex = unit.model.m_base == Unit.Base.Dubi ? GetLotteryIndex(array2) : j;
                if (state == GridState.Foe)
                    stacks.Add(lotteryIndex, database.units[unit.model.m_type].GetWarfareUnit(lotteryIndex, transform.position, 180));
                else
                    stacks.Add(lotteryIndex, database.units[unit.model.m_type].GetWarfareUnit(lotteryIndex, transform.position));
                array2[lotteryIndex] = 0; // 抽中後將權重改為0
            }
            UpdateList();
            return true;
        }
        void UpdateList()
        {
            index.Clear();
            order.Clear();
            Dictionary<int, GameObject> dic1Asc = stacks.OrderBy(o => o.Key).ToDictionary(o => o.Key, p => p.Value);
            index = stacks.Keys.ToList();
            order = dic1Asc.Keys.ToList();
        }
        public static int GetLotteryIndex(int[] rates)
        {
            if (rates == null)
            {
                return -1;
            }
            int num = 0;
            for (int i = 0; i < rates.Length; i++)
            {
                num += rates[i];
            }
            int num2 = Random.Range(1, num + 1);
            for (int j = 0; j < rates.Length; j++)
            {
                num2 -= rates[j];
                if (num2 <= 0)
                {
                    return j;
                }
            }
            return rates.Length - 1;
        }
        public void Disarmament()
        {
            List<GameObject> list = stacks.Values.ToList();
            stacks.Clear();
            for (int i = list.Count - 1; i >= 0; i--)
            {
                GameObject go = list[i];
                Destroy(go);
            }
            unit = null;
        }
        // void OnMouseDown ()
        // {
        //     // Debug.Log (transform.name);
        // }
        // void OnMouseUpAsButton ()
        // {
        //     // Debug.Log (transform.name + "--- up");

        // }
        void OnMouseEnter()
        {
            if (state == GridState.Disable) return;
            gridSprite.color = enterColor;
        }
        void OnMouseExit()
        {
            if (state == GridState.Disable) return;
            gridSprite.color = exitColor;
        }

        public void Fire(int timer)
        {
            if (stacks.Count == 0) return;
            if (timer % unit.model.m_fire != 0) return;
            UpdateList();
            target.attackers.Add(unit);
            for (int i = 0; i < stacks.Count; i++)
            {
                if (stacks[index[i]].GetComponentInChildren<EffectController>())
                    stacks[index[i]].GetComponentInChildren<EffectController>().Fire();
            }
        }
        public void Hit()
        {
            if (attackers.Count == 0) return;
            UpdateList();
            int totalDamage = 0;
            int totalAttackers = 0;
            Unit.Range range = Unit.Range.Near;
            for (int i = 0; i < attackers.Count; i++)
            {
                totalDamage += attackers[i].TotalDamage;
                totalAttackers += attackers[i].stack;
                if (attackers[i].model.m_range > range)
                    range = attackers[i].model.m_range;
            }
            unit.hp = Mathf.Max(0, unit.hp - totalDamage);
            // 先記錄陣亡位置
            int countByDestroy = stacks.Count - unit.stack;
            Debug.LogWarning(unit.model.m_type.ToString() + " / " + countByDestroy + " / " + stacks.Count + " / " + totalDamage + " / " + unit.hp);

            // int countByDamage = Mathf.Min (stacks.Count, Mathf.CeilToInt ((float) totalDamage / (unit.model.m_hp)));

            List<int> listDestroy = new List<int>();
            for (int i = 0; i < countByDestroy; i++)
            {
                if (range == Unit.Range.Far)
                {
                    // Debug.LogWarning (unit.model.m_type.ToString () + " / " + i + " : " + index[i] + " : " + index.Count);
                    listDestroy.Add(index[0]);
                    index.RemoveAt(0);
                }
                else
                {
                    // Debug.LogWarning (unit.model.m_type.ToString () + " / " + i + " : " + order[0] + " : " + order.Count);
                    listDestroy.Add(order[0]);
                    order.RemoveAt(0);
                }
            }
            // 需要補Hit特效
            if (totalAttackers > countByDestroy)
            {
                if (range == Unit.Range.Far)
                {
                    int count = Mathf.Min(index.Count, totalAttackers - countByDestroy);
                    for (int i = 0; i < count; i++)
                    {
                        if (stacks[index[i]].GetComponentInChildren<EffectController>())
                            stacks[index[i]].GetComponentInChildren<EffectController>().Hit();
                    }
                }
                else
                {
                    int count = Mathf.Min(order.Count, totalAttackers - countByDestroy);
                    for (int i = 0; i < count; i++)
                    {
                        if (stacks[order[i]].GetComponentInChildren<EffectController>())
                            stacks[order[i]].GetComponentInChildren<EffectController>().Hit();
                    }
                }
            }
            // 陣亡
            for (int i = 0; i < listDestroy.Count; i++)
            {
                GameObject go = stacks[listDestroy[i]];
                stacks.Remove(listDestroy[i]);
                Destroy(go);
            }
            Debug.Log(unit.model.m_type.ToString() + " / " + countByDestroy + " / " + stacks.Count);
            attackers.Clear();
        }
    }
    public enum GridDefinition
    {
        Front = 0,
        Middle = 1,
        Back = 2,
        Left = 100,
        Right = 200
    }
    public enum GridState
    {
        Deploy = 0,
        Disable = -1,
        Friend = 100,
        Foe = 101,
    }
}