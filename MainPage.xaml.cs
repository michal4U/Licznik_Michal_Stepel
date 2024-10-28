using System.IO;
using System.Text.Json;
using Microsoft.Maui.Storage;

namespace Counter
{
    public partial class MainPage : ContentPage
    {
        private const string FileName = "counters.txt"; 

        public MainPage()
        {
            InitializeComponent();
            LoadCounters(); 
        }

        //dodawanie licznika
        private void OnAddCounterClicked(object sender, EventArgs e)
        {
            var counterName = CounterNameEntry.Text;

            if (string.IsNullOrWhiteSpace(counterName))
            {
                DisplayAlert("Błąd", "Proszę podać nazwę licznika", "OK");
                return;
            }

            AddCounter(counterName, 0);
            CounterNameEntry.Text = string.Empty; 
        }
        //tworzenie liucznika
        private void AddCounter(string counterName, int initialValue)
        {
            var counterLayout = new StackLayout { Orientation = StackOrientation.Horizontal };

            var nameLabel = new Label
            {
                Text = counterName,
                VerticalOptions = LayoutOptions.Center
            };

            var valueLabel = new Label
            {
                Text = initialValue.ToString(),
                VerticalOptions = LayoutOptions.Center,
                HorizontalOptions = LayoutOptions.CenterAndExpand
            };

            var minusButton = new Button
            {
                Text = "-",
                Command = new Command(() =>
                {
                    var currentValue = int.Parse(valueLabel.Text);
                    valueLabel.Text = (currentValue - 1).ToString();
                    SaveCounters(); 
                })
            };

            var plusButton = new Button
            {
                Text = "+",
                Command = new Command(() =>
                {
                    var currentValue = int.Parse(valueLabel.Text);
                    valueLabel.Text = (currentValue + 1).ToString();
                    SaveCounters();
                })
            };

            counterLayout.Children.Add(nameLabel);
            counterLayout.Children.Add(minusButton);
            counterLayout.Children.Add(valueLabel);
            counterLayout.Children.Add(plusButton);

            CountersLayout.Children.Add(counterLayout);
            SaveCounters(); 
        }

       //zapisywanie
        private async void SaveCounters()
        {
            var counters = new List<CounterData>();

            foreach (var child in CountersLayout.Children)
            {
                if (child is StackLayout counterLayout && counterLayout.Children[0] is Label nameLabel && counterLayout.Children[2] is Label valueLabel)
                {
                    var counterData = new CounterData
                    {
                        Name = nameLabel.Text,
                        Value = int.Parse(valueLabel.Text)
                    };
                    counters.Add(counterData);
                }
            }

            var json = JsonSerializer.Serialize(counters);
            var filePath = Path.Combine(FileSystem.AppDataDirectory, FileName);

            await File.WriteAllTextAsync(filePath, json); 
        }

       //ladowanie licznika
        private async void LoadCounters()
        {
            var filePath = Path.Combine(FileSystem.AppDataDirectory, FileName);

            if (File.Exists(filePath))
            {
                var json = await File.ReadAllTextAsync(filePath); 
                if (!string.IsNullOrEmpty(json))
                {
                    var counters = JsonSerializer.Deserialize<List<CounterData>>(json);
                    foreach (var counter in counters)
                    {
                        AddCounter(counter.Name, counter.Value);
                    }
                }
            }
        }
    }
    //przechowywanie danych
    public class CounterData
    {
        public string Name { get; set; }
        public int Value { get; set; }
    }
}
