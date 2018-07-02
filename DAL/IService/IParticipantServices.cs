using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.IService
{
   public interface IParticipantServices
    {
        void AddParticipant(Participant participant);
        bool UpdateResponse(ParticipantAnswer response);
    }
}
