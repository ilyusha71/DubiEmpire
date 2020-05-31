using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Warfare
{
    public class GridManager : MonoBehaviour
    {
        public GridState state;
        [HeaderAttribute("Battle")]
        public GridManager target;
        public List<Legion.Squadron> attackers = new List<Legion.Squadron>();
        public List<int> index = new List<int>(); // 無序
        public List<int> order = new List<int>(); // 有序
        [HeaderAttribute("Unit")]
        public Unit.DataModel data;
        public Unit.MasterModel model;
        public Dictionary<int, GameObject> stacks = new Dictionary<int, GameObject>();

        public Legion.Squadron unit;


        [HeaderAttribute("Grid")]
        public Color32 orange = new Color32(227, 79, 0, 255);
        public Color32 gray97 = new Color32(97, 97, 97, 255);

        private Color32 targetColor;
        private Color32 enterColor;
        private Color32 exitColor;

        private MeshRenderer gridRender;
        private SpriteRenderer gridSprite;
        public int Order { get; set; }

        [HeaderAttribute("UI")]
        public Image avatar;
        public TextMeshProUGUI textType, textHP, textCount, textDubi, textMech, textAir;


        void Awake()
        {
            // avatar.sprite = null;
            textType.text = "";
            textHP.text = "";
            textCount.text = "";
            textDubi.text = "";
            textMech.text = "";
            textAir.text = "";


            gridRender = GetComponent<MeshRenderer>();
            gridSprite = GetComponentInChildren<SpriteRenderer>();
            enterColor = orange;
            if (state == GridState.Deploy)
                exitColor = gray97;
            else
            {
                gridRender.enabled = false;
                exitColor = Color.clear;
                targetColor = Color.red;
            }
            gridSprite.color = exitColor;
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
            if (data == null || data.HP == 0) return;
            avatar.sprite = model.Sprite;
            textType.text = Property.Type(data.Type);
            textHP.text = data.HP.ToString();
            textCount.text = model.UnitCount(data.HP).ToString();
            textDubi.text = (model.UnitCount(data.HP) * model.ATK[0]).ToString();
            textMech.text = (model.UnitCount(data.HP) * model.ATK[1]).ToString();
            textAir.text = (model.UnitCount(data.HP) * model.ATK[2]).ToString();
        }
        void OnMouseOver()
        {
            if (data == null || data.HP == 0) return;
            textHP.text = data.HP.ToString();
            textCount.text = model.UnitCount(data.HP).ToString();
            textDubi.text = (model.UnitCount(data.HP) * model.ATK[0]).ToString();
            textMech.text = (model.UnitCount(data.HP) * model.ATK[1]).ToString();
            textAir.text = (model.UnitCount(data.HP) * model.ATK[2]).ToString();
        }
        void OnMouseExit()
        {
            if (state == GridState.Disable) return;
            gridSprite.color = exitColor;
        }
        public void Ready()
        {
            gridRender.enabled = true;
            exitColor = gray97;
            gridSprite.color = exitColor;
        }
        public void Disable()
        {
            gridRender.enabled = false;
            gridSprite.enabled = false;
        }
        public void Battle()
        {
            gridRender.enabled = false;
            exitColor = Color.clear;
            gridSprite.color = exitColor;
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
            data = null;
            model = null;
        }
        public bool Deploy(Unit.DataModel data, Unit.MasterModel model)
        {
            if (state != GridState.Deploy)
            {
                gridRender.enabled = true;
                exitColor = gray97;
                gridSprite.color = exitColor;
            }
            this.data = data;
            this.model = model;
            int[] array = new int[model.UnitCount(data.HP)]; // 目前數量
            int[] array2 = new int[model.Formation.Length]; // 最大數量
            for (int i = 0; i < array2.Length; i++)
            {
                array2[i] = 1; // 取得權重，各位置權重相同
            }
            for (int j = 0; j < array.Length; j++)
            {
                int lotteryIndex = model.Field == Unit.Field.Dubi ? GetLotteryIndex(array2) : j; // 只有Dubi要抽位置

                if (state == GridState.Foe)
                    stacks.Add(lotteryIndex, Instantiate(model.Instance, transform.position + model.Formation[lotteryIndex] * 1, Quaternion.Euler(0, 180, 0)));
                else
                    stacks.Add(lotteryIndex, Instantiate(model.Instance, transform.position + model.Formation[lotteryIndex] * 1, Quaternion.identity));
                array2[lotteryIndex] = 0; // 抽中後將權重改為0
            }
            if (state != GridState.Deploy)
                UpdateList();
            return true;
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
        void UpdateList()
        {
            index.Clear();
            order.Clear();
            Dictionary<int, GameObject> dic1Asc = stacks.OrderBy(o => o.Key).ToDictionary(o => o.Key, p => p.Value);
            index = stacks.Keys.ToList();
            order = dic1Asc.Keys.ToList();
        }








        public void Fire()
        {
            UpdateList();
            for (int i = 0; i < stacks.Count; i++)
            {
                // if (stacks[index[i]].GetComponentInChildren<EffectController>())
                stacks[index[i]].GetComponentInChildren<EffectController>().Fire();
                if (i < 2)
                    stacks[index[i]].GetComponentInChildren<EffectController>().FireSound();

            }
        }

        public void Hit(Unit.Range range, int countDestroy, int countHit)
        {
            UpdateList();
            List<int> listDestroy = new List<int>();
            for (int i = 0; i < countDestroy; i++)
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
            if (range == Unit.Range.Far)
            {
                for (int i = 0; i < countHit; i++)
                {
                    // if (stacks[index[i]].GetComponentInChildren<EffectController>())
                    stacks[index[i]].GetComponentInChildren<EffectController>().Hit();
                }
            }
            else
            {
                for (int i = 0; i < countHit; i++)
                {
                    // if (stacks[order[i]].GetComponentInChildren<EffectController>())
                    stacks[order[i]].GetComponentInChildren<EffectController>().Hit();
                }
            }
            // 陣亡
            for (int i = 0; i < listDestroy.Count; i++)
            {
                GameObject go = stacks[listDestroy[i]];
                stacks.Remove(listDestroy[i]);
                Destroy(go);
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
                totalDamage += attackers[i].TotalDamage(unit.model.Field);
                totalAttackers += attackers[i].UnitCount;
                if (attackers[i].model.Range > range)
                    range = attackers[i].model.Range;
            }
            unit.hp = Mathf.Max(0, unit.hp - totalDamage);
            // 先記錄陣亡位置
            int countByDestroy = stacks.Count - unit.UnitCount;
            Debug.LogWarning(unit.model.Type.ToString() + " / " + countByDestroy + " / " + stacks.Count + " / " + totalDamage + " / " + unit.hp);

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
            Debug.Log(unit.model.Type.ToString() + " / " + countByDestroy + " / " + stacks.Count);
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