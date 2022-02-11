using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Game : MonoBehaviour
{
    public GameObject square;
    public GameObject flag;
    public GameObject crossed_flag;
    public GameObject mine;
    public GameObject red_mine;

    public GameObject One;
    public GameObject Two;
    public GameObject Three;
    public GameObject Four;
    public GameObject Five;
    public GameObject Six;
    public GameObject Seven;
    public GameObject Eight;

    public Text flags;

    int n_flags = 99;
    int covered_mines = 0;

    GameObject[] nums = new GameObject[8];

    bool reset = false;

    public struct Cell {
        public int num;
        public bool mine;
        public bool flag;
        public bool open;
    }

    Cell[,] board = new Cell[16,30];
    GameObject[,] squares = new GameObject[16, 30];

    void NewGame() {
        reset = false;
        n_flags = 99;
        covered_mines = 0;
        flags.text = n_flags + "";
        for (int i = 0; i < 16; ++i)
        {
            for (int j = 0; j < 30; ++j)
            {
                board[i, j].num = 0;
                board[i, j].mine = false;
                board[i, j].flag = false;
                board[i, j].open = false;
                squares[i, j] = Instantiate(square, new Vector3(j - 14.5f, i - 7.5f, -1), Quaternion.identity);
            }
        }
        for (int i = 0; i < 99; ++i)
        {
            int n = Random.Range(0, 16 * 30 - 1);
            if (board[n % 16, n / 16].mine == false)
            {
                board[n % 16, n / 16].mine = true;
                if (n % 16 > 0) ++board[n % 16 - 1, n / 16].num;
                if (n / 16 > 0) ++board[n % 16, n / 16 - 1].num;
                if (n % 16 < 15) ++board[n % 16 + 1, n / 16].num;
                if (n / 16 < 29) ++board[n % 16, n / 16 + 1].num;
                if (n % 16 > 0 && n / 16 > 0) ++board[n % 16 - 1, n / 16 - 1].num;
                if (n % 16 < 15 && n / 16 > 0) ++board[n % 16 + 1, n / 16 - 1].num;
                if (n % 16 > 0 && n / 16 < 29) ++board[n % 16 - 1, n / 16 + 1].num;
                if (n % 16 < 15 && n / 16 < 29) ++board[n % 16 + 1, n / 16 + 1].num;
            }
            else --i;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        NewGame();
        nums[0] = One;
        nums[1] = Two;
        nums[2] = Three;
        nums[3] = Four;
        nums[4] = Five;
        nums[5] = Six;
        nums[6] = Seven;
        nums[7] = Eight;
    }

    void CleanBoard() {
        for (int i = 0; i < 16; ++i)
        {
            for (int j = 0; j < 30; ++j)
            {
                Destroy(squares[i, j]);
            }
        }

    }

    void EndGame(bool win) {
        for (int i = 0; i < 16; ++i)
        {
            for (int j = 0; j < 30; ++j)
            {
                if (board[i, j].flag == false && board[i, j].open == false) {
                    Destroy(squares[i, j]);
                    if (board[i, j].mine == true) squares[i, j] = Instantiate(mine, new Vector3(j - 14.5f, i - 7.5f, -1), Quaternion.identity);
                    else if (board[i, j].num != 0 && win == true) squares[i, j] = Instantiate(nums[board[i, j].num - 1], new Vector3(j - 14.5f, i - 7.5f, -1), Quaternion.identity);
                    else if (win == false) squares[i, j] = Instantiate(square, new Vector3(j - 14.5f, i - 7.5f, -1), Quaternion.identity);
                }
                else if (board[i, j].flag == true && board[i, j].open == false && board[i, j].mine == false) {
                    Destroy(squares[i, j]);
                    squares[i, j] = Instantiate(crossed_flag, new Vector3(j - 14.5f, i - 7.5f, -1), Quaternion.identity);
                }
            }
        }
        reset = true;
    }

    void UpdateBoard(int x, int y) {
        if (board[y, x].open == false && board[y, x].flag == false) {
            board[y, x].open = true;
            Destroy(squares[y, x]);
            if (board[y, x].mine == true) {
                squares[y, x] = Instantiate(red_mine, new Vector3(x - 14.5f, y - 7.5f, -1), Quaternion.identity);
                EndGame(false);
            } 
            else if (board[y, x].num != 0) squares[y, x] = Instantiate(nums[board[y, x].num - 1], new Vector3(x - 14.5f, y - 7.5f, -1), Quaternion.identity);
            else {
                if (y > 0) UpdateBoard(x, y - 1);
                if (x > 0) UpdateBoard(x - 1, y);
                if (y < 15) UpdateBoard(x, y + 1);
                if (x < 29) UpdateBoard(x + 1, y);
                if (y > 0 && x > 0) UpdateBoard(x - 1, y - 1);
                if (y > 0 && x < 29) UpdateBoard(x + 1, y - 1);
                if (y < 15 && x > 0) UpdateBoard(x - 1, y + 1);
                if (y < 15 && x < 29) UpdateBoard(x + 1, y + 1);
            }
        }
    }

    void Flag(int x, int y) {
        if (board[y, x].open == false && board[y, x].flag == false && n_flags > 0)
        {
            board[y, x].flag = true;
            Destroy(squares[y, x]);
            squares[y, x] = Instantiate(flag, new Vector3(x - 14.5f, y - 7.5f, -1), Quaternion.identity);
            --n_flags;
            flags.text = n_flags + "";
            if (board[y, x].mine == true) {
                ++covered_mines;
                if (covered_mines == 99) EndGame(true);
            }
            
        }
        else if (board[y, x].open == false && board[y, x].flag == true)
        {
            board[y, x].flag = false;
            Destroy(squares[y, x]);
            squares[y, x] = Instantiate(square, new Vector3(x - 14.5f, y - 7.5f, -1), Quaternion.identity);
            ++n_flags;
            flags.text = n_flags + "";
            if (board[y, x].mine == true) --covered_mines;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (reset == true)
            {
                CleanBoard();
                NewGame();
            }
            else {
                Vector3 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                int xpos = (int)Mathf.Round(pos.x + 14.5f);
                int ypos = (int)Mathf.Round(pos.y + 7.5f);
                UpdateBoard(xpos, ypos);
            }
        }
        else if (Input.GetMouseButtonDown(1))
        {
            if (reset == true)
            {
                CleanBoard();
                NewGame();
            }
            else {
                Vector3 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                int xpos = (int)Mathf.Round(pos.x + 14.5f);
                int ypos = (int)Mathf.Round(pos.y + 7.5f);
                Flag(xpos, ypos);
            }
        }
    }
}
