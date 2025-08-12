namespace Tic_Tac_Toe.Components;

public class Board
{
    private string[,] _field;
    private int[,] _fieldValues;
    public readonly int _size;
    
    public Board(int size)
    {
        _size = size;
        _field = new string[size, size];
        _fieldValues = new int[size, size];
    }

    public string? this[int r, int c]
    {
        get => _field[r, c];
        set
        {
            if (value != "X" && value != "O" && value != null)
                throw new ArgumentException("Value must be 'X', 'O' or null.");
            _field[r, c] = value;
            _fieldValues[r, c] = value switch
            {
                "X" => 1,
                "O" => -1,
                _   => 0
            };
        }
    }

    public void CheckWinner(out string? symbol)
    {
        // Никто не победил
        symbol = null;
        
        // Проверка по строкам
        for (int r = 0; r < _size; r++)
        {
            int sum = 0;
            for (int c = 0; c < _size; c++)
                sum += _fieldValues[r, c];
            if (sum == _size)  symbol = "X";   // X победил
            if (sum == -_size) symbol = "O";  // O победил
        }

        // Проверка по колонкам
        for (int c = 0; c < _size; c++)
        {
            int sum = 0;
            for (int r = 0; r < _size; r++)
                sum += _fieldValues[r, c];
            if (sum == _size)  symbol = "X";
            if (sum == -_size) symbol = "O";
        }

        // Главная диагональ
        {
            int sum = 0;
            for (int i = 0; i < _size; i++)
                sum += _fieldValues[i, i];
            if (sum == _size)  symbol = "X";
            if (sum == -_size) symbol = "O";
        }

        // Побочная диагональ
        {
            int sum = 0;
            for (int i = 0; i < _size; i++)
                sum += _fieldValues[i, _size - i - 1];
            if (sum == _size)  symbol = "X";
            if (sum == -_size) symbol = "O";
        }
    }

    public bool CheckDraw()
    {
        if (_field.Cast<string?>().All(c => c != null)) return true;
        
        return false;
    }

    public IEnumerable<string?> GetField()
    {
        return _field.Cast<string?>();
    }
}