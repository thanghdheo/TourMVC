using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using QuanLyTour.DAO;
using QuanLyTour.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Tour_MVC.Controllers
{
    public class ThongKeController : Controller
    {
        private List<Tour> tourList;

        private TourDAO _tour;

        private ThongKeDAO _thongke;

        private IEnumerable<BangGiaVe> bangGiaVes;

        public ThongKeController()
        {
            tourList = new List<Tour>();
            _tour = new TourDAO();
            _thongke = new ThongKeDAO();
        }
        public IActionResult Index()
        {
            if (tourList.Count() == 0)
            {
                tourList = _tour.DanhSachTour();
            }
            ViewBag.tourList = new SelectList(tourList, "MaTour", "TenTour");
            return View();
        }

        public IActionResult TKChiPhi(string MaTour, DateTime ThoiGianBatDau, DateTime ThoiGianKetThuc)
        {
            if (tourList.Count() == 0)
            {
                tourList = _tour.DanhSachTour();
            }
            Tour tmp = new Tour();
            foreach (Tour t in tourList)
            {
                if (t.MaTour.Equals(MaTour))
                    tmp = t;
            }

            var json = _thongke.TKChiPhi(tmp, ThoiGianBatDau, ThoiGianKetThuc);
            return Json(json);
        }
        

        public JsonResult ajaxGetNgay(string MaTour)
        {
            bangGiaVes = new BangGiaVeDAO().DanhSachGiaVe().Where(i => i.MaTour == MaTour);
            return Json(bangGiaVes);
        }

        
    }
}
