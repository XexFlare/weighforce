using System;
using System.IO.Ports;
using System.Text;
using System.Reactive.Subjects;
using System.Threading.Tasks;
using WeighForce.Data;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Hosting;

namespace WeighForce.Services
{
    public class SerialPortInterface
    {
        private SerialPort _serialPort = new SerialPort();
        private string _serialType;
        public int weight = 0;
        private bool _isOpen = false;
        private readonly Subject<ScaleWeight> _weight = new Subject<ScaleWeight>();
        private string tString = string.Empty;
        private string _tempWeight = string.Empty;
        private bool _startString = false;
        private bool _updated = false;
        private bool _parsed = false;
        private int weightStringLength = 6;
        private bool logging = false;
        private bool _isDevelopment = false;
        public SerialPortInterface(IConfiguration configuration, IWebHostEnvironment env)
        {
            _serialType = configuration.GetValue<string>("SerialPort:Type", "LA2");
            _isDevelopment = env.IsDevelopment();
            _serialPort.PortName = configuration.GetValue<string>("SerialPort:Port", "/dev/cu.usbserial-1410");
            _serialPort.BaudRate = configuration.GetValue<int>("SerialPort:Baud", 4800);
            switch (configuration.GetValue<int>("SerialPort:StopBit", 1))
            {
                case 0:
                    _serialPort.StopBits = StopBits.None;
                    break;
                case 1:
                    _serialPort.StopBits = StopBits.One;
                    break;
                case 2:
                    _serialPort.StopBits = StopBits.Two;
                    break;
                case 3:
                    _serialPort.StopBits = StopBits.OnePointFive;
                    break;
                default:
                    _serialPort.StopBits = StopBits.One;
                    break;
            }
            switch (configuration.GetValue<int>("SerialPort:HandShake", 1))
            {
                case 0:
                    _serialPort.Handshake = Handshake.None;
                    break;
                case 1:
                    _serialPort.Handshake = Handshake.XOnXOff;
                    break;
                case 2:
                    _serialPort.Handshake = Handshake.RequestToSend;
                    break;
                case 3:
                    _serialPort.Handshake = Handshake.RequestToSendXOnXOff;
                    break;
                default:
                    _serialPort.Handshake = Handshake.XOnXOff;
                    break;
            }
            _serialPort.ReadTimeout = 500;
            _serialPort.WriteTimeout = 500;
            _serialPort.DataReceived += new SerialDataReceivedEventHandler(this.serialPort_DataReceived);
            updateWeight("0");
            // Test();
            // _serialPort.DataBits = SerialPortInterface.SetPortDataBits(_serialPort.DataBits);
            // _serialPort.Parity = SerialPortInterface.SetPortParity(_serialPort.Parity);
            // _serialPort.StopBits = SerialPortInterface.SetPortStopBits(_serialPort.StopBits);
            // _serialPort.Handshake = SerialPortInterface.SetPortHandshake(_serialPort.Handshake);
        }

        public bool isOpen => _serialPort.IsOpen;

        public async void randomize()
        {
            var rand = new Random();
            int weight = updateWeight(rand.Next(3000, 15000).ToString());
            await Task.Delay(1000);
            if (_isOpen) randomize();
        }

        public bool Open()
        {
            try
            {
                _isOpen = true;
                _serialPort.Open();
                Console.WriteLine("*** Is Port Open: {0}", _isOpen);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                GetPortNames();
                if (_isDevelopment) randomize();
                return false;
            }
        }
        public void Close()
        {
            _isOpen = false;
            _serialPort.Close();
            Console.WriteLine("*** Is Port Open: {0}", _isOpen);
        }

        public void serialPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            byte[] buffer = new byte[_serialPort.ReadBufferSize];
            int bytesRead = _serialPort.Read(buffer, 0, buffer.Length);
            tString = Encoding.ASCII.GetString(buffer, 0, bytesRead);
            // if (logging) Console.WriteLine("str: {0} ", tString);
            foreach (char c in tString)
            {
                decodeString(c.ToString());
            }
        }
        public void Test(){
            System.Console.WriteLine("Testing");
            var tString = "$        0         0 kg 9201$        0         0 kg 9201$        0         0 kg 9201$        0         0 kg 9201$        0         0 kg 9201$        0         0 kg 9201$        0         0 kg 9201$        0         0 kg 9201$        0         0 kg 9201$        0         0 kg 9201$        0         0 kg 9201$        0         0 kg 9201$        0         0 kg 9201";
            foreach (char c in tString)
            {
                decodeString(c.ToString());
            }
        }

        private void decodeString(String ch)
        {
            switch (_serialType)
            {
                case "LA2":
                    LA2(ch);
                    break;
                case "D2008":
                    D2008(ch);
                    break;
                case "D400":
                    D400(ch);
                    break;
                default:
                    LA2(ch);
                    break;
            }
        }

        private void D2008(String ch)
        {
            if (_tempWeight == String.Empty && ch == "+")
            {
                if (logging) Console.WriteLine("Start Here {0}", ch);
                _startString = true;
                return;
            }
            int number;

            if (!Int32.TryParse(ch, out number))
            {
                if (logging) Console.WriteLine("invalid character: {0}", ch);
                _tempWeight = String.Empty;
                return;
            }
            if (_startString)
            {
                if (logging) Console.WriteLine("Plus: {0}", ch);
                _tempWeight = String.Empty;
                _tempWeight += ch;
                _startString = false;
                return;
            }
            if (_tempWeight != String.Empty && _tempWeight.Length < weightStringLength)
            {
                if (logging) Console.WriteLine("Add Digit: {0}", ch);
                _tempWeight += ch;
                return;
            }
            if (_tempWeight.Length == weightStringLength)
            {
                if (logging) Console.WriteLine("Max Len {0}", ch);
                updateWeight(_tempWeight);
                _tempWeight = String.Empty;
                return;
            }
            if(!_startString && ch == " ") {
                if (logging) Console.WriteLine("Plus: {0}", ch);
                _parsed = true;
                return;
            }
            if (logging) Console.WriteLine("Left Overs: {0}", ch);
        }
        private void D400(String ch){
            if (_tempWeight == String.Empty && ch == "$")
            {
                if (logging) Console.WriteLine("Start Here {0}", ch);
                _startString = true;
                _parsed = false;
                _updated = false;
                return;
            }
            int number;
            if (!Int32.TryParse(ch, out number) && !_startString)
            {
                if (logging) Console.WriteLine("adc: {0}", ch);
                if(!_parsed && !_updated)
                _parsed = true;
                return;
            }
            if (!Int32.TryParse(ch, out number))
            {
                if (logging) Console.WriteLine("invalid character: {0}", ch);
                _tempWeight = String.Empty;
                return;
            }
            if (!_parsed && _startString)
            {
                if (logging) Console.WriteLine("Plus: {0}", ch);
                _tempWeight += ch;
                _startString = false;
                return;
            }
            if (!_parsed && _tempWeight != String.Empty && _tempWeight.Length < weightStringLength)
            {
                if (logging) Console.WriteLine("Add Digit: {0}", ch);
                _tempWeight += ch;
                return;
            }
            if (_parsed) // _tempWeight.Length == weightStringLength)
            {
                if (logging) Console.WriteLine("Got String {0}....", _tempWeight);
                updateWeight(_tempWeight);
                _parsed = false;
                _updated = true;
                _tempWeight = String.Empty;
                return;
            }
            if (logging) Console.WriteLine("Left Overs: {0}", ch);
        }
        private void LA2(String ch)
        {
            if (_tempWeight == String.Empty && ch == "+")
            {
                if (logging) Console.WriteLine("Start Here {0}", ch);
                _startString = true;
                return;
            }
            int number;

            if (!Int32.TryParse(ch, out number))
            {
                if (logging) Console.WriteLine("invalid character: {0}", ch);
                _tempWeight = String.Empty;
                return;
            }
            if (_startString)
            {
                if (logging) Console.WriteLine("Plus: {0}", ch);
                _tempWeight = String.Empty;
                _tempWeight += ch;
                _startString = false;
                return;
            }
            if (_tempWeight != String.Empty && _tempWeight.Length < weightStringLength)
            {
                if (logging) Console.WriteLine("Add Digit: {0}", ch);
                _tempWeight += ch;
                return;
            }
            if (_tempWeight.Length == weightStringLength)
            {
                if (logging) Console.WriteLine("Max Len {0}", ch);
                updateWeight(_tempWeight);
                _tempWeight = String.Empty;
                return;
            }
            if (logging) Console.WriteLine("Left Overs: {0}", ch);
        }

        private int updateWeight(String weightString)
        {
            int _tWeight = int.Parse(weightString);
            weight = _tWeight;
            _weight.OnNext(new ScaleWeight
            {
                Amount = weight,
            });
            if (logging) Console.WriteLine("weightString: {0} - weight: {1} || ", weightString, weight);
            return weight;
        }

        public IObservable<ScaleWeight> StreamWeight()
        {
            return _weight;
        }

        public void GetPortNames()
        {
            if (SerialPort.GetPortNames().Length == 0)
                Console.WriteLine("No Serial Ports available");
            else
            {
                Console.WriteLine("Available Ports:");
                foreach (string s in SerialPort.GetPortNames())
                {
                    Console.WriteLine("{0}", s);
                }
            }
        }
    }
}