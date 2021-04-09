using AgToolkit.AgToolkit.Core.Timer;
using AgToolkit.Core.Helper;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class MapEvent : MonoBehaviour
{
    private bool _isMoving = false;
    private bool _isShutingDown = false;
    private MapEventData _data = null;
    private Timer _lifeTime;
    private Vector4 _mapLimit;
    private Dictionary<GameObject, Vector3> _gameObjects = new Dictionary<GameObject, Vector3>(); // instance - direction
    private Dictionary<GameObject, Player> _players = new Dictionary<GameObject, Player>();

    private void OnDestroy()
    {
        if (!_isShutingDown)
        {
            Finish();
        }
    }

    private void Update()
    {
        if (!_isMoving && _data.MoveTime > 0 && !_isShutingDown)
        {
            CoroutineManager.Instance.StartCoroutine(Move());
        }
    }

    private IEnumerator Move()
    {
        if (_isMoving || _isShutingDown) yield break;

        float t = 0f;
        List<Vector3> positions = _gameObjects.Keys.Select(item => item.transform.localPosition).ToList();

        while (t < _data.MoveTime)
        {
            if (_isShutingDown) yield break;

            // if player is hooked, check gameobject(dictionary) is not outside grid, release player
            for (int i = 0; i < _gameObjects.Keys.Count; i++)
            {
                GameObject item = _gameObjects.Keys.ElementAt(i);
                item.transform.localPosition = Vector3.Lerp(positions[i], positions[i] + _gameObjects[item], t/_data.MoveTime);

                if (_players.ContainsKey(item))
                {
                    if (item.transform.position.x < _mapLimit.x || item.transform.position.x > _mapLimit.y || 
                        item.transform.position.z < _mapLimit.z || item.transform.position.z > _mapLimit.w)
                    {
                        ReleasePlayer(item);
                    }
                }
            }

            t += Time.deltaTime;

            yield return null;
        }

        _isMoving = false;
    }

    private void ReleasePlayer(GameObject item)
    {
        Debug.Log("Player is released");
        _players[item].transform.SetParent(MapManager.Instance.transform, true);
        _players[item].IsFrozen = false;
        _players.Remove(item);
    }

    private void Finish()
    {
        //todo unsubscribe timer, destroy gamesObjects, freedom player
        _isShutingDown = true;
        TimerManager.Instance?.RemoveTimer(_lifeTime);

        for (int i = 0; i < _gameObjects.Keys.Count; i++)
        {
            GameObject item = _gameObjects.Keys.ElementAt(i);

            if (_players.ContainsKey(item))
            {
                ReleasePlayer(item);
            }

            GameObject.Destroy(item);
        }

        Destroy(gameObject); // Can Delete MapManager, check it
    }

    public void Init(MapEventData data)
    {
        _data = data;
        _isMoving = _data.MoveTime < 0;
        UnityEvent lifeTimeEvent = new UnityEvent();
        lifeTimeEvent.AddListener(Finish);
        _lifeTime = new Timer($"{data.Id}_lifeTime", data.LifeTime, lifeTimeEvent);

        //MAP LIMIT
        Vector3 mapPos = MapManager.Instance.gameObject.transform.position;
        Vector3 mapScale = MapManager.Instance.gameObject.GetComponent<Renderer>().bounds.size / 2;

        float maxX = mapPos.x + mapScale.x - _data.Prefab.transform.lossyScale.x;
        float minX = mapPos.x - mapScale.x + _data.Prefab.transform.lossyScale.x;
        float maxY = mapPos.z + mapScale.z - _data.Prefab.transform.lossyScale.z;
        float minY = mapPos.z - mapScale.z + _data.Prefab.transform.lossyScale.z;

        _mapLimit = new Vector4(minX, maxX, minY, maxY);

        //Instantiate all gameobject
        for (int i = 0; i < _data.PositionsDirections.Keys.Count; i++)
        {
            Vector3 pos = _data.PositionsDirections.Keys.ElementAt(i);
            GameObject item = GameObject.Instantiate(_data.Prefab, MapManager.Instance.transform);
            item.SetActive(false);
            item.transform.rotation = Quaternion.identity;
            item.transform.localPosition = pos;
            item.GetComponent<MapEventInstance>().Parent = this;

            _gameObjects.Add(item, _data.PositionsDirections[pos]);
            item.SetActive(true);
        }

        TimerManager.Instance.StartTimer(_lifeTime);
    }

    public void PlayerCollision(Player p, GameObject child)
    {
        if (_players.ContainsValue(p)) return;
        if (child.transform.position.x < _mapLimit.x || child.transform.position.x > _mapLimit.y ||
                        child.transform.position.z < _mapLimit.z || child.transform.position.z > _mapLimit.w) return;
        if (_isShutingDown) return;

        _players.Add(child, p);

        if (_data.FreezePlayer)
        {
            p.IsFrozen = true;
        }

        if (_data.HookPlayer)
        {
            p.transform.SetParent(child.transform, false);
        }

        p.DropResources();
    }

    public void BaseCollision(Base b)
    {
        if (_isShutingDown) return;
        b.TakeOfPourcentOfLifeTime(_data.Damage);
    }
}
