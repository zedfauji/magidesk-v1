using System.Collections.Generic;
using Magidesk.Domain.Entities;
using Magidesk.Infrastructure.Printing.Models;

namespace Magidesk.Infrastructure.Printing.Drivers;

public class EscPosDriver : IPrintDriver
{
    public byte[] Render(PrintDocument doc, PrinterMapping mapping)
    {
        var cmds = new List<byte[]>();
        cmds.Add(EscPosHelper.Initialize());

        foreach (var element in doc.Elements)
        {
            switch (element)
            {
                case TextElement text:
                    // Alignment
                    if (text.Align == "Center") cmds.Add(EscPosHelper.AlignCenter());
                    else if (text.Align == "Right") cmds.Add(EscPosHelper.AlignRight());
                    else cmds.Add(EscPosHelper.AlignLeft());

                    // Style
                    if (text.Bold) cmds.Add(EscPosHelper.BoldOn());
                    if (text.DoubleHeight) cmds.Add(EscPosHelper.DoubleHeightOn());

                    // Content
                    cmds.Add(EscPosHelper.GetBytes(text.Content));
                    cmds.Add(EscPosHelper.NewLine());

                    // Reset Style
                    if (text.Bold) cmds.Add(EscPosHelper.BoldOff());
                    if (text.DoubleHeight) cmds.Add(EscPosHelper.NormalSize());
                    
                    // Reset Align
                    cmds.Add(EscPosHelper.AlignLeft());
                    break;

                case LineBreakElement _:
                    cmds.Add(EscPosHelper.NewLine());
                    break;

                case SeparatorElement sep:
                    int width = mapping.PrintableWidthChars > 0 ? mapping.PrintableWidthChars : 32;
                    cmds.Add(EscPosHelper.GetBytes(new string(sep.Char, width)));
                    cmds.Add(EscPosHelper.NewLine());
                    break;
                
                case CutElement _:
                    cmds.Add(EscPosHelper.Cut());
                    break;
            }
        }

        return EscPosHelper.GenerateTicketData(cmds);
    }
}
