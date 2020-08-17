using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;
using TCSProject.Models;

namespace TCSProject.Controllers
{
    public class AdminController : Controller
    {

        SqlConnection con = new SqlConnection();
        SqlCommand com = new SqlCommand();
        SqlDataReader dr;

        private int _nextId = 1;
        SqlDataAdapter da;
        DataSet ds = new DataSet();


        // GET: Admin
        public ActionResult Index()
        {


            return View();
        }

        void connectionstring()
        {
            con.ConnectionString = "data source = SURYATEJA\\MS; initial catalog = EMP; user id = sa; password = monuSurya; multipleactiveresultsets = True; application name = EntityFramework";
        }

        [HttpPost]
        public ActionResult VerifyAdmin(Admin adm)
        {
            try
            {


                connectionstring();
                con.Open();
                com.Connection = con;
                com.CommandText = "select * from AdmTable where AdmId='" + adm.AdmId + "' and Password='" + adm.Password + "'";
                dr = com.ExecuteReader();


            }
            catch (Exception ex)
            {
                Response.Write(ex.ToString());
            }


            if (dr.Read() == false)
            {

                adm.LoginErrorMessage = "Invalid Credentials !";
                //return View("Login", adm);
                //Response.Write("<script language='javascript'>alert('Invalid Credentials !');</script>");

                return RedirectToAction("Instructions", "Admin");
            }
            else
            {
                con.Close();
                Session["AdmId"] = adm.AdmId;
                return RedirectToAction("Home", "Admin");

            }

        }

        public ActionResult Logout()
        {
            Session.Abandon();
            return RedirectToAction("Login", "Employee");
        }

        public ActionResult Instructions()
        {
            return View();

        }

        public ActionResult Home()
        {
            connectionstring();
            con.Open();
            da = new SqlDataAdapter("Select * from EmplTable", con);
            da.Fill(ds);

            List<Employee> emp = new List<Employee>();

            foreach (DataRow dr in ds.Tables[0].Rows)

            {

                emp.Add(new Employee() { EmpId = int.Parse(dr[0].ToString()), FirstName = dr[1].ToString(), LastName = dr[2].ToString(), Email = dr[6].ToString(), ProjectId = int.Parse(dr[8].ToString()), WONNumber = int.Parse(dr[9].ToString()), ProjectDetails = dr[10].ToString() });

            }

            ViewData.Model = emp;
            return View();
        }


        public ActionResult SearchForEmployee(int Id)
        {
            ViewBag.Message = Id.ToString();
            //Employee emp = new Employee();
            return View();
        }


        [HttpGet]
        public ActionResult ViewEmployee()
        {

            return View();
        }

        [HttpPost]
        public ActionResult ViewEmployee(Employee emp)
        {

            //connectionstring();
            //con.Open();
            //com.Connection = con;
            //com.CommandText = "select * from AdmTable where AdmId='" + emp.EmpId + "'";
            //dr = com.ExecuteReader();

            connectionstring();
            con.Open();
            da = new SqlDataAdapter("Select * from EmplTable where EmpId='" + emp.EmpId + "'", con);
            da.Fill(ds);
            List<Employee> emp1 = new List<Employee>();

            foreach (DataRow dr in ds.Tables[0].Rows)

            {

                emp1.Add(new Employee() { EmpId = int.Parse(dr[0].ToString()), FirstName = dr[1].ToString(), LastName = dr[2].ToString(),  Email = dr[6].ToString(), ProjectId = int.Parse(dr[8].ToString()), WONNumber = int.Parse(dr[9].ToString()), ProjectDetails = dr[10].ToString() });

            }
            if (emp1.Count() == 0)
            {
                ViewBag.IsEmployeePresent = false;
                return View();
            }
            else
            {
                ViewBag.IsEmployeePresent = true;
            }

            ViewData.Model = emp1[0];
            return View();

        }


        [HttpGet]
        public ActionResult AddEmployee()
        {
            return View();

        }

        [HttpPost]
        public ActionResult AddEmployee(Employee emp)
        {
            connectionstring();
            con.Open();

            //String MyString;
            //DateTime MyDateTime;
            //MyDateTime = new DateTime();
            //MyString = emp.DOB.Date.ToString();
            //MyDateTime = DateTime.ParseExact(MyString, "yyyy-MM-dd",null);

            string insertQuery = "select count(EmpId) from EmplTable where Email='" + emp.Email + "'";
            SqlCommand cmd = new SqlCommand(insertQuery, con);
            int count = Convert.ToInt32(cmd.ExecuteScalar().ToString());
            if (count != 0)
            {
                ViewBag.AddEmpStatus = false;
                ViewBag.Message = "There's already an employee with given mail";

            }
            else
            {

                insertQuery = "insert into EmplTable(FirstName,LastName,Email,ProjectId,WONNumber, ProjectDetails) values('" + emp.FirstName + "','" + emp.LastName + "','" + emp.Email + "','" + emp.ProjectId + "','" + emp.WONNumber + "','" + emp.ProjectDetails + "');" + " SELECT SCOPE_IDENTITY();";
                cmd = new SqlCommand(insertQuery, con);
                int res = Convert.ToInt32(cmd.ExecuteScalar().ToString());
                if (res != 0)
                {
                    ViewBag.AddEmpStatus = true;
                    ViewBag.Message = "Successfully added employee " + Convert.ToString(res) + "!";
                    ModelState.Clear();
                }

            }
            return View();
        }


        [HttpGet]
        public ActionResult DeleteEmployee()
        {
            return View();
        }

        [HttpPost]
        public ActionResult DeleteEmployee(Employee emp)
        {
            connectionstring();
            con.Open();
            string insertQuery = "Delete from EmplTable where EmpId='" + emp.EmpId + "' and Email='" + emp.Email + "'";
            SqlCommand cmd = new SqlCommand(insertQuery, con);
            int res = cmd.ExecuteNonQuery();
            if (res != 0)
            {
                ViewBag.DeleteStatus = true;
                ViewBag.Message = "Successfully deleted Employee " + emp.EmpId;
                return View();
            }
            else
            {
                ViewBag.DeleteStatus = false;
                ViewBag.Message = "Could not find an employee with given details.\nEnter correct details.";
            }
            return View();
        }

        [HttpGet]
        public ActionResult UpdateEmployeeDetails()
        {
            return View();
        }



        [HttpPost]
        public ActionResult UpdateEmployeeDetails(Employee emp)
        {
            connectionstring();
            con.Open();
            SqlCommand cmd;
             string insertQuery = "select count(EmpId) from EmplTable where EmpId=' " + emp.EmpId + "'";
             cmd = new SqlCommand(insertQuery, con);
             int count = Convert.ToInt32(cmd.ExecuteScalar().ToString());
             if (count > 0)
             {
                        ViewBag.UpdateEmpStatus = true;
                        bool first = true;

                        string text = "";
               
                        if (emp.FirstName != "" )
                        {
                            if (first)
                            {
                                text += " FirstName= @FN ";
                            first = false;
                            }
                        
                        }

                        if (emp.LastName != "")
                        {
                            if (first)
                            {
                                text += " LastName= @LN ";
                                first = false;
                            }


                            else
                            {
                                text += " , LastName= @LN ";
                            }
                        }

                        if (emp.Email != null)
                        {
                            if (first)
                            {
                                text += " Email= @Email ";
                                first = false;
                            }
                            else
                            {
                                text += " , Email= @Email ";
                            }
                        }

                        if (emp.ProjectId != 0)
                        {
                            if (first)
                            {
                                text += " ProjectId = @PID ";
                                first = false;
                            }
                            else
                            {
                                text += " , ProjectId= @PID ";
                            }
                        }


                        if (emp.WONNumber != 0)
                        {
                            if (first)
                            { 
                                text += " WONNumber = @WON ";
                                first = false;
                            }
                            else
                            {
                                text += " , WONNumber = @WON ";
                            }
                        }

                        if (emp.ProjectDetails != null)
                        {
                            if (first)
                            { 
                                text += " ProjectDetails = @PDetails "; 
                                first = false;
                            }
                            else
                            {
                                text += " , ProjectDetails = @PDetails ";
                            }
                        }


                        string query = "Update EmplTable SET " + text + " where EmpId= " + emp.EmpId + "";

                        
                        //Response.Write(query);
                        cmd = new SqlCommand(query, con);

                        if(emp.FirstName != "")
                        {
                            cmd.Parameters.AddWithValue("@FN", emp.FirstName);
                        }
                if (emp.LastName != "")
                {
                    cmd.Parameters.AddWithValue("@LN", emp.LastName);
                }
                if (emp.Email != "")
                {
                    cmd.Parameters.AddWithValue("@Email", emp.Email);
                }
               
                if (emp.ProjectId != 0)
                {
                    cmd.Parameters.AddWithValue("@PID", emp.ProjectId);
                }
                if (emp.WONNumber != 0)
                {
                    cmd.Parameters.AddWithValue("@WON", emp.WONNumber);
                }

                if (emp.ProjectDetails != "")
                {
                    cmd.Parameters.AddWithValue("@PDetails", emp.ProjectDetails);
                }

                int res = cmd.ExecuteNonQuery();


                        if (res != 0)
                        {
                            ViewBag.Message = "Updated employee : '" + emp.EmpId + "' successfully !";
                        }
                        else
                        {
                        ViewBag.UpdateEmpStatus = "Error";
                        ViewBag.Message = "Error while updating please checck th eprovided details and rerty !";
                        }

                 


                 }
                 else
                 {

                        ViewBag.UpdateEmpStatus = false;
                        ViewBag.Message = "No such Employee found";
                 }
            
            

            
                return View();

        }




        [HttpGet]
        public ActionResult ForgotPassword()
        {

            return View();
        }


        [NonAction]
        public void SendVerificationLinkEmail(string emailID, string activationCode, string emailFor = "VerifyAccount")
        {
            var verifyUrl = "/Admin/" + emailFor + "/" + activationCode;
            var link = Request.Url.AbsoluteUri.Replace(Request.Url.PathAndQuery, verifyUrl);

            var fromEmail = new MailAddress("fanofmyplayer@gmail.com", "Employee Management Portal");
            var toEmail = new MailAddress(emailID);
            var fromEmailPassword = "monuSurya"; // Replace with actual password

            string subject = "";
            string body = "";
            if (emailFor == "VerifyAccount")
            {
                subject = "Your account is successfully created!";
                body = "<br/><br/>We are excited to tell you that your Dotnet Awesome account is" +
                    " successfully created. Please click on the below link to verify your account" +
                    " <br/><br/><a href='" + link + "'>" + link + "</a> ";
            }
            else if (emailFor == "ResetPassword")
            {
                subject = "Reset Password";
                body = "Hi,<br/>We got request for reset your account password. Please click on the below link to reset your password" +
                    "<br/><br/><a href=" + link + ">Reset Password link</a>";
            }


            var smtp = new SmtpClient
            {
                Host = "smtp.gmail.com",
                Port = 587,
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(fromEmail.Address, fromEmailPassword)
            };

            using (var message = new MailMessage(fromEmail, toEmail)
            {
                Subject = subject,
                Body = body,
                IsBodyHtml = true
            })
                smtp.Send(message);
        }

        [HttpPost]
        public ActionResult ForgotPassword(string EmailId)
        {
            string message = "";
            bool status = false;

            connectionstring();
            con.Open();
            string insertQuery = "select count(AdmId) from AdmTable where EmailId='" + EmailId + "'";
            SqlCommand cmd = new SqlCommand(insertQuery, con);
            int count = Convert.ToInt32(cmd.ExecuteScalar().ToString());
            if (count > 0)
            {
                string resetCode = Guid.NewGuid().ToString();
                SendVerificationLinkEmail(EmailId, resetCode, "ResetPassword");
                insertQuery = "Update AdmTable SET ResetPasswordCode= @RPC where EmailId= @mail";
                cmd = new SqlCommand(insertQuery, con);
                cmd.Parameters.AddWithValue("@RPC", resetCode);
                cmd.Parameters.AddWithValue("@mail", EmailId);
                count = cmd.ExecuteNonQuery();
                message = "Reset password link has been sent to your email id.";
            }
            else
            {
                message = "Account not found";
            }
            ViewBag.Message = message;

            return View();


        }



        public ActionResult ResetPassword(string id)
        {
            //Verify the reset password link
            //Find account associated with this link
            //redirect to reset password page
            if (string.IsNullOrWhiteSpace(id))
            {
                return HttpNotFound();
            }

            connectionstring();
            con.Open();
            string insertQuery = "select count(AdmId) from AdmTable where ResetPasswordCode='" + id + "'";
            SqlCommand cmd = new SqlCommand(insertQuery, con);
            int count = Convert.ToInt32(cmd.ExecuteScalar().ToString());
            if (count > 0)
            {

                ResetPasswordModel model = new ResetPasswordModel();
                model.ResetCode = id;
                return View(model);


            }
            else
            {
                return HttpNotFound();
            }
        }





        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ResetPassword(ResetPasswordModel model)
        {
            var message = "Entered Action.... ";

            connectionstring();
            con.Open();
            string insertQuery = "select count(AdmId) from AdmTable where ResetPasswordCode='" + model.ResetCode + "'";
            SqlCommand cmd = new SqlCommand(insertQuery, con);
            int count = Convert.ToInt32(cmd.ExecuteScalar().ToString());

            if (count > 0)
            {
                message = "Employee exists ";

                insertQuery = "select AdmId from AdmTable where ResetPasswordCode= '" + model.ResetCode + "'";
                SqlCommand cmd2 = new SqlCommand(insertQuery, con);
                string eid = cmd2.ExecuteScalar().ToString();


                insertQuery = "Update AdmTable SET Password= @pwd , ResetPasswordCode= @rpc where AdmId= @eid";
                SqlCommand cmd1 = new SqlCommand(insertQuery, con);
                cmd1.Parameters.AddWithValue("@pwd", model.NewPassword);
                cmd1.Parameters.AddWithValue("@rpc", "");
                cmd1.Parameters.AddWithValue("@eid", eid);

                int res = cmd1.ExecuteNonQuery();
                if (res > 0)
                    message += "New password updated successfully";
                ViewBag.message = message;
                return View(model);
            }


            message = "Something invalid";

            ViewBag.Message = message;
            return View(model);
        }





    }

}
        

 