using MyPaxos.MyValues;

namespace MyPaxos.Messages
{
    public class Accept : Message
    {
        public int ProposalNumber { get; set; }
        public MyValue ProposalValue { get; set; }

        public override string ToString()
        {
            return string.Format("ProposalNumber: {0}, ProposalValue: {1}", ProposalNumber, ProposalValue);
        }
    }
}