using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestScript : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            float y = transform.position.y;
            transform.Translate(new Vector3(0, -0.5f, 0));
            Physics.autoSimulation = false;
            Physics.Simulate(Time.fixedDeltaTime);
            Physics.autoSimulation = true;
            print("bilo = " + y + ", stalo = " + transform.position.y + " delta = "  + (-y + transform.position.y));
        }
    }
}
