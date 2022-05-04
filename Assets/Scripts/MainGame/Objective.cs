using MainGame;
using UnityEngine;
using UnityEngine.Serialization;

namespace ScenesFolders.MainGame
{
    [System.Serializable]
    public class Objective
    {
        public string name;
        public Sprite sprite;
        public int Points { get; private set; }
        public ScoringCriterion Criterion => Criteria.Dict[name];

        public void UpdatePoints(GameManager gm)
        {
            Points = 0;
            for (var x = 0; x < gm.GameBoard.GetLength(0); x++)
            for (var y = 0; y < gm.GameBoard.GetLength(1); y++)
                Points += Criterion(x, y, gm);
        }
    }
}