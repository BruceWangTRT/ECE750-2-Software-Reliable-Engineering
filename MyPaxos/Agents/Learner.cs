using System;
using System.Collections.Generic;
using MyPaxos.Messages;
using MyPaxos.MyValues;

namespace MyPaxos.Agents
{
    public class Learner : Agent
    {
        private readonly int acceptorsCount;
        public List<int> ProposalNumber { get; set; }
        public IList<MyValue> Values { get; set; }
        private List<Agent> knownAcceptors { get; set; }
        public bool AcceptedConfirm { get; set; }


        //constructor
        public Learner(int acceptorsCount)
        {
            this.acceptorsCount = acceptorsCount;
            ProposalNumber = new List<int>();
            Values = new List<MyValue>();
            knownAcceptors = new List<Agent>();
            AcceptedConfirm = false;
        }

        public override void DispatchMessage(Message result)
        {
            if (result.GetType().Name.ToString() == "Accepted")
            {
                Accepted accepted = (Accepted)result;
                this.ProposalNumber.Add(accepted.ProposalNumber);
                this.Values.Add(accepted.AcceptedValue);
                this.knownAcceptors.Add(result.Originator);
                if (this.knownAcceptors.Count > acceptorsCount / 2)
                    AcceptedConfirm = true;
                Console.WriteLine("Accepted Value: {0}", AppliedValue());
            }
        }
        
        //public void DispatchMessage(Accepted result)
        //{
        //    this.ProposalNumber.Add(result.ProposalNumber);
        //    this.Values.Add(result.AcceptedValue);
        //    this.knownAcceptors.Add(result.Originator);
        //    if (this.knownAcceptors.Count > acceptorsCount / 2)
        //        AcceptedConfirm = true;
        //    Console.WriteLine("Accepted Value: {0}", AppliedValue());
        //    ExecuteWork = false;
        //}

        public string AppliedValue()
        {
            if (AcceptedConfirm)
                return Values[0].ToString();
            else
                return "no value";
        }
    }
}