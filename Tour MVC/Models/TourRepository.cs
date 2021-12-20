
using QuanLyTour.DAO;
using QuanLyTour.Models;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;

namespace Tour_MVC.Models
{
    public class TourRepository
    {
        private TourDAO tour;

        public Tour tourModel { get; set; }

        public IEnumerable<BangGiaVe> bangGiaVes { get; set; }

        public IEnumerable<ChiTietDiaDiem> diaDiemThamQuans { get; set; }

        public IEnumerable<DoanDuLich> doanDuLiches { get; set; }
        public IEnumerable<KhachDuLich> khachDuLiches { get; set; }
        public TourRepository()
        {
            tour = new TourDAO();
        }

        private List<Tour> doSort(List<Tour> tours,string sort, SortOrder sortOrder)
        {
            if (sort.ToLower() == "tentour" || sort.ToLower() == "stt" || sort.ToLower() == "desc" || sort.ToLower() == "namelh")
            {
                if (sortOrder == SortOrder.Ascending)
                {
                    tours = tours.OrderBy(s => s.MaTour).ToList();
                }
                else
                {
                    tours = tours.OrderByDescending(s => s.MaTour).ToList();
                }
            }
            else
            {
                if (sortOrder == SortOrder.Ascending)
                {
                    tours = tours.OrderBy(s => s.MaTour).ToList();
                }
                else
                {
                    tours = tours.OrderByDescending(s => s.MaTour).ToList();
                }

            }
            return tours;
        }

        public Paginated<Tour> getItems(string sort, SortOrder sortOrder,string search="",int pageIndex=1,int pageSize=5)
        {
            List<Tour> tours;

            if(search != "" &&search !=null)
            {
                tours = tour.DanhSachTour().Where(n => n.TenTour.Trim().ToLower()!.Contains(search.Trim().ToLower()) || n.MaLoaiHinhNavigation.TenLoaiHinh.Trim().ToLower()!.Contains(search.Trim().ToLower())).ToList();
            }
            else
               tours = tour.DanhSachTour();
          

            tours = doSort(tours,sort, sortOrder);

            Paginated<Tour> reTours = new Paginated<Tour>(tours, pageIndex, pageSize);

            return reTours;
        }
    }
   
}
