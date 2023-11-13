using CommandForge.Enums;
using CommandForge.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Threading.Tasks;

namespace CommandForge.ViewModels
{
    public partial class ZmqCommunicationsViewModel : ObservableObject
    {
        #region Member Variables
        private readonly ConfigFile _configFile;
        private readonly ZmqCommunications _zmqCommunications;
        #endregion

        #region Constructor
        public ZmqCommunicationsViewModel(ConfigManager configManager,
                                          ZmqCommunications zmqCommunications)
        {
            _configFile = configManager.Config;
            _zmqCommunications = zmqCommunications;
            _zmqCommunications.Init();

            SubscriberIpv4 = _configFile.Defaults.ZmqSubscriberIPv4;
            SubscriberPort = _configFile.Defaults.ZmqSubscriberPort.ToString();
            SubscriberStatus = ZmqStatus.OFF;

            PublisherIpv4 = _configFile.Defaults.ZmqPublisherIPv4;
            PublisherPort = _configFile.Defaults.ZmqPublisherPort.ToString();
            PublisherStatus = ZmqStatus.OFF;
        }
        #endregion

        #region Properties
        [ObservableProperty]
        private string _subscriberIpv4;

        [ObservableProperty]
        private string _subscriberPort;

        [ObservableProperty]
        private ZmqStatus _subscriberStatus;

        [ObservableProperty]
        private string _publisherIpv4;

        [ObservableProperty]
        private string _publisherPort;

        [ObservableProperty]
        private ZmqStatus _publisherStatus;
        #endregion

        #region Commands / Command Definitions
        [RelayCommand]
        private async Task SubscriberConfiguration(string param)
        {
            if (param == null)
            {
                return;
            }

            ZmqConfiguration configure = (ZmqConfiguration)Enum.Parse(typeof(ZmqConfiguration), param);

            await Task.Run(() => ConfigureSubscriber(configure));
        }

        [RelayCommand]
        private async Task PublisherConfiguration(string param)
        {
            if (param == null)
            {
                return;
            }

            ZmqConfiguration configure = (ZmqConfiguration)Enum.Parse(typeof(ZmqConfiguration), param);

            await Task.Run(() => ConfigurePublisher(configure));
        }
        #endregion

        #region Methods
        /// <summary>
        /// Connect / disconnect / bind / unbind zmq subscriber.
        /// </summary>
        /// <param name="configure"></param>
        private void ConfigureSubscriber(ZmqConfiguration configure)
        {
            bool isSuccess = _zmqCommunications.ConfigureSubscriber(configure, SubscriberIpv4, SubscriberPort);

            switch (configure)
            {
                case ZmqConfiguration.connect:
                    SubscriberStatus = isSuccess ? ZmqStatus.CONNECTED : ZmqStatus.ERROR;
                    break;

                case ZmqConfiguration.disconnect:
                    SubscriberStatus = isSuccess ? ZmqStatus.OFF : ZmqStatus.ERROR;
                    break;

                case ZmqConfiguration.bind:
                    SubscriberStatus = isSuccess ? ZmqStatus.BOUND : ZmqStatus.ERROR;
                    break;

                case ZmqConfiguration.unbind:
                    SubscriberStatus = isSuccess ? ZmqStatus.OFF : ZmqStatus.ERROR;
                    break;

                default:
                    break;
            }
        }

        /// <summary>
        /// Connect / disconnect / bind / unbind zmq publisher.
        /// </summary>
        /// <param name="configure"></param>
        private void ConfigurePublisher(ZmqConfiguration configure)
        {
            bool isSuccess = _zmqCommunications.ConfigurePublisher(configure, PublisherIpv4, PublisherPort);

            switch (configure)
            {
                case ZmqConfiguration.connect:
                    PublisherStatus = isSuccess ? ZmqStatus.CONNECTED : ZmqStatus.ERROR;
                    break;

                case ZmqConfiguration.disconnect:
                    PublisherStatus = isSuccess ? ZmqStatus.OFF : ZmqStatus.ERROR;
                    break;

                case ZmqConfiguration.bind:
                    PublisherStatus = isSuccess ? ZmqStatus.BOUND : ZmqStatus.ERROR;
                    break;

                case ZmqConfiguration.unbind:
                    PublisherStatus = isSuccess ? ZmqStatus.OFF : ZmqStatus.ERROR;
                    break;

                default:
                    break;
            }
        }
        #endregion
    }
}