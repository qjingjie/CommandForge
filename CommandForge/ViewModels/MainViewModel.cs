namespace CommandForge.ViewModels
{
    public class MainViewModel
    {
        #region Constructor
        public MainViewModel(ZmqCommunicationsViewModel zmqCommunicationsViewModel)
        {
            ZmqCommunicationsViewModel = zmqCommunicationsViewModel;
        }
        #endregion

        #region Child ViewModels
        public ZmqCommunicationsViewModel ZmqCommunicationsViewModel
        {
            get;
            private set;
        }
        #endregion
    }
}