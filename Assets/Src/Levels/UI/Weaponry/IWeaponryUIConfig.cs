using Levels.Core;

namespace Levels.UI.Weaponry
{
    public interface IWeaponryUIConfig
    {
        public IWeaponUIConfigItem this[IAbility weapon] { get; }
    }
}