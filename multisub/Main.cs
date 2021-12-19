using Gtk;

namespace multisub;

class Multisub
{
    private static Entry? _multicastGroup;
    private static ToggleButton? _bind;
    static void Main()
    {
        Application.Init();

        Window window = new Window("Multisub");
        window.DeleteEvent += delegate { Application.Quit(); };
        VBox outerVBox = new VBox();
        HBox groupHBox = new HBox();
        groupHBox.Add(new Label("Multicast Group"));
        _multicastGroup = new Entry();
        _multicastGroup.MarginStart = 5;
        groupHBox.Add(_multicastGroup);
        outerVBox.Add(groupHBox);
        _bind = new ToggleButton();
        _bind.Active = false;
        _bind.Label = "Bind";
        _bind.Clicked += delegate
        {
            if (_bind.Active)
            {
                _bind.Label = "Unbind";
                _multicastGroup.Sensitive = false;
            }
            else
            {
                _bind.Label = "Bind";
                _multicastGroup.Sensitive = true;
            }
        };
        outerVBox.Add(_bind);
        window.Add(outerVBox);
        window.ShowAll();
        
        Application.Run();
    }
}