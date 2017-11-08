using MyPaxos.MyValues;

namespace MyPaxos.Messages
{
    public class Accepted : Message
    {
        public int ProposalNumber { get; set; }
        public MyValue AcceptedValue { get; set; }

        public override string ToString()
        {
            return string.Format("ProposalNumber: {0}, AcceptedValue: {1}", ProposalNumber, AcceptedValue);
        }
    }
}