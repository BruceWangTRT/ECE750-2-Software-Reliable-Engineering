using MyPaxos.Agents;

namespace MyPaxos.Messages
{
    public class Message
    {
        public Agent Originator { get; set; }
        public Agent Destination { get; set; }
    }
}
