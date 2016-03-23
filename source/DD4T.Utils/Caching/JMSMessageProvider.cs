using Apache.NMS;
using DD4T.ContentModel.Contracts.Logging;
using DD4T.ContentModel.Contracts.Caching;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using DD4T.ContentModel.Contracts.Configuration;
using log4net.Config;

namespace DD4T.Utils.Caching
{
    
    public class JMSMessageProvider : IMessageProvider<ICacheEvent>, IDisposable
    {     
       
        private ILogger Logger { get; set; }
        private IDD4TConfiguration DD4TConfiguration { get; set; }
        private int NumberOfRetries { get; set; }
        private static TimeSpan ReceiveTimeout = TimeSpan.FromDays(2);
        private List<IObserver<ICacheEvent>> observers;
        private ILogger _backgroundlogger;

        public JMSMessageProvider(ILogger logger, IDD4TConfiguration dd4tConfiguration)
        {
            Logger = logger;
            DD4TConfiguration = dd4tConfiguration;
            observers = new List<IObserver<ICacheEvent>>();
            _backgroundlogger = new FSLogger();
        }

        #region properties
       

        #endregion

        #region IMessageProvider

        public void Start()
        {
            Thread worker = new Thread(DoWork);
            worker.IsBackground = true;
            worker.Start();
        }
        private void DoWork()
        {
            XmlConfigurator.Configure(); // THIS MUST BE REMOVED!!! It creates a dependency on log4net in the core (QS, 4 March 2016)
            try
            {
                StartConnection();
            }
            catch (NMSConnectionException e)
            {
                Logger.Warning("Unable to connect to JMS service. {0}", e.Message);
            }
            catch (NMSException e)
            {
                Logger.Warning("Unable to connect to JMS service. {0}", e.Message);
            }
            finally
            {
                if (NumberOfRetries < DD4TConfiguration.JMSNumberOfRetriesToConnect)
                {
                    NumberOfRetries++;
                    Logger.Debug("Trying to reconnect to JMS server in {0} seconds.... This is attempt {1} of {2}", DD4TConfiguration.JMSSecondsBetweenRetries, NumberOfRetries, DD4TConfiguration.JMSNumberOfRetriesToConnect);
                    Thread.Sleep(DD4TConfiguration.JMSSecondsBetweenRetries * 1000); //Wait a couple of seconds before trying to reconnect
                    DoWork();
                }
                else
                {
                    Logger.Error("Unable to connect to JMS service (tried {0} but no luck). Restart the JMS servic and then restart the web application", NumberOfRetries);
                    Logger.Information("The web application will continue to serve content, but it will NOT invalidate the caches.");
                }
            }



        }
        private void StartConnection()
        {
            Uri connecturi = new Uri(string.Format("activemq:tcp://{0}:{1}", DD4TConfiguration.JMSHostname, DD4TConfiguration.JMSPort));

            // NOTE: ensure the nmsprovider-activemq.config file exists in the executable folder.
            IConnectionFactory factory = new NMSConnectionFactory(connecturi);

            using (IConnection connection = factory.CreateConnection())
            {
                connection.ClientId = "DD4TJMSListener-" + Guid.NewGuid().ToString();
                connection.ExceptionListener += connection_ExceptionListener;
                connection.Start();
                using (ISession session = connection.CreateSession(AcknowledgementMode.ClientAcknowledge))
                {
   
                    //IDestination destination = session.GetDestination(Topic);
                    IDestination destination = session.GetTopic(DD4TConfiguration.JMSTopic);
                    _backgroundlogger.Debug("connected to JMS server using hostname {0}, port {1} and destination {2}", LoggingCategory.Background, DD4TConfiguration.JMSHostname, DD4TConfiguration.JMSPort, destination);
 
                    using (IMessageConsumer consumer = session.CreateConsumer(destination))
                    {
                        IMessage message;

                        try
                        {
                            while ((message = consumer.Receive(ReceiveTimeout)) != null)
                            {
                                HandleMessage(message);
                            }
                        }
                        catch (Exception e)
                        {
                            _backgroundlogger.Error("Error receiving messages: {0}", LoggingCategory.Background, e.ToString());
                        }
                    }
                }
            }
        }


        private void connection_ExceptionListener(Exception e)
        {
            Logger.Error("Exception occurred while connecting to JMS", e);
            Logger.Debug("Restarting JMS connection");
            StartConnection();
        }

        protected void HandleMessage(IMessage receivedMsg)
        {

            ITextMessage message = receivedMsg as ITextMessage;
            if (message == null)
            {
                _backgroundlogger.Warning("received JMS message with id {0} which is not a text message", receivedMsg.NMSMessageId);
                receivedMsg.Acknowledge();
                return;
            }

            _backgroundlogger.Debug("received text message with id {0} and text {1}", message.NMSMessageId, message.Text);

            try
            {
                ICacheEvent cacheEvent = CacheEventSerializer.Deserialize(message.Text);
                foreach (IObserver<ICacheEvent> observer in observers)
                {
                    observer.OnNext(cacheEvent);
                }
            }
            catch (Exception e)
            {
                _backgroundlogger.Error("error in invalidation transaction: {0}", e.ToString());
                foreach (IObserver<ICacheEvent> observer in observers)
                {
                    observer.OnError(e);
                }
            }
            finally
            {
                message.Acknowledge();
                _backgroundlogger.Debug(string.Format("acknowledged received message with id {0}", message.NMSMessageId));
            }
        }


        #endregion


        #region IObservable
        private class Unsubscriber : IDisposable
        {
            private List<IObserver<ICacheEvent>> _observers;
            private IObserver<ICacheEvent> _observer;

            public Unsubscriber(List<IObserver<ICacheEvent>> observers, IObserver<ICacheEvent> observer)
            {
                this._observers = observers;
                this._observer = observer;
            }

            public void Dispose()
            {
                if (!(_observer == null)) _observers.Remove(_observer);
            }
        }

        public IDisposable Subscribe(IObserver<ICacheEvent> observer)
        {
            if (!observers.Contains(observer))
                observers.Add(observer);

            return new Unsubscriber(observers, observer);
        }

        #endregion

        #region IDisposable
        public void Dispose()
        {
           // Running = false; TODO: check if this is needed
        }
        #endregion

        public class CacheEventSerializer
        {
            private static JsonSerializer _serializer = null;
            private static JsonSerializer Serializer
            {
                get
                {
                    if (_serializer == null)
                    {
                        _serializer = new JsonSerializer();
                    }

                    return _serializer;
                }
            }
            public static ICacheEvent Deserialize(string s)
            {
                using (var stringReader = new StringReader(s))
                {
                    JsonTextReader reader = new JsonTextReader(stringReader);
                    return (ICacheEvent)Serializer.Deserialize<CacheEvent>(reader);
                }
            }
        }
    }

 




 }
