namespace RicartAgrawalaAlgorithm
{
    public class Presenter
    {
        private readonly IView _view;


        public Presenter(IView view)
        {
            _view = view;
        }
    }
}