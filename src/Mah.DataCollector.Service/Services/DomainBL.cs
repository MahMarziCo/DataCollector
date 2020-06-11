using Mah.DataCollector.Entity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Logic
{
    public class DomainBL
    {
        private DataCollectorContext _DbContext;
        public DomainBL(DataCollectorContext context)
        {
            _DbContext = context;
        }
        public List<string> GetFieldsDomainValue(Fields pField)
        {
            if (pField.Domain_ID == null)
            {
                return null;
            }
            return _DbContext.DomainValue.Where(d => d.Domain_ID == pField.Domain_ID).OrderBy(a => a.Value).Select(a => a.Value).ToList();

        }
        public bool IsDomainMultiSelect(Fields pField)
        {
            if (pField.Domain_ID == null)
            {
                return false;
            }
            return _DbContext.Domain.Where(d => d.ID == pField.Domain_ID).FirstOrDefault().MultiSelect ?? false;

        }

        public List<Domain> GetAllDomains()
        {
            List<Domain> oList = new List<Domain>();
            oList.Add(new Domain() { ID = -1, Caption = "" });
            try
            {
                foreach (var item in _DbContext.Domain.AsNoTracking().ToList())
                {
                    oList.Add(new Domain()
                    {
                        ID = item.ID,
                        DomainValue = item.DomainValue,
                        Caption = item.Caption,
                        MultiSelect = item.MultiSelect
                    }
                        );
                }

            }
            catch
            {

            }
            return oList;
        }

        public List<DomainValue> GetDomainValues(int pDomainID)
        {
            List<DomainValue> oList = new List<DomainValue>();
            try
            {

                return _DbContext.DomainValue.Where(a => a.Domain_ID == pDomainID).ToList();
            }
            catch
            {

            }
            return oList;
        }

        public int AddValueToDomain(string pDomainValue, int? pDomainId)
        {
            int oID = -1;
            try
            {
                Domain oDomain = _DbContext.Domain.Where(a => a.ID == pDomainId).First();
                if (oDomain != null)
                {
                    DomainValue oDomainValue = _DbContext.DomainValue.Create();
                    oDomainValue.Domain_ID = pDomainId;
                    oDomainValue.Value = pDomainValue;
                    _DbContext.DomainValue.Add(oDomainValue);
                    oID = oDomainValue.ID;
                }
                _DbContext.SaveChanges();


            }
            catch
            {
                oID = -1;
            }
            return oID;
        }
        public bool UpdateDomainValue(DomainValue pDomainValue)
        {
            try
            {
                _DbContext.DomainValue.Attach(pDomainValue);
                _DbContext.Entry(pDomainValue).State = System.Data.Entity.EntityState.Modified;
                _DbContext.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        public bool DeleteDomainValue(int pDomainValueID)
        {
            try
            {
                DomainValue oDomainValue = new DomainValue { ID = pDomainValueID };
                _DbContext.DomainValue.Attach(oDomainValue);

                _DbContext.DomainValue.Remove(oDomainValue);
                _DbContext.SaveChanges();

                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool DeleteDomain(int pID)
        {
            try
            {
                Domain oDomain = new Domain { ID = pID };
                _DbContext.Domain.Attach(oDomain);

                _DbContext.Domain.Remove(oDomain);
                _DbContext.SaveChanges();

                return true;
            }
            catch
            {
                return false;
            }
        }

        public int AddDomain(Domain pDomainValue)
        {
            int id = -1;
            try
            {

                Domain oDomain = _DbContext.Domain.Create();
                oDomain.Caption = pDomainValue.Caption;
                oDomain.MultiSelect = pDomainValue.MultiSelect;

                _DbContext.Domain.Add(oDomain);
                _DbContext.SaveChanges();
                id = oDomain.ID;


            }
            catch
            {
                id = -1;
            }
            return id;
        }

        public bool UpdateDomain(Domain pDomainValue)
        {
            try
            {

                Domain oDomain = new Domain { ID = pDomainValue.ID };
                _DbContext.Domain.Attach(oDomain);

                oDomain.Caption = pDomainValue.Caption;
                oDomain.MultiSelect = pDomainValue.MultiSelect;

                _DbContext.SaveChanges();

                return true;
            }
            catch
            {
            }
            return false;
        }

    }
}
