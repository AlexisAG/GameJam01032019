using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using AgToolkit.AgToolkit.Core.Timer;
using UnityEngine.Events;
using AgToolkit.Core.GameModes;

public class Player : MonoBehaviour
{
    [SerializeField]
    private float _averageSpeed = 5f;
    [SerializeField]
    private float _cooldownMine = 5f;
    [SerializeField]
    private float _PowerUpCooldownTimer = 5f;
    [SerializeField]
    private GameObject _minePrefab = null;
    [SerializeField]
    private ParticleSystem _slowEffect = null;
    [SerializeField]
    private ParticleSystem _speedEffect = null;

    private float m_joystickNumber;
    private bool m_isCarryingMine;
    private float _actualSpeed;
    private GameObject m_mine;
    private Rigidbody m_rb;
    private Vector4 _mapLimit;
    private Timer _mineTimer;
    private Timer _powerUpTimer;
    private Timer _speedTimer;
    private List<Ressource> _Ressources = new List<Ressource>();

    private MovementCategory _movementCategory = MovementCategory.e_ForceBased;
    private float _wallElasticity = 0f;

    private Vector2 _movement;
    private Vector2 _movementOrder;
    private float _maxForceSpeed = 4.0f;
    private float _currentMaxForceSpeed = 4.0f;

    public bool PowerUpCooldown = false;
    public Base PlayerBase { get; private set; }
    public GameObject MinePrefab => _minePrefab;

    public enum MovementCategory
    {
        e_Basic,
        e_ForceBased
    }

    // Start is called before the first frame update
    private void Start()
    {
        _actualSpeed = _averageSpeed;
        m_rb = GetComponent<Rigidbody>();
        m_isCarryingMine = false;
        _slowEffect.Stop(true);
        _speedEffect.Stop(true);

        //MAP LIMIT
        Vector3 mapPos = MapManager.Instance.gameObject.transform.position;
        Vector3 mapScale = MapManager.Instance.gameObject.GetComponent<Renderer>().bounds.size / 2;

        float maxX = mapPos.x + mapScale.x - transform.lossyScale.x;
        float minX = mapPos.x - mapScale.x + transform.lossyScale.x;
        float maxY = mapPos.z + mapScale.z - transform.lossyScale.z;
        float minY = mapPos.z - mapScale.z + transform.lossyScale.z;
  
        _mapLimit = new Vector4(minX, maxX, minY, maxY);

        //MAP PROPERTY
        switch (MapManager.Instance.MapProperty)
        {
            case MapManager.MapProperties.e_Ice:
                _movementCategory = MovementCategory.e_ForceBased;
                _averageSpeed /= 2;
                _actualSpeed = _averageSpeed;
                _wallElasticity = MapManager.Instance.WallElasticity;
                break;
            case MapManager.MapProperties.e_Basic:
            default:
                _movementCategory = MovementCategory.e_Basic;
                _wallElasticity = 0f;
                break;
        }

        //TIMERS
        UnityEvent mineEvent = new UnityEvent(); 
        UnityEvent powerUpEvent = new UnityEvent(); 
        UnityEvent speedEvent = new UnityEvent();

        mineEvent.AddListener(() => { m_isCarryingMine = true; });
        powerUpEvent.AddListener(() => { PowerUpCooldown = false; });
        speedEvent.AddListener(() =>
        {
           _slowEffect.Stop(true);
           _speedEffect.Stop(true);
           _actualSpeed = _averageSpeed;
           _currentMaxForceSpeed = _maxForceSpeed;
        });

        _mineTimer = new Timer($"mine_{m_joystickNumber}", _cooldownMine, mineEvent);
        _powerUpTimer = new Timer($"powerUp_{m_joystickNumber}", _PowerUpCooldownTimer, powerUpEvent);
        _speedTimer = new Timer($"speedEffect_{m_joystickNumber}", _PowerUpCooldownTimer, speedEvent);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<Ressource>() != null)
        {
            if (!other.GetComponent<Ressource>().IsUsed && _Ressources.Count < 5)
            {
                Ressource ressource = other.GetComponent<Ressource>();
                _Ressources.Add(ressource);
                ressource.IsPick(this);
                RegisterManager.Instance.GetGameObjectInstance("ResourcesSE")?.GetComponent<AudioSource>()?.Play();
            }
        }
        else if (other.GetComponentInParent<Base>() != null)
        {
            if(other.GetComponentInParent<Base>() == PlayerBase)
            {
                if (_Ressources.Count <= 0) return;

                RegisterManager.Instance.GetGameObjectInstance("BaseSE")?.GetComponent<AudioSource>()?.Play();

                foreach (Ressource ressource in _Ressources)
                {
                    ressource.Activate();
                }

                _Ressources.Clear();
            }
        }
        else if (other.GetComponent<Mine>() != null)
        {
            if(other.GetComponent<Mine>().m_PlayerTag != tag)
            {
                other.GetComponent<Mine>().MakeExplosionEffect();
                RegisterManager.Instance.GetGameObjectInstance("PlayerSE")?.GetComponent<AudioSource>()?.Play();

                foreach (Ressource ressource in _Ressources)
                {
                    ressource.Respawn();
                }

                _Ressources.Clear();
            }
        }
        else if(other.GetComponent<Enemy>() != null)
        {
            DropResources();
            RegisterManager.Instance.GetGameObjectInstance("PlayerSE")?.GetComponent<AudioSource>()?.Play();
        }
    }

    // Update is called once per frame
    private void Update()
    {
        if (!IsPlayable()) return;

        if (Input.GetButton("Fire_P" + m_joystickNumber))
            PutTheMine();

        if(!m_isCarryingMine && !_mineTimer.IsActive)
        {
            TimerManager.Instance.StartTimer(_mineTimer);
        }

        //reset speed if changed with powerup
        if (_actualSpeed != _averageSpeed)
        {
            if (_actualSpeed < _averageSpeed)
            {
                if (!_slowEffect.isPlaying)
                {
                    _slowEffect.Play(true);
                }
            }
            else
            {
                if (!_speedEffect.isPlaying)
                {
                    _speedEffect.Play(true);
                }
            }
        }

        //manage pickup cooldown
        if (PowerUpCooldown && !_powerUpTimer.IsActive)
        {
            TimerManager.Instance.StartTimer(_powerUpTimer);
        }
    }

    private void FixedUpdate()
    {
        if (!IsPlayable()) return;

        switch (_movementCategory)
        {
            case MovementCategory.e_ForceBased:
                MoveWithControllerBasedOnForce(m_joystickNumber);
                break;
            case MovementCategory.e_Basic:
            default:
                MoveWithController(m_joystickNumber);
                break;
        }        
    }

    private bool IsPlayable()
    {
        if (GameManager.Instance.GetCurrentGameMode<SoloGameMode>()?.GameIsOver ?? true) return false;
        if (!(gameObject.tag == "Player " + m_joystickNumber)) return false;

        return true;
    }

    private void MoveAndRotatePlayer(Vector3 _position, Vector2 _forwardVector)
    {
        //Move player to the new position
        m_rb.MovePosition(_position);

        if (_forwardVector.x != 0 && _forwardVector.y != 0)
        {
            float radius = Mathf.Atan2(_forwardVector.x, _forwardVector.y);
            radius = (radius * 180f) / Mathf.PI;
            m_rb.MoveRotation(Quaternion.Euler(0, radius, 0));
        }
    }

    private void MoveWithController(float p_joystickNumber)
    {
        //get controller axis
        Vector2 input = new Vector2(Input.GetAxis("LeftJoystickX_P" + p_joystickNumber), -Input.GetAxis("LeftJoystickY_P" + p_joystickNumber));
        input.Normalize();

        //New position
        Vector3 l_newPos = m_rb.position + new Vector3(input.x, 0f, input.y) * Time.fixedDeltaTime * _actualSpeed;

        //Check if player is in bounds
        l_newPos.x = Mathf.Clamp(l_newPos.x, _mapLimit.x, _mapLimit.y);
        l_newPos.z = Mathf.Clamp(l_newPos.z, _mapLimit.z, _mapLimit.w);

        //Move player to the new position
        MoveAndRotatePlayer(l_newPos, input);
    }

    private void MoveWithControllerBasedOnForce(float p_joystickNumber)
    {
        //get controller axis
        _movementOrder = new Vector2(Input.GetAxis("LeftJoystickX_P" + p_joystickNumber) * _currentMaxForceSpeed, -Input.GetAxis("LeftJoystickY_P" + p_joystickNumber) * _currentMaxForceSpeed);

        //Executing order
        if (_movement.y < _movementOrder.y)
        {
            _movement.y += _actualSpeed * Time.fixedDeltaTime;
        }
        if (_movement.y > _movementOrder.y)
        {
            _movement.y -= _actualSpeed * Time.fixedDeltaTime;
        }
        if (_movement.x < _movementOrder.x)
        {
            _movement.x += _actualSpeed * Time.fixedDeltaTime;
        }
        if (_movement.x > _movementOrder.x)
        {
            _movement.x -= _actualSpeed * Time.fixedDeltaTime;
        }

        //Updating the position
        

        //New position
        Vector3 l_newPos = m_rb.position + new Vector3(_movement.x, 0, _movement.y) * _actualSpeed * Time.fixedDeltaTime;
        // To debug
        //Debug.DrawLine(gameObject.transform.position, gameObject.transform.position + new Vector3(movement.x, 0, movement.y), Color.black, 0.1f,false);

        //Check if player is in bounds
        float clampResult = Mathf.Clamp(l_newPos.x, _mapLimit.x, _mapLimit.y);
        if (clampResult != l_newPos.x)
        {
            l_newPos.x = clampResult;
            _movement.x *= -_wallElasticity;
        }
        clampResult = Mathf.Clamp(l_newPos.z, _mapLimit.z, _mapLimit.w);
        if (clampResult != l_newPos.z)
        {
            l_newPos.z = clampResult;
            _movement.y *= -_wallElasticity;
        }

        //Move player to the new position
        MoveAndRotatePlayer(l_newPos, _movementOrder);
    }

    public void Init(int index, Base b)
    {
        PlayerBase = b;
        m_joystickNumber = index;
        gameObject.tag = $"Player {index}";
    }

    public void DropResources()
    {
        foreach (Ressource ressource in _Ressources)
        {
            ressource.Respawn();
        }

        _Ressources.Clear();
    }

    public int GetNbOfRessources()
    {
        return _Ressources.Count;
    }

    public void ApplySpeedEffect(float effect)
    {
        _actualSpeed *= effect;
        _currentMaxForceSpeed *= effect;
        TimerManager.Instance.StartTimer(_speedTimer);
    }

    public bool HaveMine()
    {
        return m_isCarryingMine;
    }

    public void PutTheMine()
    {

        if (m_isCarryingMine && gameObject.tag == "Player " + m_joystickNumber) {

            if (m_mine != null)
                Destroy(m_mine);

            m_mine = Instantiate<GameObject>(_minePrefab, transform.position, Quaternion.Euler(-90f,0f,0f), MapManager.Instance.transform);
            m_mine.SetActive(false);

            if (MapManager.Instance.AddGameObjectOnTheGrid(Mathf.FloorToInt(transform.localPosition.x), Mathf.FloorToInt(transform.localPosition.z), m_mine, MapManager.TypeObject.e_Mine, false))
            {
                m_mine.GetComponent<Mine>().m_PlayerTag = tag;
                m_isCarryingMine = false;
                m_mine.SetActive(true);
            }
            else
            {
                Destroy(m_mine);
            }
        }
    }
}
