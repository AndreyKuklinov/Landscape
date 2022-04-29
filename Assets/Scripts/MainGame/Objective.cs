namespace ScenesFolders.MainGame
{
    public class Objective
    {
        public ScoringCriterion Criterion { get; private set; }
        public int Points { get; private set; }

        public Objective(ScoringCriterion criterion)
        {
            Criterion = criterion;
            Points = 0;
        }

        public void UpdatePoints(GameManager gm)
        {
            Points = 0;
            for (var x = 0; x < gm.GameBoard.GetLength(0); x++)
            for (var y = 0; y < gm.GameBoard.GetLength(1); y++)
                Points += Criterion(x, y, gm);
        }
    }
}