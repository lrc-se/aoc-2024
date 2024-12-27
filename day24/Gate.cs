internal abstract class Gate(string input1, string input2, string output)
{
    public string Input1 { get; } = input1;
    public string Input2 { get; } = input2;
    public string Output { get; set; } = output;

    public abstract bool Execute(bool value1, bool value2);
}

internal class AndGate(string input1, string input2, string output) : Gate(input1, input2, output)
{
    public override bool Execute(bool value1, bool value2) => value1 && value2;
}

internal class OrGate(string input1, string input2, string output) : Gate(input1, input2, output)
{
    public override bool Execute(bool value1, bool value2) => value1 || value2;
}

internal class XorGate(string input1, string input2, string output) : Gate(input1, input2, output)
{
    public override bool Execute(bool value1, bool value2) => value1 != value2;
}
