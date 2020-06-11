using Mah.DataCollector.Entity.Entities;
using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Objects;
using System.Linq;

namespace DataAccess.Logic
{
    public class UserLocationBL
    {
        private DataCollectorContext _DbContext;
        public UserLocationBL(DataCollectorContext context)
        {
            _DbContext = context;
        }
        public bool LogUserLocation(string userName, double[] coords)
        {
            try
            {

                User_location userLocation = _DbContext.User_location.Create();
                userLocation.DateTime = DateTime.Now;
                userLocation.User_name = userName;
                userLocation.Coordinate = System.Data.Entity.Spatial.DbGeometry.FromText(string.Format("POINT({0} {1})", coords[0], coords[1]), 4326);
                _DbContext.User_location.Add(userLocation);
                _DbContext.SaveChanges();

                return true;
            }
            catch { return false; }
        }
        public List<UserLocation> UsersLastLocation()
        {
            List<UserLocation> user = new List<UserLocation>();

            foreach (var item in _DbContext.User_location.GroupBy(group => group.User_name)
                         .SelectMany(
                             group =>
                                 group.Where(
                                     r => r.DateTime == group.Max(x => x.DateTime))
                                     .Select(r => new { r.User_name, r.DateTime, r.Coordinate })).ToList())
            {
                if (item.User_name != "SYSADMIN")
                {
                    user.Add(new UserLocation()
                    {
                        User_name = item.User_name,
                        DateTime = item.DateTime,
                        Coordinate = string.Format("{0},{1}", item.Coordinate.PointAt(1).XCoordinate, item.Coordinate.PointAt(1).YCoordinate)
                    });
                }
            }

            return user;
        }

        public List<UserLocation> UsersLocationTrack(string UserName, DateTime date)
        {
            List<UserLocation> user = new List<UserLocation>();

            foreach (var item in _DbContext.User_location.Where(a => EntityFunctions.TruncateTime(a.DateTime) == EntityFunctions.TruncateTime(date) && a.User_name == UserName).OrderBy(a => a.DateTime)
                                     .Select(r => new { r.User_name, r.DateTime, r.Coordinate }).ToList())
            {
                if (item.User_name != "SYSADMIN")
                {
                    user.Add(new UserLocation()
                    {
                        User_name = item.User_name,
                        DateTime = item.DateTime,
                        Coordinate = string.Format("{0},{1}", item.Coordinate.PointAt(1).XCoordinate, item.Coordinate.PointAt(1).YCoordinate)
                    });
                }
            }

            return user;
        }
    }

    public class UserLocation
    {
        public string User_name { get; set; }

        public string Coordinate { get; set; }

        public DateTime? DateTime { get; set; }
    }
}
