using Gtk;
using System.Net.NetworkInformation;

namespace multisub;

class Multisub
{
    private static Entry? _multicastGroup;
    private static ComboBox? _interfaceCombo;
    private static ToggleButton? _bind;

    private static List<string> _interfaceListStore = new();
    
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
        HBox interfaceHBox = new HBox();
        interfaceHBox.Add(new Label("Interface"));
        foreach (var networkInterface in NetworkInterface.GetAllNetworkInterfaces())
        {
            _interfaceListStore.Add(networkInterface.Name);
        }
        _interfaceCombo = new ComboBox(_interfaceListStore.ToArray());
        interfaceHBox.Add(_interfaceCombo);
        outerVBox.Add(interfaceHBox);
        _bind = new ToggleButton();
        _bind.Active = false;
        _bind.Label = "Bind";
        _bind.Clicked += delegate
        {
            if (_bind.Active)
            {
                _bind.Label = "Unbind";
                _multicastGroup.Sensitive = false;
                _interfaceCombo.Sensitive = false;
            }
            else
            {
                _bind.Label = "Bind";
                _multicastGroup.Sensitive = true;
                _interfaceCombo.Sensitive = true;
            }
        };
        outerVBox.Add(_bind);
        window.Add(outerVBox);
        window.ShowAll();
        
        Application.Run();
    }
}