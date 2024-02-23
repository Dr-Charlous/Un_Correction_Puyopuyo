using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class MainGame : MonoBehaviour
{
    //Snigleton
    public static MainGame Instance;

    [SerializeField] GameObject[] _puyoPrefab;
    [SerializeField] PuyoController _puyoActive;
    [SerializeField] PuyoController[,] _puyosInGrid;
    [SerializeField] Transform _puyoTransformParent;
    [SerializeField] Vector2Int _gridSize;
    [SerializeField] float _offset;
    [SerializeField] float _timeBetweenFall;
    [SerializeField] float _timerSpeed;
    [SerializeField] float _timer;


    bool[,] _puyosPassed;
    bool[,] _puyosPassed2;
    PuyoController[,] _puyosPosition;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        //DebugGrid();

        CreateGrid();
        CreatePuyo();
    }

    private void Update()
    {
        _timer += _timerSpeed * Time.deltaTime;

        if (_timer >= _timeBetweenFall)
        {
            _timer = 0;

            if (_puyoActive.GridPosition.y > 0 && _puyosPosition[_puyoActive.GridPosition.x, _puyoActive.GridPosition.y - 1] == null)
                _puyoActive.Moving(_puyoActive.GridPosition + Vector2Int.down);
            else
            {
                PuyoStop();
                CreatePuyo();
            }
        }

        Vector2Int _direction = new Vector2Int(0, 0);

        if (Input.GetKeyDown(KeyCode.LeftArrow) && _puyoActive.GridPosition.x > 0 && _puyosPosition[_puyoActive.GridPosition.x - 1, _puyoActive.GridPosition.y] == null)
        {
            _direction += Vector2Int.left;
        }

        if (Input.GetKeyDown(KeyCode.RightArrow) && _puyoActive.GridPosition.x < _gridSize.x - 1 && _puyosPosition[_puyoActive.GridPosition.x + 1, _puyoActive.GridPosition.y] == null)
        {
            _direction += Vector2Int.right;
        }

        _puyoActive.Moving(_puyoActive.GridPosition + _direction);
    }



    //Fonction conversion
    public Vector3 GridToWorld(int x, int y)
    {
        return new Vector2(x * _offset, y * _offset);
    }
    //Posibilitée de mettre divers paramètres :O
    public Vector3 GridToWorld(Vector2Int position)
    {
        return new Vector2(position.x * _offset, position.y * _offset);
    }



    //Afficher la grille avec des puyos
    void DebugGrid()
    {
        for (int y = 0; y < _gridSize.y; y++)
        {
            for (int x = 0; x < _gridSize.x; x++)
            {
                int number = Random.Range(0, _puyoPrefab.Length);
                Instantiate(_puyoPrefab[number], GridToWorld(x, y), Quaternion.identity, _puyoTransformParent);
            }
        }
    }

    void CreatePuyo()
    {
        //Instance des puyos
        int number = Random.Range(0, _puyoPrefab.Length);
        number = 0;
        GameObject puyo = Instantiate(_puyoPrefab[number], GridToWorld(_gridSize.x / 2, _gridSize.y - 1), Quaternion.identity);
        puyo.GetComponent<PuyoController>().Initialize(_gridSize.x / 2, _gridSize.y - 1);

        _puyoActive = puyo.GetComponent<PuyoController>();
    }

    void PuyoStop()
    {
        _puyosPosition[_puyoActive.GridPosition.x, _puyoActive.GridPosition.y] = _puyoActive;
        _puyosInGrid[_puyoActive.GridPosition.x, _puyoActive.GridPosition.y] = _puyoActive;

        ComboCheck( _puyoActive.GridPosition.x, _puyoActive.GridPosition.y,  _puyoActive.Color, 0);

        int combo = 0;

        for (int y = 0; y < _gridSize.y; y++)
        {
            for (int x = 0; x < _gridSize.x; x++)
            {
                if (_puyosPassed[x, y])
                    combo++;
            }
        }

        Debug.Log(combo);

        ClearGridPuyoCombo();
    }

    void CreateGrid()
    {
        _puyosPosition = new PuyoController[_gridSize.x, _gridSize.y];
        _puyosInGrid = new PuyoController[_gridSize.x, _gridSize.y];
        _puyosPassed = new bool[_gridSize.x, _gridSize.y];
        _puyosPassed2 = new bool[_gridSize.x, _gridSize.y];
    }

    void ClearGridPuyoCombo()
    {
        for (int y = 0; y < _gridSize.y; y++)
        {
            for (int x = 0; x < _gridSize.x; x++)
            {
                _puyosPassed[x, y] = false;
            }
        }
    }

    int ComboCheck( int x, int y, PuyoController.ColorPuyo color, int comboValue)
    {
        if (x < 0 || x >= _gridSize.x || y < 0 || y >= _gridSize.y)
            return 0;
        if (_puyosInGrid[x, y] == null || _puyosInGrid[x, y].Color != color || _puyosPassed[x, y] )
            return 0;

        comboValue++;
        _puyosPassed[x, y] = true;

        //if (comboValue >= 4)
        //    return comboValue;

        int right = ComboCheck(x + 1, y, color, comboValue);
        int left = ComboCheck(x - 1, y, color, comboValue);
        int up = ComboCheck(x, y + 1, color, comboValue);
        int down = ComboCheck(x, y - 1, color, comboValue);

        return Mathf.Max( comboValue,  right , left , up , down );
    }

    private void OnDrawGizmos()
    {
        //if (_puyosPassed == null)
        //    return;

        //for (int y = 0; y < _gridSize.y; y++)
        //{
        //    for (int x = 0; x < _gridSize.x; x++)
        //    {
        //        if(_puyosPassed2[x,y])
        //        {
        //            Gizmos.DrawSphere(GridToWorld(x, y), 0.1f);
        //        }
        //    }
        //}
    }
}
