

namespace Assets.Game.Core
{
    /// <summary>
    /// All possible game states
    /// </summary>
    public enum GameState
    {
        Menu = 0,
        Playing = 1,
        Paused = 2,
        GameOver = 3,
        Victory = 4
    }

    /// <summary>
    /// Types of bricks with different properties
    /// </summary>
    public enum BrickType
    {
        Standard = 0,
        Strong = 1,
        Indestructible = 2
    }
}

