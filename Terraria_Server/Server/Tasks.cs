using System;
using System.Collections.Generic;

namespace Terraria_Server
{
    public struct Task
    {
        private DateTime _insertedAt;
        
        public bool Triggered
        {
            get
            {
                var span = (DateTime.Now - _insertedAt).TotalSeconds;
                return span >= Trigger;
            }
        }
        
        /// <summary>
        /// Gets or sets the trigger in Seconds.
        /// </summary>
        /// <value>
        /// The trigger.
        /// </value>
        public int Trigger          { get; set; }
        
        /// <summary>
        /// Gets or sets the method to be called.
        /// </summary>
        /// <value>
        /// The method.
        /// </value>
		public Action Method { get; set; }
	        
        public void Reset()
        {
            _insertedAt = DateTime.Now;
        }
        
        public Task Init()
        {
            Reset ();
            return this;
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
            if(init) task.Init ();
            lock(_tasks) _tasks.Push(task);
        }
        
        internal static void CheckTasks()
        {
            lock(_tasks)
            {
                for(var i = 0; i < _tasks.Count; i++)
                {
                    Task task = _tasks.Pop ();                
                    if(task.Triggered)
                    {
                        task.Method.BeginInvoke 
                        (
                            (IAsyncResult res) =>
                            {
                                task.Method.EndInvoke(res);
                            }, null
                        );
                        task.Reset ();
                    }
                    _tasks.Push(task);
                }
            }
        }
    }
}

