using CommunityToolkit.Mvvm.ComponentModel;
using System;

namespace CommandForge.ViewModels.CollectionObjects
{
    public partial class SubscriberTopicObject : ObservableObject
    {
        #region Member Variables
        private readonly string _topicType;
        Action<string, bool> _toggleFunction;
        #endregion

        #region Constructor
        public SubscriberTopicObject(string topic, Action<string, bool> toggleFunction)
        {
            _topicType = topic;
            TopicName = topic.ToString();
            IsSubscribed = false;
            _toggleFunction = toggleFunction;
        }
        #endregion

        #region Properties
        [ObservableProperty]
        private string _topicName;

        [ObservableProperty]
        private bool _isSubscribed;
        #endregion

        #region Methods
        /// <summary>
        /// Subscribe / unsubscribe from topics.
        /// </summary>
        /// <param name="value"></param>
        partial void OnIsSubscribedChanged(bool value)
        {
            _toggleFunction(_topicType, value);
        }
        #endregion
    }
}