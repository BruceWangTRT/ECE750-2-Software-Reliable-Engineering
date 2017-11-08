using System;
using System.Threading;
using System.Collections.Generic;
using MyPaxos.Messages;
using MyPaxos.MyValues;

namespace MyPaxos.Agents
{
    public class Acceptor : Agent
    {
        private readonly Learner[] learners;

        public int ProposalNumber { get; set; }
        public int PreviousAcceptedNumber { get; set; }
        public MyValue PreviousAcceptedValue { get; set; }
 

        public Acceptor(Learner[] learners)
        {
            this.learners = learners;
            this.PreviousAcceptedNumber = 0;
            this.PreviousAcceptedValue = new MyValue(null);
        }

        public override void DispatchMessage(Message result)
        {
            //Console.WriteLine("{0}", result.GetType().Name.ToString());
            if(result.GetType().Name.ToString()=="Proposal")
            {
                Proposal propose = (Proposal)result;
                if (propose.ProposalNumber >= this.ProposalNumber)
                {
                //Console.WriteLine("!!!!!!!!!!!!!!!!!!!!!!!{0}", this.PreviousAcceptedNumber);
                SendMessage(new Promise
                  {
                       Originator = this,
                       Destination = propose.Originator,
                       ProposalNumber = propose.ProposalNumber,
                       PreviousAcceptedNumber = this.PreviousAcceptedNumber,
                       PreviousAcceptedValue = this.PreviousAcceptedValue
                   });
                   this.ProposalNumber = propose.ProposalNumber;
                }
                Console.WriteLine("send Promise");
            }
            else if (result.GetType().Name.ToString() == "Accept")
            {
                Accept accept = (Accept)result;
                foreach (var learner in learners)
                {
                    SendMessage(new Accepted
                    {
                        Originator = this,
                        Destination = learner,
                        ProposalNumber = accept.ProposalNumber,
                        AcceptedValue = accept.ProposalValue
                    });
                }
                Console.WriteLine("send Accepted to learners");
                this.PreviousAcceptedNumber = accept.ProposalNumber;
                this.PreviousAcceptedValue = accept.ProposalValue;
                //ExecuteWork = false;
            }
            //Thread.Sleep(500);
            //waitForMessages.Release(1);
            //Thread.Sleep(10);

        }

        //public void DispatchMessage(Proposal result)
        //{
        //    Proposal propose=result;
        //    if (propose.ProposalNumber >= this.ProposalNumber)
        //    {
        //        Console.WriteLine("send Promise");  
        //        SendMessage(new Promise
        //          {
        //               Originator = this,
        //               Destination = propose.Originator,
        //               ProposalNumber = propose.ProposalNumber,
        //               PreviousAcceptedNumber = this.PreviousAcceptedNumber,
        //               PreviousAcceptedValue = this.PreviousAcceptedValue
        //           });
        //           this.ProposalNumber = propose.ProposalNumber;
        //     }
        //}

        //public void DispatchMessage(Accept result)
        //{
        //    Accept accept=result;
        //    //Console.WriteLine("send Accepted to proposer");
        //    //SendMessage(new Accepted
        //    //{
        //    //    Originator = this,
        //    //    Destination = accept.Originator,
        //    //    ProposalNumber = accept.ProposalNumber,
        //    //    AcceptedValue = accept.ProposalValue
        //    //});
        //    Console.WriteLine("send Accepted to learners");
        //    foreach (var learner in learners)
        //    {
        //        SendMessage(new Accepted
        //        {
        //            Originator = this,
        //            Destination = learner,
        //            ProposalNumber = accept.ProposalNumber,
        //            AcceptedValue = accept.ProposalValue
        //        });
        //    }
        //}

    }
}