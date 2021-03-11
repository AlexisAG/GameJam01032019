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

    public bool PowerUpCooldown = false;
    public Base PlayerBase { get; private set; }
    public GameObject MinePrefab => _minePrefab;

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
                ressource.IsPick(this);
                _Ressources.Add(ressource);
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
        if (GameManager.Instance.GetCurrentGameMode<SoloGameMode>()?.GameIsOver ?? true) return;
        if (!(gameObject.tag == "Player " + m_joystickNumber)) return;

        MoveWithController(m_joystickNumber);

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

    private void MoveWithController(float p_joystickNumber)
    {
        //get controller axis
        Vector2 l_controllerAxis = new Vector2(Input.GetAxis("LeftJoystickX_P" + p_joystickNumber), -Input.GetAxis("LeftJoystickY_P" + p_joystickNumber));
        l_controllerAxis.Normalize();

        //Movement vector
        Vector3 l_movement = new Vector3((l_controllerAxis.x * _actualSpeed), 0.0f, (l_controllerAxis.y * _actualSpeed)) * Time.deltaTime ;

        //New position
        Vector3 l_newPos = m_rb.position + l_movement;

        //Check if player is in bounds
        l_newPos.x = Mathf.Clamp(l_newPos.x, _mapLimit.x, _mapLimit.y);
        l_newPos.z = Mathf.Clamp(l_newPos.z, _mapLimit.z, _mapLimit.w);

        //Move player to the new position
        m_rb.MovePosition(l_newPos);

        if(l_movement.x != 0 && l_movement.z != 0)
        {
            float radius = Mathf.Atan2(l_movement.x, l_movement.z);
            radius = (radius * 180f) / Mathf.PI;
            m_rb.MoveRotation(Quaternion.Euler(0, radius, 0));
        }
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
        return _Ressources.Count - 1;
    }

    public void ApplySpeedEffect(float effect)
    {
        _actualSpeed *= effect;
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
            MapManager.Instance.AddGameObjectOnTheGrid(Mathf.FloorToInt(transform.localPosition.x), Mathf.FloorToInt(transform.localPosition.z), m_mine, MapManager.TypeObject.e_Mine);
            m_mine.GetComponent<Mine>().m_PlayerTag = tag;
            m_isCarryingMine = false;
        }
    }
}
