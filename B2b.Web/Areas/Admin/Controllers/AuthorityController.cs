using B2b.Web.v4.Areas.Admin.Models;
using B2b.Web.v4.Models.EntityLayer;
using B2b.Web.v4.Models.Helper;
using B2b.Web.v4.Models.Log;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;
namespace B2b.Web.v4.Areas.Admin.Controllers
{
    public class AuthorityController : AdminBaseController
    {
        // GET: Admin/Authority
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Definition()
        {
            return View();
        }

        [HttpPost]
        public JsonResult GetListSalesman()
        {

            var list = Salesman.GetList();
            return Json(list);
        }

        [HttpPost]
        public JsonResult GetAuthorityGroupHeader()
        {
            List<AuthorityGroupHeader> list = AuthorityGroupHeader.GetList();
            return Json(list);
        }

        [HttpPost]
        public JsonResult GetAuthorityDefinitionGroup()
        {
            List<AuthorityDefinitionGroup> list = AuthorityDefinitionGroup.GetList();
            return Json(list);
        }

        [HttpPost]
        public JsonResult GetDefinitionList(int definitionGroupId)
        {
            List<AuthorityDefinition> list = AuthorityDefinition.GetList(definitionGroupId);
            return Json(list);
        }

        [HttpPost]
        public JsonResult SaveAuthorityGroupHeader(string name)
        {
             bool result = false;

            AuthorityGroupHeader item = new AuthorityGroupHeader()
            {
                Name = name,
                CreateId = AdminCurrentSalesman.Id
            };
            result = item.Add();

            var message = result ? new MessageBox(MessageBoxType.Success, "İşleminiz Gerçekleştirilmiştir .") : new MessageBox(MessageBoxType.Error, "İşleminizde Hata Gerçekleşmiştir.");
            return Json(message);

        }


        [HttpPost]
        public JsonResult SaveAuthorityDefinition(string[] groupList, string[] userList, AuthorityDefinitionGroup definitionGroup)
        {
            bool result = false;


            AuthorityDefinition deleteItem = new AuthorityDefinition();
            deleteItem.DefinitionGroupId = definitionGroup.Id;
            deleteItem.EditId = AdminCurrentSalesman.Id;
            deleteItem.Delete();

            for (int i = 0; i < groupList.Length; i++)
            {
                for (int j = 0; j < userList.Length; j++)
                {
                    AuthorityDefinition item = new AuthorityDefinition();
                    item.AuthorityGroupHeaderId = Convert.ToInt32(groupList[i]);
                    item.SalesmanId = Convert.ToInt32(userList[j]);
                    item.DefinitionGroupId = definitionGroup.Id;
                    result = item.Add();
                }

            }

            //AuthorityGroupHeader item = new AuthorityGroupHeader()
            //{
            //    Name = name,
            //    CreateId = AdminCurrentSalesman.Id
            //};
            //result = item.Add();

            var message = result ? new MessageBox(MessageBoxType.Success, "İşleminiz Gerçekleştirilmiştir .") : new MessageBox(MessageBoxType.Error, "İşleminizde Hata Gerçekleşmiştir.");
            return Json(message);

        }


        [HttpPost]
        public JsonResult AddOrUpdateDefinitionGroup(AuthorityDefinitionGroup definitionGroup)
        {
             bool result = false;

            if (definitionGroup.Id == 0)
            {
                definitionGroup.CreateId = AdminCurrentSalesman.Id;
                result = definitionGroup.Add();
            }
            else
            {
                definitionGroup.EditId = AdminCurrentSalesman.Id;
                result = definitionGroup.Update();
            }

            var message = result ? new MessageBox(MessageBoxType.Success, "İşleminiz Gerçekleştirilmiştir .") : new MessageBox(MessageBoxType.Error, "İşleminizde Hata Gerçekleşmiştir.");
            return Json(message);

        }

        [HttpPost]
        public JsonResult DeleteAuthorityGroupHeader(int id)
        {
             bool result = false;

            AuthorityGroupHeader item = new AuthorityGroupHeader()
            {
                Id = id,
                EditId = AdminCurrentSalesman.Id
            };
            result = item.Delete();

            var message = result ? new MessageBox(MessageBoxType.Success, "İşleminiz Gerçekleştirilmiştir .") : new MessageBox(MessageBoxType.Error, "İşleminizde Hata Gerçekleşmiştir.");
            return Json(message);

        }

        [HttpPost]
        public JsonResult GetStepGroupList()
        {
            List<AuthoritySteps> list = AuthoritySteps.GetStepGroupList();
            return Json(list);
        }

        [HttpPost]
        public JsonResult GetStepList(int groupId, int headerId)
        {
            List<AuthoritySteps> list = AuthoritySteps.GetStepList(groupId, headerId);
            return Json(list);
        }

        [HttpPost]
        public JsonResult InsertAuthorityGroup(AuthoritySteps aStep)
        {
            var result = false;
            int id = 0;
            if (!aStep.IsChecked)
            {

                AuthorityGroup item = new AuthorityGroup()
                {
                    AgHeaderId = aStep.AgHeaderId,
                    HeaderId = aStep.HeaderId,
                    AsGroupId = aStep.GroupId,
                    AsId = aStep.Id,
                    GroupName = aStep.GroupName,
                    CreateId = AdminCurrentSalesman.Id,

                };

                id = item.Add();
                result = true;
            }
            else
            {
                AuthorityGroup item = new AuthorityGroup()
                {
                    Id = aStep.AuthorityGroupId,
                    EditId = AdminCurrentSalesman.Id
                };
                result = item.Delete();
            }
            var message = result ? new MessageBox(MessageBoxType.Success, "İşleminiz Gerçekleştirilmiştir .", id) : new MessageBox(MessageBoxType.Error, "İşleminizde Hata Gerçekleşmiştir.");

            return Json(message);
        }


    }
}