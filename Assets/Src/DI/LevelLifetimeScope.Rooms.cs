using System.Collections.Generic;
using Levels.Core.Room;
using VContainer;

namespace DI
{
    public partial class LevelLifetimeScope
    {
        private readonly Dictionary<int, Room> _rooms = new();

        private void ConfigureRooms(IContainerBuilder builder)
        {
            builder.RegisterFactory<int, Room>
            (
                resolver => roomId =>
                {
                    if (_rooms.TryGetValue(roomId, out var room))
                    {
                        return room;
                    }

                    room = new Room(new RoomHealing(), new RoomUnits());
                    _rooms.Add(roomId, room);

                    var roomScope = resolver.CreateScope(roomBuilder =>
                    {
                        roomBuilder.RegisterInstance(room.Healing);
                        roomBuilder.RegisterInstance(room.Units);
                    });

                    roomScope.Inject(room.Units);
                    roomScope.Inject(room.Healing);

                    room.Healing.Init();

                    return room;
                },
                Lifetime.Singleton
            );
        }

        private void ClearRooms()
        {
            
            foreach (var (healing, units) in _rooms.Values)
            {
                healing.Clear();
                units.Clear();
            }
        }
    }
}