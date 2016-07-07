using DD4T.ContentModel.Contracts.Caching;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DD4T.Core.Test
{

    public class MockMessageProvider : IMessageProvider<ICacheEvent>, IDisposable
    {
        private List<IObserver<ICacheEvent>> observers;

        public void Dispose()
        {
            // Do nothing
        }

        public void Start()
        {
            observers = new List<IObserver<ICacheEvent>>();
        }

        public void BroadcastCacheEvent(ICacheEvent cacheEvent)
        {
            foreach (var observer in observers)
            {
                observer.OnNext(cacheEvent);
            }
        }
       

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
    }
}
