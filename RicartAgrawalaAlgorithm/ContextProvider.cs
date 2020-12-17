using System;
using System.Threading;

namespace RicartAgrawalaAlgorithm
{
    public class ContextProvider
    {
        private static ContextProvider _instance;

        private SynchronizationContext _context;

        public SynchronizationContext Context
        {
            get => _context;
            set => _context = value ?? throw new ArgumentNullException(nameof(value));
        }

        public static void Send(SendOrPostCallback d, object state)
        {
            lock (_instance)
            {
                GetInstance()._context.Send(d, state);
            }
        }

        private static readonly object Lock = new object();

        public static ContextProvider GetInstance()
        {
            if (_instance == null)
                lock (Lock)
                    if (_instance == null)
                        _instance = new ContextProvider();
            return _instance;
        }
    }
}