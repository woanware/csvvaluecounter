namespace csvvaluecounter
{
    /// <summary>
    /// 
    /// </summary>
    public class ValueCounter
    {
        public string Value { get; set; }
        public int Count { get; set; }

        public ValueCounter(string value)
        {
            Value = value;
            Count = 1;
        }
    }
}
