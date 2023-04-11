using System.Threading;
using System.Runtime.InteropServices;

public class Program
{
    [DllImport("user32.dll")]
    private static extern void keybd_event(byte bVk, byte bScan, uint dwFlags, UIntPtr dwExtraInfo);

    private const byte VK_E = 0x45; // Virtual key code for E
    private const byte VK_F7 = 0x76; // Virtual key code for F7
    private const byte VK_F8 = 0x77; // Virtual key code for F8
    private const uint KEYEVENTF_EXTENDEDKEY = 0x0001;
    private const uint KEYEVENTF_KEYUP = 0x0002;

    private static bool isEKeyPressed = false;
    private static bool isF7KeyPressed = false;

    public static void Main()
    {
        Console.SetWindowSize(64, 3); // Only on Windows, lolololo
        Console.WriteLine("");
        Console.WriteLine("                  Press F7 to toggle pressing E.");

        var thread = new Thread(() =>
        {
            Console.Clear();
            Console.WriteLine("");
            Console.WriteLine("         If you see this, it's working. Press F8 to exit.");
            while (true)
            {
                isF7KeyPressed = true;

                if (!isEKeyPressed) // If E key is not pressed, start pressing it
                {
                    isEKeyPressed = true;

                    while (isF7KeyPressed) // Loop until F7 key is released
                    {
                        keybd_event(VK_E, 0, KEYEVENTF_EXTENDEDKEY, UIntPtr.Zero); // Send E key down
                        try
                        {
                            System.Threading.Thread.Sleep(50); // Sleep
                        }
                        catch (ThreadInterruptedException) // Catch to prevent exception on F8 press (that quits out of the program anyway.)
                        { Environment.Exit(0); }
                        keybd_event(VK_E, 0, KEYEVENTF_EXTENDEDKEY | KEYEVENTF_KEYUP, UIntPtr.Zero); // Send E key up
                        try
                        {
                            System.Threading.Thread.Sleep(50); // Sleep
                        }
                        catch (ThreadInterruptedException) // Catch to prevent exception on F8 press (that quits out of the program anyway.)
                        { Environment.Exit(0); }
                    }
                }
            }
        });
        while (true)
        {
            if (GetKeyState(VK_F7) < 0) // Check if F7 key is pressed
            {
                try { thread.Start(); } // Try and catch to ignore repeated F7 presses
                catch(ThreadStateException)
                {}
            }
            if (GetKeyState(VK_F8) < 0) // F8 to exit application
                try { thread.Interrupt(); }
                //catch { Environment.Exit(0); }
                catch { }
            {
                System.Threading.Thread.Sleep(50); // Sleep to reduce CPU use while checking for key presses
            }
        }
    }
    [DllImport("user32.dll")]
    private static extern short GetKeyState(int nVirtKey);
}