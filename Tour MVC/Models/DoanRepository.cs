using QuanLyTour.DAO;
using QuanLyTour.Models;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;

namespace Tour_MVC.Models
{
    public class DoanRepository
    {
        public DoanDuLichDAO doan;

        public DoanDuLich doanModel { get; set; }

        public IEnumerable<ChitietCp> chitietCps { set; get; }

        public IEnumerable<ChiTietDoanKhach> khachDuLiches { set; get; }

        public IEnumerable<PhanBo> phanBos { set; get; }

        public IEnumerable<Tour> tours { set; get; }

        public DoanRepository()
        {
            doan = new DoanDuLichDAO();
        }

        private List<DoanDuLich> doSort(List<DoanDuLich> doans, string sort, SortOrder sortOrder)
        {
            if (sort.ToLower() == "tendoan" || sort.ToLower() == "stt" || sort.ToLower() == "dt" || sort.ToLower() == "ngkh"
                || sort.ToLower() == "ngkt" || sort.ToLower() == "tenct" || sort.ToLower() == "tentour")
            {
                if (sortOrder == SortOrder.Ascending)
                {
                    doans = doans.OrderBy(s => s.MaSoDoan).ToList();
                }
                else
                {
                    doans = doans.OrderByDescending(s => s.MaSoDoan).ToList();
                }
            }
            else
            {
                if (sortOrder == SortOrder.Ascending)
                {
                    doans = doans.OrderBy(s => s.MaSoDoan).ToList();
                }
                else
                {
                    doans = doans.OrderByDescending(s => s.MaSoDoan).ToList();
                }

            }
            return doans;
        }

        public Paginated<DoanDuLich> getItems(string sort, SortOrder sortOrder, string search = "", int pageIndex = 1, int pageSize = 5)
        {
            List<DoanDuLich> doans;

            if (search != "" && search != null)
            {
                doans = doan.DanhSachDoan().Where(n => n.TenDoanDuLich.Trim().ToLower()!.Contains(search.Trim().ToLower()) || n.MaTourNavigation.TenTour.Trim().ToLower()!.Contains(search.Trim().ToLower())).ToList();
            }
            else
                doans = doan.DanhSachDoan();


            doans = doSort(doans, sort, sortOrder);

            Paginated<DoanDuLich> reDoans = new Paginated<DoanDuLich>(doans, pageIndex, pageSize);

            return reDoans;
        }
    }
}
