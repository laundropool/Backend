using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;
using WasherDAL.Models;
using System.Linq;
using GeoCoordinatePortable;

namespace WasherDAL
{
    public class WasherRepository
    {
        WasherDBContext context;
        private readonly WasherDBContext _context;
        SqlConnection conObj = new SqlConnection();
        SqlCommand cmdObj;
        public WasherRepository()
        {
            context = new WasherDBContext();
            conObj.ConnectionString = "Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=WasherDB; Integrated Security=SSPI";
            cmdObj = new SqlCommand();
            _context = new WasherDBContext();
        }
        public static double GetDistance(double sLatitude, double sLongitude, double eLatitude, double eLongitude)
        {
            var sCoord = new GeoCoordinate(sLatitude, sLongitude);
            var eCoord = new GeoCoordinate(eLatitude, eLongitude);

            return sCoord.GetDistanceTo(eCoord);
        }
        public string UserSignUp(string userName, string userEmail, string userPassword, string userMobile, string lat, string lon, bool washing)
        {

            string result = "-1";
            try
            {
                SqlParameter prmUserName = new SqlParameter("@UserName", userName);
                SqlParameter prmUserEmail = new SqlParameter("@UserEmail", userEmail);
                SqlParameter prmUserPass = new SqlParameter("@UserPassword", userPassword);
                SqlParameter prmUserMob = new SqlParameter("@UserMobile", userMobile);
                SqlParameter prmLat = new SqlParameter("@lat", lat);
                SqlParameter prmLon = new SqlParameter("@lon", lon);
                SqlParameter prmWash = new SqlParameter("@Washing", washing);

                SqlParameter prmUserId = new SqlParameter("@UserId", System.Data.SqlDbType.VarChar, 20);
                prmUserId.Direction = System.Data.ParameterDirection.Output;

                context.Database.ExecuteSqlCommand("EXEC dbo.usp_SignUp @UserName,@UserEmail,@UserMobile,@lat,@lon,@UserPassword,@Washing, @UserId OUT", new[] { prmUserName, prmUserEmail, prmUserMob, prmLat, prmLon, prmUserPass, prmWash, prmUserId });

                result = Convert.ToString(prmUserId.Value);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                result = "-99";
            }
            return result;
        }

        public string UserLogin(string userEmail, string userPassword)
        {

            string returnValue;

            cmdObj = new SqlCommand(@"SELECT [dbo].ufn_Login(@UserEmail,@Password)", conObj);
            cmdObj.Parameters.AddWithValue("@UserEmail", userEmail);
            cmdObj.Parameters.AddWithValue("@Password", userPassword);
            try
            {
                conObj.Open();
                returnValue = Convert.ToString(cmdObj.ExecuteScalar());
            }
            catch (SqlException ex)
            {
                returnValue = "-1";
            }
            finally
            {
                conObj.Close();
            }
            return returnValue;
        }

        public Users GetUserInfo(string userId)
        {
            userId = userId.Replace(" ", string.Empty);
            try
            {
                var user = context.Users.Where(u => u.Userid == userId).FirstOrDefault();
                return user;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public string UpdateUserInfo(Users user)
        {
            return null;
        }

        //Raising a new request
        public int RaiseRequest(LaundryRequest laundryRequest)
        {
            int status = 0;
            try
            {
                laundryRequest.Status = "Inactive";
                _context.LaundryRequest.Add(laundryRequest);
                _context.SaveChanges();
                MatchRequests(laundryRequest);
                status = 1;
            }
            catch (Exception ex)
            {
                status = 0;
            }
            return status;
        }

        //Match requests
        //Match requests
        //Match requests
        public void MatchRequests(LaundryRequest laundryRequest)
        {
            List<LaundryRequest> laundryRequests = new List<LaundryRequest>();
            MatchedRequest matchedRequest;
            try
            {
                if (laundryRequest.WashingMachine == true)
                {
                    laundryRequests = (from lr in _context.LaundryRequest
                                       where lr.Status.ToLower() == "inactive"
                                       && lr.WashingMachine == false
                                       select lr).ToList();
                }
                else
                {
                    laundryRequests = (from lr in _context.LaundryRequest
                                       where lr.Status.ToLower() == "inactive"
                                       && lr.WashingMachine == true
                                       select lr).ToList();
                }

                if (laundryRequests.Any())
                {
                    foreach (var request in laundryRequests)
                    {
                        if (request.UserId == laundryRequest.UserId)
                            continue;
                        if (laundryRequest.WhitesOnly == request.WhitesOnly &&
                            laundryRequest.UnderGarmentsOnly == request.UnderGarmentsOnly &&
                            laundryRequest.GarmentsOnly == request.GarmentsOnly &&
                            laundryRequest.DenimsOrTrousersOnly == request.DenimsOrTrousersOnly)
                        {
                            matchedRequest = new MatchedRequest();
                            if (laundryRequest.WashingMachine == true)
                            {
                                matchedRequest.OwnerId = laundryRequest.UserId;
                                matchedRequest.WasherId = request.UserId;
                                matchedRequest.OwnerRequestId = laundryRequest.RequestId;
                                matchedRequest.WasherRequestId = request.RequestId;
                            }
                            else
                            {
                                matchedRequest.OwnerId = request.UserId;
                                matchedRequest.WasherId = laundryRequest.UserId;
                                matchedRequest.OwnerRequestId = request.RequestId;
                                matchedRequest.WasherRequestId = laundryRequest.RequestId;
                            }

                            matchedRequest.Status = "Inactive";
                            Users owner = (from lr in _context.Users
                                           where lr.Userid == matchedRequest.OwnerId
                                           select lr).FirstOrDefault();
                            Users washer = (from lr in _context.Users
                                            where lr.Userid == matchedRequest.WasherId
                                            select lr).FirstOrDefault();
                            matchedRequest.Distance = Convert.ToDecimal(GetDistance(Convert.ToDouble(owner.Latitude), Convert.ToDouble(owner.Longitude),
                                Convert.ToDouble(washer.Latitude), Convert.ToDouble(washer.Longitude)));
                            //laundryRequest.Status = "Active";
                            //request.Status = "Active";
                            _context.MatchedRequest.Add(matchedRequest);
                            _context.SaveChanges();
                        }
                    }
                }
            }
            catch (Exception ex)
            {

            }
        }

        //Lists all the matched request for given user id
        public List<MatchedRequest> ViewMatchedRequests(string userId)
        {
            List<MatchedRequest> matchedRequests = new List<MatchedRequest>();
            try
            {
                SqlParameter user = new SqlParameter("@UserId", userId);
                matchedRequests = _context.MatchedRequest.FromSql("Select * from ufn_ViewMatchedRequests(@UserId)", user).Where(x=>x.Status!="Pending").ToList();
            }
            catch (Exception e)
            {
                matchedRequests = null;
            }
            return matchedRequests;
        }
        public LaundryRequest GetUserLaundryInfo(int requestId)
        {
            LaundryRequest laundryRequest;
            try
            {
                laundryRequest = (from l in _context.LaundryRequest
                                  where l.RequestId == requestId
                                  select l).FirstOrDefault();
            }
            catch (Exception ex)
            {
                laundryRequest = null;
            }
            return laundryRequest;
        }

        //Send request
        public bool SendRequest(string senderUserId, string receiverUserId,
            int senderRequestId, int receiverRequestId, string userId)
        {
            bool status = false;
            try
            {
                MatchedRequest matchedRequest = new MatchedRequest();
                matchedRequest = (from mr in _context.MatchedRequest
                                  where (mr.OwnerId == senderUserId &&
                                  mr.WasherId == receiverUserId &&
                                  mr.OwnerRequestId == senderRequestId &&
                                  mr.WasherRequestId == receiverRequestId) ||
                                  (mr.WasherId == senderUserId &&
                                      mr.OwnerId == receiverUserId &&
                                      mr.WasherRequestId == senderRequestId &&
                                      mr.OwnerRequestId == receiverRequestId)
                                  select mr).FirstOrDefault();
                if (matchedRequest != null)
                {
                    matchedRequest.Status = "Pending";
                    matchedRequest.RequestSentBy = userId;
                    _context.SaveChanges();
                    status = true;
                }
            }
            catch (Exception ex)
            {
                status = false;
            }
            return status;
        }

        //View pending requests
        public List<MatchedRequest> ViewPendingRequests(string userId)
        {
            List<MatchedRequest> matchedRequests = new List<MatchedRequest>();
            try
            {
                matchedRequests = (from mr in _context.MatchedRequest
                                   where (mr.OwnerId == userId
                                   || mr.WasherId == userId) &&
                                   mr.Status.ToLower() == "pending" && mr.RequestSentBy != userId
                                   select mr).ToList();
            }
            catch (Exception ex)
            {
                matchedRequests = null;
            }
            return matchedRequests;
        }


        //Accepting request
        public bool AcceptOrRejectRequest(int matchedRequestId, string newStatus)
        {
            bool status = false;
            MatchedRequest matchedRequest = new MatchedRequest();
            try
            {
                matchedRequest = (from mr in _context.MatchedRequest
                                  where mr.MatchedRequestId == matchedRequestId
                                  select mr).FirstOrDefault();
                if (matchedRequest != null && matchedRequest.Status != "Rejected")
                {
                    matchedRequest.Status = newStatus;
                    _context.SaveChanges();
                    //Add to Accepted Request
                    if (newStatus == "Accepted")
                        status = AcceptedRequest(matchedRequest);
                }
            }
            catch (Exception e)
            {
                status = false;
            }
            return status;
        }

        //Create accepted request
        public bool AcceptedRequest(MatchedRequest matchedRequest)
        {
            bool status = false;
            try
            {
                AcceptedRequest acceptedRequest = new AcceptedRequest();
                acceptedRequest.OwnerId = matchedRequest.OwnerId;
                acceptedRequest.Status = "Incomplete";
                acceptedRequest.OwnerRequestId = matchedRequest.OwnerRequestId;
                acceptedRequest.WasherId = matchedRequest.WasherId;
                acceptedRequest.WasherRequestId = matchedRequest.WasherRequestId;
                acceptedRequest.TimeStamp = DateTime.Now;
                _context.AcceptedRequest.Add(acceptedRequest);
                _context.SaveChanges();
                status = UpdateCapacity(acceptedRequest);
            }
            catch (Exception ex)
            {
                status = false;
            }
            return status;
        }

        //Update capacity
        public bool UpdateCapacity(AcceptedRequest acceptedRequest)
        {
            bool status = false;
            try
            {
                LaundryRequest ownerRequest = (from lr in _context.LaundryRequest
                                               where lr.UserId == acceptedRequest.OwnerId
                                               select lr).FirstOrDefault();
                LaundryRequest washerRequest = (from lr in _context.LaundryRequest
                                                where lr.UserId == acceptedRequest.WasherId
                                                select lr).FirstOrDefault();
                int newCapacity = 0;
                if (ownerRequest.Weight >= washerRequest.Weight)
                {
                    newCapacity = ownerRequest.Weight - washerRequest.Weight;
                    ownerRequest.Weight = newCapacity;
                    _context.SaveChanges();
                    if (newCapacity == 0)
                    {
                        acceptedRequest.Status = "Active";
                        status = StartWashCycle(acceptedRequest.AcceptedRequestId);
                    }
                }
            }
            catch (Exception ex)
            {
                status = false;
            }
            return status;
        }

        //Start wash cycle -> will call UpdateWashStatus automatically after 2 hours
        public bool StartWashCycle(int acceptedRequestId)
        {
            bool status = false;
            try
            {
                //Call after two hours
                status = UpdateWashStatus(acceptedRequestId);
            }
            catch (Exception ex)
            {
                status = false;
            }
            return status;
        }

        //Update wash status
        public bool UpdateWashStatus(int acceptedRequestId)
        {
            bool status = false;
            try
            {
                AcceptedRequest acceptedRequest = new AcceptedRequest();
                acceptedRequest = (from ar in _context.AcceptedRequest
                                   where ar.AcceptedRequestId == acceptedRequestId
                                   select ar).FirstOrDefault();
                if (acceptedRequest != null)
                {
                    string ownId = acceptedRequest.OwnerId;
                    var accRequests = (from ar in _context.AcceptedRequest
                                       where ar.OwnerId == ownId
                                       select ar).ToList();
                    if(accRequests.Any())
                    {
                        foreach (var item in accRequests)
                        {
                            //Update status as completed
                            item.Status = "Complete";

                            //Debit transaction
                            Transaction debitTransaction = new Transaction();
                            debitTransaction.UserId = item.WasherId;
                            int? washerReqId = item.WasherRequestId;
                            LaundryRequest washerRequest = (from lr in _context.LaundryRequest
                                                            where lr.RequestId == washerReqId
                                                            select lr).FirstOrDefault();
                            debitTransaction.Laundrocash = washerRequest.Weight;
                            debitTransaction.TransactionType = "D";
                            debitTransaction.Message = "Debited successfully";
                            debitTransaction.TransactionDateTime = DateTime.Now;

                            //Credit transaction
                            Transaction creditTransaction = new Transaction();
                            creditTransaction.UserId = item.OwnerId;
                            int? ownerReqId = item.OwnerRequestId;
                            LaundryRequest ownerRequest = (from lr in _context.LaundryRequest
                                                           where lr.RequestId == ownerReqId
                                                           select lr).FirstOrDefault();
                            creditTransaction.Laundrocash = washerRequest.Weight;
                            creditTransaction.TransactionType = "C";
                            creditTransaction.Message = "Credited successfully";
                            creditTransaction.TransactionDateTime = DateTime.Now;

                            //Add transactions
                            _context.Transaction.Add(debitTransaction);
                            _context.Transaction.Add(creditTransaction);

                            //Update request status as Inactive
                            washerRequest.Status = "Completed";
                            ownerRequest.Status = "Completed";
                        }
                    }
                    

                    //Save changes
                    _context.SaveChanges();
                    status = true;
                }
            }
            catch (Exception ex)
            {
                status = false;
            }
            return status;
        }

        //Buy coins
        public bool BuyCoins(string userId, long accountNumber, int numberOfCoins)
        {
            bool status = false;
            try
            {
                //logic to validate account number
                Transaction transaction = new Transaction();
                transaction.UserId = userId;
                transaction.Laundrocash = numberOfCoins;
                transaction.TransactionType = "C";
                transaction.TransactionDateTime = DateTime.Now;
                transaction.Message = "Laundrocash added";
                _context.Transaction.Add(transaction);
                _context.SaveChanges();
                status = true;
            }
            catch (Exception ex)
            {
                status = false;
            }
            return status;
        }

        //Fetch laundrocash for a user
        public int FetchLaundrocashForUser(string userId)
        {
            int laundrocash = 0;
            try
            {
                List<int> totalDebit = (from t in _context.Transaction
                                        where t.TransactionType == "D"
                                        && t.UserId == userId
                                        select t.Laundrocash).ToList();
                List<int> totalCredit = (from t in _context.Transaction
                                         where t.TransactionType == "C"
                                         && t.UserId == userId
                                         select t.Laundrocash).ToList();
                laundrocash = totalCredit.Sum() - totalDebit.Sum();

            }
            catch (Exception ex)
            {
                laundrocash = -99;
            }
            return laundrocash;
        }


        //Fetch all transactions for a user
        public List<Transaction> GetTransactions(string userId)
        {
            List<Transaction> lst = null;
            try
            {
                lst = (from t in _context.Transaction where t.UserId == userId select t).ToList();
            }
            catch (Exception e)
            {
                lst = null;
            }
            return lst;

        }
        public int savedwater(string userId)
        {
            List<int> lst = null;
            int water = 0;
            try
            {
                lst = (from t in _context.Transaction where t.UserId == userId && t.TransactionType=="D"  select t.Laundrocash).ToList();
                water = lst.Sum() * 120 / 8;
            }
            catch (Exception e)
            {
                lst = null;
            }
            return water;
        }

        public int checkpassword(string userId,string password)
        {
            int status = 0;
            try
            {
                cmdObj = new SqlCommand(@"SELECT [dbo].UFN_CHECKPASSWORD(@UserId,@Password)", conObj);
                cmdObj.Parameters.AddWithValue("@UserId", userId);
                cmdObj.Parameters.AddWithValue("@Password", password);
                conObj.Open();
                status = Convert.ToInt32(cmdObj.ExecuteScalar());
            }
            catch (Exception ex)
            {

            }
            finally
            {
                conObj.Close();
            }
            return status;

        }

        //public int changePassword(string userId, string oldPassword,string newPassword)
        //{
        //    int a = checkpassword(userId, oldPassword);
        //    if (a == 1)
        //    {
        //        Users u = new Users();
        //        u = (from ar in _context.Users
        //                           where ar.Userid == userId
        //                           select ar).FirstOrDefault();
        //        u.Userpassword = newPassword;

        //    }
        //    else
        //    {
        //        return 0;
        //    }
        //}

        //Fetch status from laundry request
        public string FetchLaundryStatus(string userId)
        {
            string status = "";
            try
            {
                status = (from s in _context.LaundryRequest
                          where s.UserId == userId orderby s.WashingTime descending
                          select s.Status).FirstOrDefault();
            }
            catch (Exception e)
            {
                status = "";
            }
            return status;
        }

        //Fetch status from matched request
        public bool FetchMatchedStatus(string userId)
        {
            bool status = false;
            try
            {
                var temp = (from s in _context.AcceptedRequest
                          where ( s.OwnerId == userId || s.WasherId==userId ) && s.Status=="Incomplete"
                          select s.Status).FirstOrDefault();
                if (temp == null)
                {
                    status = false;
                }
                else
                {
                    status = true;
                }
            }
            catch (Exception e)
            {
                status = false;
            }
            return status;
        }
    }
}
