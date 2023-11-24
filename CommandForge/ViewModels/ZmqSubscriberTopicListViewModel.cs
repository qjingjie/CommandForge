using CommandForge.Enums;
using CommandForge.Models;
using CommandForge.ViewModels.CollectionObjects;
using CommunityToolkit.Mvvm.ComponentModel;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;

namespace CommandForge.ViewModels
{
    public partial class ZmqSubscriberTopicListViewModel : ObservableObject
    {
        #region Member Variables
        private readonly ZmqCommunications _zmqCommunications;
        #endregion

        #region Constructor
        public ZmqSubscriberTopicListViewModel(ZmqCommunications zmqCommunications)
        {
            _zmqCommunications = zmqCommunications;

            SubscriberInterfaceList = new ObservableCollection<SubscriberInterfaceObject>();

            ReadTopicsToSubscribe();

            _zmqCommunications.OnZmqSubscriberStatusChangeEvent += CheckToggleEnableState;
        }
        #endregion

        #region Properties
        [ObservableProperty]
        private bool _isToggleButtonsEnabled;

        public ObservableCollection<SubscriberInterfaceObject> SubscriberInterfaceList
        {
            get;
            private set;
        }
        #endregion

        #region Methods
        /// <summary>
        /// Check folder path for topics to subscribe, if folder path does not exist, create it.
        /// </summary>
        private void ReadTopicsToSubscribe()
        {
            string folderPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                                             "CommandForge",
                                             "ZmqSubscribe");

            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }
            else
            {
                string[] files = Directory.GetFiles(folderPath, "*.json");

                if (files.Length > 0)
                {
                    foreach (string file in files)
                    {
                        string jsonString = File.ReadAllText(file);
                        dynamic parsedObject = JsonConvert.DeserializeObject(jsonString);

                        if (parsedObject.InterfaceName != null && parsedObject.Topics != null && parsedObject.Topics.Count > 0)
                        {
                            string name = parsedObject.InterfaceName;
                            JArray topics = parsedObject.Topics;

                            SubscriberInterfaceList.Add(new SubscriberInterfaceObject(name, topics.Select(topic => topic.ToString()).ToList(), _zmqCommunications.SetSubscribeStatus));
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Determine if user is allowed to toggle topic subscribe status.
        /// </summary>
        /// <param name="status"></param>
        private void CheckToggleEnableState(ZmqStatus status)
        {
            switch (status)
            {
                case ZmqStatus.OFF:
                    // Toggle off all subscribed topics first
                    foreach (SubscriberInterfaceObject subscriberInterface in SubscriberInterfaceList)
                    {
                        subscriberInterface.ToggleAllSubscribedTopics(false);
                    }

                    IsToggleButtonsEnabled = false;
                    break;

                case ZmqStatus.BOUND:
                    IsToggleButtonsEnabled = true;
                    break;

                case ZmqStatus.CONNECTED:
                    IsToggleButtonsEnabled = true;
                    break;

                case ZmqStatus.ERROR:
                    // Toggle off all subscribed topics first
                    foreach (SubscriberInterfaceObject subscriberInterface in SubscriberInterfaceList)
                    {
                        subscriberInterface.ToggleAllSubscribedTopics(false);
                    }

                    IsToggleButtonsEnabled = false;
                    break;

                default:
                    break;
            }
        }
        #endregion
    }
}