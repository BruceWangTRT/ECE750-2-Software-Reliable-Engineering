using System;
using System.Threading;
using System.Collections.Generic;
using MyPaxos.Messages;
using MyPaxos.MyValues;

namespace MyPaxos.Agents
{
    public class Proposer : Agent
    {
        private readonly Acceptor[] acceptors; // this include our acceptor;
        private int proposalNumber;
        private List<int> ReceivedNumber;
        private List<MyValue> ReceivedValue;
        private MyValue MyProposedValue;

        //constructor
        public Proposer(Acceptor[] acceptors)
        {
            //this.myAcceptor = myAcceptor;
            this.acceptors = acceptors;
            proposalNumber = GenerateNextProposalNumber();
            MyProposedValue = new MyValue("Yanxin Wang");
        }

        private int GenerateNextProposalNumber()
        {
                proposalNumber = (proposalNumber + 1);
            return proposalNumber;
        }

        public void Propose(int proposalnumber)
        {
            Console.WriteLine("send Proposal");
            foreach (var acceptor in acceptors)
            {
                SendMessage(new Proposal
                {
                    Originator = this,
                    Destination = acceptor,
                    ProposalNumber = proposalnumber                    
                });
            }
            Console.WriteLine("release");
            waitForMessages.Release(1);
        }

        public override void DispatchMessage(Message result)
        {
            if (result.GetType().Name.ToString() == "Promise")
            {
                Promise promise = (Promise)result;
                if (promise.ProposalNumber != this.proposalNumber)
                    return;
                if (promise.PreviousAcceptedNumber == 0 || promise.PreviousAcceptedValue == null)//first vote
                {
                    foreach (var acceptor in acceptors)
                    {
                        SendMessage(new Accept
                        {
                            Originator = this,
                            Destination = acceptor,
                            ProposalNumber = this.proposalNumber,
                            ProposalValue = this.MyProposedValue
                        });
                    }
                    Console.WriteLine("send Accept");
                }
                else
                {
                    this.ReceivedNumber.Add(promise.PreviousAcceptedNumber);
                    this.ReceivedValue.Add(promise.PreviousAcceptedValue);
                    if (this.ReceivedNumber.Count <= acceptors.Length / 2)
                        return;
                    //this.QuorumReached = true;
                    var v = this.ReceivedNumber;
                    v.Sort();
                    v.Reverse();//find the greatest ReceivedNumber
                    foreach (var acceptor in acceptors)
                    {
                        SendMessage(new Accept
                        {
                            Originator = this,
                            Destination = acceptor,
                            ProposalNumber = this.proposalNumber,
                            ProposalValue = this.ReceivedValue[this.ReceivedNumber.IndexOf(v[0])]
                        });
                    }
                    Console.WriteLine("send Accept");
                    //ExecuteWork = false;
                    //Thread.CurrentThread.Abort();
                }
            }
            //Thread.Sleep(500);
            //waitForMessages.Release(1);
        }
        
        //public void DispatchMessage(Promise result)
        //{
        //    if (result.ProposalNumber != this.proposalNumber)
        //        return;
        //    if (result.PreviousAcceptedNumber == 0 || result.PreviousAcceptedValue==null)
        //        foreach (var acceptor in acceptors)
        //        {
        //            SendMessage(new Accept
        //            {
        //                Originator = this,
        //                Destination = acceptor,
        //                ProposalNumber = this.proposalNumber,
        //                ProposalValue = this.MyProposedValue
        //            });
        //        }
        //    this.ReceivedNumber.Add(result.PreviousAcceptedNumber);
        //    this.ReceivedValue.Add(result.PreviousAcceptedValue);
        //    if (this.ReceivedNumber.Count<= acceptors.Length / 2)
        //        return;
        //    //this.QuorumReached = true;
        //    var v = this.ReceivedNumber;
        //    v.Sort();
        //    v.Reverse();//find the greatest ReceivedNumber
        //    Console.WriteLine("send Accept");
        //    foreach (var acceptor in acceptors)
        //    {
        //        SendMessage(new Accept
        //        {
        //            Originator = this,
        //            Destination= acceptor,
        //            ProposalNumber = this.proposalNumber,
        //            ProposalValue = this.ReceivedValue[this.ReceivedNumber.IndexOf(v[0])]
        //        });
        //    }
        //}
    }
}