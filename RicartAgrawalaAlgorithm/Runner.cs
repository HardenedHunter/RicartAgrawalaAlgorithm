using System;
using System.Collections.Generic;

namespace RicartAgrawalaAlgorithm
{
    public class Runner
    {
        private const uint TotalThreads = 3;
        private uint _mainThreadId;
        private readonly List<uint> _otherThreadIds = new List<uint>();

        private void CreateThreads()
        {
            Logger.Log("Создание потоков...");
            for (uint i = 1; i <= TotalThreads; i++)
            {
                var w = new Worker(_mainThreadId, TotalThreads, i);
                w.Start();
            }

            Logger.Log("Успешно");
        }

        private void GetThreadIds()
        {
            Logger.Log("Получение ID всех потоков...");
            int recievedMessages = 0;
            while (recievedMessages != TotalThreads)
            {
                var messageExists = ThreadApi.PeekMessage(out var message);
                if (messageExists)
                {
                    _otherThreadIds.Add(message.FromId);
                    recievedMessages++;
                }
            }

            Logger.Log("Успешно");
        }

        private void DistributeThreadIds()
        {
            Logger.Log("Распределение ID...");
            for (var i = 0; i < _otherThreadIds.Count; i++)
            {
                var targetThreadId = _otherThreadIds[i];
                for (var j = 0; j < _otherThreadIds.Count; j++)
                {
                    var otherId = _otherThreadIds[j];
                    if (i == j) continue;
                    var message = new ApiMessage(MessageType.Request, otherId, targetThreadId, CriticalSection.Main);
                    ThreadApi.PostThreadMessage(message);
                }
            }

            Logger.Log("Успешно");
        }

        public void Run()
        {
            _mainThreadId = ThreadApi.GetCurrentThreadId();
            CreateThreads();
            GetThreadIds();
            DistributeThreadIds();
        }
    }
}