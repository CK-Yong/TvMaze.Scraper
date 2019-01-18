using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace TvMaze.Scraper.TestUtilities
{
    /// <summary>
    /// Provides a pump that supports running asynchronous methods on the current thread.
    /// </summary>
    /// <remarks>
    /// https://blogs.msdn.microsoft.com/pfxteam/2012/02/02/await-synchronizationcontext-and-console-apps-part-3/
    /// </remarks>
    public static class AsyncPump
    {
        /// <summary>Runs the specified asynchronous method.</summary>
        /// <param name="asyncMethod">The asynchronous method to execute.</param>
        [DebuggerStepThrough]
        public static void Run(Func<Task> asyncMethod)
        {
            if (asyncMethod == null)
            {
                throw new ArgumentNullException(nameof(asyncMethod));
            }

            SynchronizationContext prevCtx = SynchronizationContext.Current;
            try
            {
                // Establish the new context
                var syncCtx = new SingleThreadSynchronizationContext(false);
                SynchronizationContext.SetSynchronizationContext(syncCtx);

                // Invoke the function and alert the context to when it completes
                Task t = asyncMethod();
                if (t == null)
                {
                    throw new InvalidOperationException("No task provided.");
                }

                t.ContinueWith(delegate { syncCtx.Complete(); }, TaskScheduler.Default);

                // Pump continuations and propagate any exceptions
                syncCtx.RunOnCurrentThread();
                t.GetAwaiter().GetResult();
            }
            finally
            {
                SynchronizationContext.SetSynchronizationContext(prevCtx);
            }
        }

        /// <summary>
        /// Provides a SynchronizationContext that's single-threaded.
        /// </summary>
        private sealed class SingleThreadSynchronizationContext : SynchronizationContext
        {
            /// <summary>The queue of work items.</summary>
            private readonly BlockingCollection<KeyValuePair<SendOrPostCallback, object>> _queue =
                new BlockingCollection<KeyValuePair<SendOrPostCallback, object>>();

            /// <summary>Whether to track operations m_operationCount.</summary>
            private readonly bool _trackOperations;

            /// <summary>The number of outstanding operations.</summary>
            private int _operationCount;

            /// <summary>
            /// Initializes the context.
            /// </summary>
            /// <param name="trackOperations">Whether to track operation count.</param>
            internal SingleThreadSynchronizationContext(bool trackOperations)
            {
                _trackOperations = trackOperations;
            }

            /// <summary>
            /// Dispatches an asynchronous message to the synchronization context.
            /// </summary>
            /// <param name="d">The System.Threading.SendOrPostCallback delegate to call.</param>
            /// <param name="state">The object passed to the delegate.</param>
            public override void Post(SendOrPostCallback d, object state)
            {
                if (d == null)
                {
                    throw new ArgumentNullException(nameof(d));
                }
                _queue.Add(new KeyValuePair<SendOrPostCallback, object>(d, state));
            }

            /// <summary>
            /// Not supported.
            /// </summary>
            public override void Send(SendOrPostCallback d, object state)
            {
                throw new NotSupportedException("Synchronously sending is not supported.");
            }

            /// <summary>
            /// Runs an loop to process all queued work items.
            /// </summary>
            public void RunOnCurrentThread()
            {
                foreach (KeyValuePair<SendOrPostCallback, object> workItem in _queue.GetConsumingEnumerable())
                {
                    workItem.Key(workItem.Value);
                }
            }

            /// <summary>
            /// Notifies the context that no more work will arrive.
            /// </summary>
            public void Complete()
            {
                _queue.CompleteAdding();
            }

            /// <summary>Invoked when an async operation is started.</summary>
            public override void OperationStarted()
            {
                if (_trackOperations)
                {
                    Interlocked.Increment(ref _operationCount);
                }
            }

            /// <summary>Invoked when an async operation is completed.</summary>
            public override void OperationCompleted()
            {
                if (_trackOperations && Interlocked.Decrement(ref _operationCount) == 0)
                {
                    Complete();
                }
            }
        }
    }
}
