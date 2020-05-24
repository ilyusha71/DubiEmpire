using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Warfare
{
    public class BattlefieldManager : MonoBehaviour
    {
        public WarfareManager warfare;
        public GridManager[] gridsFriend;
        public GridManager[] gridsFoe;
        public GridManager wall;

        void Awake()
        {
            warfare.playerData.battlefield[0] = 0;
            warfare.playerData.battlefield[1] = 1;
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
        }

        // Update is called once per frame
        void Update()
        {

        }
    }

}