using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net.Http;
using System.Windows;

namespace WpfApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            var clientHandler = new HttpClientHandler();
            clientHandler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => { return true; };

            var response = await new HttpClient(clientHandler).GetAsync("https://localhost:12345/weatherforecast");
            string content = await response.Content.ReadAsStringAsync();

            weatherTextBlock.Text = JToken.Parse(content).ToString(Formatting.Indented); ;
        }
    }
}
