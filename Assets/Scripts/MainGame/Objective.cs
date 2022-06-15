using UnityEngine;

namespace MainGame
{
    [System.Serializable]
    public class Objective
    {
        public string name;
        public string text;
        public int Points { get; private set; }
        public ScoringCriterion Criterion => Criteria.dict[name];

        public void UpdatePoints(GameManager gm)
        {
            Points = 0;
            for (var x = 0; x < gm.GameBoard.GetLength(0); x++)
            for (var y = 0; y < gm.GameBoard.GetLength(1); y++)
                Points += Criterion(x, y, gm);
        }
    }
}