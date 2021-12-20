using QuanLyTour.DAO;
using QuanLyTour.Models;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;

namespace Tour_MVC.Models
{
    public class KhachHangRepository
    {
        public KhachHangDAO khachHang;



        public KhachHangRepository()
        {
            khachHang = new KhachHangDAO();
        }

        private List<KhachDuLich> doSort(List<KhachDuLich> khachDuLichs, string sort, SortOrder sortOrder)
        {
            if (sort.ToLower() == "tenkh" || sort.ToLower() == "cmnd" || sort.ToLower() == "dc" || sort.ToLower() == "gt"
                || sort.ToLower() == "sdt" || sort.ToLower() == "qt")
            {
                if (sortOrder == SortOrder.Ascending)
                {
                    khachDuLichs = khachDuLichs.OrderBy(s => s.MaKhachHang).ToList();
                }
                else
                {
                    khachDuLichs = khachDuLichs.OrderByDescending(s => s.MaKhachHang).ToList();
                }
            }
            else
            {
                if (sortOrder == SortOrder.Ascending)
                {
                    khachDuLichs = khachDuLichs.OrderBy(s => s.MaKhachHang).ToList();
                }
                else
                {
                    khachDuLichs = khachDuLichs.OrderByDescending(s => s.MaKhachHang).ToList();
                }

            }
            return khachDuLichs;
        }

        public Paginated<KhachDuLich> getItems(string sort, SortOrder sortOrder, string search = "", int pageIndex = 1, int pageSize = 5)
        {
            List<KhachDuLich> khachDuLichs;

            if (search != "" && search != null)
            {
                khachDuLichs = khachHang.DanhSachKhach().Where(n => n.TenKhachHang.Trim().ToLower()!.Contains(search.Trim().ToLower()) || n.Sdt.Trim().ToLower()!.Contains(search.Trim().ToLower())
                                || n.Cmnd.Trim().ToLower()!.Contains(search.Trim().ToLower()) || n.DiaChi.Trim().ToLower()!.Contains(search.Trim().ToLower()) || n.QuocTich.Trim().ToLower()!.Contains(search.Trim().ToLower())).ToList();
            }
            else
                khachDuLichs = khachHang.DanhSachKhach();


            khachDuLichs = doSort(khachDuLichs, sort, sortOrder);

            Paginated<KhachDuLich> reKhachDuLichs = new Paginated<KhachDuLich>(khachDuLichs, pageIndex, pageSize);

            return reKhachDuLichs;
        }
    }
}
