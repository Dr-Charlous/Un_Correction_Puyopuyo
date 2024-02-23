using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
///
/// </summary>
public class PuyoController : MonoBehaviour
{
    public enum ColorPuyo
    {
        Red,
        Green, 
        Blue
    }

    public Vector2Int GridPosition => _gridPosition;

    public ColorPuyo Color;
    
    Vector2Int _gridPosition;



    public void Initialize(int x, int y)
    {
        _gridPosition = new Vector2Int(x, y);
    }

    //Chute position
    public void Moving(int x, int y) 
    {
        _gridPosition = new Vector2Int(x, y);
        UpdatePosition();
    }

    public void Moving(Vector2Int position)
    {
        _gridPosition = position;
        UpdatePosition();
    }

    ///Changer la position de l'obj
    void UpdatePosition()
    {
        this.transform.position = MainGame.Instance.GridToWorld(_gridPosition);
    }
}
