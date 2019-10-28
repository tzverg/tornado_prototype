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

    // Start is called before the first frame update
    void Start()
    {
        tornadoRB = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
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

        //#if UNITY_IOS || UNITY_ANDROID
        //if (Input.touchCount > 0)
        //{
        //    Vector2 touchStartPos = Vector2.zero;

        //    for (int i = 0; i < Input.touchCount; ++i)
        //    {
        //        Touch currentTouch = Input.GetTouch(i);
        //        if (currentTouch.phase == TouchPhase.Began)
        //        {
        //            touchStartPos = currentTouch.position;
        //        }
        //        if (currentTouch.phase == TouchPhase.Moved)
        //        {                    
        //            Vector3 touchPosition = Camera.main.ScreenToWorldPoint(currentTouch.position);
        //            touchPosition.y = 0F;

        //            tornadoRB.position = Vector3.Lerp(tornadoRB.position, touchPosition, Time.deltaTime * tornadoMotionSpeed);
        //        }
        //    }
        //}
        //#endif

        //#if UNITY_STANDALONE || UNITY_EDITOR
        //float moveHorizontal = Input.GetAxis("Horizontal");
        //float moveVertical = Input.GetAxis("Vertical");

        //Vector3 movement = new Vector3(moveHorizontal, 0F, moveVertical);
        //tornadoRB.velocity = movement * tornadoMotionSpeed;
        //#endif

        //tornadoRB.position = new Vector3(
        //    Mathf.Clamp(tornadoRB.position.x, boundary.xMin, boundary.xMax),
        //    0F,
        //    Mathf.Clamp(tornadoRB.position.z, boundary.zMin, boundary.zMax)
        //);
    }
}
