﻿namespace OkayegTeaTimeCSharp.Time
{
    public static class Minute
    {
        private const long InMilliseconds = 60000;

        public static long ToMilliseconds(uint minutes = 1)
        {
            return InMilliseconds * minutes;
        }
    }
}
