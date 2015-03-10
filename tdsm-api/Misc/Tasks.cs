using System;
using System.Collections.Generic;

namespace tdsm.api.Misc
{
    public struct Task
    {
        public static readonly Task Empty;

        private DateTime _insertedAt;
        private bool _enabled;

        public object Data;

        public bool Triggered
        {
            get
            {
                var span = (DateTime.Now - _insertedAt).TotalSeconds;
                return _enabled && span >= Trigger;
            }
        }

        public bool Enabled
        {
            get
            {
                return _enabled;
            }
            set
            {
                _enabled = value;

                if (_enabled) Reset();
            }
        }

        /// <summary>
        /// Gets or sets the trigger in Seconds.
        /// </summary>
        /// <value>
        /// The trigger.
        /// </value>
        public int Trigger { get; set; }

        /// <summary>
        /// Gets or sets the method to be called.
        /// </summary>
        /// <value>
        /// The method.
        /// </value>
        public Action<Task> Method { get; set; }

        public void Reset(bool clearData = true)
        {
            _insertedAt = DateTime.Now;

            if (clearData) Data = null;
        }

        public Task Init()
        {
            Reset();
            return this;
        }

        public bool IsEmpty()
        {
            return this.Trigger == Empty.Trigger && this.Method == Empty.Method;
        }
    }

    public static class Tasks
    {
        static Stack<Task> _tasks;

        static Tasks()
        {
            _tasks = new Stack<Task>();
        }

        public static void Schedule(Task task, bool init = true)
        {
            if (init) task.Init();
            lock (_tasks) _tasks.Push(task);
        }

        internal static void CheckTasks()
        {
            lock (_tasks)
            {
                for (var i = 0; i < _tasks.Count; i++)
                {
                    Task task = _tasks.Pop();
                    if (task.Triggered)
                    {
                        task.Method.BeginInvoke
                        (task,
                            (IAsyncResult res) =>
                            {
                                task.Method.EndInvoke(res);
                            }, null
                        );
                        task.Reset();
                    }
                    _tasks.Push(task);
                }
            }
        }
    }
}
