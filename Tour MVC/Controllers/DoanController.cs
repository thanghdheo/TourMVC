using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using QuanLyTour.DAO;
using QuanLyTour.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using Tour_MVC.Models;

namespace Tour_MVC.Controllers
{
    
    public class DoanController : Controller
    {
        private List<DoanDuLich> doanDuLiches;

        private List<ChiTietDoanKhach> khachDuLiches;

        private List<Tour> tours;

        private List<ChiPhi> chiPhis;

        private List<NhanVien> nhanViens;

        private IEnumerable<BangGiaVe> bangGiaVes;

        private Paginated<DoanDuLich> doanDuLichs;

        private DoanDuLichDAO _doan;

        private ChitietCpDAO _ctChiPhi;

        private ChiTietDoanKhachDAO _chiTietDoanKhach;

        private TourDAO _tour;

        private PhanBoDAO _phanBo;

        private DoanRepository _doanRepository;

        public DoanController()
        {
            doanDuLiches = new List<DoanDuLich>();
            chiPhis = new List<ChiPhi>();
            nhanViens = new List<NhanVien>();
            khachDuLiches = new List<ChiTietDoanKhach>();
            tours = new List<Tour>();   
            _doan = new DoanDuLichDAO();
            _tour = new TourDAO();
            _ctChiPhi = new ChitietCpDAO();
            _chiTietDoanKhach = new ChiTietDoanKhachDAO();
            _phanBo = new PhanBoDAO();
            _doanRepository = new DoanRepository();
        }
        // GET: DoanController
        public ActionResult Index(string sort = "", string search = "", int pg = 1, int pageSize = 5)
        {
            sortOrder sortOrder = new sortOrder();
            sortOrder.AddColum("stt");
            sortOrder.AddColum("tên đoàn du lịch");
            sortOrder.AddColum("doanh thu");
            sortOrder.AddColum("ngày khởi hành");
            sortOrder.AddColum("ngày kết thúc");
            sortOrder.AddColum("chi tiết chương trình");
            sortOrder.AddColum("tên tour");
            sortOrder.ApplySort(sort);
            ViewData["sortModel"] = sortOrder;
            ViewBag.search = search;

            doanDuLichs = _doanRepository.getItems(sortOrder.sortedStr, sortOrder.sortedOrder, search, pg, pageSize);

            //int toRecs = ((Paginated<Tour>)tours).TotalRecords;
            var paper = new PageModel(doanDuLichs.TotalRecords, pg, pageSize);

            paper.sort = sort;

            this.ViewBag.Paper = paper;


            return View(doanDuLichs);
        }

        // GET: DoanController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: DoanController/Create
        [HttpGet]
        public IActionResult ThemDoan()
        {
            if (tours.Count == 0)
            {
                tours = _tour.TourComboBox();
            }
            ViewBag.TenTourList = new SelectList(tours, "MaTour", "TenTour");

            return View();
        }

        [HttpPost]
        public IActionResult ThemDoan(DoanDuLich doan)
        {
            if (ModelState.IsValid)
            {
                DoanDuLich doann = new DoanDuLich();
                if (_doan.ThemDoan(doan)){
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

        public IActionResult SuaDoan(string MaSoDoan)
        {
            if (doanDuLiches.Count() == 0)
            {
                tours = _tour.TourComboBox();
                doanDuLiches = _doan.DanhSachDoan();
            }

            ViewBag.TenTourList = new SelectList(tours, "MaTour", "TenTour");

            for (int i = 0; i < doanDuLiches.Count; i++)
            {
                if (doanDuLiches[i].MaSoDoan.Equals(MaSoDoan))
                {

                    DoanDuLich doan = doanDuLiches[i];
                    ViewBag.gia = layGiaVe(doan.MaTour, (DateTime)doan.NgayKetThuc, (DateTime)doan.NgayKhoiHanh);
                    return View(doan);
                }
            }
           

            return View();
        }

        [HttpPost]
        public IActionResult SuaDoan(DoanDuLich doan)
        {
            if (_doan.SuaDoan(doan))
            {
                TempData["Message"] = "Sửa thành công";
                return RedirectToAction("ChiTietDoan", new { MaSoDoan = doan.MaSoDoan });
            }
            else
            {
                TempData["Message"] = "Sửa thất bại";
                return RedirectToAction("ChiTietDoan", new { MaSoDoan = doan.MaSoDoan });
            }
        }
        public IActionResult ChiTietDoan(string MaSoDoan)
        {
            if(doanDuLiches.Count() == 0)
            {
                doanDuLiches = _doan.DanhSachDoan();
            }

            for (int i = 0; i < doanDuLiches.Count; i++)
            {
                if (doanDuLiches[i].MaSoDoan.Equals(MaSoDoan))
                {

                    _doanRepository.doanModel = doanDuLiches[i];
                    _doan = new DoanDuLichDAO(doanDuLiches[i]);
                    _doanRepository.chitietCps = _doan.DanhSachChiPhi();
                    _doanRepository.phanBos = _doan.DanhSachPhanBo();
                    _doanRepository.khachDuLiches = _doan.DanhSachKhach();
                    ViewBag.khachCount = _doan.DanhSachKhach().Count;
                    return View(_doanRepository);
                }
            }
            return View();
        }

       

        public IActionResult ThemChiPhi(string MaSoDoan)
        {
            if (doanDuLiches.Count() == 0)
            {
                doanDuLiches = _doan.DanhSachDoan();
            }
            for (int i = 0; i < doanDuLiches.Count(); i++)
            {
                if (doanDuLiches[i].MaSoDoan.Equals(MaSoDoan))
                {
                    ViewBag.Id = doanDuLiches[i].MaSoDoan;
                    _doan = new DoanDuLichDAO(doanDuLiches[i]);
                    chiPhis = _doan.ChiPhiComboBox();
                    ViewBag.chiPhiList = new SelectList(chiPhis, "MaChiPhi", "TenChiPhi");
                }
            }
            return View();
        }

        [HttpPost]
        public IActionResult ThemChiPhi(ChitietCp cp)
        {
            if (ModelState.IsValid)
            {
                if (_ctChiPhi.Them(cp))
                {
                    TempData["Message"] = "Thêm thành công";
                    return RedirectToAction("ChiTietDoan", new { MaSoDoan = cp.MaSoDoan });
                }
                else
                {
                    TempData["Message"] = "Thêm thất bại";
                    return RedirectToAction("ChiTietDoan", new { MaSoDoan = cp.MaSoDoan });
                }
            }
            return View();
        }

        public IActionResult ThemPhanBo(string MaSoDoan)
        {
            if (doanDuLiches.Count() == 0)
            {
                doanDuLiches = _doan.DanhSachDoan();
            }
            for (int i = 0; i < doanDuLiches.Count(); i++)
            {
                if (doanDuLiches[i].MaSoDoan.Equals(MaSoDoan))
                {
                    ViewBag.Id = doanDuLiches[i].MaSoDoan;
                    _doan = new DoanDuLichDAO(doanDuLiches[i]);
                    nhanViens = _doan.NhanVienComboBox();
                    ViewBag.nhanvienList = new SelectList(nhanViens, "MaNhanVien", "TenNhanVien");
                }
            }
            return View();
        }

        [HttpPost]
        public IActionResult ThemPhanBo(PhanBo pb)
        {
            if (ModelState.IsValid)
            {
                if (_phanBo.Them(pb))
                {
                    TempData["Message"] = "Thêm thành công";
                    return RedirectToAction("ChiTietDoan", new { MaSoDoan = pb.MaSoDoan });
                }
                else
                {
                    TempData["Message"] = "Thêm thất bại";
                    return RedirectToAction("ChiTietDoan", new { MaSoDoan = pb.MaSoDoan });
                }
            }
            return View();
        }

        public IActionResult SuaKhachHang(string MaKhachHang)
        {
            if (khachDuLiches.Count == 0)
            {
                khachDuLiches = _doan.DanhSachKhachHang();
            }
            for (int i = 0; i < khachDuLiches.Count; i++)
            {
                if (khachDuLiches[i].MaKhachHang.Equals(MaKhachHang))
                {

                    ChiTietDoanKhach tmp = khachDuLiches[i];
                    return View(tmp);
                }
            }
            return View();
        }

        [HttpPost]
        public IActionResult SuaKhachHang(ChiTietDoanKhach chiTiet)
        {
            if (ModelState.IsValid)
            {
                if (_chiTietDoanKhach.Sua(chiTiet))
                {
                    TempData["Message"] = "Sửa thành công";
                    return RedirectToAction("ChiTietDoan", new { MaSoDoan = chiTiet.MaSoDoan });
                }
                else
                {
                    TempData["Message"] = "Sửathất bại";
                    return RedirectToAction("ChiTietDoan", new { MaSoDoan = chiTiet.MaSoDoan });
                }
            }
            return View();
        }


        public JsonResult ajaxGetGiaVe(string MaTour)
        {
            bangGiaVes = new BangGiaVeDAO().DanhSachGiaVe().Where(i => i.MaTour == MaTour);
            return Json(bangGiaVes);
        }

        public string ajaxKiemTraNgayBatDau(string MaTour,DateTime ThoiGianBatDau, DateTime ThoiGianKetThuc)
        {
            return layGiaVe(MaTour, ThoiGianKetThuc, ThoiGianBatDau);
        }
        public string ajaxKiemTraNgayKetThuc(string MaTour, DateTime ThoiGianKetThuc, DateTime ThoiGianBatDau)
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
