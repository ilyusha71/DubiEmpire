using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Warfare
{
    public class GridManager : MonoBehaviour
    {
        public Database database;
        public Color32 enter = new Color32 (227, 79, 0, 255);
        public Color32 exit = new Color32 (97, 97, 97, 255);
        public Unit.Model m_unit;
        private SpriteRenderer gridSprite;

        public List<GameObject> listUnits = new List<GameObject> ();
        // private int maxCount;
        void Awake ()
        {
            gridSprite = GetComponentInChildren<SpriteRenderer> ();
            gridSprite.color = exit;
        }
        public void Deploy (Unit.Model unit)
        {
            // 先判斷是否 Deploy

            Unit.Data data = database.units[unit.type];
            int stackCount = data.GetStackCount (unit.hp);
            for (int i = 0; i < stackCount; i++)
            {
                data.GetWarfareUnit (transform.position, i);
            }
        }
        public void Disarmament ()
        {

        }
        // public bool AddUnit (UnitModel unit)
        // {
        //     if (listUnits.Count == 0)
        //     {
        //         listUnits.Add (unit);
        //         // maxCouUnitnt == 
        //     }
        //     else if (listUnits.Count == maxCount)
        //         return false;
        //     else
        //     {
        //         listUnits.Add (unit);
        //     }
        //     return false;
        // }

        // void OnMouseDown ()
        // {
        //     // Debug.Log (transform.name);
        // }
        // void OnMouseUpAsButton ()
        // {
        //     // Debug.Log (transform.name + "--- up");

        // }
        void OnMouseEnter ()
        {
            gridSprite.color = enter;
        }
        void OnMouseExit ()
        {
            gridSprite.color = exit;
        }
        private Vector3[] formation101 = {
            new Vector3 (2, 0, 2),
            new Vector3 (-2, 0, -2),
        };
    }
}