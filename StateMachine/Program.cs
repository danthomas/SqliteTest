using System;
using System.Windows.Forms;

namespace StateMachine
{
    public static class Program
    {
        [STAThread]
        public static void Main()
        {
            Designer designer = new Designer();

            Application.Run(designer);
        }
    }
}