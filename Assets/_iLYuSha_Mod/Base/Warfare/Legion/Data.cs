using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UI;

namespace Warfare.Legion
{
    [CreateAssetMenu(fileName = "Data", menuName = "Warfare/Legion/Create Warfare Legion Data")]
    public class Data : ScriptableObject
    {
        public int m_index;
        public Faction m_faction;
        public int m_legion;
        public SquadronInspector[] m_squadron = new SquadronInspector[13];

        public void SetIndex()
        {
            m_index = (int)m_faction * 100 + m_legion;
            for (int i = 0; i < m_squadron.Length; i++)
            {
                m_squadron[i].SetUnit();
            }
        }
        public Squadron[] GetSquadrons()
        {
            Squadron[] squadrons = new Squadron[13];
            for (int i = 0; i < squadrons.Length; i++)
            {
                squadrons[i].type = m_squadron[i].type;
                squadrons[i].hp = m_squadron[i].Stack * m_squadron[i].data.model.m_hp;
            }
            return squadrons;
        }
    }

    [System.Serializable]
    public class SquadronInspector
    {
        public Texture m_texture;
        public Unit.Type type;
        public Unit.Data data;
        [Range(0f, 1f)]
        public float m_percent = 1.0f;

        public void SetUnit()
        {
            if (m_texture)
            {
                type = (Unit.Type)int.Parse(m_texture.name.Split(new char[2] { '[', ']' })[1]);
                data = AssetDatabase.LoadAssetAtPath<Unit.Database>("Assets/_iLYuSha_Mod/Base/Warfare/Unit/Database.asset").units[type];
            }
            else
            {
                type = Unit.Type.None;
                data = null;
            }

        }
        public int Stack
        {
            get
            {
                return type == 0 ? 0 : Mathf.Max(1, Mathf.CeilToInt(data.model.m_formation.Length * m_percent));
            }
            set
            {
                m_percent = type == 0 ? 0 : Mathf.Clamp01((float)value / data.model.m_formation.Length);
            }
        }
        public int HP
        {
            get
            {
                return Stack * data.model.m_hp;
            }
        }
    }
}