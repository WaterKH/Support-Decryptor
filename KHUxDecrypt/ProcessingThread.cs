using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace KHUxDecrypt
{
    // Courtesy of Oxin8
    public class ProcessingThread<T> : IDisposable
    {
        private Thread _thread;
        private Queue<T> _queue = new Queue<T>();
        private readonly object _queueLock = new object();
        private volatile bool _running = false;
        private Action<T> _processingAction;

        public ProcessingThread(Action<T> processingAction)
        {
            _processingAction = processingAction;
            _thread = new Thread(ThreadMethod);
            _thread.IsBackground = true;
            Start();
        }

        private void Start()
        {
            _running = true;
            _thread.Start();
        }

        public void StopWhenEmpty()
        {
            bool empty = false;
            while (!empty)
            {
                lock (_queueLock)
                {
                    if (_queue.Count == 0)
                    {
                        empty = true;
                    }
                }
                Thread.Sleep(1000);
            }
            _running = false;
            _thread.Abort();
        }

        public void Add(T item)
        {
            lock (_queueLock)
            {
                _queue.Enqueue(item);
            }
        }

        private void ThreadMethod()
        {
            T itemToProcess = default(T);
            while (_running)
            {
                try
                {
                    bool hasItem = false;
                    while (_running)
                    {
                        lock (_queueLock)
                        {
                            if (_queue.Count > 0)
                            {
                                itemToProcess = _queue.Dequeue();
                                hasItem = true;
                            }
                        }
                        if (hasItem)
                        {
                            _processingAction(itemToProcess);
                            hasItem = false;
                        }
                        Thread.Sleep(0);
                    }
                }
                catch { }
                Thread.Sleep(100);
            }
        }

        public void Dispose()
        {
            StopWhenEmpty();
        }
    }
}
