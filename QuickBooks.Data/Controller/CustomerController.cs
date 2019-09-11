using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QuickBooks.Data.Helper;
using QBFC13Lib;
using QuickBooks.Data.Entity;

namespace QuickBooks.Data.Controller
{
    public class CustomerController:QuickBooksManager
    {
        public List<CustomerEntity> GetAll()
        {
            // change to be list of dictionalry            
            var mapping = new QuickBooksXmlEntity()
            {
                Name = "Customer",
                Req = "CustomerQueryRq",
                Ret = new List<string>()
                {
                    "CustomerRet",
                }
            };

            string xml = ReadText(mapping);
            if (string.IsNullOrEmpty(xml))
                return null;


            int index = xml.IndexOf("<CustomerRet>");
            xml = xml.Substring(index);
            xml = xml.Replace("</CustomerQueryRs>", "")
                .Replace("</QBXMLMsgsRs>", "").Replace("</QBXML>", "");

            xml = xml.Replace("CustomerRet", "CustomerEntity");
            xml = $"<?xml version=\"1.0\" encoding=\"utf-16\"?><ArrayOfCustomerEntity xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\">{xml}</ArrayOfCustomerEntity>";

            var data = (List<CustomerEntity>)xml.ParseToObject(typeof(List<CustomerEntity>));
            return data;

        }

        public Dictionary<string, string> Create(CustomerEntity entity)
        {
            Dictionary<string, string> data = null;
            try
            {
                IMsgSetRequest requestMsgSet = QuickBooksConection();

                //add items 
                ICustomerAdd customerAdd = requestMsgSet.AppendCustomerAddRq();                
                if (!string.IsNullOrEmpty(entity.Name)) customerAdd.Name.SetValue(entity.Name);
                if (!string.IsNullOrEmpty(entity.CompanyName)) customerAdd.CompanyName.SetValue(entity.CompanyName);
                if (!string.IsNullOrEmpty(entity.Email)) customerAdd.Email.SetValue(entity.Email);
                if (!string.IsNullOrEmpty(entity.Phone)) customerAdd.Phone.SetValue(entity.Phone);
                if (entity.Balance > 0) customerAdd.OpenBalance.SetValue(entity.Balance.Value);
                if (entity.IsActive.HasValue) customerAdd.IsActive.SetValue(entity.IsActive.Value);

                // end add items
                data = QuickBooksExcuet(requestMsgSet);

            }
            catch (Exception ex)
            {
                data = new Dictionary<string, string>();
            }

            return data;

        }
        public Dictionary<string, string> Modify(string id, CustomerEntity entity)
        {
            Dictionary<string, string> data = null;
            try
            {
                IMsgSetRequest requestMsgSet = QuickBooksConection();

                //edit items 
                ICustomerMod customerMod = requestMsgSet.AppendCustomerModRq();
                customerMod.ListID.SetValue(id);
                customerMod.EditSequence.SetValue(entity.EditSequence);
                if (!string.IsNullOrEmpty(entity.Name)) customerMod.Name.SetValue(entity.Name);
                if (!string.IsNullOrEmpty(entity.CompanyName)) customerMod.CompanyName.SetValue(entity.CompanyName);
                if (!string.IsNullOrEmpty(entity.Email)) customerMod.Email.SetValue(entity.Email);
                if (!string.IsNullOrEmpty(entity.Phone)) customerMod.Phone.SetValue(entity.Phone);
                if (entity.IsActive.HasValue) customerMod.IsActive.SetValue(entity.IsActive.Value);
                // end edit items
                data = QuickBooksExcuet(requestMsgSet);

            }
            catch (Exception ex)
            {
                data = new Dictionary<string, string>();
            }

            return data;

        }
        public Dictionary<string, string> Delete(string id)
        {
            Dictionary<string, string> data = null;
            try
            {
                IMsgSetRequest requestMsgSet = QuickBooksConection();

                //edit items 
                IListDel listDel = requestMsgSet.AppendListDelRq();
                listDel.ListDelType.SetValue(ENListDelType.ldtCustomer);
                listDel.ListID.SetValue(id);
                // end edit items
                data = QuickBooksExcuet(requestMsgSet);

            }
            catch (Exception ex)
            {
                data = new Dictionary<string, string>();
            }

            return data;

        }



    }
}
