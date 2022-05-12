using ScenesFolders.MainGame;

namespace TestProject1;

public class GameBoard
{
    private TwoSidedList<TwoSidedList<Tile>> list = new TwoSidedList<TwoSidedList<Tile>>();
    private int size = 0;

    public void Expand()
    {
        
    }

    public Tile GetTileAt(int x, int y)
    {
        return list[x][y];
    }
}