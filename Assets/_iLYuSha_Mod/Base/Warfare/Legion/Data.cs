using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Warfare.Legion
{
    [CreateAssetMenu (fileName = "Data", menuName = "Warfare/Legion/Create Warfare Legion Data")]
    public class Data : ScriptableObject
    {
        public int m_index;
        public Faction m_faction;
        public int m_legion;
        public Squadron[] m_squadron = new Squadron[13];

        public void SetIndex ()
        {
            m_index = (int) m_faction * 100 + m_legion;
        }
    }

    [System.Serializable]
    public class Squadron
    {
        public Texture m_texture;
        [Range (0f, 1f)]
        public float m_percent = 1.0f;

        public Unit.Type m_type;
        public int m_hp;
        
        public void SetType ()
        {
            m_type = (Unit.Type) int.Parse (m_texture.name.Split (new char[2] { '[', ']' }) [1]);
        }
    }
}