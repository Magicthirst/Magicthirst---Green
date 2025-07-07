using System;
using System.Diagnostics.CodeAnalysis;

namespace Web.Sync
{
    /// <summary>
    /// Defines the purpose of a message using bit flags, allowing a single integer
    /// to describe the message's role, type, and subtype
    /// </summary>
    [Flags]
    [SuppressMessage("ReSharper", "ShiftExpressionZeroLeftOperand")]
    public enum MessageMark : ushort
    {
        // ROLES
        FilterRole = 1 << 0,
        /// <summary>A message originating from the server</summary>
        SourceOfTruth = 1 << 0,
        /// <summary>A message related to the Source of Truth player</summary>
        Server = SourceOfTruth,
    
        // TYPES
        FilterType = 0b11 << 1,
        /// <summary>A message containing a full or partial state update</summary>
        Update = 0b00 << 1,
        /// <summary>A message containing a player-initiated action or command</summary>
        Command = 0b01 << 1,
        /// <summary>Session management message, primarily used by <see cref="SessionsRouter"/></summary>
        Session = 0b10 << 1,
        /// <summary>Error message</summary>
        Error = 0b11 << 1,
    
        // SUBTYPES
        FilterSubType = ushort.MaxValue ^ FilterRole,
        // SESSION SUBTYPES
        /// <summary>A meta message from client to server of request to join a session</summary>
        Connected = Session | 0 << 3,
        /// <summary>A meta message from server to client that theirs request is accepted</summary>
        Accepted = Session | 1 << 3,
    
        // ERROR SUBTYPES
        // - Used HTTP error codes, see https://en.wikipedia.org/wiki/List_of_HTTP_status_codes
        Error400 = Error | 0 << 3,
        Error403 = Error | 1 << 3,
        Error404 = Error | 2 << 3,

        // UPDATE SUBTYPES
        PlayerUpdate = Update | 0 << 3
    }
    
    public static class MessageMarkSupply
    {
        public static string DescriptionOf(ushort mark) => DescriptionOf((MessageMark) mark);
    
        public static string DescriptionOf(MessageMark mark)
        {
    #pragma warning disable CS8509 // The switch expression does not handle all possible values of its input type (it is not exhaustive).
            var role = (mark & MessageMark.FilterRole) switch
            {
                 MessageMark.SourceOfTruth => "SOT",
                 0 => "-0-"
            };
            var (type, subType) = (mark & MessageMark.FilterType) switch
            {
                MessageMark.Update => ("UPD", "-0-"),
                MessageMark.Command => ("COM", "-0-"),
                MessageMark.Session => ("SES", (mark & MessageMark.FilterSubType) switch
                {
                    MessageMark.Connected => "CON",
                    MessageMark.Accepted => "ACC",
                    _ => "-0-"
                }),
                MessageMark.Error => ("ERR", (mark & MessageMark.FilterSubType) switch
                {
                    MessageMark.Error400 => "400",
                    MessageMark.Error403 => "403",
                    MessageMark.Error404 => "404",
                    _ => "-0-"
                })
            };
            return ($"{role},{type},{subType}:{(ushort) mark:b16}");
    #pragma warning restore CS8509 // The switch expression does not handle all possible values of its input type (it is not exhaustive).
        }
    }
}
