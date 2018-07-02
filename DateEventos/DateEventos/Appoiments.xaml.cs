using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace DateEventos
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Appoiments : ContentPage
    {
        public Appoiments()
        {
            InitializeComponent();
        }

        private void NewAppoiment()
        {

            Navigation.PushAsync(new DatePickerPage());
        }
        private void VerCitas()
        {
            Navigation.PushAsync(new VerCitasPage());
        }

        private void InvitadoCitas()
        {
            Navigation.PushAsync(new InvitPage());
        }

        private void ExpCitas()
        {
            Navigation.PushAsync(new CitasExp());
        }

       
    }
}

