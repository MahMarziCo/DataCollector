using Mah.DataCollector.Entity.Entities;
using System;
using System.Data.SqlClient;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using Mah.Common.Encrypt;

namespace DataAccess.Logic
{
    public class SettingBL
    {
        private DataCollectorContext _DbContext;
        private string _GdbConnection;
        private Cryptor _Cryptor;
        public SettingBL(DataCollectorContext context, Cryptor cryptor, string gdbConnection)
        {
            _DbContext = context;
            _GdbConnection = gdbConnection;
            _Cryptor = cryptor;

        }
        public enum SettingParameters
        {
            MapDefCentroidX,
            MapDefCentroidY,
            MapDefultZoom,
            NeedUserPositionLog,
            ExpireDateTime,
            LicenseText,
            SnapTolorance
        }

        public bool insertSetting(SettingParameters pset_name, object value, string userName)
        {
            string set_name = pset_name.ToString();
            string type = null;
            string set_val = value.ToString();
            if (value.GetType() == typeof(string))
                type = typeof(string).ToString();
            else if (value.GetType() == typeof(DateTime))
            {
                type = typeof(DateTime).ToString();
                set_val = ((DateTime)value).ToString("yyyyMMdd-H:mm:ss");
            }
            else if (value.GetType() == typeof(double))
                type = typeof(double).ToString();
            else if (value.GetType() == typeof(int))
                type = typeof(int).ToString();
            else if (value.GetType() == typeof(bool))
                type = typeof(bool).ToString();
            if (string.IsNullOrEmpty(type))
                return false;

            try
            {
                if (_DbContext.TB_Setting.Where(a => a.SET_Key.ToUpper() == set_name.ToUpper() && (a.User_name == "SYSTEM" || a.User_name == userName.ToUpper())).Count() > 0)
                {
                    TB_Setting setting = _DbContext.TB_Setting.Where(a => a.SET_Key.ToUpper() == set_name.ToUpper()).First();
                    setting.Value = _Cryptor.Encrypt(set_val);
                    setting.SET_Type = type;
                    setting.User_name = userName.ToUpper();
                    _DbContext.SaveChanges();
                }
                else
                {
                    TB_Setting setting = new TB_Setting();
                    setting.Value = _Cryptor.Encrypt(set_val);
                    setting.SET_Type = type;
                    setting.SET_Key = set_name.ToUpper();
                    setting.User_name = userName.ToUpper();
                    _DbContext.TB_Setting.Add(setting);
                    _DbContext.SaveChanges();
                }
                return true;
            }
            catch
            {
                return false;
            }
        }
        public bool getSettingAsBool(SettingParameters pset_name, string userId)
        {
            string set_name = pset_name.ToString();
            bool value = false;
            try
            {

                if (_DbContext.TB_Setting.Where(a => a.SET_Key.ToUpper() == set_name.ToUpper()).Count() > 0)
                {
                    TB_Setting setting = _DbContext.TB_Setting.Where(a => a.SET_Key.ToUpper() == set_name.ToUpper() && (a.User_name == "SYSTEM" || a.User_name == userId.ToUpper())).First();

                    if (setting.SET_Type == typeof(bool).ToString())
                    {
                        string set_val = _Cryptor.Decrypt(setting.Value);
                        value = bool.Parse(set_val);
                    }
                }
            }
            catch { }
            return value;
        }

        public string getSettingAsString(SettingParameters pset_name, string userId)
        {
            string set_name = pset_name.ToString();
            string value = "";
            try
            {
                if (_DbContext.TB_Setting.Where(a => a.SET_Key.ToUpper() == set_name.ToUpper()).Count() > 0)
                {
                    TB_Setting setting = _DbContext.TB_Setting.Where(a => a.SET_Key.ToUpper() == set_name.ToUpper() && (a.User_name == "SYSTEM" || a.User_name == userId.ToUpper())).First();

                    if (setting.SET_Type == typeof(string).ToString())
                    {
                        string set_val = _Cryptor.Decrypt(setting.Value);
                        value = set_val;
                    }
                }
            }
            catch { }
            return value;
        }


        public int getSettingAsInt(SettingParameters pset_name, string userId)
        {
            string set_name = pset_name.ToString();
            int value = -1;
            try
            {
                if (_DbContext.TB_Setting.Where(a => a.SET_Key.ToUpper() == set_name.ToUpper()).Count() > 0)
                {
                    TB_Setting setting = _DbContext.TB_Setting.Where(a => a.SET_Key.ToUpper() == set_name.ToUpper() && (a.User_name == "SYSTEM" || a.User_name == userId.ToUpper())).First();

                    if (setting.SET_Type == typeof(int).ToString())
                    {
                        string set_val = _Cryptor.Decrypt(setting.Value);
                        value = int.Parse(set_val);
                    }
                }
            }
            catch { }
            return value;
        }

        public double getSettingAsDouble(SettingParameters pset_name, string userId)
        {
            string set_name = pset_name.ToString();
            double value = -1;
            try
            {

                if (_DbContext.TB_Setting.Where(a => a.SET_Key.ToUpper() == set_name.ToUpper()).Count() > 0)
                {
                    TB_Setting setting = _DbContext.TB_Setting.Where(a => a.SET_Key.ToUpper() == set_name.ToUpper() && (a.User_name == "SYSTEM" || a.User_name == userId.ToUpper())).First();

                    if (setting.SET_Type == typeof(double).ToString())
                    {
                        string set_val = _Cryptor.Decrypt(setting.Value);
                        value = double.Parse(set_val);
                    }
                }
            }
            catch (Exception ex) { }
            return value;
        }

        public DateTime? getSettingAsDate(SettingParameters pset_name, string userId)
        {
            string set_name = pset_name.ToString();
            DateTime? value = null;
            try
            {

                if (_DbContext.TB_Setting.Where(a => a.SET_Key.ToUpper() == set_name.ToUpper()).Count() > 0)
                {
                    TB_Setting setting = _DbContext.TB_Setting.Where(a => a.SET_Key.ToUpper() == set_name.ToUpper() && (a.User_name == "SYSTEM" || a.User_name == userId.ToUpper())).First();

                    if (setting.SET_Type == typeof(DateTime).ToString())
                    {
                        string set_val = _Cryptor.Decrypt(setting.Value);
                        value = DateTime.ParseExact(set_val, "yyyyMMdd-H:mm:ss", null);
                    }
                }

            }
            catch { }
            return value;
        }


        public void ConfigSpatialLogic()
        {
            using (SqlConnection cnn = new SqlConnection(_GdbConnection))
            {
                cnn.Open();

                using (SqlCommand cmd = cnn.CreateCommand())
                {
                    cmd.CommandType = System.Data.CommandType.Text;
                    cmd.CommandText = "EXEC	[InsertTRIGGER]";

                    cmd.ExecuteNonQuery();
                }
            }
        }

    }
}
