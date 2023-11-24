using CommandForge.Enums;
using NetMQ;
using NetMQ.Sockets;
using System;
using System.Collections.Concurrent;
using System.Text;
using System.Threading;

namespace CommandForge.Models
{
    public class ZmqCommunications
    {
        #region Member Variables
        private SubscriberSocket _subscriber;
        private PublisherSocket _publisher;
        private ConcurrentQueue<ZmqMessage> _sendQueue;

        private ZmqStatus _subscriberStatus;
        private ZmqStatus _publisherStatus;
        #endregion

        #region Constructor
        public ZmqCommunications()
        {
        }
        #endregion

        #region Properties
        public ZmqStatus SubscriberStatus
        {
            get => _subscriberStatus;
            set { _subscriberStatus = value; OnZmqSubscriberStatusChangeEvent?.Invoke(value); }
        }

        public ZmqStatus PublisherStatus
        {
            get => _publisherStatus;
            set { _publisherStatus = value; OnZmqPublisherStatusChangeEvent?.Invoke(value); }
        }
        #endregion

        #region Methods
        /// <summary>
        /// Initialise various monitoring threads.
        /// </summary>
        public void Init()
        {
            _subscriber = new SubscriberSocket();
            _publisher = new PublisherSocket();
            _sendQueue = new ConcurrentQueue<ZmqMessage>();

            Thread messageSendThread = new(SendThread)
            {
                IsBackground = true
            };
            messageSendThread.Start();

            Thread messageReceiveThread = new(ReceiveThread)
            {
                IsBackground = true
            };
            messageReceiveThread.Start();
        }

        /// <summary>
        /// Connect / disconnect / bind / unbind subscriber.
        /// </summary>
        /// <param name="configure"></param>
        /// <param name="ipv4"></param>
        /// <param name="port"></param>
        /// <returns></returns>
        public bool ConfigureSubscriber(ZmqConfiguration configure, string ipv4, string port)
        {
            bool isSuccess = true;
            string configurationString = "tcp://" + ipv4 + ":" + port;

            switch (configure)
            {
                case ZmqConfiguration.connect:
                    try
                    {
                        _subscriber.Connect(configurationString);
                    }
                    catch
                    {
                        isSuccess = false;
                    }
                    break;

                case ZmqConfiguration.disconnect:
                    try
                    {
                        _subscriber.Disconnect(configurationString);
                    }
                    catch
                    {
                        isSuccess = false;
                    }
                    break;

                case ZmqConfiguration.bind:
                    try
                    {
                        _subscriber.Bind(configurationString);
                    }
                    catch
                    {
                        isSuccess = false;
                    }
                    break;

                case ZmqConfiguration.unbind:
                    try
                    {
                        _subscriber.Unbind(configurationString);
                    }
                    catch
                    {
                        isSuccess = false;
                    }
                    break;

                default:
                    break;
            }

            return isSuccess;
        }

        /// <summary>
        /// Connect / disconnect / bind / unbind publisher.
        /// </summary>
        /// <param name="configure"></param>
        /// <param name="ipv4"></param>
        /// <param name="port"></param>
        /// <returns></returns>
        public bool ConfigurePublisher(ZmqConfiguration configure, string ipv4, string port)
        {
            bool isSuccess = true;
            string configurationString = "tcp://" + ipv4 + ":" + port;

            switch (configure)
            {
                case ZmqConfiguration.connect:
                    try
                    {
                        _publisher.Connect(configurationString);
                    }
                    catch
                    {
                        isSuccess = false;
                    }
                    break;

                case ZmqConfiguration.disconnect:
                    try
                    {
                        _publisher.Disconnect(configurationString);
                    }
                    catch
                    {
                        isSuccess = false;
                    }
                    break;

                case ZmqConfiguration.bind:
                    try
                    {
                        _publisher.Bind(configurationString);
                    }
                    catch
                    {
                        isSuccess = false;
                    }
                    break;

                case ZmqConfiguration.unbind:
                    try
                    {
                        _publisher.Unbind(configurationString);
                    }
                    catch
                    {
                        isSuccess = false;
                    }
                    break;

                default:
                    break;
            }

            return isSuccess;
        }

        /// <summary>
        /// Subscribe / unsubscribe from topics.
        /// </summary>
        /// <param name="topic"></param>
        /// <param name="status"></param>
        public void SetSubscribeStatus(string topic, bool status)
        {
            if (status)
            {
                _subscriber.Subscribe(topic);
            }
            else
            {
                _subscriber?.Unsubscribe(topic);
            }
        }

        /// <summary>
        /// Publish message.
        /// </summary>
        /// <param name="topic"></param>
        /// <param name="message"></param>
        public void SendMessage(string topic, string message)
        {
            byte[] byteMessage = Encoding.Default.GetBytes(message);
            ZmqMessage zmqMessage = new(topic, byteMessage);
            _sendQueue.Enqueue(zmqMessage);
        }

        /// <summary>
        /// Zmq message sending thread.
        /// </summary>
        private void SendThread()
        {
            while (!App.IsQuit)
            {
                _sendQueue.TryDequeue(out ZmqMessage message);

                if (message != null)
                {
                    _publisher.SendMoreFrame(message.Topic).SendMoreFrame(message.Message);
                }

                Thread.Sleep(1);
            }
        }

        /// <summary>
        /// Zmq message receive thread.
        /// </summary>
        private void ReceiveThread()
        {
            byte[] message;

            while (!App.IsQuit)
            {
                _subscriber.TryReceiveFrameString(out string topic, out bool more);

                if (more && topic != null)
                {
                    message = _subscriber.ReceiveFrameBytes();
                }

                Thread.Sleep(1);
            }
        }
        #endregion

        #region Events
        public event Action<ZmqStatus> OnZmqSubscriberStatusChangeEvent;
        public event Action<ZmqStatus> OnZmqPublisherStatusChangeEvent;
        #endregion
    }
}