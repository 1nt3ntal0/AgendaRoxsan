using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Agenda.Models
{
    public class Citas
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Servicio { get; set; }
        public DateTime Dia { get; set; }
        public TimeSpan Hora { get; set; }
        public bool Completada { get; set; }
    }
}
