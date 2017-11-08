namespace MyPaxos.MyValues
{
    public class MyValue
    {
        string Execute;

        public MyValue(string value)
        {
            Execute = value;
        }

        public override string ToString()
        {
            return Execute;
        }
    }
}
