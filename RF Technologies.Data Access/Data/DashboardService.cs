using ResortManagement.Web.ViewModels;
using RF_Technologies.Data_Access.Repository.IRepository;
using RF_Technologies.Utility;

namespace RF_Technologies.Data_Access.Data
{
    public class DashboardService : IDashboardService
    {
        private readonly IUnitOfWork _unitOfWork;
        static int previousMonth = DateTime.Now.Month == 1 ? 12 : DateTime.Now.Month - 1;
        readonly DateTime previousMonthStartDate = new(DateTime.Now.Year, previousMonth, 1);
        readonly DateTime currentMonthStartDate = new(DateTime.Now.Year, DateTime.Now.Month, 1);

        public DashboardService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<LineChartDto> GetMemberAndBookingLineChartData()
        {
            var registrationData = _unitOfWork.RegistrationForm.GetAll(u => u.RegistrationDate >= DateTime.Now.AddDays(-30) &&
           u.RegistrationDate.Date <= DateTime.Now)
               .GroupBy(b => b.RegistrationDate)
               .Select(u => new
               {
                   DateTime = u.Key,
                   NewRegistrationCount = u.Count()
               });

            var customerData = _unitOfWork.User.GetAll(u => u.CreatedAt >= DateTime.Now.AddDays(-30) &&
           u.CreatedAt.Date <= DateTime.Now)
               .GroupBy(b => b.CreatedAt.Date)
               .Select(u => new
               {
                   DateTime = u.Key,
                   NewCustomerCount = u.Count()
               });

            var leftJoin = registrationData.GroupJoin(customerData, booking => booking.DateTime, customer => customer.DateTime,
                (booking, customer) => new
                {
                    booking.DateTime,
                    booking.NewRegistrationCount,
                    NewCustomerCount = customer.Select(x => x.NewCustomerCount).FirstOrDefault()
                });

            var rightJoin = customerData.GroupJoin(registrationData, customer => customer.DateTime, booking => booking.DateTime,
                (customer, booking) => new
                {
                    customer.DateTime,
                    NewRegistrationCount = booking.Select(x => x.NewRegistrationCount).FirstOrDefault(),
                    customer.NewCustomerCount
                });

            var mergedData = leftJoin.Union(rightJoin).OrderBy(x => x.DateTime).ToList();

            var newBookingData = mergedData.Select(x => x.NewRegistrationCount).ToArray();
            var newCustomerData = mergedData.Select(x => x.NewCustomerCount).ToArray();
            var categories = mergedData.Select(x => x.DateTime.ToString("MM/dd/yyyy")).ToArray();

            List<ChartData> chartDataList = new()
            {
                new ChartData
                {
                    Name = "New Registration",
                    Data = newBookingData
                },
                new ChartData
                {
                    Name = "New Students",
                    Data = newCustomerData
                },
            };

            LineChartDto lineChartDto = new()
            {
                Categories = categories,
                Series = chartDataList
            };

            return lineChartDto;
        }


        public async Task<PieChartDto> GetBookingPieChartData()
        {
            var totalregistration = _unitOfWork.RegistrationForm.GetAll(u => u.RegistrationDate >= DateTime.Now.AddDays(-30) &&
           (u.Status != SD.StatusPending || u.Status == SD.StatusCancelled));

            var studentWithOneRegistration = totalregistration.GroupBy(b => b.UserId).Where(x => x.Count() == 1).Select(x => x.Key).ToList();

            int registrationByNewStudent = studentWithOneRegistration.Count();
            int registrationByReturingStudent = totalregistration.Count() - registrationByNewStudent;

            PieChartDto pieChartDto = new()
            {
                Labels = new string[] { "New Student Registration", "Returing Student Registration" },
                Series = new decimal[] { registrationByNewStudent, registrationByReturingStudent }
            };
            return pieChartDto;
        }


        public async Task<RadialBarChartDto> GetRegisteredUserChartData()
        {
            var totalUsers = _unitOfWork.User.GetAll();

            var countByCurrentMonth = totalUsers.Count(u => u.CreatedAt >= currentMonthStartDate && u.CreatedAt <= DateTime.Now);

            var countByPreviousMonth = totalUsers.Count(u => u.CreatedAt >= previousMonthStartDate && u.CreatedAt <= currentMonthStartDate);

            return SD.GetRadialChartDataModel(totalUsers.Count(), countByCurrentMonth, countByPreviousMonth);

        }

        //public async Task<RadialBarChartDto> GetRevenueChartData()
        //{
        //    var totalBookings = _unitOfWork.RegistrationForm.GetAll(u => u.Status != SD.StatusPending || u.Status == SD.StatusCancelled);
        //    var totalRevenue = Convert.ToInt32(totalBookings.Sum(u => u.TotalCost));

        //    var countByCurrentMonth = totalBookings.Where(u => u.BookingDate >= currentMonthStartDate && u.BookingDate <= DateTime.Now).Sum(u => u.TotalCost);

        //    var countByPreviousMonth = totalBookings.Where(u => u.BookingDate >= previousMonthStartDate && u.BookingDate <= currentMonthStartDate).Sum(u => u.TotalCost);

        //    return SD.GetRadialChartDataModel(totalRevenue, countByCurrentMonth, countByPreviousMonth);
        //}

        public async Task<RadialBarChartDto> GetTotalBookingRadialChartData()
        {
            var totalregistration = _unitOfWork.RegistrationForm.GetAll(u => u.Status != SD.StatusPending || u.Status == SD.StatusCancelled);

            var countByCurrentMonth = totalregistration.Count(u => u.RegistrationDate >= currentMonthStartDate && u.RegistrationDate <= DateTime.Now);

            var countByPreviousMonth = totalregistration.Count(u => u.RegistrationDate >= previousMonthStartDate && u.RegistrationDate <= currentMonthStartDate);

            return SD.GetRadialChartDataModel(totalregistration.Count(), countByCurrentMonth, countByPreviousMonth);
        }
    }
}
