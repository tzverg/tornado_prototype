using System.Collections;
using UnityEngine;

public class TornadoController : MonoBehaviour
{
    [Range(0F, 2F)] public float tornadoScale = 1F;
    private float tornadoNextScale;
    [SerializeField] private bool tornadoTapMotion;

    public GameObject UIGO;
    public GameObject CoreGameplayCGO;

    private bool mousePosSaved = false;

    private UIController uiController;
    private CoreGameplayController coreGameplayC;
    private Rigidbody tornadoRB;

    private RaycastHit mouseHit;
    private Vector3 mousePosStart;

    void Start()
    {
        tornadoRB = GetComponent<Rigidbody>();
        uiController = UIGO.GetComponent<UIController>();
        coreGameplayC = CoreGameplayCGO.GetComponent<CoreGameplayController>();
    }

    public float GetTornadoScale()
    {
        return tornadoScale;
    }

    void FixedUpdate()
    {
        MoveTornado();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "RedMarker")
        {
            StartCoroutine(FadeCoroutine(other.gameObject));
            //ScaleDownTornado();   //TODO
            coreGameplayC.EndTheGame();
        }
        else if (other.tag == "GreenMarker")
        {
            StartCoroutine(FadeCoroutine(other.gameObject));
            ScaleUpTornado();
        }
        else if (other.tag == "BlackMarker")
        {
            //StartCoroutine(FadeCoroutine(other.gameObject));
            Destroy(other.gameObject);
            coreGameplayC.CreateTornado(tornadoRB.transform.position, tornadoRB.transform.localScale);
            Destroy(gameObject);
        }
    }

    private void ScaleDownTornado()
    {
        tornadoNextScale = tornadoScale - coreGameplayC.modifiers.modR;
        StartCoroutine(ScaleDownTornadoCoroutine());
        uiController.AddScore(-5);
    }

    private void ScaleUpTornado()
    {
        tornadoNextScale = tornadoScale + coreGameplayC.modifiers.modG;
        StartCoroutine(ScaleUpTornadoCoroutine());
        uiController.AddScore(5);
    }

    private IEnumerator ScaleUpTornadoCoroutine()
    {
        for (float value = tornadoScale; value <= tornadoNextScale; value += 0.05f)
        {
            tornadoScale = value;
            SetTornadoLocalScale();
            yield return new WaitForSeconds(.1f);
        }

        tornadoScale = tornadoNextScale;
        SetTornadoLocalScale();
    }

    private IEnumerator ScaleDownTornadoCoroutine()
    {
        for (float value = tornadoScale; value >= tornadoNextScale; value -= 0.05f)
        {
            tornadoScale = value;
            SetTornadoLocalScale();
            yield return new WaitForSeconds(.1f);
        }

        tornadoScale = tornadoNextScale;
        SetTornadoLocalScale();
    }

    private IEnumerator FadeCoroutine(GameObject targetGO)
    {
        for (float f = 1f; f >= 0; f -= 0.1f)
        {
            if (targetGO != null)
            {
                Renderer renderer = targetGO.GetComponent<Renderer>();
                Color c = renderer.material.color;
                c.a = f;
                renderer.material.color = c;
            }
            yield return new WaitForSeconds(.1f);
        }
        Destroy(targetGO);
    }

    private void SetTornadoLocalScale()
    {
        if (transform.localScale.x != tornadoScale || transform.localScale.y != tornadoScale)
        {
            transform.localScale = new Vector3(tornadoScale, 1F, tornadoScale);
        }

        if (tornadoScale <= 0F || tornadoScale >= 2F)
        {
            coreGameplayC.EndTheGame();
        }
    }

    private void MoveTornado()
    {
        if (Input.GetMouseButton(0))
        {
            if (tornadoTapMotion)
            {
                Vector3 mousePos = Vector3.zero;
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

                if (Physics.Raycast(ray, out mouseHit))
                {
                    mousePos = mouseHit.point;
                }

                tornadoRB.position = Vector3.Lerp(tornadoRB.position, mousePos, Time.deltaTime * coreGameplayC.config.tornadoMotionSpeed);
            }
            else
            {
                if (!mousePosSaved)
                {
                    mousePosStart = Input.mousePosition;
                    mousePosSaved = true;
                }
                Vector3 tapDirection = Input.mousePosition - mousePosStart;
                tapDirection.Normalize();
                Vector3 movePoint = new Vector3(tornadoRB.position.x + tapDirection.x, 0F, tornadoRB.position.z + tapDirection.y);
                tornadoRB.position = Vector3.Lerp(tornadoRB.position, movePoint, Time.deltaTime * coreGameplayC.config.tornadoMotionSpeed);
            }
            tornadoRB.position = new Vector3(
                Mathf.Clamp(tornadoRB.position.x, coreGameplayC.config.xMin, coreGameplayC.config.xMax),
                0F,
                Mathf.Clamp(tornadoRB.position.z, coreGameplayC.config.zMin, coreGameplayC.config.zMax)
            );
        }
        else
        {
            mousePosSaved = false;
            tornadoRB.velocity = Vector3.zero;
        }
    }
}
