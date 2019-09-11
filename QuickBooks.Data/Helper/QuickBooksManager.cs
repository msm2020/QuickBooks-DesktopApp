using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QBFC13Lib;
using QBXMLRP2Lib;
using QuickBooks.Data.Entity;


namespace QuickBooks.Data.Helper
{
    public class QuickBooksManager
    {
        public static string ErrorMessage { get; set; }
        protected static bool SessionBegun = false;
        protected static bool ConnectionOpen = false;
        protected static QBSessionManager SessionManager = null;
        protected static IMsgSetRequest QuickBooksConection()
        {
            try
            {
                SessionManager = new QBSessionManager();
                IMsgSetRequest requestMsgSet = SessionManager.CreateMsgSetRequest("US", 13, 0);
                requestMsgSet.Attributes.OnError = ENRqOnError.roeContinue;
                return requestMsgSet;
            }
            catch (Exception ex)
            {
                // -- log
            }
            return null;

        }

        public static IQBBase DoProcessRequest(IMsgSetRequest requestMsgSet)
        {
            try
            {
                SessionManager.OpenConnection("QuickBooks Application", "QuickBooks Application");
                ConnectionOpen = true;
                SessionManager.BeginSession("", ENOpenMode.omDontCare);
                SessionBegun = true;

                IMsgSetResponse responseMsgSet = SessionManager.DoRequests(requestMsgSet);
                IResponseList responseList = responseMsgSet?.ResponseList;
                if (responseList == null) return null;

                if (responseList.Count > 0)
                {
                    IResponse response = responseList.GetAt(0);
                    if (response.StatusCode != 0)
                    {
                        ErrorMessage = response.StatusMessage;
                        return null;
                    }

                    if (response.Detail != null)
                    {
                        return response.Detail;
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = ex.Message;
                return null;
            }
            finally
            {
                if (SessionBegun)
                {
                    SessionManager?.EndSession();
                }
                if (ConnectionOpen)
                {
                    SessionManager?.CloseConnection();
                }
                SessionManager = null;
            }
            return null;
        }

        public static void CloseConnection()
        {
            try
            {
                SessionManager?.EndSession();
                SessionManager?.CloseConnection();
            }
            catch (Exception e)
            {

            }
        }

        protected Dictionary<string, string> QuickBooksExcuet(IMsgSetRequest requestMsgSet, string externalId = null)
        {
            try
            {
                SessionManager.OpenConnection("QuickBooks Application", "QuickBooks Application");
                ConnectionOpen = true;

                SessionManager.BeginSession("", ENOpenMode.omDontCare);
                SessionBegun = true;

                IMsgSetResponse responseMsgSet = SessionManager.DoRequests(requestMsgSet);
                SessionManager.EndSession();
                SessionBegun = false;

                SessionManager.CloseConnection();
                ConnectionOpen = false;

                if (responseMsgSet != null)
                {

                    IResponseList responseList = responseMsgSet.ResponseList;
                    for (int i = 0; i < responseList.Count; i++)
                    {
                        IResponse response = responseList.GetAt(i);
                        //check the status code of the response, 0=ok, >0 is warning
                        if (response.StatusCode >= 0)
                        {
                            var result = new Dictionary<string, string>();
                            result.Add("StatusCode", response.StatusCode + "");
                            //the request-specific response is in the details, make sure we have some
                            if (response.Detail != null)
                            {
                                //make sure the response is the type we're expecting
                                ENResponseType responseType = (ENResponseType)response.Type.GetValue();
                                if (responseType == ENResponseType.rtCustomerAddRs)
                                {
                                    //upcast to more specific type here, this is safe because we checked with response.Type check above
                                    ICustomerRet customerRet = (ICustomerRet)response.Detail;
                                    if (customerRet != null)
                                    {
                                        string newId = customerRet.ListID.GetValue();
                                        result.Add("Message", "Customer has been Added successfully");
                                        result.Add("ListID", newId);
                                        return result;
                                    }
                                }
                                if (responseType == ENResponseType.rtCustomerModRs)
                                {
                                    ICustomerRet ItemInventoryRet = (ICustomerRet)response.Detail;
                                    if (ItemInventoryRet != null)
                                    {
                                        string newId = ItemInventoryRet.ListID.GetValue();
                                        result.Add("Message", $"Customer with Id {newId} has been updated Successfully");
                                        return result;
                                    }
                                }
                                if (responseType == ENResponseType.rtListDelRs)
                                {
                                    IQBENListDelTypeType ListDelType = (IQBENListDelTypeType)response.Detail;
                                    result.Add("Message", $"Customer with Id {externalId} has been deleted Successfully");
                                    return result;
                                }
                                if (responseType == ENResponseType.rtAccountQueryRs)
                                {
                                    IAccountRetList AccountRet = (IAccountRetList)response.Detail;
                                    for (int j = 0; j < AccountRet.Count; j++)
                                    {
                                        var acc = AccountRet.GetAt(j);
                                        string FullName = (string)acc.FullName.GetValue();
                                        result.Add("Name", FullName);
                                    }
                                }



                            }
                            else
                            {
                                result.Add("Error", response.StatusMessage);
                                return result;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                if (SessionBegun)
                {
                    SessionManager.EndSession();
                }
                if (ConnectionOpen)
                {
                    SessionManager.CloseConnection();
                }
                return new Dictionary<string, string>() { { "Error", ex.Message } };
            }

            return new Dictionary<string, string>();

        }


        public bool TestConnect()
        {
            try
            {
                var qb = QuickBooksConection();
                SessionManager.OpenConnection("QuickBooks Application", "QuickBooks Application");
                ConnectionOpen = true;
                SessionManager.BeginSession("", ENOpenMode.omDontCare);
                SessionBegun = true;
                return true;
            }
            catch (Exception ex)
            {
                ErrorMessage = ex.Message;
                return false;
            }
            finally
            {
                if (SessionBegun)
                {
                    SessionManager?.EndSession();
                }
                if (ConnectionOpen)
                {
                    SessionManager?.CloseConnection();
                }
                SessionManager = null;
            }

            return false;
        }



        private string Ticket { get; set; }
        private RequestProcessor2 QuickBooksConection2()
        {
            try
            {
                var request = new RequestProcessor2();
                request.OpenConnection("QuickBooks Application", "QuickBooks Application");
                Ticket = request.BeginSession("", QBFileMode.qbFileOpenDoNotCare);
                return request;
            }
            catch (Exception ex)
            {
                // -- log
            }
            return null;

        }
        private string RequestTemp = "<?xml version=\"1.0\" encoding=\"utf-8\"?><?qbxml version=\"13.0\"?><QBXML> <QBXMLMsgsRq onError=\"stopOnError\">  <{0}  requestID=\"whatever\">{1}</{0}> </QBXMLMsgsRq></QBXML>";
        protected string ReadText(QuickBooksXmlEntity module, string data = null)
        {
            RequestProcessor2 request = null;
            string input = string.Format(RequestTemp, module.Req, data);
            if (module.RemoveId)
            {
                input = input.Replace("  requestID=\"whatever\"", "");
            }

            string response = "";
            try
            {
                request = QuickBooksConection2();
                response = request.ProcessRequest(Ticket, input);
            }
            catch (System.Runtime.InteropServices.COMException ex)
            {
                //LogClass.WriteLogLine(string.Format("ERROR Table{0}: {1} *:* {2}", datamap.Req, ex.StackTrace, ex.Message));
                //MessageBox.Show("COM Error Description = " + ex.Message, "COM error");
                //return;
            }
            catch (Exception ex)
            {
                // -- log
            }
            finally
            {
                if (Ticket != null)
                {
                    request?.EndSession(Ticket);
                }
                request?.CloseConnection();
            }

            return response;
        }




    }
}
