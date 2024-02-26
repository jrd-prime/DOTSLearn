using System;

namespace Sources.Scripts.JTimer
{
    public readonly struct TimeWrapper
    {
        private readonly long _ticks;
        public DateTime Value => new(_ticks);

        public TimeWrapper(DateTime dateTime) => _ticks = dateTime.Ticks;
    }
}