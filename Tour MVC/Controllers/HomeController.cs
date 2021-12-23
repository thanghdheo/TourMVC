using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;
using QuanLyTour.DAO;
using QuanLyTour.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Tour_MVC.Models;

namespace Tour_MVC
{

    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        private static List<LoaiHinhDuLich> loaiHinhDuLichs;

        private List<DiaDiemThamQuan> diaDiemThamQuans;

        private List<ChiTietDiaDiem> chiTietDiaDiems;

        private List<Tour> tourList;

        private List<BangGiaVe> bangGiaVes;

        private List<DoanDuLich> doans;

        private Paginated<Tour> tours;

        private TourDAO _tour;

        private BangGiaVeDAO _bangGiaVe;

        private LoaiHinhDAO _loaiHinhDAO;

        private BangGiaVeDAO _bangGiaVeDAO;

        private TourRepository _unitRepository;

        private ChiTietDiaDiemRepository _diemRepository;
        private bool isSuccess = false;
        public HomeController(ILogger<HomeController> logger)
        {
            loaiHinhDuLichs = new List<LoaiHinhDuLich>();
            diaDiemThamQuans = new List<DiaDiemThamQuan>();
            tourList = new List<Tour>();
            bangGiaVes = new List<BangGiaVe>();
            doans = new List<DoanDuLich>();
            chiTietDiaDiems = new List<ChiTietDiaDiem>();
            _tour = new TourDAO();
            _bangGiaVe = new BangGiaVeDAO();
            _loaiHinhDAO = new LoaiHinhDAO();
            _bangGiaVeDAO = new BangGiaVeDAO();
            _unitRepository = new TourRepository();
            _diemRepository = new ChiTietDiaDiemRepository();
            _logger = logger;
        }
        public IActionResult Index(string sort = "", string search = "", int pg = 1, int pageSize = 5)
        {

            sortOrder sortOrder = new sortOrder();
            sortOrder.AddColum("stt");
            sortOrder.AddColum("tên tour");
            sortOrder.AddColum("đặc điểm");
            sortOrder.AddColum("tên loại hình");
            sortOrder.ApplySort(sort);
            ViewData["sortModel"] = sortOrder;
            ViewBag.search = search;

            tours = _unitRepository.getItems(sortOrder.sortedStr, sortOrder.sortedOrder, search, pg, pageSize);

            var paper = new PageModel(tours.TotalRecords, pg, pageSize);

            paper.sort = sort;

            this.ViewBag.Paper = paper;

            return View(tours);
        }

        [HttpGet]
        public IActionResult ThemTour()
        {
            if (loaiHinhDuLichs.Count() == 0)
            {
                loaiHinhDuLichs = _loaiHinhDAO.DanhSachLoaiHinh();
            }
            ViewBag.TenLoaiHinhList = new SelectList(loaiHinhDuLichs, "MaLoaiHinh", "TenLoaiHinh");


            return View();
        }


        [HttpPost]
        public IActionResult ThemTour(Tour tour)
        {
            if (ModelState.IsValid)
            {
                if (isSuccess = _tour.ThemTour(tour))
                {
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

        public IActionResult ThemLoaiHinh()
        {
            return View();
        }

        [HttpPost]
        public IActionResult ThemLoaiHinh(LoaiHinhDuLich loaiHinhDuLich)
        {
            if (ModelState.IsValid)
            {
                if (isSuccess = _loaiHinhDAO.ThemLoaiHinh(loaiHinhDuLich.TenLoaiHinh))
                {
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

        [HttpGet]
        public IActionResult SuaTour(string MaTour)
        {
            if (tourList.Count() == 0)
            {
                tourList = _tour.DanhSachTour();
                loaiHinhDuLichs = _loaiHinhDAO.DanhSachLoaiHinh();
            }
            ViewBag.TenLoaiHinhList = new SelectList(loaiHinhDuLichs, "MaLoaiHinh", "TenLoaiHinh");
            foreach(Tour t in tourList)
            {
                if (t.MaTour.Equals(MaTour))
                {
                    
                    Tour tmp = t;
                    return View(tmp);
                }
            }
            return View();
        }

        [HttpPost]
        public IActionResult SuaTour(Tour tour)
        {
            if (ModelState.IsValid)
            {
                if (isSuccess = _tour.SuaTour(tour))
                {
                    TempData["Message"] = "Sửa thành công";
                    return RedirectToAction("ChiTietTour",new {MaTour = tour.MaTour});
                }
                else
                {
                    TempData["Message"] = "Sửa thất bại";
                    return RedirectToAction("ChiTietTour", new { MaTour = tour.MaTour });
                }
            }
            return View();
        } 

        public IActionResult ChiTietTour(string MaTour)
        {
            if (tourList.Count() == 0)
            {
                tourList = _tour.DanhSachTour();
                loaiHinhDuLichs = _loaiHinhDAO.DanhSachLoaiHinh();
            }
            ViewBag.TenLoaiHinhList = new SelectList(loaiHinhDuLichs, "MaLoaiHinh", "TenLoaiHinh");
           
            foreach(Tour t in tourList)
            {
                if (t.MaTour.Equals(MaTour))
                {

                    _unitRepository.tourModel = t;
                    _tour = new TourDAO(t);
                    _unitRepository.bangGiaVes = _tour.DanhSachGiaTour();
                    _unitRepository.diaDiemThamQuans = _tour.DanhSachDiaDiem();
                    _unitRepository.doanDuLiches = _tour.DanhSachDoan();
                    return View(_unitRepository);
                }
            }
            return View();
        }

        public IActionResult ThemGiaTour(string MaTour)
        {
            if (tourList.Count() == 0)
            {
                tourList = _tour.DanhSachTour();
            }
            foreach(Tour t in tourList)
            {
                if (t.MaTour.Equals(MaTour))
                {
                    ViewBag.Id = t.MaTour;
                }
            }
            
            return View();
        }

        [HttpPost]
        public IActionResult ThemGiaTour(BangGiaVe bangGiaVe)
        {
                if(bangGiaVe.ThoiGianBatDau > bangGiaVe.ThoiGianKetThuc)
                {
                    TempData["Message"] = "Thời gian kết thúc không được lớn hơn ngày bắt đầu!";
                    return RedirectToAction("ThemGiaTour", new { MaTour = bangGiaVe.MaTour });
                }
                else
                {
                    if (!isValidate((DateTime)bangGiaVe.ThoiGianBatDau, (DateTime)bangGiaVe.ThoiGianBatDau, bangGiaVe.MaTour))
                    {
                        TempData["Message"] = "Trùng khoảng thời gian, vui lòng kiểm tra lại!";
                        return RedirectToAction("ThemGiaTour", new { MaTour = bangGiaVe.MaTour });
                    }
                    else
                    {
                        
                        if (isSuccess = _bangGiaVeDAO.ThemGiaVe(bangGiaVe))
                        {
                            TempData["Message"] = "Thêm thành công";
                            return RedirectToAction("ChiTietTour", new { MaTour = bangGiaVe.MaTour });
                        }
                        else
                        {
                            TempData["Message"] = "Thêm thất bại";
                            return RedirectToAction("ChiTietTour", new { MaTour = bangGiaVe.MaTour });
                        }
                    }
                   
                }
        }

        public IActionResult ThemDiaDiem(string MaTour)
        {
            if (tourList.Count() == 0)
            {
                tourList = _tour.DanhSachTour();
            }
            foreach(Tour t in tourList)
            {
                if (t.MaTour.Equals(MaTour))
                {
                    ViewBag.Id = t.MaTour;
                    _tour = new TourDAO(t);
                    diaDiemThamQuans = _tour.DiaDiemComboBox();
                    ViewBag.diaDiemList = new SelectList(diaDiemThamQuans, "MaDiaDiem", "TenDiaDiem");
                }
            }

            return View();
        }

        [HttpPost]
        public IActionResult ThemDiaDiem(ChiTietDiaDiem chiTietDiaDiem)
        {
            if (isSuccess = _tour.ThemDiaDiem(chiTietDiaDiem))
            {
                TempData["Message"] = "Thêm thành công";
                return RedirectToAction("ChiTietTour", new { MaTour = chiTietDiaDiem.MaTour });
            }
            else
            {
                TempData["Message"] = "Thêm thất bại";
                return RedirectToAction("ChiTietTour", new { MaTour = chiTietDiaDiem.MaTour });
            }

        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        private bool isValidate(DateTime from, DateTime to,string MaTour)
        {
            
            if (tourList.Count() == 0)
            {
                tourList = _tour.DanhSachTour();
            }
            Tour tmp = new Tour();

            foreach(Tour t in tourList)
            {
                if (t.MaTour.Equals(MaTour))
                    tmp = t;
            }

            foreach (BangGiaVe giaVe in tmp.BangGiaVes)
            {
               
                if (from >= giaVe.ThoiGianBatDau && from <= giaVe.ThoiGianKetThuc)
                {
                    return false;
                }


                if (to >= giaVe.ThoiGianBatDau && to <= giaVe.ThoiGianKetThuc)
                {
                    return false;
                }

            }
            return true;
        }

        [HttpGet]
        public IActionResult SuaGiaTour(string MaTour,string MaBangGia)
        {
            if (tourList.Count() == 0)
            {
                tourList = _tour.DanhSachTour();
            }

            foreach (Tour t in tourList)
            {
                if (t.MaTour.Equals(MaTour))
                {
                    TourDAO tmp = new TourDAO(t);
                    bangGiaVes = tmp.DanhSachGiaTour();
                    foreach (BangGiaVe bg in bangGiaVes)
                    {
                        if (bg.MaBangGia == MaBangGia)
                        {
                            return View(bg);
                        }
                    }
                }
            }
            return View();
        }

        [HttpPost]
        public IActionResult SuaGiaTour(BangGiaVe bangGiaVe)
        {
            if (ModelState.IsValid)
            {
                if (_bangGiaVe.SuaGiaVe(bangGiaVe))
                {
                    TempData["Message"] = "Sửa thành công";
                    return RedirectToAction("ChiTietTour", new { MaTour = bangGiaVe.MaTour });
                }
                else
                {
                    TempData["Message"] = "Sửa thất bại";
                    return RedirectToAction("ChiTietTour", new { MaTour = bangGiaVe.MaTour });
                }
            }
            return View();
        }

        public IActionResult SuaDiaDiem(string MaTour,string MaDiaDiem)
        {
            if (tourList.Count() == 0)
            {
                tourList = _tour.DanhSachTour();
            }

            foreach (Tour t in tourList)
            {
                if (t.MaTour.Equals(MaTour))
                {
                    TourDAO tmp = new TourDAO(t);
                    chiTietDiaDiems = tmp.DanhSachDiaDiem();
                    ViewBag.listDiaDiem = new SelectList(chiTietDiaDiems, "Stt", "Stt");
                    foreach(ChiTietDiaDiem ct in chiTietDiaDiems)
                    {
                        if(ct.MaDiaDiem == MaDiaDiem)
                        {
                            _diemRepository.chiTietDiaDiem = ct;
                            return View(_diemRepository);
                        }
                    }
                }
            }
            return View();
        }

        [HttpPost]
        public IActionResult SuaDiaDiem(ChiTietDiaDiemRepository ct)
        {
            if (_tour.SuaDiaDiem(Int16.Parse(ct.sttMoi), ct.chiTietDiaDiem))
            {
                TempData["Message"] = "Thêm thành công";
                return RedirectToAction("ChiTietTour", new { MaTour = ct.chiTietDiaDiem.MaTour });
            }
            else
            {
                TempData["Message"] = "Thêm thất bại";
                return RedirectToAction("ChiTietTour", new { MaTour = ct.chiTietDiaDiem.MaTour });
            }
        }

        [HttpGet]
        public IActionResult XoaDiaDiem(string MaTour, string MaDiaDiem)
        {
            if (tourList.Count() == 0)
            {
                tourList = _tour.DanhSachTour();
            }
            foreach (Tour t in tourList)
            {
                if (t.MaTour.Equals(MaTour))
                {
                    _tour = new TourDAO(t);
                    chiTietDiaDiems = _tour.DanhSachDiaDiem();

                    foreach (ChiTietDiaDiem dd in chiTietDiaDiems)
                    {
                        if (dd.MaDiaDiem.Equals(MaDiaDiem))
                        {
                            if (_tour.XoaDiaDiem(dd))
                            {
                                TempData["Message"] = "Xoá thành công";
                                return RedirectToAction("ChiTietTour", new { MaTour = dd.MaTour });
                            }
                            else
                            {
                                TempData["Message"] = "Xoá thất bại";
                                return RedirectToAction("ChiTietTour", new { MaTour = dd.MaTour });
                            }
                        }
                    }
                }
            }
            return RedirectToAction("ChiTietTour", new { MaTour = MaTour });
        }

    }


}
