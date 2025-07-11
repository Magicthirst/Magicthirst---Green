using System;

namespace Web
{
    [Flags]
    public enum ConnectionRole
    {
        Offline = 0b00,
        Guest = 0b01,
        Host = 0b11,
    }
}
