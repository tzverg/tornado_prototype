using System;
using UnityEngine;

[Serializable]
public class Boundary
{
    public float xMin, xMax, zMin, zMax;
}

public class TornadoController : MonoBehaviour
{
    [Range(1F, 2F)][SerializeField] private float tornadoScale = 1F;
    [SerializeField] private float tornadoMotionSpeed;
    private Rigidbody tornadoRB;

    public Boundary boundary;

    void Start()
    {
        tornadoRB = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        SetTornadoLocalScale();
        MoveTornado();
    }

    private void SetTornadoLocalScale()
    {
        if (transform.localScale.x != tornadoScale || transform.localScale.y != tornadoScale)
        {
            transform.localScale = new Vector3(tornadoScale, 1F, tornadoScale);
        }
    }

    private void MoveTornado()
    {
        if (Input.GetMouseButton(0))
        {
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mousePos.y = 0F;

            tornadoRB.position = Vector3.Lerp(tornadoRB.position, mousePos, Time.deltaTime * tornadoMotionSpeed);
            tornadoRB.position = new Vector3(
                Mathf.Clamp(tornadoRB.position.x, boundary.xMin, boundary.xMax),
                0F,
                Mathf.Clamp(tornadoRB.position.z, boundary.zMin, boundary.zMax)
            );
        }
    }
}
