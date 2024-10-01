using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum PlayerOption
{
    NONE, //0
    X, // 1
    O // 2
}

public class TTT : MonoBehaviour
{
    public int Rows;
    public int Columns;
    [SerializeField] BoardView board;

    PlayerOption currentPlayer = PlayerOption.X;
    Cell[,] cells;
    //int[,] corners = { { 0, 0 }, { 0, 2 }, { 2, 0 }, { 2, 2 } };

    // Start is called before the first frame update
    void Start()
    {
        cells = new Cell[Columns, Rows];

        board.InitializeBoard(Columns, Rows);

        for (int i = 0; i < Rows; i++)
        {
            for (int j = 0; j < Columns; j++)
            {
                cells[j, i] = new Cell();
                cells[j, i].current = PlayerOption.NONE;
            }
        }
    }



    public void MakeOptimalMove()
    {
        // check for current player
        PlayerOption opponent = PlayerOption.X == currentPlayer ? PlayerOption.O : PlayerOption.X;

        // center cell
        Cell center = cells[1, 1];

        // corner cells
        Cell[] corners = { cells[0, 0], cells[0, 2], cells[2, 0], cells[2, 2] };
        (int, int)[] cornersIndex = { (0, 0), (0, 2), (2, 0), (2, 2) };

        // check corners
        int currentCorner = Array.FindIndex(corners, cell => cell.current == currentPlayer);
        int oppCorner = Array.FindIndex(corners, cell => cell.current == opponent);
        int openCorner = Array.FindIndex(corners, cell => cell.current == PlayerOption.NONE);

        // make winning move if available
        if (WinAvailable(currentPlayer))
        {
            return;
        }
        // block opponent from win if available
        if (WinAvailable(opponent))
        {
            return;
        }

        // if current hold corner and center occupied, take adjacent
        if (currentCorner > -1 && center.current != PlayerOption.NONE)
        {
            int x = cornersIndex[currentCorner].Item1;
            int y = cornersIndex[currentCorner].Item2;

            // choose new x positions
            if (x + 1 < 3 && cells[x + 1, y].current == PlayerOption.NONE)
            {
                ChooseSpace(x + 1, y);
                return;
            }
            else if (x - 1 >= 0 && cells[x - 1, y].current == PlayerOption.NONE)
            {
                ChooseSpace(x - 1, y);
                return;
            }

            // choose new y positions
            else if (y + 1 < 3 && cells[x, y + 1].current == PlayerOption.NONE)
            {
                ChooseSpace(x, y + 1);
                return;
            }
            else if (y - 1 >= 0 && cells[x, y - 1].current == PlayerOption.NONE)
            {
                ChooseSpace(x, y - 1);
                return;
            }

        }

        // check if opponent has corner, take center if available
        if (oppCorner > -1 && center.current == PlayerOption.NONE)
        {
            ChooseSpace(1, 1);
            return;
        }

        // take corner if center taken and corner available
        else if (center.current != PlayerOption.NONE && openCorner > -1)
        {
            int x = cornersIndex[openCorner].Item1;
            int y = cornersIndex[openCorner].Item2;
            ChooseSpace(x, y);
            return;
        }

        // if nothing else, choose empty space available
        ChooseEmpty();


    }

    // check if winning move available
    public bool WinAvailable(PlayerOption currentPlayer)
    {
        // determine current player to sum values
        int currentPlayerMarks = currentPlayer == PlayerOption.X ? 1 : -1;

        // check rows
        for (int i = 0; i < Rows; i++)
        {
            int sum = 0;
            int openCell = -1;
            for (int j = 0; j < Columns; j++)
            {

                if (cells[j, i].current == PlayerOption.X)
                {
                    sum += 1;

                }
                else if (cells[j, i].current == PlayerOption.O)
                {
                    sum += -1;

                }
                // store open cell
                if (cells[j, i].current == PlayerOption.NONE)
                {
                    openCell = j;

                }

            }

            if (sum == 2 * currentPlayerMarks && openCell != -1)
            {
                ChooseSpace(openCell, i);
                return true;
            }

        }

        // check columns
        for (int j = 0; j < Columns; j++)
        {
            int sum = 0;
            int openCell = -1;
            for (int i = 0; i < Rows; i++)
            {
                if (cells[j, i].current == PlayerOption.X)
                {
                    sum += 1;

                }
                else if (cells[j, i].current == PlayerOption.O)
                {
                    sum += -1;

                }
                // store open cell
                if (cells[j, i].current == PlayerOption.NONE)
                {
                    openCell = i;

                }
            }

            if (sum == 2 * currentPlayerMarks && openCell != -1)
            {
                ChooseSpace(j, openCell);
                return true;
            }

        }

        // check diagonals
        // top left to bottom right
        int d1Sum = 0;
        int d1Open = -1;
        for (int i = 0; i < Rows; i++)
        {

            if (cells[i, i].current == PlayerOption.X)
                d1Sum += 1;
            else if (cells[i, i].current == PlayerOption.O)
                d1Sum += -1;

            // store open cell
            if (cells[i, i].current == PlayerOption.NONE)
            {
                d1Open = i;

            }

        }
        if (d1Sum == 2 * currentPlayerMarks && d1Open != -1)
        {
            ChooseSpace(d1Open, d1Open);
            return true;
        }

        // top right to bottom left
        int d2Sum = 0;
        int d2Open = -1;
        for (int i = 0; i < Rows; i++)
        {

            if (cells[Columns - 1 - i, i].current == PlayerOption.X)
                d2Sum += 1;
            else if (cells[Columns - 1 - i, i].current == PlayerOption.O)
                d2Sum += -1;

            // store open cell
            if (cells[Columns - 1 - i, i].current == PlayerOption.NONE)
            {
                d2Open = i;
            }

        }

        if (d2Sum == 2 * currentPlayerMarks && d2Open != -1)
        {
            ChooseSpace(Columns - 1 - d2Open, d2Open);
            return true;
        }

        return false;
    }

    // take empty cell if available
    public void ChooseEmpty()
    {
        for (int i = 0; i < Rows; i++)
        {
            for (int j = 0; j < Columns; j++)
            {
                if (cells[j, i].current == PlayerOption.NONE)
                {
                    ChooseSpace(j, i);
                    return;
                }
            }
        }
    }

    // check if board is full
    public bool BoardIsFull()
    {
        for (int i = 0; i < Rows; i++)
        {
            for (int j = 0; j < Columns; j++)
            {
                if (cells[j, i].current == PlayerOption.NONE)
                {
                    return false;
                }
            }
        }
        return true;
    }

    public void ChooseSpace(int column, int row)
    {
        // can't choose space if game is over
        if (GetWinner() != PlayerOption.NONE)
            return;

        // can't choose a space that's already taken
        if (cells[column, row].current != PlayerOption.NONE)
            return;

        // set the cell to the player's mark
        cells[column, row].current = currentPlayer;

        // update the visual to display X or O
        board.UpdateCellVisual(column, row, currentPlayer);

        // if there's no winner, keep playing, otherwise end the game
        if (GetWinner() == PlayerOption.NONE)
            EndTurn();

        // reset game on game over
        if (GetWinner() != PlayerOption.NONE || BoardIsFull())
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            Debug.Log("GAME OVER!");
        }
    }

    public void EndTurn()
    {
        // increment player, if it goes over player 2, loop back to player 1
        currentPlayer += 1;
        if ((int)currentPlayer > 2)
            currentPlayer = PlayerOption.X;
    }

    public PlayerOption GetWinner()
    {
        // sum each row/column based on what's in each cell X = 1, O = -1, blank = 0
        // we have a winner if the sum = 3 (X) or -3 (O)
        int sum = 0;

        // check rows
        for (int i = 0; i < Rows; i++)
        {
            sum = 0;
            for (int j = 0; j < Columns; j++)
            {
                var value = 0;
                if (cells[j, i].current == PlayerOption.X)
                    value = 1;
                else if (cells[j, i].current == PlayerOption.O)
                    value = -1;

                sum += value;
            }

            if (sum == 3)
                return PlayerOption.X;
            else if (sum == -3)
                return PlayerOption.O;

        }

        // check columns
        for (int j = 0; j < Columns; j++)
        {
            sum = 0;
            for (int i = 0; i < Rows; i++)
            {
                var value = 0;
                if (cells[j, i].current == PlayerOption.X)
                    value = 1;
                else if (cells[j, i].current == PlayerOption.O)
                    value = -1;

                sum += value;
            }

            if (sum == 3)
                return PlayerOption.X;
            else if (sum == -3)
                return PlayerOption.O;

        }

        // check diagonals
        // top left to bottom right
        sum = 0;
        for (int i = 0; i < Rows; i++)
        {
            int value = 0;
            if (cells[i, i].current == PlayerOption.X)
                value = 1;
            else if (cells[i, i].current == PlayerOption.O)
                value = -1;

            sum += value;
        }

        if (sum == 3)
            return PlayerOption.X;
        else if (sum == -3)
            return PlayerOption.O;

        // top right to bottom left
        sum = 0;
        for (int i = 0; i < Rows; i++)
        {
            int value = 0;

            if (cells[Columns - 1 - i, i].current == PlayerOption.X)
                value = 1;
            else if (cells[Columns - 1 - i, i].current == PlayerOption.O)
                value = -1;

            sum += value;
        }

        if (sum == 3)
            return PlayerOption.X;
        else if (sum == -3)
            return PlayerOption.O;

        return PlayerOption.NONE;
    }
}
