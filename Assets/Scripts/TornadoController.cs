using System;
using System.Collections;
using UnityEngine;

public class TornadoController : MonoBehaviour
{
    //[SerializeField] private bool tornadoTapMotion;

	public float TornadoScale => _tornadoScale;

    public float _tornadoScale;
    private float tornadoNextScale;

	[SerializeField]
    private UIController uiController;
	[SerializeField]
    private CoreGameplayController coreGameplayController;
    //private Rigidbody tornadoRB;

    //private RaycastHit mouseHit;

    private Vector3 pointerPosStart;
    //private Vector3 pointerPosEnd;

    //private bool pointerPosSaved = false;

    // check mouse motion
    //private bool timerLaunched = false;
    //private Vector3 pointerTimerPos;

	private float _tornadoMovingSpeed = 1.0f;

	private bool _mouseIsPressed = false;
	private float _minLenghtOfMovingVector = 0.1f;
	
	void Update()
	{
		if (Input.GetMouseDown(0))
        {
			_mouseIsPressed = true;
			pointerPosStart = Camera.main.ScreenToWorldPoint(Input.mousePosition);
		}
		else if (Input.GetMouseUp(0))
        {
			_mouseIsPressed = false;
		}
		
		if(_mouseIsPressed)
		{
			MoveTornado();
		}
	}
	
	private void MoveTornado()
	{
		Vector3 pointerPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
		Vector3 diff = pointerPosStart - pointerPos;
		diff.y = 0f;
		if(diff.magnit > _minLenghtOfMovingVector)
		{
			Vector3 newPosition = transform.position + diff * _tornadoMovingSpeed * Time.deltaTime;
			newPosition = ClampTornadoPosition(newPosition);
			transform.position = newPosition;
		}
	}

    private void OnTriggerEnter(Collider other)
    {
		BlockerModel blocker = other.gameObject.GetComponent<BlockerModel>();
		if(blocker != null)
		{
			switch(other.tag)
			{
				//TODO: move from tag to enum in BlockerModel
				case "RedMarker": OnRedMarker(blocker); break;
				case "GreenMarker": OnGreenMarker(blocker); break;
				case "BlackMarker": OnBlackMarker(blocker); break;
			}
		}
    }
	
	private void OnRedMarker(BlockerModel blocker)
	{
		Destroy(blocker.gameObject);
        coreGameplayController.EndTheGame();
	}
	
	private void OnGreenMarker(BlockerModel blocker)
	{
		AddXPForDestroy(blocker.gameObject);
        uiController.AddScore(5);
        Destroy(blocker.gameObject);
	}
	
	private void OnBlackMarker(BlockerModel blocker)
	{
		Destroy(blocker.gameObject);
		coreGameplayController.CreateTornado(transform.position, transform.localScale);
		Destroy(gameObject);
	}

    private void AddXPByDestroy(int value)
    {
        if (coreGameplayController != null)
        {
            coreGameplayController.AddXP(value);
        }

		//TODO: move xp and level to tornado model, and checking levelup in it
        if (coreGameplayController.UpdateTornadoTier())
        {
            ScaleUpTornado();
        }
    }

    private void ScaleUpTornado()
    {
        //tornadoNextScale = tornadoScale + coreGameplayC.config.modG;
        int tornadoScaleID = (int)coreGameplayController.config.tornadoTier;
        float tornadoScaleLevel = coreGameplayController.config.scaleLevel[tornadoScaleID];
        tornadoNextScale = tornadoScaleLevel;
        //Debug.Log("tornadoScaleLevel: " + (TierType)tornadoScaleID);
        StartCoroutine(ScaleUpTornadoCoroutine());
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
    }

    private Vector3 ClampTornadoPosition(Vector3 position)
    {
        Borders borders = coreGameplayController.config.borders;

        return new Vector3(
            Mathf.Clamp(tornadoRB.position.x, borders.xMin, borders.xMax),
            0F,
            Mathf.Clamp(tornadoRB.position.z, borders.zMin, borders.zMax)
        );
    }
}
