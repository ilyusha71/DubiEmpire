using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Warfare.Legion
{
    public class LegionManager : MonoBehaviour
    {
        public WarfareManager warfare;
        public GridManager[] grids;
        public int id = 1000;

        [HeaderAttribute("UI Setting")]
        public Transform reserveGroup;
        public GameObject prefabUnitButton;
        public List<Toggle> listReserveUnits = new List<Toggle>();
        public Toggle btnSelected;
        public Unit.Data dataSelected;

        RectTransform rectTransform;
        GridLayoutGroup gridLayout;
        Scrollbar scrollbar;

        void Awake()
        {
            gridLayout = reserveGroup.GetComponent<GridLayoutGroup>();
            rectTransform = reserveGroup.GetComponent<RectTransform>();
            scrollbar = reserveGroup.parent.GetComponentInChildren<Scrollbar>();

            warfare.MasterModelCollector();
            warfare.SynchronizeLegionsToPlayerData();
            warfare.SynchronizeUnitsToPlayerData();
        }

        void Start()
        {
            // DataModel2<Unit.BattleModel> ddd = new DataModel2<Unit.BattleModel>();
            // ddd.squadron[2].

            CreateLegionUnit();
            CreateReserveUnit();
        }

        void Update()
        {
            for (int k = 0; k < 10; k++)
            {
                if (Input.GetKeyDown((KeyCode)(k + 48)))
                {
                    id = 1000 + k;
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
                        if (grid.unit == null)
                        {
                            if (btnSelected != null && dataSelected != null)
                            {
                                // if (grid.Deploy(dataSelected, warfare.units[dataSelected.Type]))
                                // {
                                //     warfare.playerData.units.Remove(dataSelected);
                                //     warfare.playerData.legions[id].squadron.Add(grid.Order, dataSelected);
                                //     listReserveUnits.Remove(btnSelected);
                                //     int index = btnSelected.transform.GetSiblingIndex();
                                //     DestroyImmediate(btnSelected.gameObject);
                                //     if (reserveGroup.GetComponentsInChildren<Toggle>().Length > 0)
                                //     {
                                //         if (reserveGroup.GetComponentsInChildren<Toggle>().Length > index)
                                //             btnSelected = reserveGroup.GetComponentsInChildren<Toggle>()[index];
                                //         else
                                //             btnSelected = reserveGroup.GetComponentsInChildren<Toggle>()[index - 1];
                                //         btnSelected.isOn = true;
                                //     }
                                //     else
                                //     {
                                //         btnSelected = null;
                                //         dataSelected = null;
                                //     }
                                //     ResetReserveGroup();
                                // }
                            }
                        }
                        else
                        {
                            warfare.playerData.units.Add(grid.unit.data);
                            RegisterReserveUnit(grid.unit.data);
                            ResetReserveGroup();
                            warfare.playerData.legions[id].squadron.Remove(grid.Order);
                            grid.Disarmament();
                        }
                    }
                }
            }

            if (Input.GetKeyDown(KeyCode.F3))
                warfare.Save(0);
            if (Input.GetKeyDown(KeyCode.F4))
            {
                if (warfare.Load(0))
                {
                    CreateLegionUnit();
                    CreateReserveUnit();
                }
            }
        }

        // Legion Unit Grid
        public void CreateLegionUnit()
        {
            Data data;
            if (warfare.playerData.legions.ContainsKey(id))
                data = warfare.playerData.legions[id];
            else
            {
                data = new Data(id);
                warfare.playerData.legions.Add(id, data);
            }
            for (int order = 0; order < 13; order++)
            {
                grids[order].Disarmament();
                grids[order].Manage(order);
                // if (data.squadron.ContainsKey(order))
                //     grids[order].Deploy(data.squadron[order], warfare.units[data.squadron[order].Type]);
            }
        }

        // Reserve Unit Bar
        public void CreateReserveUnit()
        {
            int count = listReserveUnits.Count;
            for (int i = 0; i < count; i++)
            {
                Destroy(listReserveUnits[i].gameObject);
            }
            listReserveUnits.Clear();

            count = warfare.playerData.units.Count;
            for (int i = 0; i < count; i++)
            {
                RegisterReserveUnit(warfare.playerData.units[i]);
            }
            ResetReserveGroup();
        }
        public void RegisterReserveUnit(Unit.Data data)
        {
            Unit.Model model = warfare.units[data.Type];
            Toggle btn = Instantiate(prefabUnitButton, reserveGroup).GetComponent<Toggle>();
            btn.gameObject.name = model.Type.ToString();
            btn.group = btn.GetComponentInParent<ToggleGroup>();
            btn.GetComponent<Image>().sprite = model.Sprite;
            btn.GetComponentsInChildren<TextMeshProUGUI>()[0].text = model.Field == Unit.Field.Dubi ? "x " + model.UnitCount(data.HP).ToString() : data.HP.ToString();
            listReserveUnits.Add(btn);
            btn.onValueChanged.AddListener(isOn =>
           {
               if (isOn)
               {
                   dataSelected = data;
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
            int count = listReserveUnits.Count;
            rectTransform.sizeDelta = new Vector2(gridLayout.cellSize.x * count + gridLayout.spacing.x * (count - 1), rectTransform.sizeDelta.y);
        }
    }
}