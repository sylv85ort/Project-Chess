//using System.Data.SqlClient;
//using System.Net;
//using Microsoft.AspNetCore.Http;
//using Microsoft.AspNetCore.Mvc;

//namespace ChessBackend.Controllers


//{
//    [Route("api/user")]
//    [ApiController]
//    public class UserController : ControllerBase
//    {
//        Users = [
//            {"userID": 1, "username": "RichMinion", "password": "Banana123"},
//            {"userID": 2, "username": "PurpleOverlord", "password": "GruRules"}
//        ]

//        [HttpPost, Route("signup")]
//        public HttpResponseMessage SignUp([FromBody] Users users)
//        {

//            try 
//            {
//                return null;
//            }
//            catch (Exception e)
//            {
//                return Request.CreateResponse(HttpStatusCode.InternalServerError, e);
//            }
//        }
//    }
//}
