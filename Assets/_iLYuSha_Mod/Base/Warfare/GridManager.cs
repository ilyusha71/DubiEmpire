using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Warfare
{
    public class GridManager : MonoBehaviour
    {
        private SpriteRenderer gridSprite;
        public Database database;
        public Color32 enter = new Color32(227, 79, 0, 255);
        public Color32 exit = new Color32(97, 97, 97, 255);
        public List<GameObject> listUnits = new List<GameObject>();
        // private int maxCount;
        void Awake()
        {
            gridSprite = GetComponentInChildren<SpriteRenderer>();
            gridSprite.color = exit;
        }
        public void Deploy()
        {
            Unit.Data data = database.units[Unit.Type.MiniDorara];
            int stack = data.GetStackCount(840);
            for (int i = 0; i < stack; i++)
            {
                data.GetWarfareUnit(transform.position, i);

            }
            // switch (unit.type)
            // {
            //     case Unit.Type.Energy:
            //     case Unit.Type.RedBullEnergy:
            //     case Unit.Type.MonsterEnergy:

            //         // listUnits.Add(Instantiate());
            //         break;
            //     case Unit.Type.Ceti:
            //     case Unit.Type.Putin:
            //     case Unit.Type.Kells:
            //         break;
            // }
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
        void OnMouseEnter()
        {
            gridSprite.color = enter;
        }
        void OnMouseExit()
        {
            gridSprite.color = exit;
        }
        private Vector3[] formation101 = {
            new Vector3 (2, 0, 2),
            new Vector3 (-2, 0, -2),
        };
    }
}