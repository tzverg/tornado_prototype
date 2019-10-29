using System;
using System.Collections;
using UnityEngine;

[Serializable]
public class Boundary
{
    public float xMin, xMax, zMin, zMax;
}

[Serializable]
public class BlockerModifiers
{
    public float modR, modG;
}

public class TornadoController : MonoBehaviour
{
    [Range(0F, 2F)] [SerializeField] private float tornadoScale = 1F;
    private float tornadoNextScale;
    [SerializeField] private float tornadoMotionSpeed;

    [SerializeField] private GameObject UIGO;

    private UIController uiController;
    private Rigidbody tornadoRB;

    public Boundary boundary;
    public BlockerModifiers modifiers;

    void Start()
    {
        tornadoRB = GetComponent<Rigidbody>();
        uiController = UIGO.GetComponent<UIController>();
    }

    public float GetTornadoScale()
    {
        return tornadoScale;
    }

    void FixedUpdate()
    {
        //SetTornadoLocalScale();
        MoveTornado();
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Trigger with " + other.name);

        if (other.tag == "RedMarker")
        {
            StartCoroutine(FadeCoroutine(other.gameObject));
            ScaleDownTornado();
        }
        else if (other.tag == "GreenMarker")
        {
            StartCoroutine(FadeCoroutine(other.gameObject));
            ScaleUpTornado();
        }
    }

    private void ScaleDownTornado()
    {
        tornadoNextScale = tornadoScale - modifiers.modR;
        StartCoroutine(ScaleDownTornadoCoroutine());
        uiController.AddScore(-5);
    }

    private void ScaleUpTornado()
    {
        tornadoNextScale = tornadoScale + modifiers.modG;
        StartCoroutine(ScaleUpTornadoCoroutine());
        uiController.AddScore(5);
    }

    private IEnumerator ScaleUpTornadoCoroutine()
    {
        for (float value = tornadoScale; value < tornadoNextScale; value += 0.05f)
        {
            tornadoScale = value;
            SetTornadoLocalScale();
            yield return new WaitForSeconds(.1f);
        }
    }

    private IEnumerator ScaleDownTornadoCoroutine()
    {
        for (float value = tornadoScale; value > tornadoNextScale; value -= 0.05f)
        {
            tornadoScale = value;
            SetTornadoLocalScale();
            yield return new WaitForSeconds(.1f);
        }
    }

    private IEnumerator FadeCoroutine(GameObject targetGO)
    {
        for (float f = 1f; f >= 0; f -= 0.1f)
        {
            Renderer renderer = targetGO.GetComponent<Renderer>();
            Color c = renderer.material.color;
            c.a = f;
            renderer.material.color = c;
            if (f <= .1F)
            {
                Destroy(targetGO);
            }
            yield return new WaitForSeconds(.1f);
        }
    }

    private void SetTornadoLocalScale()
    {
        if (transform.localScale.x != tornadoScale || transform.localScale.y != tornadoScale)
        {
            transform.localScale = new Vector3(tornadoScale, 1F, tornadoScale);
        }

        if (tornadoScale <= 0)
        {
            Debug.Log("Tornado is gone");
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
