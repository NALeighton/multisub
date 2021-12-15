using Gtk;

class Hello
{
    static void Main()
    {
        Application.Init();

        Window window = new Window("Hello World");
        window.DeleteEvent += delegate { Application.Quit(); };
        window.Show();
        
        Application.Run();
    }
}