using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using MyPaxos.Messages;
using MyPaxos;

namespace MyPaxos.Agents
{
    public class Agent
    {
        public readonly string name;
        private static readonly Dictionary<Type, int> counters = new Dictionary<Type, int>();//denote multiple agents
        public bool ExecuteWork { get; set; }//work state

        static readonly object _locker = new object();//thread control
        private static ConcurrentQueue<Message> queue = new ConcurrentQueue<Message>();//message queue
        public static Semaphore waitForMessages = new Semaphore(0,1);//thread
        private readonly Random random = new Random();//for unreliablesend
        private readonly ConcurrentBag<Message> unordered = new ConcurrentBag<Message>();//out of order message


        //constructor
        public Agent()
        {
            int value;
            counters.TryGetValue(GetType(), out value);
            value += 1;
            counters[GetType()] = value;
            name = string.Format("{0,-12}", GetType().Name + " #" + (value));
            name = name.Trim();
            ExecuteWork = true;
        }

        public void SendMessage(Message message)
        {
            //EnqueueMessage(message); //-- if you want reliable message processing
            SendInUnreliableManner(message);//-- if you want unreliable message processing
        }

        public virtual void DispatchMessage(Message result) {}

        private void EnqueueMessage(Message message)
        {
            queue.Enqueue(message);
            //Console.WriteLine("Successfully Sent");
        }

        private void SendInUnreliableManner(Message message)
        {
            switch (random.Next(1, 10)) // simulate strangeness in message passing
            {
                default:
                    EnqueueMessage(message);
                    break;
                case 2: // message lost 10% of the time
                    Console.WriteLine("!!Message lost!!");
                    break;
                case 3: // message arrive multiple times 10% of the time
                    int times = random.Next(2, 5);//?why [2,5]
                    for (int i = 0; i < times; i++)
                    {
                        EnqueueMessage(message);
                    }
                    Console.WriteLine("!!Message duplicate!!");
                    break;
                case 4: // message will arrive out of order 10% of the time
                    unordered.Add(message);
                    Thread.Sleep(1000);
                    //waitForMessages.Release(1);
                    Message result; // send message out of order
                    while (unordered.TryTake(out result))
                    {
                        EnqueueMessage(result);
                    }
                    Console.WriteLine("!!Message delay!!");
                    break;
            }


        }

        public void ExecuteMultiThreaded()//for thread, thread starting
        {
            Console.WriteLine("Thread {0} begins",Thread.CurrentThread.Name);
            while (ExecuteWork)
            {

                lock(_locker)//allow one thread in
                {
                    waitForMessages.WaitOne();//wait for semaphore control
                    ConsumeAllMessages();
                    Thread.Sleep(500);
                }
            }
            Console.WriteLine("Thread {0} ends", Thread.CurrentThread.Name);
        }
        
        public bool ConsumeAllMessages()
        {
            //Console.WriteLine("consume,{0}",this.ToString());
            var hadMessages = false;
            Message result;
            while (queue.TryPeek(out result))//without removing
            {
                if (result.Destination == this)
                {
                    queue.TryDequeue(out result);//remove message
                    hadMessages = true;
                    Console.WriteLine("CurrentThread:{0}", Thread.CurrentThread.Name);
                    DispatchMessage(result);
                    waitForMessages.Release(1);//thread leaving
                    break;
                }
                else
                {
                    waitForMessages.Release(1);//thread leaving
                    break;
                }
            }
            return hadMessages;
        }

        public override string ToString()
        {
            return name;
        }

    }
}