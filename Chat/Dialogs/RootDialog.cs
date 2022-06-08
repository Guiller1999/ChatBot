using Chat.Common;
using Chat.Helpers;
using Chat.Models;
using Microsoft.Bot.Builder.Dialogs;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AdaptiveCards;
using Microsoft.Bot.Schema;
using Microsoft.Bot.Builder;
using Newtonsoft.Json.Linq;
using System.Globalization;

namespace Chat.Dialogs
{
    public class RootDialog : ComponentDialog
    {
        private Cita cita;
        private List<Paciente> pacientes = new List<Paciente>();
        private List<Especilidades> especilidades = new List<Especilidades>();
        private List<Odontologo> odontologos = new List<Odontologo>();
        private List<HorasDisponibles> horas = new List<HorasDisponibles>();

        public RootDialog(UserState userState)
            : base(nameof(RootDialog))
        {
            cita = new Cita();

            /* Para Pruebas */
            // Se crea 3 objetos de tipo Paciente
            Paciente paciente1 = new Paciente { Id = 1, Name = "Guillermo Rivera" };
            Paciente paciente2 = new Paciente { Id = 2, Name = "José Rivera" };
            Paciente paciente3 = new Paciente { Id = 3, Name = "Ana García" };

            // Los objetos los colocamos en una lista

            pacientes.Add(paciente1);
            pacientes.Add(paciente2);
            pacientes.Add(paciente3);

            // Se crea 3 objetos de tipo Especialidades
            Especilidades especialidad1 = new Especilidades { Id = 1, Description = "Limpieza Dental" };
            Especilidades especialidad2 = new Especilidades { Id = 2, Description = "Ortodoncia" };
            Especilidades especialidad3 = new Especilidades { Id = 3, Description = "Brackets" };

            // Los objetos los colocamos en una lista
            especilidades.Add(especialidad1);
            especilidades.Add(especialidad2);
            especilidades.Add(especialidad3);

            // Se crea 3 objetos de tipo Odontologo
            Odontologo odontologo1 = new Odontologo { Id = 1, Name = "Karina Ronquillo" };
            Odontologo odontologo2 = new Odontologo { Id = 2, Name = "Verónica Zambrano" };
            Odontologo odontologo3 = new Odontologo { Id = 3, Name = "Cristopher Vera" };

            odontologos.Add(odontologo1);
            odontologos.Add(odontologo2);
            odontologos.Add(odontologo3);

            HorasDisponibles hora1 = new HorasDisponibles { Id = 1, Hora = "8:30" };
            HorasDisponibles hora2 = new HorasDisponibles { Id = 2, Hora = "17:30" };
            HorasDisponibles hora3 = new HorasDisponibles { Id = 3, Hora = "12:20" };

            horas.Add(hora1);
            horas.Add(hora2);
            horas.Add(hora3);

            var waterfallSteps = new WaterfallStep[]
            {
                ShowPatientOptions,
                ShowServicesOptions,
                ShowDoctorsOptions,
                ShowDateForm,
                ShowSchedules,
                ShowAppointmentData
            };

            AddDialog(new WaterfallDialog(nameof(WaterfallDialog), waterfallSteps));
            AddDialog(new AdaptiveCardPrompt("AdaptiveCardId", ValidateForm));
        }

        private Task<bool> ValidateSelectSchedule(PromptValidatorContext<string> promptContext, CancellationToken cancellationToken)
        {
            var result = promptContext.Context.Activity.Value;

            return result != null ? Task.FromResult(true) : Task.FromResult(false);
        }

        private Task<bool> ValidateForm(PromptValidatorContext<JObject> promptContext, CancellationToken cancellationToken)
        {
            var result = promptContext.Context.Activity.Value;

            return result != null ? Task.FromResult(true) : Task.FromResult(false);
        }

        private async Task<DialogTurnResult> ShowPatientOptions(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var choiceList = pacientes.Select(paciente => new AdaptiveChoice { 
                Title = paciente.Name, Value = paciente.Id.ToString()}).ToList();

            var attachment = FactoryAdaptivecard.CreateSingleChoiceCard("CardChoicePatient.json", choiceList);

            var opts = new PromptOptions
            {
                Prompt = new Activity
                {
                    Attachments = new List<Attachment>() { attachment },
                    Type = ActivityTypes.Message,

                },
                RetryPrompt = MessageFactory.Text("Paciente no válido. Escoja un paciente")
            };

            return await stepContext.PromptAsync("AdaptiveCardId", opts, cancellationToken);
        }

        private async Task<DialogTurnResult> ShowServicesOptions(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            // Recuperamos la selección del usuario
            var value = stepContext.Context.Activity.Value;
            JObject jObject = JObject.Parse(value.ToString());
            var id = (string)jObject["PatientId"];

            cita.PacienteId = Int32.Parse(id);

            var choiceList = especilidades.Select(especialidad => new AdaptiveChoice
            {
                Title = especialidad.Description,
                Value = especialidad.Id.ToString()
            }).ToList();

            var attachment = FactoryAdaptivecard.CreateSingleChoiceCard("CardChoiceService.json", choiceList);

            var opts = new PromptOptions
            {
                Prompt = new Activity
                {
                    Attachments = new List<Attachment>() { attachment },
                    Type = ActivityTypes.Message,

                },
                RetryPrompt = MessageFactory.Text("Especialidad no válida. Escoja una especialidad")
            };

            return await stepContext.PromptAsync("AdaptiveCardId", opts, cancellationToken);
        }

        private async Task<DialogTurnResult> ShowDoctorsOptions(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            // Recuperamos la selección del usuario
            var value = stepContext.Context.Activity.Value;
            JObject jObject = JObject.Parse(value.ToString());
            var id = (string)jObject["ServiceId"];

            cita.EspecialidadId = Int32.Parse(id);

            var choiceList = odontologos.Select(odontologo => new AdaptiveChoice
            {
                Title = odontologo.Name,
                Value = odontologo.Id.ToString()
            }).ToList();

            var attachment = FactoryAdaptivecard.CreateSingleChoiceCard("CardChoiceDoctor.json", choiceList);

            var opts = new PromptOptions
            {
                Prompt = new Activity
                {
                    Attachments = new List<Attachment>() { attachment },
                    Type = ActivityTypes.Message,

                },
                RetryPrompt = MessageFactory.Text("Odontólogo no válido. Escoja un odontólogo")
            };

            return await stepContext.PromptAsync("AdaptiveCardId", opts, cancellationToken);
        }

        private async Task<DialogTurnResult> ShowDateForm(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            // Recuperamos la selección del usuario
            var value = stepContext.Context.Activity.Value;
            JObject jObject = JObject.Parse(value.ToString());
            var id = (string)jObject["DoctorId"];

            cita.DoctorId = Int32.Parse(id);

            var attachment = FactoryAdaptivecard.CreateDateCard("DateAppoinmentCard.json");

            var opts = new PromptOptions
            {
                Prompt = new Activity
                {
                    Attachments = new List<Attachment>() { attachment },
                    Type = ActivityTypes.Message,

                },
                RetryPrompt = MessageFactory.Text("Fecha no válida. Escoja una fecha")
            };

            return await stepContext.PromptAsync("AdaptiveCardId", opts, cancellationToken);
        }

        private async Task<DialogTurnResult> ShowSchedules(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            // Recuperamos la selección del usuario
            var value = stepContext.Context.Activity.Value;
            JObject jObject = JObject.Parse(value.ToString());
            var date = (string)jObject["Date"];

            // Formateamos fecha
            date = date.Replace("-", "/");
            cita.Fecha = DateTime.Parse(date);

            var activity = HeroCardDialog.CreateSchedulesCarousel(horas);

            await stepContext.Context.SendActivityAsync("Seleccione la hora para su cita" );
            return await stepContext.PromptAsync(
                "AdaptiveCardId",
                new PromptOptions
                {
                    Prompt = activity,
                    RetryPrompt = MessageFactory.Text("Error horario no válido. Escoja un horario")
                },
                cancellationToken
            );
        }

        private async Task<DialogTurnResult> ShowAppointmentData(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            // Obtenemos y formateamos la hora
            var schedule = stepContext.Context.Activity.Value;
            var hour = TimeSpan.Parse(schedule.ToString());

            // Agregamos la hora en la fecha
            var newDate = cita.Fecha.Add(hour);
            cita.Fecha = newDate;

            var date = String.Format("{0:dd/MM/yyyy HH:mm tt}", cita.Fecha);

            var patient = pacientes.Where(paciente => paciente.Id == cita.PacienteId).FirstOrDefault();
            var service = especilidades.Where(especialidad => especialidad.Id == cita.EspecialidadId).FirstOrDefault();
            var doctor = odontologos.Where(odontologos => odontologos.Id == cita.DoctorId).FirstOrDefault();
            
            await stepContext.Context.SendActivityAsync(
                $"CITA AGENDADA\n\n" +
                $"Paciente: {patient.Name}\n\n" +
                $"Especialidad: {service.Description}\n\n" +
                $"Odontólogo: {doctor.Name}\n\n " +
                $"Fecha y Hora: {date}",
                cancellationToken: cancellationToken);

            await stepContext.Context.SendActivityAsync("Gracias por usar nuestro servicio\n\n" +
                "Si quiere agendar otra cita, escriba algo para empezar de nuevo el proceso", cancellationToken: cancellationToken);
            return await stepContext.EndDialogAsync(cancellationToken: cancellationToken);
        }
    }
}
