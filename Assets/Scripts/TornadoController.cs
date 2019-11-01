using System;
using System.Collections;
using UnityEngine;

public class TornadoController : MonoBehaviour
{
    [Range(0F, 2F)] public float tornadoScale = 1F;
    private float tornadoNextScale;
    [SerializeField] private bool tornadoTapMotion;

    public GameObject UIGO;
    public GameObject CoreGameplayCGO;

    private UIController uiController;
    private CoreGameplayController coreGameplayC;
    private Rigidbody tornadoRB;

    private RaycastHit mouseHit;

    private Vector3 pointerPosStart;
    private Vector3 pointerPosEnd;

    private bool pointerPosSaved = false;

    // check mouse motion
    private bool timerLaunched = false;
    private Vector3 pointerTimerPos;

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
            //StartCoroutine(FadeCoroutine(other.gameObject));
            //ScaleDownTornado();   //TODO
            Destroy(other.gameObject);
            coreGameplayC.EndTheGame();
        }
        else if (other.tag == "GreenMarker")
        {
            if (other.transform.localScale.x <= tornadoScale)
            {
                //StartCoroutine(FadeCoroutine(other.gameObject));
                Destroy(other.gameObject);
                ScaleUpTornado();
            }
            else
            {
                coreGameplayC.EndTheGame();
            }
        }
        else if (other.tag == "BlackMarker")
        {
            if (other.transform.localScale.x <= tornadoScale)
            {
                Destroy(other.gameObject);
                coreGameplayC.CreateTornado(tornadoRB.transform.position, tornadoRB.transform.localScale);
                Destroy(gameObject);
            }
            else
            {
                coreGameplayC.EndTheGame();
            }
        }
    }

    private void ScaleDownTornado()
    {
        tornadoNextScale = tornadoScale - coreGameplayC.config.modR;
        StartCoroutine(ScaleDownTornadoCoroutine());
        uiController.AddScore(-5);
    }

    private void ScaleUpTornado()
    {
        tornadoNextScale = tornadoScale + coreGameplayC.config.modG;
        StartCoroutine(ScaleUpTornadoCoroutine());
        uiController.AddScore(5);
        coreGameplayC.SetTornadoTier(tornadoScale);
        coreGameplayC.UpdateBlockerPrefab();
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
            transform.localScale = new Vector3(tornadoScale, tornadoScale, tornadoScale);
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
                #if UNITY_EDITOR_WIN || UNITY_STANDALONE
                MouseMotion();
                #elif  UNITY_ANDROID
                TouchMotion();
                #endif
            }
            ClampTornadoPosition();
        }
        else
        {
            pointerPosSaved = false;
            tornadoRB.velocity = Vector3.zero;
        }
    }

    private void MouseMotion()
    {
        if (!pointerPosSaved)
        {
            pointerPosStart = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            pointerPosSaved = true;
        }

        if (Vector3.Distance(pointerPosStart, Input.mousePosition) > coreGameplayC.config.minSwipeDistance)
        {
            Vector3 pointerPosLast = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector3 swipeDir = pointerPosLast - pointerPosStart;
            swipeDir.y = 0.1F;
            swipeDir.Normalize();

            tornadoRB.velocity = swipeDir * coreGameplayC.config.tornadoMotionSpeed;
        }
    }

    private void TouchMotion()
    {
        if (Input.touchCount > 0)
        {
            for (int cnt = 0; cnt < Input.touchCount; cnt++)
            {
                Touch currentTouch = Input.GetTouch(cnt);

                switch (currentTouch.phase)
                {
                    case TouchPhase.Began:
                        pointerPosStart = Camera.main.ScreenToWorldPoint(currentTouch.position);
                        break;
                    case TouchPhase.Moved:
                        pointerPosEnd = Camera.main.ScreenToWorldPoint(currentTouch.position);
                        if (Vector3.Distance(pointerPosStart, pointerPosEnd) > coreGameplayC.config.minSwipeDistance)
                        {
                            Vector3 swipeDir = pointerPosEnd - pointerPosStart;
                            swipeDir.y = 0.1F;
                            swipeDir.Normalize();
                            tornadoRB.velocity = swipeDir * coreGameplayC.config.tornadoMotionSpeed;
                        }
                        break;
                    case TouchPhase.Ended:
                        tornadoRB.velocity = Vector3.zero;
                        break;
                    case TouchPhase.Stationary:
                        pointerPosStart = Camera.main.ScreenToWorldPoint(currentTouch.position);
                        tornadoRB.velocity = Vector3.zero;
                        break;
                }
            }
        }
    }

    private void ClampTornadoPosition()
    {
        Borders borders = coreGameplayC.config.borders;

        tornadoRB.position = new Vector3(
            Mathf.Clamp(tornadoRB.position.x, borders.xMin, borders.xMax),
            0F,
            Mathf.Clamp(tornadoRB.position.z, borders.zMin, borders.zMax)
        );
    }
}
