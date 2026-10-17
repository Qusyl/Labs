namespace Source.AesStep
{
    public class AesStep
    {
        public string Name { get; set; } = "";
        public string Description { get; set; } = "";   
        public string Explanation { get; set; } = "";   
        public List<string> Trace { get; set; } = [];   

        public byte[,]? Before { get; set; }
        public string BeforeLabel { get; set; } = "До";
        public string Operator { get; set; } = "";      
        public byte[,]? Operand { get; set; }
        public string OperandLabel { get; set; } = "";
        public byte[,] State { get; set; } = new byte[4, 4];
        public string AfterLabel { get; set; } = "После";
    }
}