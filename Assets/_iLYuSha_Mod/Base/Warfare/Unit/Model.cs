using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/***************************
 * 文本 - 創造unitModel
 *
 *
 ***************************/
namespace Warfare.Unit
{
    public class Model
    {
        public int faction, legion, squadron ; //100101 = 10 01 01 = faction legion team
        public Type type;
        public int hp, level, exp;
    }
}