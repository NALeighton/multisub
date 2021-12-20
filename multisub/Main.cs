using Gtk;
using System.Net;
using System.Net.Sockets;
using System.Text.RegularExpressions;

namespace multisub;

class Multisub
{
    private static Entry? _multicastGroup;
    private static ToggleButton? _bind;
    private static Switch? _portSwitch;
    private static Entry? _port;

    private static UdpClient? _client;

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
        _multicastGroup.TextInserted += ValidateMulticastGroup;
        groupHBox.Add(_multicastGroup);
        outerVBox.Add(groupHBox);
        HBox portHBox = new HBox();
        portHBox.Add(new Label("Port"));
        _portSwitch = new Switch();
        _portSwitch.MarginStart = 5;
        _portSwitch.StateChanged += delegate
        {
            if (_portSwitch.Active)
            {
                if (_port != null)
                {
                    _port.Sensitive = true;
                }
            }
            else
            {
                if (_port != null)
                {
                    _port.Sensitive = false;
                }
            }
            ValidateBind();
        };
        portHBox.Add(_portSwitch);
        _port = new Entry();
        _port.MarginStart = 5;
        _port.TextInserted += ValidatePort;
        portHBox.Add(_port);
        outerVBox.Add(portHBox);
        _bind = new ToggleButton();
        _bind.Active = false;
        _bind.Sensitive = false;
        _bind.Label = "Bind";
        _bind.Clicked += delegate
        {
            if (_bind.Active)
            {
                _bind.Label = "Unbind";
                _multicastGroup.Sensitive = false;
                if (!MulticastGroupIsValid(_multicastGroup.Text))
                {
                    _bind.Active = false;
                    return;
                }
                
                if (_portSwitch.Active && !PortIsValid(_port.Text))
                {
                    _bind.Active = false;
                    return;
                }

                try
                {
                    _client = new UdpClient(_multicastGroup.Text, int.Parse(_port.Text));
                    _client.JoinMulticastGroup(IPAddress.Parse(_multicastGroup.Text));
                }
                catch
                {
                    _bind.Active = false;
                }
            }
            else
            {
                _bind.Label = "Bind";
                _multicastGroup.Sensitive = true;
                _client?.DropMulticastGroup(IPAddress.Parse(_multicastGroup.Text));
                _client?.Close();
                _client = null;
            }
        };
        outerVBox.Add(_bind);
        window.Add(outerVBox);
        window.ShowAll();

        Application.Run();
    }

    private static void ValidateMulticastGroup(object o, TextInsertedArgs args)
    {
        if (_multicastGroup?.Text.Length > 15)
        {
            _multicastGroup.Text = _multicastGroup.Text.Substring(0, 15);
        }
        
        ValidateBind();
    }

    private static bool MulticastGroupIsValid(string multicastGroup)
    {
        var regex = new Regex(@"\d{3}\.\d{1,3}\.\d{1,3}\.\d{1,3}");
        var flag = regex.IsMatch(multicastGroup);
        if (flag)
        {
            var blocks = multicastGroup.Split('.');
            flag = flag && int.Parse(blocks[0]) >= 224 && int.Parse(blocks[0]) <= 239;
            for (var i = 1; i < 4; i++)
            {
                flag = flag && int.Parse(blocks[i]) >= 0 && int.Parse(blocks[i]) <= 255;
            }
        }
        return flag;
    }
    
    private static void ValidatePort(object o, TextInsertedArgs args)
    {
        if (_port?.Text.Length > 4)
        {
            _port.Text = _port.Text.Substring(0, 4);
        }

        ValidateBind();
    }

    private static bool PortIsValid(string port)
    {
        var regex = new Regex(@"\d{1,4}");
        var flag = regex.IsMatch(port);
        if (flag)
        {
            flag = flag && int.Parse(port) >= 1 && int.Parse(port) <= 65535;
        }
        return flag;
    }

    private static void ValidateBind()
    {
        var flag = true;
        flag = flag && _multicastGroup?.Text.Length > 0 && MulticastGroupIsValid(_multicastGroup.Text);
        flag = flag && _portSwitch != null && !_portSwitch.Active || _port?.Text.Length > 0 && _port.Text.Length <= 4 && PortIsValid(_port.Text);
        if (_bind != null) _bind.Sensitive = flag;
    }
}