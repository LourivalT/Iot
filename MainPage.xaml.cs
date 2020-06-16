using System;
using System.Threading.Tasks;
using Windows.Devices.Gpio;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;


// O modelo de item de Página em Branco está documentado em https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x416

namespace Controll_Vest
{
    /// <summary>
    /// Uma página vazia que pode ser usada isoladamente ou navegada dentro de um Quadro.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        /*
         * User: Lourival Tavares
         * Date: 18/05/2020
         * Time: 07:41
         */

        //Sensores
        private const int PB_PIN01 = 21;
        private const int PB_PIN02 = 13;
        private const int PB_PIN03 = 19;
        private const int PB_PIN04 = 26;
        private GpioPin sensor01;
        private GpioPin sensor02;
        private GpioPin sensor03;
        private GpioPin sensor04;
        private GpioPinValue entradaFrente01;
        private GpioPinValue entradaFrente02;
        private GpioPinValue entradaTraseira01;
        private GpioPinValue entradaTraseira02;
        //sirene
        private GpioPin sirene;
        private GpioPinValue sireneOut;
        private const int PB_PIN05 = 4;
        //timers
        private DispatcherTimer timerHora;
        private DispatcherTimer timerPorta01;
        private DispatcherTimer timerS1IN;
        private DispatcherTimer timerS1OUT;
        private DispatcherTimer timerPorta02;
        private DispatcherTimer timerS2IN;
        private DispatcherTimer timerS2OUT;
        //Contador
        public int count;
        public MainPage()
        {
            this.InitializeComponent();

            timerHora = new DispatcherTimer() { Interval = new TimeSpan(0, 0, 0,0,100) };
            timerHora.Tick += hora_Tick;
            timerHora.Start();
            ///-------------------porta01------------------------
            timerPorta01 = new DispatcherTimer();
            timerPorta01.Interval = new TimeSpan(0, 0, 0, 0, 100);
            timerPorta01.Tick += porta01Start_Tick;
            timerPorta01.Start();
            timerS1IN = new DispatcherTimer();
            timerS1IN.Interval = new TimeSpan(0, 0, 0, 0, 100);
            timerS1IN.Tick += S1IN_Tick;
            timerS1OUT = new DispatcherTimer();
            timerS1OUT.Interval = new TimeSpan(0, 0, 0, 0, 100);
            timerS1OUT.Tick += S1OUT_Tick;
            ///-------------------porta02------------------------
            timerPorta02 = new DispatcherTimer();
            timerPorta02.Interval = new TimeSpan(0, 0, 0, 0, 100);
            timerPorta02.Tick += porta02tart_Tick;
            timerPorta02.Start();
            timerS2IN = new DispatcherTimer();
            timerS2IN.Interval = new TimeSpan(0, 0, 0, 0, 100);
            timerS2IN.Tick += S2IN_Tick;
            timerS2OUT = new DispatcherTimer();
            timerS2OUT.Interval = new TimeSpan(0, 0, 0, 0, 100);
            timerS2OUT.Tick += S2OUT_Tick;


            InitGPIO();
        }
        private void hora_Tick(object sender, object e)
        {

            
            lblSuperior.Text = "NÚMERO DE PESSOAS NO VESTIÁRIO.";
            lblcont.Text = Convert.ToString ( count );
            if (count < 0) {

                count = 0;
            }
            if (count < 25)
            {
                acessoOk();
                Desliga_Sirene();

            }
            if (count == 25)
            {

                acessoNok();
                Desliga_Sirene();
            }
            if (count >= 26)
            {


                acessolimite();
                Liga_Sirene();

            }




        }
        private void Grid_Loaded(object sender, RoutedEventArgs e)
        {
           
        }
        private void InitGPIO()
        {
            var gpio = GpioController.GetDefault();

            if (gpio == null)
            {

                return;
            }
            sensor01 = gpio.OpenPin(PB_PIN01); // pin 3
            sensor02 = gpio.OpenPin(PB_PIN02); // pin 3
            sensor03 = gpio.OpenPin(PB_PIN03); // pin 3
            sensor04 = gpio.OpenPin(PB_PIN04); // pin 3
            sirene   = gpio.OpenPin(PB_PIN05);// pin 3

        }
        private void porta01Start_Tick(object sender, object e)
        {

            entradaFrente01 = sensor01.Read();
            entradaFrente02 = sensor02.Read();

            if (entradaFrente01 == GpioPinValue.High)
            {
                timerPorta01.Stop();
                System.Threading.Thread.Sleep(500);
                timerS1IN.Start();
            }
            if (entradaFrente02 == GpioPinValue.High)
            {
                timerPorta01.Stop();
                System.Threading.Thread.Sleep(500);
                timerS1OUT.Start();
            }


        }
        private void S1IN_Tick(object sender, object e)
        {

            entradaFrente02 = sensor02.Read();
            if (entradaFrente02 == GpioPinValue.High)
            {
                count += 1;
                timerS1IN.Stop();
                System.Threading.Thread.Sleep(500);
                timerPorta01.Start();
                
            }

        }
        private void S1OUT_Tick(object sender, object e)
        {

            entradaFrente01 = sensor01.Read();
            if (entradaFrente01 == GpioPinValue.High)
            {
                count -= 1;
                timerS1OUT.Stop();
                System.Threading.Thread.Sleep(500);
                timerPorta01.Start();
                
            }

        }
        private void porta02tart_Tick(object sender, object e)
        {

            entradaTraseira01 = sensor03.Read(); //frente
            entradaTraseira02 = sensor04.Read(); // traseira

            if (entradaTraseira01 == GpioPinValue.High)
            {
                timerPorta02.Stop();
                System.Threading.Thread.Sleep(500);
                timerS2IN.Start();
            }
            if (entradaTraseira02 == GpioPinValue.High)
            {
                timerPorta02.Stop();
                System.Threading.Thread.Sleep(500);
                timerS2OUT.Start();

            }


        }
        private void S2IN_Tick(object sender, object e)
        {

            entradaTraseira02 = sensor04.Read();
            if (entradaTraseira02 == GpioPinValue.High)
            {
                count += 1;
                timerS2IN.Stop();
                System.Threading.Thread.Sleep(500);
                timerPorta02.Start();

            }

        }
        private void S2OUT_Tick(object sender, object e)
        {

            entradaTraseira01 = sensor03.Read();
            if ( entradaTraseira01 == GpioPinValue.High)
            {
                count -= 1;
                timerS2OUT.Stop();
                System.Threading.Thread.Sleep(500);
                timerPorta02.Start();

            }

        }
        //Comando da sirene
        private void Liga_Sirene()
        {

            sirene.SetDriveMode(GpioPinDriveMode.Output);
            sireneOut = GpioPinValue.Low;
            sirene.Write(sireneOut);

        }
        private void Desliga_Sirene()
        {

            sirene.SetDriveMode(GpioPinDriveMode.Output);
            sireneOut = GpioPinValue.High;
            sirene.Write(sireneOut);

        }
        //Contador
        public void  acessoOk()
        {
            lblSuperior.Text = " NÚMERO DE PESSOAS NO VESTIÁRIO.";
            retangulo.Fill = new SolidColorBrush(Windows.UI.Colors.Green);
            lblInferior.Text = "ACESSO LIBERADO!";
        }
        public void  acessoNok()
        {
            lblSuperior.Text = " O LIMITE DE 25 PESSOAS FOI ATINGIDO.";
            retangulo.Fill = new SolidColorBrush(Windows.UI.Colors.Red);
            lblInferior.Text = " NÃO ENTRE";
        }
        public void  acessolimite()
        {
            lblSuperior.Text = " O LIMITE DE 25 PESSOAS FOI EXECIDIDO.";
            retangulo.Fill = new SolidColorBrush(Windows.UI.Colors.Red);
            lblInferior.Text = "AGUARDE ALGUÉM SAIR!";
        }

    
    }
}
