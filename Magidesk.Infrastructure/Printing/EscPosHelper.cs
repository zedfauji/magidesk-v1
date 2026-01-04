using System.Text;

namespace Magidesk.Infrastructure.Printing;

public static class EscPosHelper
{
    private const byte Esc = 0x1B;
    private const byte Gs = 0x1D;
    private const byte Lf = 0x0A;

    public static byte[] Initialize() => new byte[] { Esc, 0x40 };

    public static byte[] AlignLeft() => new byte[] { Esc, 0x61, 0 };
    public static byte[] AlignCenter() => new byte[] { Esc, 0x61, 1 };
    public static byte[] AlignRight() => new byte[] { Esc, 0x61, 2 };

    public static byte[] BoldOn() => new byte[] { Esc, 0x45, 1 };
    public static byte[] BoldOff() => new byte[] { Esc, 0x45, 0 };
    
    public static byte[] DoubleHeightOn() => new byte[] { Gs, 0x21, 0x10 }; // Just double height, normal width
    public static byte[] DoubleWidthOn() => new byte[] { Gs, 0x21, 0x20 }; // Just double width, normal height
    public static byte[] NormalSize() => new byte[] { Gs, 0x21, 0x00 };

    public static byte[] Cut() => new byte[] { Gs, 0x56, 66, 0 };
    
    public static byte[] GetBytes(string text)
    {
        // Using Western European encoding generic or ASCII
        // WinUI 3 might default to UTF8, but printers differ. Code Page 437 is standard.
        // For now, ASCII or basic encoding.
        return Encoding.ASCII.GetBytes(text);
    }

    public static byte[] NewLine() => new byte[] { Lf };

    public static byte[] GenerateTicketData(List<byte[]> commands)
    {
        var result = new List<byte>();
        foreach (var cmd in commands)
        {
            result.AddRange(cmd);
        }
        return result.ToArray();
    }
}
