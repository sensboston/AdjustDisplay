using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;

namespace AdjustDisplay
{
    internal class Program
    {
        const string HELP_MESSAGE =
@"
Usage: !AdjustDisplay.exe! $[params]$, where params are:

!ident!            : identify displays

!get!              : return connected displays information
!get! $disp=n$       : return info for all supported modes for the specific display, $n$ starts from 1

!set! [params]     : set parameters for specific display
    $disp=n$       : display number (starts from 1); if omitted, $disp=1$ will be used
    $mode=nn$      : set video mode (from range returned by $get disp=n$)
    $width=nnnn$   : desired screen width, i.e. $width=2048$
    $height=nnnn$  : desired screen height, i.e. $height=1080$
    $orient=nnn$   : diserid screen orientation; available values are: $0, 90, 180, 270$
    $freq=nnn$     : desired screen frequency (refresh rate)
    $bpp=nn$       : desired bits per pixel; available values are: $8, 16, 24, 32$
    $scale=nnn$    : disared screen scale; available values are: $[100, 125, 150...450, 500]$

Note: any [param] value can be omitted
";

        static DisplayManager displayManager = new DisplayManager();

        static int displayNum = 1;
        static int mode = -1;
        static int width = 0;
        static int height = 0;
        static int orient = -1;
        static int freq = 0;
        static int bpp = 0;
        static int scale = 0;

        const int IDENT_WAIT_TIME = 2500;

        static StringComparison sc = StringComparison.InvariantCultureIgnoreCase;

        static void Main(string[] args)
        {
            if (args.Length == 0 || args[0].Contains("?") || args[0].Contains("help"))
            {
                WriteColorLine(HELP_MESSAGE);
            }
            else
            {
                string param = args.FirstOrDefault(s => s.StartsWith("disp=", sc));
                if (param != null) int.TryParse(param.Substring("disp=".Length), out displayNum);

                if (args[0].Equals("ident", sc))
                {
                    IdentifyDisplays();
                }
                // Show curent display configuration
                else if (args[0].Equals("get", sc))
                {
                    if (args.Length == 1)
                    {
                        for (int i = 0; i < displayManager.Displays.Count; i++)
                            Console.WriteLine($"{i+1}: {displayManager.Displays[i]}");
                    }
                    else
                    {
                        Console.WriteLine($"{displayNum}: {displayManager.Displays[displayNum - 1]}");
                        Console.WriteLine("Video modes:");
                        for (int i = 0; i < displayManager.Displays[displayNum - 1].Modes.Count; i++)
                            Console.WriteLine($"\t{i+1}:\t{displayManager.Displays[displayNum - 1].Modes[i]}");
                    }
                }
                else
                {
                    param = args.FirstOrDefault(s => s.StartsWith("mode=", sc));
                    if (param != null) int.TryParse(param.Substring("mode=".Length), out mode);
                    param = args.FirstOrDefault(s => s.StartsWith("width=", sc));
                    if (param != null) int.TryParse(param.Substring("width=".Length), out width);
                    param = args.FirstOrDefault(s => s.StartsWith("height=", sc));
                    if (param != null) int.TryParse(param.Substring("height=".Length), out height);
                    param = args.FirstOrDefault(s => s.StartsWith("orient=", sc));
                    if (param != null) int.TryParse(param.Substring("orient=".Length), out orient);
                    param = args.FirstOrDefault(s => s.StartsWith("freq=", sc));
                    if (param != null) int.TryParse(param.Substring("freq=".Length), out freq);
                    param = args.FirstOrDefault(s => s.StartsWith("bpp=", sc));
                    if (param != null) int.TryParse(param.Substring("bpp=".Length), out bpp);
                    param = args.FirstOrDefault(s => s.StartsWith("scale=", sc));
                    if (param != null) int.TryParse(param.Substring("scale=".Length), out scale);

                    // Check for parameters correctnes
                    if (mode > -1 || width > 0 || height > 0 || orient > -1 || freq > 0 || bpp > 0 || scale > 0)
                    {
                        try
                        {
                            if (mode < 0)
                                displayManager.SetVideoMode(displayNum, width, height, orient, bpp, freq, scale);
                            else
                                displayManager.SetVideoMode(displayNum, mode);
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine($"Error: {e.Message}");
                        }
                    }
                    else WriteColorLine(HELP_MESSAGE);
                }
            }
        }

        /// <summary>
        /// Identifies all connected displays (like a built-in Windows feature) by showing display #
        /// </summary>
        private static void IdentifyDisplays()
        {
            int i = 1;
            foreach (var screen in Screen.AllScreens)
            {
                var disp = displayManager.Displays.FirstOrDefault(d => d.DeviceName.Equals(screen.DeviceName));
                string dispNum = (disp != null && disp.IsDuplicated) ? "1/2" : i.ToString();
                Rectangle bounds = screen.Bounds;
                float size = (float)(bounds.Height * 0.4);
                int halfSize = (int)(size * 1.2);
                var form = new Form()
                {
                    StartPosition = FormStartPosition.Manual,
                    FormBorderStyle = FormBorderStyle.None,
                    WindowState = FormWindowState.Maximized,
                    BackColor = Color.Gray,
                    TransparencyKey = Color.Gray
                };
                form.SetBounds(bounds.X, bounds.Y, bounds.Width, bounds.Height);
                var label = new Label()
                {
                    ForeColor = Color.Yellow,
                    BackColor = Color.Transparent,
                    Width = halfSize*dispNum.Length,
                    Height = halfSize,
                    TextAlign = ContentAlignment.MiddleCenter,
                    Font = new Font(FontFamily.GenericSansSerif, size),
                    Text = dispNum,
                    Left = (form.Width - halfSize * dispNum.Length) / 2,
                    Top = (form.Height - halfSize) / 2
                };
                form.Controls.Add(label);
                form.Show();
                label.Refresh();
                i++;
            }
            Thread.Sleep(IDENT_WAIT_TIME);
        }

        /// <summary>
        /// Simple helper to colorize console output
        /// </summary>
        /// <param name="str">Colorized with char tokens string</param>
        private static void WriteColorLine(string str)
        {
            var colorTokens = new Dictionary<string, ConsoleColor>() 
            {
                { "!", ConsoleColor.White }, { "$", ConsoleColor.Green },
            };
            var parts = Regex.Split(str, $"([{string.Join("", colorTokens.Select(i => i.Key).ToArray())}])");
            foreach (var s in parts)
            {
                if (colorTokens.ContainsKey(s))
                {
                    if (Console.ForegroundColor == colorTokens[s]) Console.ResetColor();
                    else Console.ForegroundColor = colorTokens[s];
                }
                else Console.Write(s);
            }
        }
    }
}
