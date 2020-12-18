using System;
using System.Threading;

namespace RicartAgrawalaAlgorithm
{
    public static class Logger
    {
        public static event Action<string> MessageLogged;

        public static void Log(object message)
        {
            ContextProvider.Send(obj => MessageLogged?.Invoke(message.ToString()), null);
            Console.WriteLine($"[WinApi] {message}");
        }
    }

    public class Presenter
    {
        private readonly IView _view;

        public Presenter(IView view)
        {
            _view = view;
            _view.ProgramStarted += Start;
            Logger.MessageLogged += _view.AddMessage;
            ContextProvider.GetInstance().Context = SynchronizationContext.Current;
        }

        private void Start()
        {
            new Runner().Run();
        }
    }
}