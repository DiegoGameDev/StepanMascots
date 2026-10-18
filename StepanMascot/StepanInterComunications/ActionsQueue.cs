using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;

namespace StepanMascot.StepanInterComunications
{
    internal static class ActionsQueue
    {
        public static ConcurrentQueue<Action> shareActions = new();
    }
}
