using System;
using UnityEngine;

namespace MainGame
{
    public class ScoreObject : MonoBehaviour
    {
        public Objective Objective { get; private set; }
        public event EventHandler CursorEntered;
        public event EventHandler CursorExited;

        public void Init(Objective objective)
            => Objective = objective;

        public void OnCursorEnter()
            => CursorEntered?.Invoke(this, EventArgs.Empty);

        public void OnCursorExit()
            => CursorExited?.Invoke(this, EventArgs.Empty);
    }
}
