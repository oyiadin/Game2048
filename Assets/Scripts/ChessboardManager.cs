using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class ChessboardManager : MonoBehaviour
{
    public GameObject tileBgPrefab;
    public GameObject tilePrefab;
    
    public int margin = 12;
    public int size = 4;
    public int winNumber = 2048;
    
    private GameObject[,] tiles;
    private int _tilesCount = 0;
    private float _spacing, _originX, _originY;

    private string status = "gaming";

    private bool playingAnim = false;

    public Text ScoreText;
    private int Score
    {
        get => _score;
        set
        {
            _score = value;
            ScoreText.text = _score.ToString();
        }
    }
    
    public Text BestScoreText;
    private int BestScore
    {
        get => _bestScore;
        set
        {
            _bestScore = value;
            BestScoreText.text = _bestScore.ToString();
        }
    }

    private int _score = 0;
    private int _bestScore = 0;
    
    void Start()
    {
        tiles = new GameObject[size, size];
        var position = transform.position;
        var width = ((RectTransform) transform).rect.width;
        _spacing = (width - margin * (size + 1)) / size + margin;
        _originX = position.x - width / 2 + margin;
        _originY = position.y + width / 2 - margin;
        
        for (int i = 0; i < size; i++)
        {
            for (int j = 0; j < size; j++)
            {
                Instantiate(
                    tileBgPrefab, 
                    transform.TransformPoint(new Vector3(
                        _originX + _spacing * i, _originY - _spacing * j, position.z)),
                    Quaternion.identity,
                    transform);
            }
        }

        AddTileRandomly();
        AddTileRandomly();

        Score = BestScore = 0;
    }

    void Update()
    {
        if (status == "win" || status == "lose")
        {
            return;
        }
        else
        {
            if (playingAnim)
            {
                
            }
            else
            {
                var left = Input.GetKeyDown(KeyCode.LeftArrow);
                var right = Input.GetKeyDown(KeyCode.RightArrow);
                var up = Input.GetKeyDown(KeyCode.UpArrow);
                var down = Input.GetKeyDown(KeyCode.DownArrow);
                var count = 0;
                int dirX = 0, dirY = 0;
                if (left)
                {
                    dirX = -1;
                    ++count;
                }

                if (right)
                {
                    dirX = 1;
                    ++count;
                }

                if (up)
                {
                    dirY = -1;
                    ++count;
                }

                if (down)
                {
                    dirY = 1;
                    ++count;
                }
                if (count > 1)
                {
                    Debug.LogError("Multikey is not allowed");
                }
                else if (count == 1)
                {
                    var anyTileMoved = MoveTiles(dirX, dirY);
                    if (anyTileMoved)
                    {
                        ClearMergedFlags();
                        AddTileRandomly();
                    }
                    else
                    {
                        if (_tilesCount == size * size)
                        {
                            if (CheckIfLose())
                            {
                                status = "lose";
                                ShowGameOverPanel("lose");
                            }
                        }
                    }
                }
            }
        }
    }

    private void AddTileRandomly()
    {
        var cell = SelectCellRandomly();
        var position = transform.position;
        var tileGO = Instantiate(
            tilePrefab,
            transform.TransformPoint(new Vector3(
                _originX + _spacing * cell.x, _originY - _spacing * cell.y, position.z)),
            Quaternion.identity,
            transform);
        tiles[(int) cell.x, (int) cell.y] = tileGO;
        ++_tilesCount;
    }

    private Vector2 SelectCellRandomly()
    {
        var availableCells = new List<Vector2>();
        for (int i = 0; i < size; i++)
        {
            for (int j = 0; j < size; j++)
            {
                if (tiles[i, j] == null)
                {
                    availableCells.Add(new Vector2(i, j));
                }
            }
        }
        
        return availableCells[Random.Range(0, availableCells.Count - 1)];
    }

    private bool MoveTiles(int dirX, int dirY)
    {
        bool result = false;
        int fakeDirX = dirX != 0 ? dirX : 1;
        int fakeDirY = dirY != 0 ? dirY : 1;
        for (int i = (fakeDirX < 0 ? 0 : size - 1) - dirX; i >= 0 && i <= size - 1; i += (dirX < 0 && fakeDirX < 0) ? 1 : -1)
        {
            for (int j = (fakeDirY < 0 ? 0 : size - 1) - dirY; j >= 0 && j <= size - 1; j += (dirY < 0 && fakeDirY < 0) ? 1 : -1)
            {
                // Debug.LogFormat("[MoveTiles] ({0}, {1})", i, j);
                if (tiles[i, j] != null)
                {
                    var isMoved = MoveTile(i, j, dirX, dirY);
                    result = result || isMoved;
                }
            }
        }

        return result;
    }

    private bool MoveTile(int x, int y, int dirX, int dirY)
    {
        var value = tiles[x, y].GetComponent<Tile>().Value;
        for (int i = x + dirX; i >= 0 && i <= size - 1; i += dirX)
        {
            for (int j = y + dirY; j >= 0 && j <= size - 1; j += dirY)
            {
                var tileGO = tiles[i, j];
                if (tileGO != null)
                {
                    var tileComp = tileGO.GetComponent<Tile>();
                    if (!tileComp.Merged && tileComp.Value == value)
                    {
                        tileComp.Value *= 2;
                        Score += tileComp.Value;
                        if (BestScore < Score)
                        {
                            BestScore = Score;
                        }
                        tileComp.Merged = true;
                        Destroy(tiles[x, y]);
                        tiles[x, y] = null;
                        --_tilesCount;

                        if (tileComp.Value >= winNumber)
                        {
                            status = "win";
                            ShowGameOverPanel("win");
                        }

                        return true;
                    }

                    return MoveTileReal(x, y, i - dirX, j - dirY);

                }

                if (dirY == 0) break;
            }

            if (dirX == 0) break;
        }
        
        // if we cannot find any tiles in the expected direction
        int nx = x, ny = y;
        if (dirX != 0)
            nx = dirX > 0 ? size - 1 : 0;
        if (dirY != 0)
            ny = dirY > 0 ? size - 1 : 0;
        return MoveTileReal(x, y, nx, ny);
    }

    private bool MoveTileReal(int ox, int oy, int nx, int ny)
    {
        if (ox == nx && oy == ny)
            return false;
        var tileGO = tiles[ox, oy];
        tiles[nx, ny] = tileGO;
        tiles[ox, oy] = null;

        tileGO.transform.localPosition = new Vector3(
            _originX + _spacing * nx, _originY - _spacing * ny, transform.localPosition.z);
        return true;
    }

    private void ClearMergedFlags()
    {
        for (int i = 0; i < size; ++i)
        {
            for (int j = 0; j < size; ++j)
            {
                var tile = tiles[i, j];
                if (tile != null)
                {
                    tile.GetComponent<Tile>().Merged = false;
                }
            }
        }
    }

    private void ShowGameOverPanel(string type)
    {
        if (type == "lose")
        {
            Debug.Log("lose!!");
        }
        else if (type == "win")
        {
            Debug.Log("win!!");
        }
        else
        {
            Debug.LogError("unknown gameover type!");
        }
    }

    private bool CheckIfLose()
    {
        for (int i = 0; i < size; ++i)
        {
            for (int j = 0; j < size; ++j)
            {
                var value = tiles[i, j].GetComponent<Tile>().Value;
                if (i > 0)
                {
                    if (tiles[i - 1, j].GetComponent<Tile>().Value == value)
                    {
                        return false;
                    }
                }
                if (i < size - 1)
                {
                    if (tiles[i + 1, j].GetComponent<Tile>().Value == value)
                    {
                        return false;
                    }
                }
                if (j > 0)
                {
                    if (tiles[i, j - 1].GetComponent<Tile>().Value == value)
                    {
                        return false;
                    }
                }
                if (j < size - 1)
                {
                    if (tiles[i, j + 1].GetComponent<Tile>().Value == value)
                    {
                        return false;
                    }
                }
            }
        }

        return true;
    }
}
