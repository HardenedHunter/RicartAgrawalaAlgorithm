using System;

namespace RicartAgrawalaAlgorithm
{
    public interface IView
    {
        event Action ProgramStarted;
        void AddMessage(string message);
    }
}