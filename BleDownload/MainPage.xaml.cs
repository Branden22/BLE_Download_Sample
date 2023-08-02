namespace BleDownload;

public partial class MainPage : ContentPage
{
    public MainPage()
	{
		InitializeComponent();
    }

    void ScanBtn_Clicked(System.Object sender, System.EventArgs e)
    {
        String command = BleCommand.Text;
        String[] commandParts = command.Split(" ");
        byte[] cmd = new byte[2];
        cmd[0] = (byte)Int32.Parse(commandParts[0]);
        cmd[1] = (byte)Int32.Parse(commandParts[1]);

        CoreBLE.Scanner.StartScan(cmd);
    }
}


