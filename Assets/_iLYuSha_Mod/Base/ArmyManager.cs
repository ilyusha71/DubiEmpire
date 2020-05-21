using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ArmyManager : MonoBehaviour
{
    // public List<unit>
    public ArmyData nowArmy;
    public List<Button> listReadyUnit = new List<Button> ();

    [Header ("Ready")]
    public Transform readyListGroup;
    public GameObject prefabUnitButton;
    public Warfare.Unit.Model modelSelection;

    // Start is called before the first frame update
    public
    void Start ()
    {

    }
    // Model to Button List
    public void RegisterReserveUnit (Warfare.Unit.Model unit)
    {
        Button btn = Instantiate (prefabUnitButton, readyListGroup).GetComponent<Button> ();
        listReadyUnit.Add (btn);
        btn.onClick.AddListener (() =>
        {
            modelSelection = unit;
        });
        btn.transform.localScale = Vector3.one;
        unit.squadron = 0;
    }

    // Update is called once per frame
    void Update ()
    {
        if (Input.GetMouseButtonDown (0))
        { // if left button pressed...
            Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast (ray, out hit))
            {
                Warfare.GridManager grid = hit.transform.GetComponent<Warfare.GridManager> ();
                if (grid)
                {
                    if (grid.m_unit == null)
                        grid.Deploy (modelSelection);
                    else
                    {
                        RegisterReserveUnit (grid.m_unit);
                        grid.Disarmament();
                    }
                }
                // the object identified by hit.transform was clicked
                // do whatever you want
                // Debug.Log(hit.transform.name);
            }
        }
    }
}