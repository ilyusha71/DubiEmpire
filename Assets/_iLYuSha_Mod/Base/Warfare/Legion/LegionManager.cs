using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace Warfare.Legion
{
    public class LegionManager : MonoBehaviour
    {
        public WarfareManager warfare;
        public GridManager[] grids;
        public int index;

        [HeaderAttribute("UI Setting")]
        public Transform reserveGroup;
        public GameObject prefabUnitButton;
        public List<Toggle> listReadyUnit = new List<Toggle>();
        public Toggle btnSelected;
        public Unit.Model unitSelected;

        RectTransform rectTransform;
        GridLayoutGroup gridLayout;
        Scrollbar scrollbar;

        void Awake()
        {
            gridLayout = reserveGroup.GetComponent<GridLayoutGroup>();
            rectTransform = reserveGroup.GetComponent<RectTransform>();
            scrollbar = reserveGroup.parent.GetComponentInChildren<Scrollbar>();
        }

        void Start()
        {
            // Method 1:  Load legion database as legion model
            int count = warfare.legionDB.legions.Count;
            List<Data> listData = warfare.legionDB.legions.Values.ToList();
            for (int j = 0; j < listData.Count; j++)
            {
                for (int i = 0; i < listData[j].m_squadron.Length; i++)
                {
                    Unit.Type type = listData[j].m_squadron[i].type;
                    if (type == Unit.Type.None)
                        continue;
                    Unit.Data data = warfare.unitDB.units[type];
                    Unit.Model unit = new Unit.Model();
                    unit.type = type;
                    unit.hp = listData[j].m_squadron[i].HP;
                    unit.stack = data.GetStackCount(unit.hp);

                    if (listData[j].m_index < 9900)
                        warfare.playerData.squadrons.Add(listData[j].m_index * 100 + i, unit);
                    else
                    {
                        warfare.playerData.units.Add(unit);
                        RegisterReserveUnit(unit);
                    }
                }
            }
            ResetReserveGroup();

            // Method 2:  Load player data as legion
            CreateLegionUnit();
        }
        KeyCode currentKey;
        void Update()
        {

            for (int k = 0; k < 10; k++)
            {
                if (Input.GetKeyDown((KeyCode)(k + 48)))
                {
                    index = k;
                    CreateLegionUnit();
                }
            }

            if (Input.GetMouseButtonDown(0))
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit))
                {
                    Warfare.GridManager grid = hit.transform.GetComponent<Warfare.GridManager>();
                    if (grid)
                    {
                        int key = 100000 + index * 100 + grid.Index;
                        if (grid.unit == null)
                        {
                            if (btnSelected != null && unitSelected != null)
                            {
                                if (grid.Deploy(unitSelected))
                                {
                                    warfare.playerData.squadrons[key] = unitSelected;
                                    warfare.playerData.units.Remove(unitSelected);
                                    listReadyUnit.Remove(btnSelected);
                                    int index = btnSelected.transform.GetSiblingIndex();
                                    DestroyImmediate(btnSelected.gameObject);
                                    if (reserveGroup.GetComponentsInChildren<Toggle>().Length > 0)
                                    {
                                        if (reserveGroup.GetComponentsInChildren<Toggle>().Length > index)
                                            btnSelected = reserveGroup.GetComponentsInChildren<Toggle>()[index];
                                        else
                                            btnSelected = reserveGroup.GetComponentsInChildren<Toggle>()[index - 1];
                                        btnSelected.isOn = true;
                                    }
                                    else
                                    {
                                        btnSelected = null;
                                        unitSelected = null;
                                    }
                                    ResetReserveGroup();
                                }
                            }
                        }
                        else
                        {
                            warfare.playerData.squadrons.Remove(key);
                            warfare.playerData.units.Add(grid.unit);
                            RegisterReserveUnit(grid.unit);
                            grid.Disarmament();
                            ResetReserveGroup();
                        }
                    }
                    // the object identified by hit.transform was clicked
                    // do whatever you want
                    // Debug.Log(hit.transform.name);
                }
            }


            if (Input.GetKeyDown(KeyCode.F3))
            {
                BinaryFormatter bf = new BinaryFormatter();
                Stream s = File.Open(Application.dataPath + "/Save.wak", FileMode.Create);
                bf.Serialize(s, warfare.playerData);
                s.Close();
                Debug.Log("Save");
            }
            if (Input.GetKeyDown(KeyCode.F4))
            {
                BinaryFormatter bf = new BinaryFormatter();
                Stream s = File.Open(Application.dataPath + "/Save.wak", FileMode.Open);
                warfare.playerData = (PlayerData)bf.Deserialize(s);

                CreateLegionUnit();
                CreateReserveUnit();
            }
        }

        public void CreateLegionUnit()
        {
            for (int i = 0; i < 13; i++)
            {
                grids[i].Disarmament();
                grids[i].unit = null;
                grids[i].Index = i;
                int key = 100000 + index * 100 + i;
                if (warfare.playerData.squadrons.ContainsKey(key))
                {
                    Unit.Model unit = warfare.playerData.squadrons[key];
                    if (unit == null)
                        continue;
                    grids[i].Deploy(unit);
                }
            }
        }
        public void CreateReserveUnit()
        {
            int count = listReadyUnit.Count;

            for (int i = 0; i < count; i++)
            {
                DestroyImmediate(listReadyUnit[i].gameObject);
            }
            listReadyUnit.Clear();

            count = warfare.playerData.units.Count;
            for (int i = 0; i < count; i++)
            {
                if (warfare.playerData.units[i].type == Unit.Type.None)
                    Debug.LogWarning(i + " is Type None");
                else
                    RegisterReserveUnit(warfare.playerData.units[i]);
            }
        }

        public void RegisterReserveUnit(Unit.Model unit)
        {
            Toggle btn = Instantiate(prefabUnitButton, reserveGroup).GetComponent<Toggle>();
            btn.gameObject.name = unit.type.ToString();
            btn.group = btn.GetComponentInParent<ToggleGroup>();
            try
            {
                btn.GetComponent<Image>().sprite = warfare.unitDB.units[unit.type].m_sprite;

            }
            catch
            {
                Debug.Log(unit.type);
            }
            btn.GetComponentsInChildren<TextMeshProUGUI>()[0].text = (int)unit.type < 2000 ? unit.hp.ToString() : unit.stack.ToString();
            listReadyUnit.Add(btn);
            btn.onValueChanged.AddListener(isOn =>
           {
               if (isOn)
               {
                   unitSelected = unit;
                   btnSelected = btn;
               }
           });
            btn.transform.localScale = Vector3.one;
            btn.transform.SetSiblingIndex(0);
            btn.isOn = true;
            scrollbar.value = 0;
        }
        void ResetReserveGroup()
        {
            int count = listReadyUnit.Count;
            rectTransform.sizeDelta = new Vector2(gridLayout.cellSize.x * count + gridLayout.spacing.x * (count - 1), rectTransform.sizeDelta.y);
        }
    }
}