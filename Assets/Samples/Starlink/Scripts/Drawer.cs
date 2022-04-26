using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof (ParticleSystem))]
public class Drawer : MonoBehaviour {

    //sattllite amount per orbit
    public int psPerCircle = 24;
    //sattllite move speed
    public float psMoveSpeed = 5f;
    //number of nodes per orbit
    public int circleSect = 60;
    //orbit amount
    [Range (1, 50)]
    public int circleCount = 5;
    //orbit radius
    public float radius = 0.6f;
    [Range (0, 1.6f)]
    //orbit angle offset
    public float angleOffset = 0.8f;
    //camera rotate speed
    public float CameraRotateSpeed = 5f;
    public Material lineMat;
    public Transform earth;
    Camera camera;
    ParticleSystem particles;
    int arrcount;
    ParticleSystem.Particle[] arr;
    float deltaRotate = 0;

    Vector3 StartVec {
        get {
            return new Vector3 (radius * Mathf.Cos (angleOffset), radius * Mathf.Sin (angleOffset), 0);
        }
    }

    Vector3 StartRotAxis {
        get {
            Vector3 secondVec = new Vector3 (-Mathf.Cos (angleOffset), -Mathf.Sin (angleOffset), 1);
            return Vector3.Cross (StartVec, secondVec).normalized;
        }
    }

    void Start () {
        camera = GetComponentInChildren<Camera> ();
        particles = GetComponent<ParticleSystem> ();
        ParticleSystem.MainModule module = particles.main; 
        module.simulationSpace = ParticleSystemSimulationSpace.Custom;
        module.customSimulationSpace = earth; 
        particles.Emit (1000);
        arr = new ParticleSystem.Particle[particles.main.maxParticles];
    }

    void Update () {
        transform.Rotate (new Vector3 (0, CameraRotateSpeed * Time.deltaTime, 0), Space.World);
        camera.transform.LookAt(transform);

        DrawParticle ();
    }

    private void DrawParticle () {
        List<Vector3> ps = new List<Vector3> ();

        float deltaAnglePerCircle = 360f / circleCount;
        float deltaAnglePerPs = 360f / psPerCircle;
        deltaRotate += Time.deltaTime * psMoveSpeed;
        for (int i = 0; i < circleCount; i++) {
            float angleCircle = deltaAnglePerCircle * i;
            Quaternion cz = Quaternion.AngleAxis (angleCircle, new Vector3 (0, 1, 0));
            for (int j = 0; j < psPerCircle; j++) {
                //卫星的动画
                float angleSec0 = deltaAnglePerPs * j + deltaRotate;
                Quaternion cx0 = Quaternion.AngleAxis (angleSec0, StartRotAxis);
                Vector3 vector = cx0 * StartVec;
                vector = cz * vector;

                ps.Add (vector);

            }
        }

        //update the particles position
        arrcount = particles.GetParticles (arr);
        if (arrcount > 0 && arrcount >= ps.Count) {
            for (int i = 0; i < ps.Count; i++) {
                arr[i].position = ps[i];
            }
        }
        particles.SetParticles (arr);

    }

    //note: GL method is draw immedialy,so do not put it in update method.if not, the line won't  be show
    //draw orbit lines
    private void OnRenderObject () {

        lineMat.SetPass (0);

        GL.PushMatrix ();

        GL.MultMatrix (earth.transform.localToWorldMatrix);
        GL.Begin (GL.LINES);
        
        float deltaAnglePerSec = 360f / circleSect;
        float deltaAnglePerCircle = 360f / circleCount;
        Vector3 rotAxis = StartRotAxis;
        for (int i = 0; i < circleCount; i++) {
            float angleCircle = deltaAnglePerCircle * i;
            Quaternion cz = Quaternion.AngleAxis (angleCircle, new Vector3 (0, 1, 0));
            for (int j = 0; j < circleSect; j++) {

                //the first point
                float angleSec0 = deltaAnglePerSec * j;
                Quaternion cx0 = Quaternion.AngleAxis (angleSec0, rotAxis);
                Vector3 vector = cx0 * StartVec;
                vector = cz * vector;

                //the second point
                float angleSec1 = deltaAnglePerSec * (j + 1);
                Quaternion cx1 = Quaternion.AngleAxis (angleSec1, rotAxis);
                Vector3 vector1 = cx1 * StartVec;
                vector1 = cz * vector1;

                //join two points on orbit
                GL.Vertex (vector);
                GL.Vertex (vector1);

            }
        }

        GL.End ();

        GL.PopMatrix ();
    }
}