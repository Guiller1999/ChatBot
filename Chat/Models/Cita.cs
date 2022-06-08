using System;

namespace Chat.Models
{
    public class Cita
    {
        public int Id { get; set; }
        public int PacienteId { get; set; }
        public int DoctorId { get; set; }
        public int EspecialidadId { get; set; }
        public DateTime Fecha { get; set; }
    }
}
