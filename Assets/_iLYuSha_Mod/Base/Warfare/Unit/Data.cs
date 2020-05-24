﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Unity.Mathematics;

namespace Warfare.Unit
{
    [CreateAssetMenu(fileName = "Data", menuName = "Warfare/Unit/Create Warfare Unit Data")]
    public class Data : ScriptableObject
    {
        public GameObject m_instance;
        public Sprite m_sprite;
        [HeaderAttribute("Parameter")]
        public Model model;

        public void SetType()
        {
            if (m_instance)
                model.m_type = (Type)int.Parse(m_instance.name.Split(new char[2] { '[', ']' })[1]);
            else if (m_sprite)
                model.m_type = (Type)int.Parse(m_sprite.name.Split(new char[2] { '[', ']' })[1]);
        }
        public void SetFormation()
        {
            if (model.m_formation.Length == 0)
                model.m_formation = new float3[1] { Vector3.zero };
            if (model.m_square == Square.None) return;
            float height = 0;
            int side = (int)model.m_square;
            float offset = 12.0f / (side + 1.0f);
            model.m_formation = new float3[side * side];
            for (int i = 0; i < side; i++)
            {
                for (int j = 0; j < side; j++)
                {
                    model.m_formation[i * side + j] = new Vector3(-6 + (j + 1) * offset, height, 6 - (i + 1) * offset);
                }
            }
        }
        public GameObject GetWarfareUnit(int index, Vector3 cantre)
        {
            return Instantiate(m_instance, cantre + (Vector3)model.m_formation[index] * 1, Quaternion.identity);
        }
        public GameObject GetWarfareUnit(int index, Vector3 cantre, float degree)
        {
            return Instantiate(m_instance, cantre + (Vector3)model.m_formation[index] * 1, Quaternion.Euler(0, degree, 0));
        }
    }
    [System.Serializable]
    public class Model
    {
        public Type m_type;
        public int m_price;
        public int m_Hour;
        public int m_hp;
        public int m_fire;
        public int m_atk;
        public Base m_base;
        public Anti m_anti;
        public Range m_range;
        public Square m_square;
        public float3[] m_formation;
    }
    [System.Serializable]
    public class Squadron
    {
        public Model model;
        public int hp, level, exp;

        public int stack
        {
            get { return Mathf.CeilToInt(hp / model.m_hp); }
        }
        public int TotalDamage
        {
            get { return stack * model.m_atk; }
        }
    }

    public enum Type
    {
        None = 0,
        Mech = 1000,
        DespicableMech = 1001,
        EvilMech = 1002,
        Energy = 1010,
        RedBullEnergy = 1011,
        MonsterEnergy = 1012,
        Ceti = 1020,
        Putin = 1021,
        Kells = 1022,
        Aeroplane = 1030,
        PaperAeroplane = 1031,
        CuckooProto = 1040,
        Cuckoo = 1041,
        Progne = 1042,
        Bullet = 1050,
        BulletBill = 1051,
        BulletSoda = 1052,
        TimeMachine = 1060,
        TimeMachineMK1 = 1061,
        TimeMachineMK2 = 1062,
        Kennel = 1070,
        AceKennel = 1071,
        Star = 1080,
        KirbyStar = 1081,
        Scopio = 1090,
        ScopioBlood = 1091,
        ScopioFirmament = 1092,
        nWidiaProto = 1100,
        nWidia = 1101,
        FastFoodManProto = 1110,
        FastFoodMan = 1111,
        Reindeer = 1120,
        XmasReindeer = 1121,
        ArcticReindeer = 1122,
        Express = 1130,
        PolarisExpress = 1131,
        YogurtExpress = 1132,
        AncientFish = 1140,
        VoidFish = 1141,
        FossilFish = 1142,
        Unicorn = 1150,
        PapoyUnicorn = 1151,
        Pumpkin = 1160,
        GhostPumpkin = 1161,
        Hunter = 1170,
        BoundyHunter = 1171,
        Inuit = 1180,
        InuitScout = 1181,
        Lisboa = 1190,
        GrandLisboa = 1191,
        PinkLisboa = 1192,
        ScarletLisboa = 1193,
        Piggy = 1200,
        PiggyCracker = 1201,
        Dave = 2001,
        EvilMinion = 2002,
        Vettel = 2011,
        WangNiMa = 2012,
        DaMao = 2021,
        MengZong = 2022,
        Dorara = 2061,
        MiniDorara = 2062,
        NoFace = 2091,
        PandamanZhang = 2101,
        PandamanJin = 2102,
        Eric = 2111,
        AwesomO4000 = 2112,
        HuaJi = 2131,
        SpongeBob = 2141,
        PinkSpongeBob = 2142,
        Kenny = 2151,
        Kyle = 2171,
        IndianStan = 2181,
        Stan = 2182,
        AngryMan = 2191,
    }
    public enum Square
    {
        None = 0,
        _3x3 = 3,
        _4x4 = 4,
        _5x5 = 5,
        _6x6 = 6,
        _7x7 = 7,
    }
    public enum Base
    {
        None = 0,
        Air = 3,
        Mech = 5,
        Dubi = 7,
    }
    public enum Anti
    {
        Normal = 0,
        Air = 3,
        Mech = 5,
        Dubi = 7,
    }
    public enum Range
    {
        Near = 0,
        Medium = 1,
        Far = 2,
    }
}