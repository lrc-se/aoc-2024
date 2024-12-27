internal enum Instruction
{
    adv = 0,
    bxl = 1,
    bst = 2,
    jnz = 3,
    bxc = 4,
    @out = 5,
    bdv = 6,
    cdv = 7
}

internal class Computer(long registerA, long registerB, long registerC, int[] program)
{
    public long RegisterA { get; private set; } = registerA;
    public long RegisterB { get; private set; } = registerB;
    public long RegisterC { get; private set; } = registerC;

    public int[] Program { get; } = program;

    public int[] Run()
    {
        long instructionPointer = 0;

        long GetOperandValue(bool isCombo = false)
        {
            int operand = Program[instructionPointer + 1];
            if (!isCombo)
                return operand;

            return operand switch
            {
                >= 0 and <= 3 => operand,
                4 => RegisterA,
                5 => RegisterB,
                6 => RegisterC,
                _ => throw new ArgumentOutOfRangeException()
            };
        }

        List<int> output = [];
        do
        {
            switch ((Instruction)Program[instructionPointer])
            {
                case Instruction.adv:
                    RegisterA = (long)Math.Truncate(RegisterA / Math.Pow(2, GetOperandValue(true)));
                    break;
                case Instruction.bxl:
                    RegisterB ^= GetOperandValue();
                    break;
                case Instruction.bst:
                    RegisterB = GetOperandValue(true) % 8;
                    break;
                case Instruction.jnz:
                    if (RegisterA != 0)
                    {
                        instructionPointer = GetOperandValue();
                        continue;
                    }
                    break;
                case Instruction.bxc:
                    RegisterB ^= RegisterC;
                    break;
                case Instruction.@out:
                    output.Add((int)(GetOperandValue(true) % 8));
                    break;
                case Instruction.bdv:
                    RegisterB = (long)Math.Truncate(RegisterA / Math.Pow(2, GetOperandValue(true)));
                    break;
                case Instruction.cdv:
                    RegisterC = (long)Math.Truncate(RegisterA / Math.Pow(2, GetOperandValue(true)));
                    break;
            }

            instructionPointer += 2;
        } while (instructionPointer < Program.Length);

        return [..output];
    }

    public void Reset(long registerA, long registerB, long registerC)
    {
        RegisterA = registerA;
        RegisterB = registerB;
        RegisterC = registerC;
    }
}
