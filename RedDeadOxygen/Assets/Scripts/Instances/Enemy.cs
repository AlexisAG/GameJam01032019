using AgToolkit.Core.GameModes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField]
    private float _enemySpeedMultiplier;
    [SerializeField]
    private float _damage = .05f;
    [SerializeField]
    private string _animatorBool = "isGonaExplose";
    [SerializeField]
    private TrailRenderer _effect;

    private Base _target;
    private Animator _animator;
    private Vector3 _walkingDirection;
    private SoloGameMode _gameMode;

    // Update is called once per frame
    private void FixedUpdate()
    {
        if (_gameMode.GameIsOver)
        {
            Disable();
        }

        transform.SetPositionAndRotation(transform.position + _walkingDirection * Time.deltaTime * _enemySpeedMultiplier, transform.rotation);  
    }

    private void OnTriggerEnter(Collider other)
    {

        if (other.gameObject.GetComponentInParent<Base>() == _target)
        {
            _animator.SetBool(_animatorBool, true);
            RegisterManager.Instance.GetGameObjectInstance("SlimeSE")?.GetComponent<AudioSource>()?.Play();
            ExplosionFinish();
        }
        else if(other.gameObject.GetComponent<Mine>() != null)
        {
            _animator.SetBool(_animatorBool, true);
            other.GetComponent<Mine>().MakeExplosionEffect();
            RegisterManager.Instance.GetGameObjectInstance("MineSE")?.GetComponent<AudioSource>()?.Play();
            Disable();
        }
    }

    private void Disable()
    {
        _effect.Clear();
        gameObject.SetActive(false);
    }

    public void Init(Base baseRef)
    {
        _target = baseRef;
        _animator = GetComponent<Animator>();
        _animator.SetBool(_animatorBool, false);
        _walkingDirection = Vector3.Normalize(baseRef.transform.position - transform.position);
        _gameMode = GameManager.Instance.GetCurrentGameMode<SoloGameMode>();

        //Look at base & add -90f to fix animation
        transform.LookAt(_target.transform.position);
        transform.Rotate(0f, -90f, 0f);
    }

    public void ExplosionFinish()
    {
        _target.TakeOfPourcentOfLifeTime(_damage);
        Disable();
    }
}
