using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Game.Core
{
    /// <summary>
    /// Represents an object that can take damage
    /// </summary>
    public interface IDamageable
    {
        void TakeDamage(int damage);
        bool IsDestroyed { get; }
    }
}
