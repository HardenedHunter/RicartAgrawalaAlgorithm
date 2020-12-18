using System;
using System.Collections.Generic;
using System.Threading;

namespace RicartAgrawalaAlgorithm
{
    public class Worker
    {
        public uint Clock { get; set; }
        public uint ThreadId { get; set; }
        public uint ParentThreadId { get; }
        public uint TotalThreads { get; }
        private Thread _thread;

        private readonly List<uint> _otherThreadIds = new List<uint>();
        private readonly uint _personalNumber;


        public Worker(uint parentThreadId, uint totalThreads, uint personalNumber)
        {
            ParentThreadId = parentThreadId;
            TotalThreads = totalThreads;
            _personalNumber = personalNumber;
        }

        public void Start()
        {
            _thread = new Thread(Run);
            _thread.Start();
        }

        private void Initialize()
        {
            ThreadId = ThreadApi.GetCurrentThreadId();
            ThreadApi.PostThreadMessage(new ApiMessage(MessageType.Request, ThreadId, ParentThreadId,
                CriticalSection.Main));

            var recievedMessages = 0;
            while (recievedMessages != TotalThreads - 1)
            {
                var messageExists = ThreadApi.PeekMessage(out var message);
                if (messageExists)
                {
                    _otherThreadIds.Add(message.FromId);
                    recievedMessages++;
                }
            }

            Thread.Sleep(1000);
        }

        private void Run()
        {
            Initialize();

            var timesToEnter = 3;
            var entered = 0;
            var timesToApprove = (TotalThreads - 1) * timesToEnter;
            var approved = 0;

            while (approved != timesToApprove || entered != timesToEnter)
            {
                approved += Listen();   

                Thread.Sleep(2000);
                if (entered != timesToEnter)
                {
                    RequestPermission();
                    var sentAt = ThreadApi.GetTickCount();
                    Logger.Log($"Поток {ThreadId} хочет войти в критическую секцию");
                    var confirmed = 0;
                    var neededConfirmations = TotalThreads - 1;

                    while (confirmed != neededConfirmations)
                    {
                        var messageExists = ThreadApi.PeekMessage(out var message);
                        if (messageExists)
                        {
                            switch (message.Type)
                            {
                                case MessageType.Response:
                                    confirmed++;
                                    break;
                                case MessageType.Request:
                                    if (message.Time < sentAt || message.Time == sentAt && ThreadId > message.FromId)
                                    {
                                        ThreadApi.PostThreadMessage(new ApiMessage(MessageType.Response, ThreadId,
                                            message.FromId, CriticalSection.Main));
                                        Logger.Log($"Заявка от {message.FromId} одобрена {ThreadId}");
                                        approved++;
                                    }
                                    else
                                    {
                                        ThreadApi.PostThreadMessage(new ApiMessage(MessageType.Request, message.FromId,
                                            ThreadId, message.Section));
                                    }

                                    break;
                            }
                        }
                        else
                        {
                            if (ThreadApi.GetTickCount() > sentAt + 10000)
                            {
                                Logger.Log($"Время ожидания превышено, поток {ThreadId} входит в критическую секцию");
                                break;
                            }
                        }
                    }

                    Logger.Log($"Поток {ThreadId} вошёл в критическую секцию");
                    Thread.Sleep(3000);
                    Logger.Log($"Поток {ThreadId} вышел из критической секции.");
                    entered++;

                    approved += InviteOthers();
                }
            }

            Logger.Log($"Поток {ThreadId} завершил работу");
        }

        public int Listen()
        {
            var approved = 0;
            Clock = ThreadApi.GetTickCount();
            var resumeAt = Clock + 50 * StaticRandom.Rand(10, 100);

            while (ThreadApi.GetTickCount() < resumeAt)
            {
                var messageExists = ThreadApi.PeekMessage(out var message);
                if (messageExists && message.Type == MessageType.Request)
                {
                    ThreadApi.PostThreadMessage(new ApiMessage(MessageType.Response, ThreadId, message.FromId,
                        CriticalSection.Main));
                    Logger.Log(
                        $"Заявка от {message.FromId} одобрена {ThreadId}");
                    approved++;
                }
            }
    
            return approved;
        }

        private void RequestPermission()
        {
            foreach (var id in _otherThreadIds)
                ThreadApi.PostThreadMessage(new ApiMessage(MessageType.Request, ThreadId, id, CriticalSection.Main));
        }

        private int InviteOthers()
        {
            var approved = 0;
            var messageExists = ThreadApi.PeekMessage(out var request);
            while (messageExists)
            {
                Logger.Log(
                    $"Отложенная заявка от {request.FromId} одобрена {ThreadId}");
                approved++;
                if (request.Section == CriticalSection.Main && request.Type == MessageType.Request)
                    ThreadApi.PostThreadMessage(new ApiMessage(MessageType.Response, ThreadId, request.FromId,
                        CriticalSection.Main));
                messageExists = ThreadApi.PeekMessage(out request);
            }

            return approved;
        }

        ~Worker()
        {
            if (_thread.IsAlive)
                _thread.Abort();
        }
    }
}