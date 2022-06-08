using AdaptiveCards;
using Chat.Helpers;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Schema;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Chat.Common
{
    public class FactoryAdaptivecard
    {
        public static Attachment CreateSingleChoiceCard(string filename, List<AdaptiveChoice> listChoices)
        {
            var cardJson = PrepareCard.ReadCard(filename);
            var card = AdaptiveCard.FromJson(cardJson).Card;

            var choices = card.Body[1] as AdaptiveChoiceSetInput;
            choices.Choices = listChoices;

            var attachment = new Attachment
            {
                ContentType = AdaptiveCard.ContentType,
                Content = JsonConvert.DeserializeObject(card.ToJson())
            };

            return attachment;
        }

        public static Attachment CreateDateCard(string filename)
        {
            // Obtenemos la fecha actual y obtenemos la fecha dentro de 2 meses
            var currentDate = DateTime.Now.Date;
            var maxDate = currentDate.AddDays(60);

            var cardJson = PrepareCard.ReadCard(filename);
            var card = AdaptiveCard.FromJson(cardJson).Card;
            var optionsDate = card.Body[1] as AdaptiveDateInput;
            
            // Asignamos las fechas mínimas y máximas para el calendario
            optionsDate.Min = currentDate.ToString("yyyy-MM-dd");
            optionsDate.Max = maxDate.ToString("yyyy-MM-dd");

            var attachment = new Attachment
            {
                ContentType = AdaptiveCard.ContentType,
                Content = JsonConvert.DeserializeObject(card.ToJson())
            };

            return attachment;
        }
    }
}
