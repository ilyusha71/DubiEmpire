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
        public Unit.Model attacker;
        [HeaderAttribute("Unit")]
        public Unit.Model unit;
        public Dictionary<int, GameObject> listStack = new Dictionary<int, GameObject>();

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

        public bool Deploy(Unit.Model unit)
        {
            if (state != GridState.Deploy)
            {
                GetComponent<MeshRenderer>().enabled = true;
                exitColor = gray97;
                gridSprite.color = exitColor;
            }
            Unit.Data data = database.units[unit.type];
            this.unit = unit;
            if (unit.type == Unit.Type.None) return false;
            int[] array = new int[unit.stack];
            int[] array2 = new int[data.m_formation.Length];
            for (int i = 0; i < array2.Length; i++)
            {
                array2[i] = 1; // 取得權重
            }
            for (int j = 0; j < array.Length; j++)
            {
                int lotteryIndex = ((int)unit.type < 2000) ? j : GetLotteryIndex(array2);
                if (state == GridState.Foe)
                    listStack.Add(lotteryIndex, data.GetWarfareUnit(lotteryIndex, transform.position, 180));
                else
                    listStack.Add(lotteryIndex, data.GetWarfareUnit(lotteryIndex, transform.position));
                array2[lotteryIndex] = 0; // 抽中後將權重改為0
            }
            Dictionary<int, GameObject> dic1Asc = listStack.OrderBy(o => o.Key).ToDictionary(o => o.Key, p => p.Value);
            listStack = dic1Asc;
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
        public void Disarmament()
        {
            List<GameObject> list = listStack.Values.ToList();
            listStack.Clear();
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
            if (listStack.Count == 0) return;
            if (timer % unit.fire != 0) return;
            target.attacker = unit;
            List<GameObject> list = listStack.Values.ToList();
            for (int i = 0; i < list.Count; i++)
            {
                if (listStack[i].GetComponentInChildren<EffectController>())
                    listStack[i].GetComponentInChildren<EffectController>().Fire();
            }
        }
        public void Hit()
        {
            if (attacker == null) return;
            List<GameObject> list = listStack.Values.ToList();


            int countByDamge = Mathf.CeilToInt((float)attacker.TotalDamage / (unit.hp * unit.stack));
            unit.hp -= attacker.TotalDamage;
            // int countByStack = unit.



            for (int i = 0; i < list.Count; i++)
            {
                if(i<countByDamge)
                
                if (listStack[i].GetComponentInChildren<EffectController>())
                    listStack[i].GetComponentInChildren<EffectController>().Hit();
            }
            attacker = null;
        }
    }
    public enum GridState
    {
        Deploy = 0,
        Disable = -1,
        Friend = 100,
        Foe = 101,
    }
}