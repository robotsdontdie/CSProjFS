namespace TestApp
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello, World!");
            string root = @"C:\temp\testapp";
            if (Directory.Exists(root))
            {
                Directory.Delete(root, true);
            }

            Directory.CreateDirectory(root);

            //ProjFS.Managed.StartVirtualizing(root);
        }
    }
}
