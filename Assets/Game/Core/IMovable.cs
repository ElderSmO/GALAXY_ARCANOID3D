
using UnityEngine;

namespace Assets.Game.Core
{
    /// <summary>
    /// Represents Move
    /// </summary>
    public interface IMovable
    {
        void Move(Vector2 direction);
        float MoveSpeed { get; set; }
    }
}
