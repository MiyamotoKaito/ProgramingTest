public class TicTacToeCell
{
    public TicTacToeCell(int x, int y)
    {
        _position.x = x;
        _position.y = y;
    }
    public bool IsFill;
    public (int x, int y) Position => _position;

    private bool _isFill = false;
    private (int x, int y) _position;
}
