using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Warfare.Legion
{
     [System.Serializable]
    public class Squadron
    {
        public int faction, legion, squadron ; //100101 = 10 01 01 = faction legion team
        public Unit.Type type;
        public int hp, level, exp;
    }
}