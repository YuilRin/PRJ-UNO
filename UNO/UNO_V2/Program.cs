namespace UNO_V2
{
    internal static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            // To customize application configuration such as set high DPI settings or default font,
            // see https://aka.ms/applicationconfiguration.
            //Server2 server = new Server2();
            //server.StartServer(); 
            ApplicationConfiguration.Initialize();
            Application.Run(new Login());
            
        }
    }
}