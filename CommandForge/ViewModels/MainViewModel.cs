namespace CommandForge.ViewModels
{
    public class MainViewModel
    {
        #region Constructor
        public MainViewModel(ZmqCommunicationsViewModel zmqCommunicationsViewModel,
                             ZmqSubscriberTopicListViewModel zmqSubscriberTopicListViewModel)
        {
            ZmqCommunicationsViewModel = zmqCommunicationsViewModel;
            ZmqSubscriberTopicListViewModel = zmqSubscriberTopicListViewModel;
        }
        #endregion

        #region Child ViewModels
        public ZmqCommunicationsViewModel ZmqCommunicationsViewModel
        {
            get;
            private set;
        }
        public ZmqSubscriberTopicListViewModel ZmqSubscriberTopicListViewModel
        {
            get;
            private set;
        }
        #endregion
    }
}