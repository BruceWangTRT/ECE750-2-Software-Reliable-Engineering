using MyPaxos.MyValues;

namespace MyPaxos.Messages
{
    public class Promise : Message
    {
        public int ProposalNumber { get; set; }
        public int PreviousAcceptedNumber { get; set; }
        public MyValue PreviousAcceptedValue { get; set; }

        public override string ToString()
        {
            return string.Format("ProposalNumber: {0},PreviousAcceptedNumber: {1},PreviousAcceptedValue: {2}",
                                 ProposalNumber, PreviousAcceptedNumber, PreviousAcceptedValue);
        }
    }
}