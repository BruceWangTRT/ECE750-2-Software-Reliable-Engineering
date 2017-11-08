using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using MyPaxos.Agents;


namespace MyPaxos
{
    public class Program
    {
        static void Main()
        {
            var learners = new[]
			{
				new Learner(acceptorsCount: 3),
				new Learner(acceptorsCount: 3),
				new Learner(acceptorsCount: 3),
			};

            var acceptors = new[]
            {
                new Acceptor(learners),
                new Acceptor(learners),
                new Acceptor(learners)
            };

            var proposers = new[]
            {
                    new Proposer(acceptors)
            };
            var agents = acceptors.OfType<Agent>().Union(proposers).Union(learners).ToArray();


            foreach (var agent in agents)
            {
                new Thread(agent.ExecuteMultiThreaded)
                {
                    Name = agent.ToString(),
                    IsBackground = false
                }.Start();
            }
            Thread.Sleep(500);// to allow all threads start

            proposers[0].Propose(1);//start to propose
        }
    }
}
