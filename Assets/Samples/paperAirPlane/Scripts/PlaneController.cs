using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaneController : MonoBehaviour
{

    public Vector3 flySpeed = new Vector3(0,3,10);
    public MainWindow main;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.S))
        {
            main.btnFly.gameObject.SetActive(true);
            transform.rotation = Quaternion.Euler(-10, 0, -165);
            main.rotateSlider.gameObject.SetActive(false);
        }
    }

    public void FlyOut()
    {
        Rigidbody rgd = GetComponent<Rigidbody>();
        rgd.isKinematic = false;
        rgd.velocity = flySpeed;
    }
}
