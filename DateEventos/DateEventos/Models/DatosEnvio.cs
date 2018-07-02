using System;
using System.Collections.Generic;
using System.Text;

namespace DatePickerService.Models
{
    public class DatosEnvio
    {
        public string Usuario { get; set; }
        public int IdUsuario { get; set; }
        public string Nombre { get; set; }
        public int idusuario { get; set; }
    }

    public class RootObject
    {
        public string bandera { get; set; }
        public string mensaje { get; set; }
        public string eventoId { get; set; }

        public DatosEnvio DatosEnvio { get; set; }
    }
}
