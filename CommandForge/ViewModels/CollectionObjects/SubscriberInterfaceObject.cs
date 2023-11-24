using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace CommandForge.ViewModels.CollectionObjects
{
    public partial class SubscriberInterfaceObject : ObservableObject
    {
        #region Constructor
        public SubscriberInterfaceObject(string interfaceName, List<string> topics, Action<string, bool> toggleFunction)
        {
            InterfaceName = interfaceName;

            Topics = new ObservableCollection<SubscriberTopicObject>();

            foreach (string topic in topics)
            {
                Topics.Add(new SubscriberTopicObject(topic, toggleFunction));
            }
        }
        #endregion

        #region Properties
        [ObservableProperty]
        private string _interfaceName;

        public ObservableCollection<SubscriberTopicObject> Topics
        {
            get;
            private set;
        }
        #endregion

        #region Commands / Command Definitions
        [RelayCommand]
        private async Task SubscribeAllAsync()
        {
            await Task.Run(() => ToggleAllSubscribedTopics(true));
        }

        [RelayCommand]
        private async Task UnsubscribeAllAsync()
        {
            await Task.Run(() => ToggleAllSubscribedTopics(false));
        }
        #endregion

        #region Methods

        /// <summary>
        /// Subscribe / unsubscribe from all topics.
        /// </summary>
        /// <param name="toggle"></param>
        public void ToggleAllSubscribedTopics(bool toggle)
        {
            foreach (SubscriberTopicObject topic in Topics)
            {
                topic.IsSubscribed = toggle;
            }
        }
        #endregion
    }
}