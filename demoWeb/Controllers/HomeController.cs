
using demoWeb.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PagedList;
using System.Net.Mail;
using System.Net;
using System.Web.Helpers;
using System.Threading.Tasks;

namespace demoWeb.Controllers
{
    public class HomeController : Controller
    {
        MyDataDataContext data = new MyDataDataContext();

        public ActionResult Index(int? page, string searchString)
        {
            
            ViewBag.Keyword = searchString;
            if (page == null) page = 1;
            var all_sanPham = (from tt in data.SANPHAMs select tt).OrderBy(m => m.MASANPHAM);
            int pageSize = 6;
            int pageNum = page ?? 1;
            return View(HomeController.GetAll(searchString).ToPagedList(pageNum, pageSize));
        }
        public static List<SANPHAM> GetAll(string searchKey)
        {
            MyDataDataContext data = new MyDataDataContext();
            searchKey = searchKey + "";
            return data.SANPHAMs.Where(p => p.TENSANPHAM.Contains(searchKey)).ToList();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }
        
        //public class MailInfor{
        //    public string From { get; set; }
        //    public string To { get; set; }
        //    public string Subject { get; set; }
        //    public string Body { get; set; }
        //}
        public ActionResult Book()
        {
            return View();
        }


        public ActionResult Menu(int? page, string searchString) 
        {
            ViewBag.Keyword = searchString;
            if (page == null) page = 1;
            var all_sanPham = (from tt in data.SANPHAMs select tt).OrderBy(m => m.MASANPHAM);
            int pageSize = 12;
            int pageNum = page ?? 1;
            return View(HomeController.GetAll(searchString).ToPagedList(pageNum, pageSize));

        }

        [HttpGet]
        public ActionResult Register()
        {
            return View();
        }
        //[HttpPost]

        //public ActionResult gido(NGUOIDUNG kh) {
        //    var check = data.NGUOIDUNGs.Where(m => m.GMAIL == kh.GMAIL).FirstOrDefault();
        //    return check;
        //}


        [HttpPost]
        public ActionResult Register(FormCollection collection, NGUOIDUNG kh)
        {
            int OTP = new Random().Next(100000, 999999);
            var check = data.NGUOIDUNGs.Where(m => m.GMAIL == kh.GMAIL).FirstOrDefault();
            if(check != null)
            {
                ViewData["CheckMail"] = "Tài Khoản Này Đã Được Sử Dụng";
            }
            else
            {
                var TenKH = collection["HOVATEN"];
                var pass = collection["PASS"];
                var Gmail = collection["GMAIL"];
                var sdt = collection["SDT"];
                var GioiTinh = collection["GIOITINH"];
                var NamSinh = string.Format("{0:dd/MM/yyyy}", collection["NAMSINH"]);
                var DiaChi = collection["DIACHI"];
                //int quyen = 0;

                if (String.IsNullOrEmpty(TenKH))
                {
                    ViewData["Error1"] = "Họ tên khách hàng không được để trống";
                }

                else
                {
                    //Gán giá trị cho đổi tượng được tạo mới (kh)
                    kh.HOVATEN = TenKH;
                    kh.GMAIL = Gmail;
                    kh.DIACHI = DiaChi;
                    kh.SDT = sdt;
                    kh.GIOITINH = int.Parse(GioiTinh);
                    kh.NAMSINH = DateTime.Parse(NamSinh);
                    kh.PASS = pass;
                    kh.USERNAME = Gmail;
                    //kh.GIOITINH = int.Parse(OTP);
                    data.NGUOIDUNGs.InsertOnSubmit(kh);
                    data.SubmitChanges();
                    // mail OTP
                    try
                    {
                        if (ModelState.IsValid)
                        {
                            var senderEmail = new MailAddress("haunguyenaaaa6@gmail.com", "Cửa Hàng Máy Ảnh Hutech");
                            var receiverEmail = new MailAddress(Gmail, "Receiver");
                            var password = "Hau121pk@1";
                            var sub = "Mã OTP Xác Nhận";
                            var body = "Mã OTP Của Bạn là: " + OTP + " Vui Lòng Sử Dụng Mã Này Để Xác Thực";
                            var smtp = new SmtpClient
                            {
                                Host = "smtp.gmail.com",
                                Port = 587,
                                EnableSsl = true,
                                DeliveryMethod = SmtpDeliveryMethod.Network,
                                UseDefaultCredentials = false,
                                Credentials = new NetworkCredential(senderEmail.Address, password)
                            };
                            using (var mess = new MailMessage(senderEmail, receiverEmail)
                            {
                                Subject = sub,
                                Body = body
                            })
                            {
                                smtp.Send(mess);
                            }
                            Session["OTPDK"] = OTP;
                        }
                        return RedirectToAction("XacNhanOTP", "Home");
                    }
                    catch (Exception ex)
                    {
                        return HttpNotFound();
                    }

                }
                
            }
            return this.Register();
        }

        public ActionResult LogOut()
        {
            Session["TaiKhoan"] = null;
            return RedirectToAction("Login", "Home");
        }
        //Xác nhận OTP
        public ActionResult XacNhanOTP()
        {
            return View();
        }
        //[HttpPost]
        //public ActionResult KiemTraOTP(string otp)
        //{
        //    bool result = false;

        //    string sessionOTP = Session["OTPDK"].ToString();
        //    if (otp == sessionOTP)
        //    {
        //        result = true;
        //    }
        //    return RedirectToAction("Login");
        //}

        [HttpPost]
        public JsonResult KiemTraOTP(string otp)
        {
            bool result = false;

            string sessionOTP = Session["OTPDK"].ToString();
            if (otp == sessionOTP)
            {
                result = true;
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }


        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Login(FormCollection collection)
        {
            var tendn = collection["USERNAME"];
            var matkhau = collection["PASS"];
            if (String.IsNullOrEmpty(tendn))
            {
                ViewData["Error1"] = "Phải nhập tên đăng nhập";
            }
            else if (String.IsNullOrEmpty(matkhau))
            {
                ViewData["Error2"] = "Phải nhập mật khẩu";
            }
            else
            {
                // Gán giá trị cho đối tượng được tạo mới(kh)
                NGUOIDUNG kh = data.NGUOIDUNGs.SingleOrDefault(n => n.USERNAME == tendn && n.PASS == matkhau);
                if (kh != null)
                {
                    Session["TaiKhoan"] = kh;
                    return RedirectToAction("Index", "Home");
                }
                else
                    ViewBag.Thongbao = "Tên đăng nhập hoặc mật khẩu không đúng";
            }
            return View();
        }

        public ActionResult Cart()
        {
            ViewBag.Message = "Your Form Login";
            return View();
        }

        public ActionResult Details (int id)
        {
            var detail = data.SANPHAMs.Where(m => m.MASANPHAM == id).First();
            return View(detail);
            //var sp = from s in data.SANPHAMs where s.MASANPHAM == id select s;
            //return View(sp.Single());
        }


    }
}