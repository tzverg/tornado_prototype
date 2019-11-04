using System;
using System.Collections;
using UnityEngine;

public class TornadoController : MonoBehaviour
{
    //[SerializeField] private bool tornadoTapMotion;

	public float TornadoScale => _tornadoScale;
    public int Level => _level;

    public float _tornadoScale;
    private float _tornadoNextScale;

    private UIController _uiController;
    private CoreGameplayController _coreGameplayController;
    //private Rigidbody tornadoRB;

    //private RaycastHit mouseHit;

    private int _xpMultipler = 25;
    private int _level = 1;
    private int _currentXP = 0;
    private int _nextLevelXP;
    private Vector3 pointerPosStart;
    //private Vector3 pointerPosEnd;

    //private bool pointerPosSaved = false;

    // check mouse motion
    //private bool timerLaunched = false;
    //private Vector3 pointerTimerPos;

	private float _tornadoMovingSpeed = -50.0f;

	private bool _mouseIsPressed = false;
	private float _minLenghtOfMovingVector = 0.1f;

    public void Init(CoreGameplayController coreGameplayController, UIController uiController, float tornadoScale, int level = 1)
    {
        _coreGameplayController = coreGameplayController;
        _uiController = uiController;
        _tornadoScale = tornadoScale;
        _level = level;
        _nextLevelXP = _level * _xpMultipler;

        _uiController.SetXPLable(_level, _currentXP, _nextLevelXP);
    }

    public void LevelUp()
    {
        _level++;
        _nextLevelXP = _level * _xpMultipler;

        ScaleUpTornado();
        _coreGameplayController.UpdateBlockerPrefab(_level);
    }

	void Update()
	{
		if (Input.GetMouseButtonDown(0))
        {
			_mouseIsPressed = true;
			pointerPosStart = Camera.main.ScreenToWorldPoint(Input.mousePosition);
		}
		else if (Input.GetMouseButtonUp(0))
        {
			_mouseIsPressed = false;
		}
		
		if(_mouseIsPressed)
		{
			MoveTornado();
            pointerPosStart = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        }
	}
	
	private void MoveTornado()
	{
		Vector3 pointerPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
		Vector3 diff = pointerPosStart - pointerPos;
        diff.z *= 2f;
        diff.y = 0f;
		Vector3 newPosition = transform.position + diff * _tornadoMovingSpeed * Time.deltaTime;
		newPosition = ClampTornadoPosition(newPosition);
		transform.position = newPosition;
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
        _coreGameplayController.EndTheGame();
	}
	
	private void OnGreenMarker(BlockerModel blocker)
	{
        int scores = blocker.XpForDestroy;
        AddXPByDestroy(scores);
        _coreGameplayController.AddScores(scores);
        _uiController.SetXPLable(_level, _currentXP, _nextLevelXP);
        Destroy(blocker.gameObject);
	}
	
	private void OnBlackMarker(BlockerModel blocker)
	{
		Destroy(blocker.gameObject);
        _coreGameplayController.CreateTornado(transform.position, transform.localScale);
		Destroy(gameObject);
	}

    private void AddXPByDestroy(int value)
    {
        _currentXP += value;
        if (_currentXP >= _nextLevelXP)
        {
            LevelUp();
        }
    }

    private void ScaleUpTornado(bool showLevelUpAnimation = true)
    {
        _tornadoNextScale = 1.0f + _level * 0.25f;
        if (showLevelUpAnimation)
        {
            StartCoroutine(ScaleUpTornadoCoroutine());
        }
        else
        {
            _tornadoScale = _tornadoNextScale;
            SetTornadoLocalScale();
        }
    }

    private IEnumerator ScaleUpTornadoCoroutine()
    {
        for (float value = _tornadoScale; value <= _tornadoNextScale; value += 0.05f)
        {
            _tornadoScale = value;
            SetTornadoLocalScale();
            yield return new WaitForSeconds(.1f);
        }

        _tornadoScale = _tornadoNextScale;
        SetTornadoLocalScale();
    }

    private void SetTornadoLocalScale()
    {
        if (transform.localScale.x != _tornadoScale || transform.localScale.y != _tornadoScale)
        {
            transform.localScale = new Vector3(_tornadoScale, _tornadoScale, _tornadoScale);
        }
    }

    private Vector3 ClampTornadoPosition(Vector3 position)
    {
        Borders borders = _coreGameplayController.config.borders;

        return new Vector3(
            Mathf.Clamp(position.x, borders.xMin, borders.xMax),
            0F,
            Mathf.Clamp(position.z, borders.zMin, borders.zMax)
        );
    }
}
