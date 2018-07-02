using DAL.IService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Service
{
    public class ParticipantService : IParticipantServices
    {
        readonly EchoDbEntities entities = new EchoDbEntities();
        public void AddParticipant(Participant participant)
        {
            entities.Participants.Add(participant);
            //participant.
            entities.SaveChanges();
        }

        public bool UpdateResponse(ParticipantAnswer response)
        {

            //the code below is in regards to the session id thingy in the controller
            //var e = entities.Participants.Where(x => x.phoneNumber == response.phoneNumber).FirstOrDefault(x => x.session_id == response.Participant.session_id);
            var e = entities.Participants.FirstOrDefault(x => x.phoneNumber == response.phoneNumber);
            if (e != null)
            {
                //e.answer = response.answer;
                //entities.Entry(e).CurrentValues.SetValues(response);
                //e.answer = e.answer == null || e.answer.Equals("") ? e.answer + response.answer : e.answer + "," + response.answer;
                var ans = entities.ParticipantAnswers.FirstOrDefault(m => m.participant_id == e.participant_id);
                //e.ParticipantAnswer.answer = e.ParticipantAnswer.answer == null || e.ParticipantAnswer.answer.Equals("") ? e.ParticipantAnswer.answer + response.answer : e.ParticipantAnswer.answer + "," + response.answer;
                if (ans != null)
                {
                    ans.answer = ans.answer == null || ans.answer.Equals("") ? ans.answer + response.answer : ans.answer + "," + response.answer;
                    ans.Date = DateTime.Now;
                    entities.Entry(ans).State = System.Data.Entity.EntityState.Modified;
                }
                else {
                    ans = response;
                    ans.participant_id = e.participant_id;
                    ans.Date = DateTime.Now;
                    entities.ParticipantAnswers.Add(ans);
                }
                entities.SaveChanges();
                return true;
            }
            else
            {
                return false;
            }
           
        }
    }
}
