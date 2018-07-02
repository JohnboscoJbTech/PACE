using DAL;
using DAL.IService;
using DAL.Service;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Web.Http;

namespace PACE.Controllers
    
    {
    [RoutePrefix("application/service")]
    public class PACEServiceController : ApiController
    {
        IParticipantServices _ParticipantServices;

        public PACEServiceController(IParticipantServices participantServices)
        {
            _ParticipantServices = participantServices;
        }
        [Route("paceservice")]//http(s)://<host>:port/application/services/paceservice
        [HttpPost, ActionName("paceservices")]

        public HttpResponseMessage httpResponseMessage([FromBody] PaceResponse paceResponse)
        {
            HttpResponseMessage responseMessage;
            string response;

            var participant = new Participant();
            var participantanswer = new ParticipantAnswer();
            var answer = new Answer();
            var resquest = Request.Content.ReadAsStringAsync();
            if (paceResponse.text == null)
            {
                paceResponse.text = "Successfull!";
            }

            if (paceResponse.text.Equals("", StringComparison.Ordinal))
            {
                response = "CON Welcome to ECHO Survey\n";
                response += "1.Register\n";
                response += "2.Answer Question\n";
            }

            else if (paceResponse.text.Equals("1", StringComparison.Ordinal))
            {
                response = $"CON Enter your \n NAME, FACILITY : {paceResponse.ParticipantName}, { paceResponse.facility} \n";
                //String vresponse = ussdResponse.text;
            }

            else if (paceResponse.text.StartsWith("1*"))
            {
                var details = paceResponse.text.Split('*')[1].Split(',');                               

                participant.participant_id = Guid.NewGuid();
                participant.session_id = Guid.NewGuid().ToString();//paceResponse.session_id
                participant.ParticipantName = details[0];
                participant.facility = details[1];
                participant.phoneNumber = paceResponse.phoneNumber;
                participantanswer.answer = paceResponse.answer;
                participantanswer.Date = DateTime.Now;
                answer.correctAnswer = paceResponse.correctAnswer;
                _ParticipantServices.AddParticipant(participant);
                response = "Successful!";
            }
            else if (paceResponse.text.Equals("2", StringComparison.Ordinal))
            {
                response = "CON 1.Nigeria is in:\nA. West Africa\nB. East Africa\nC. North Africa\nD. South Africa\nE. All of the above"; ;
            }

            else if (paceResponse.text.StartsWith("2*", StringComparison.Ordinal))
            { 
                string[] array = new string[4];
                array[0] = "CON 2.My name is:\nA.Ibrahim\nB.Bola\nC.Chidi\nD.Dooshima\nE.None of the Above";
                array[1] = "CON 3.My age is within the age ranges:\nA. 21-30 Years\nB. 31-40 Years\nC. 41-50 Years\nD. 51-60 Years";
                array[2] = "CON 4.I am:\nA. Single\nB. Married\nC. Divorced\nD. Widow/Widower\nE. None of the above";
                array[3] = "CON 5.I am:\nA. Christian\nB. Muslim\nC. Traditionalist\nD. None of the Above\nE. All of the above";

                String[] youranswer = paceResponse.text.Split('*');
                var index = youranswer.Length - 2;

                if (index <= array.Length)
                {
                    Regex r = new Regex(@"^[ABCDE]$", RegexOptions.IgnoreCase);
                    var c = youranswer[youranswer.Length - 1];
                    if (r.Match(c).Success)
                    {
                        response = index == array.Length ? "You have finshed the questions successfully" : array[index];
                        //participantanswer.answer = paceResponse.answer;
                        participantanswer.answer = c;
                        participantanswer.phoneNumber = paceResponse.phoneNumber;
                            //the code below is for checking session id when the users phone number would appear twice
                        //participantanswer.Participant.session_id = paceResponse.session_id;

                        //participant.ParticipantId = Guid.Parse("5D03D687-2639-4239-998F-03116303D8EF");
                        //participant.SessionId = Guid.Parse("D6D75B72ED7F4DC78D654B6754B76944");

                        bool status = _ParticipantServices.UpdateResponse(participantanswer);
                        if (!status)
                        {
                             response="You need to Regiter";
                        }
                    }
                    else
                    {
                        response = "CON You entered an inValid Option";
                    }
                }
                else
                {
                    response = "END The End";
                }
            }
            else
            {
                response = "CON Invalid Option\n";
            }
            responseMessage = Request.CreateResponse(HttpStatusCode.Created, response);
            responseMessage.Content = new StringContent(response, Encoding.UTF8, "text/plain");
            return responseMessage;
        }

        //public int updateResponse(string phoneNumber, string number_of_results_received)
        //{
        //   ParticipantService lnk = IParticipantServices(phoneNumber);
        //    if (lnk == null)
        //    {
        //        lnk = new lnk_return_of_results();
        //        lnk.AddParticipant = number_of_results_received;
        //        lnk.phone_number = phoneNumber;
        //        return _returnOfResultsService.Add(lnk);
        //    }
        //    else
        //        return 1;
        //}

    }
}


//todo
//1. Ask users questions and get answers
//2. Get the answers from the user
//  a. get answer and store in db
//  b. get answer and update db (get the previous answer from db, concatenate both answers, update db)
//3. When user is done? 


