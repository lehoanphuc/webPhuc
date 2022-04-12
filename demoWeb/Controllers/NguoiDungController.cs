using demoWeb.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace demoWeb.Controllers
{
    public class NguoiDungController : Controller
    {
        MyDataDataContext data = new MyDataDataContext();
        // GET: NguoiDung
        public ActionResult InFo()
        {
            var detail = data.NGUOIDUNGs.Where(m => m.MAQUYEN == 0).First();
            return View(detail);
        }
        public ActionResult EditNguoiDung(string id)
        {
            var E_NhanVien = data.NGUOIDUNGs.First(m => m.USERNAME == id);
            return View(E_NhanVien);
        }
        [HttpPost]
        public ActionResult EditNguoiDung(string id, FormCollection collection)
        {
            var E_NguoiDung = data.NGUOIDUNGs.First(m => m.USERNAME == id);
            var E_Pass = collection["PASS"];
            var E_HoTen = collection["HOVATEN"];
            var E_GioiTinh = Convert.ToInt32(collection["GIOITINH"]);
            var E_DiaChi = collection["DIACHI"];
            E_NguoiDung.USERNAME = id;
            if (string.IsNullOrEmpty(E_HoTen))
            {
                ViewData["Error"] = "Tên người dùng không được bỏ trống";
            }
            else
            {
                E_NguoiDung.PASS = E_Pass;
                E_NguoiDung.HOVATEN = E_HoTen;
                E_NguoiDung.GIOITINH = E_GioiTinh;
                E_NguoiDung.DIACHI = E_DiaChi;
                UpdateModel(E_NguoiDung);
                data.SubmitChanges();
                return RedirectToAction("InFo");
            }
            return this.EditNguoiDung(id);
        }
    }
}