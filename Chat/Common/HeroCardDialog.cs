using Chat.Models;
using System.Linq;
using Microsoft.Bot.Schema;
using System.Collections.Generic;
using Microsoft.Bot.Builder;

namespace Chat.Common
{
    public class HeroCardDialog
    {
        public static Activity CreateSchedulesCarousel(List<HorasDisponibles> schedules)
        {
            var listHeroCard = schedules.Select(schedule => new HeroCard { 
                Title = schedule.Hora,
                Buttons = new List<CardAction>
                {
                    new CardAction
                    {
                        Title = "Seleccionar",
                        Text = schedule.Id.ToString(),
                        Value = schedule.Hora,
                        Type = ActionTypes.MessageBack
                    }
                }
            }).ToList();

            var optionAttachment = new List<Attachment>();

            listHeroCard.ForEach(heroCard => optionAttachment.Add(heroCard.ToAttachment()));

            var reply = MessageFactory.Attachment(optionAttachment);
            reply.AttachmentLayout = AttachmentLayoutTypes.Carousel;

            return reply as Activity;
        }
    }
}
