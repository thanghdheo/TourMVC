using Microsoft.AspNetCore.Mvc;
using QuanLyTour.Models;
using QuanLyTour.DAO;
using System.Collections.Generic;
using Tour_MVC.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Linq;
using System;

namespace Tour_MVC.Controllers
{
    public class KhachHangController : Controller
    {
        private List<KhachDuLich> khachDuLiches;

        private List<Tour> tours;

        private IEnumerable<DoanDuLich> doanDuLiches;

        private IEnumerable<BangGiaVe> bangGiaVes;

        private Paginated<KhachDuLich> khachDuLichs;

        private KhachHangRepository khachHangRepository;

        private KhachHangDAO _khachHang;

        private ChiTietDoanKhachDAO _chiTietDoanKhach;

        private TourDAO _tour;

        private bool isSuccess = false;

        public KhachHangController()
        {
            khachDuLiches = new List<KhachDuLich>();
            tours = new List<Tour>();
            _khachHang = new KhachHangDAO();
            _tour = new TourDAO();
            _chiTietDoanKhach = new ChiTietDoanKhachDAO();
            khachHangRepository = new KhachHangRepository();
        }
        public IActionResult Index(string sort = "", string search = "", int pg = 1, int pageSize = 5)
        {
            sortOrder sortOrder = new sortOrder();
            sortOrder.AddColum("stt");
            sortOrder.AddColum("tên khách hàng");
            sortOrder.AddColum("cmnd");
            sortOrder.AddColum("địa chỉ");
            sortOrder.AddColum("giới tính");
            sortOrder.AddColum("số điện thọai");
            sortOrder.AddColum("quốc tịch");
            sortOrder.ApplySort(sort);
            ViewData["sortModel"] = sortOrder;
            ViewBag.search = search;

            khachDuLichs = khachHangRepository.getItems(sortOrder.sortedStr, sortOrder.sortedOrder, search, pg, pageSize);

            //int toRecs = ((Paginated<Tour>)tours).TotalRecords;
            var paper = new PageModel(khachDuLichs.TotalRecords, pg, pageSize);

            paper.sort = sort;

            this.ViewBag.Paper = paper;

            //tours = tours.Skip((pg-1)*pageSize).Take(pageSize).ToList();    

            return View(khachDuLichs);
        }

        public IActionResult ThemKhachHang()
        {
            return View();
        }

        [HttpPost]
        public IActionResult ThemKhachHang(KhachDuLich khachDuLich)
        {
            if (ModelState.IsValid)
            {
                if (isSuccess = _khachHang.ThemKhach(khachDuLich))
                    TempData["Message"] = "Thêm thành công";
                    return Redirect("Index");
                }
                else
                {
                    TempData["Message"] = "Thêm thất bại";
                    return Redirect("Index");
                }
            return View();
        }

        public IActionResult SuaKhachHang(string MaKhachHang)
        {
            if(khachDuLiches.Count == 0)
            {
                khachDuLiches = _khachHang.DanhSachKhach();
            }
            for (int i = 0; i < khachDuLiches.Count; i++)
            {
                if (khachDuLiches[i].MaKhachHang.Equals(MaKhachHang))
                {

                    KhachDuLich tmp = khachDuLiches[i];
                    return View(tmp);
                }
            }
            return View();
        }

        [HttpPost]
        public IActionResult SuaKhachHang(KhachDuLich khachDuLich)
        {
            if (ModelState.IsValid)
            {
                if (isSuccess = _khachHang.SuaKhach(khachDuLich))
                {
                    TempData["Message"] = "Sửa thành công";
                    return Redirect("Index");
                }
                else
                {
                    TempData["Message"] = "Sửa thất bại";
                    return Redirect("Index");
                }
            }
            return View();
        }

        [HttpGet]
        public IActionResult ThemDoan(string MaKhachHang)
        {
            if (khachDuLiches.Count == 0)
            {
                khachDuLiches = _khachHang.DanhSachKhach();
            }
            if (tours.Count == 0)
            {
                tours = _tour.TourComboBox();
            }
            ViewBag.TenTourList = new SelectList(tours, "MaTour", "TenTour");
            for (int i = 0; i < khachDuLiches.Count; i++)
            {
                if (khachDuLiches[i].MaKhachHang.Equals(MaKhachHang))
                {
                    ViewBag.Id = khachDuLiches[i].MaKhachHang;
                }
            }
            return View();
        }

        [HttpPost]

        public IActionResult ThemDoan(ChiTietDoanKhach ct)
        {
            if (ModelState.IsValid)
            {
                if (_chiTietDoanKhach.Them(ct)){
                    TempData["Message"] = "Thêm thành công";
                    return Redirect("Index");
                }
                else
                {
                    TempData["Message"] = "Thêm thất bại";
                    return Redirect("Index");
                }
            }
            return View();
        }


        public JsonResult ajaxGetDoan(string MaTour)
        {
            doanDuLiches = new DoanDuLichDAO().DanhSachDoanNe().Where(i => i.MaTour == MaTour);
            
            return Json(doanDuLiches);
        }

        public JsonResult ajaxGetNgayDoan(string MaDoan)
        {
            doanDuLiches = new DoanDuLichDAO().DanhSachDoanNe().Where(i => i.MaSoDoan== MaDoan);

            return Json(doanDuLiches);
        }

        public string ajaxGetNgayDoanGia(DateTime ThoiGianBatDau,DateTime ThoiGianKetThuc)
        { 
            bangGiaVes = new BangGiaVeDAO().DanhSachGiaVe();
            foreach (BangGiaVe giaVes in bangGiaVes)
                if (ThoiGianBatDau <= ThoiGianKetThuc && giaVes.ThoiGianBatDau.Value <= ThoiGianBatDau && giaVes.ThoiGianKetThuc >= ThoiGianKetThuc)
                    return giaVes.Gia.ToString();
            return "";
        }

        public string ajaxGetGia(string MaTour, DateTime ThoiGianBatDau, DateTime ThoiGianKetThuc)
        {
            return layGiaVe(MaTour, ThoiGianKetThuc, ThoiGianBatDau);
        }

        public string layGiaVe(string MaTour, DateTime ThoiGianKetThuc, DateTime ThoiGianBatDau)
        {
            bangGiaVes = new BangGiaVeDAO().DanhSachGiaVe().Where(i => i.MaTour == MaTour);
            foreach (BangGiaVe giaVes in bangGiaVes)
                if (ThoiGianBatDau <= ThoiGianKetThuc && giaVes.ThoiGianBatDau.Value <= ThoiGianBatDau && giaVes.ThoiGianKetThuc >= ThoiGianKetThuc)
                    return giaVes.Gia.ToString();
            return "";
        }
    }
}
