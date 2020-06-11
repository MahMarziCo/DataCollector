using System.Collections.Generic;
using Mah.DataCollector.Entity.Entities;

namespace Mah.DataCollector.Interface.Dto.Feature
{
    public class FeatureEditModel
    {
        public Classes Class;
        public int ObjectId;
        public List<FieldsValueModel> FieldsValue;
    }

    public class FieldsValueModel
    {
        public FieldsValueModel(Fields pField, object pValues, List<string> pDomains, bool pDomainIsMultiSelect, bool pIsDefault)
        {
            field = pField;
            value = pValues;
            domains = pDomains;
            domainIsMultiSelect = pDomainIsMultiSelect;
            isDefault = pIsDefault;
        }
        public object value;
        public Fields field;
        public List<string> domains;
        public bool domainIsMultiSelect;
        public bool isDefault = false;
    }

}