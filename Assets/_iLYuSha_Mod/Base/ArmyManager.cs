using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ArmyManager : MonoBehaviour
{
    public ArmyData nowArmy;
    public List<Button> listReadyUnit = new List<Button> ();

    // Start is called before the first frame update
    public
    void Start ()
    {

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
                // the object identified by hit.transform was clicked
                // do whatever you want
                Debug.Log(hit.transform.name);
            }
        }
    }
}