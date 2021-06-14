using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web.Mvc;
using System.Web.UI.WebControls.WebParts;
using B2b.Web.v4.Areas.Admin.Models;
using B2b.Web.v4.Models.EntityLayer;
using B2b.Web.v4.Models.Helper;
using B2b.Web.v4.Models.Log;
using Newtonsoft.Json;

namespace B2b.Web.v4.Areas.Admin.Controllers
{
    public class ProductsController : AdminBaseController
    {
        // GET: Admin/Products
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult ngTable()
        {
            return View();
        }
        public PartialViewResult ProductsSelect()
        {
            return PartialView();
        }
        public PartialViewResult ProductsItemSelect()
        {
            return PartialView();
        }
        public PartialViewResult ProductGroupsSelect()
        {
            return PartialView();
        }
        public PartialViewResult ManufacturerSelect()
        {
            return PartialView();
        }
        public PartialViewResult RuleSelect()
        {
            return PartialView();
        }
        public PartialViewResult VehicleSelect()
        {
            return PartialView();
        }

        #region HttpPost Methods

        [HttpPost]
        public string GetProductSearch(SearchCriteria searchCriteria)
        {
            SearchCriteria search = searchCriteria;
            List<Product> products = Product.AdminGetProductListBySearch(search);
            return JsonConvert.SerializeObject(products);
        }
        [HttpPost]
        public string GetAlternativeProducts(int groupId)
        {
            List<Product> product = Product.GetProductListByGroupId(groupId);
            return JsonConvert.SerializeObject(product);
        }
        [HttpPost]
        public string GetProductPrices(int productId)
        {
            List<ProductPrice> product = ProductPrice.GetListProductPrice(productId);
            return JsonConvert.SerializeObject(product);
        }
        [HttpPost]
        public string GetProductQuantity(int id)
        {
            List<WarehouseQuantity> quantity = WarehouseQuantity.GetListProductQuantity(id);
            return JsonConvert.SerializeObject(quantity);
        }
        [HttpPost]
        public string UpdateWarehouseQuantity(int id, double warehouseQuantityOnWay, double warehouseQuantity,int warehouseQuantityType)
        {
            bool result = WarehouseQuantity.UpdateQuantity(id, warehouseQuantityOnWay, warehouseQuantity, warehouseQuantityType);
            MessageBox messageBox = result ? new MessageBox(MessageBoxType.Success, "İşleminiz Tamamlandı") : new MessageBox(MessageBoxType.Error, "İşlem Sırasında Hata Oluştu.");
            return JsonConvert.SerializeObject(messageBox);
        }

        [HttpPost]
        public string GetLinkedProducts(int productId)
        {
            List<Product> product = Product.GetProductListLinked(productId);
            return JsonConvert.SerializeObject(product);
        }

        [HttpPost]
        public string GetOemNoList(int groupId)
        {
            List<Oem> oem = Oem.GetListByGroupId(groupId, OemType.OemNo);
            return JsonConvert.SerializeObject(oem);
        }

        [HttpPost]
        public string GetRivalList(int groupId)
        {
            List<Oem> rival = Oem.GetListByGroupId(groupId, OemType.RivalCode);
            return JsonConvert.SerializeObject(rival);
        }

        [HttpPost]
        public string GetVehicleBrandModelList(int groupId)
        {
            List<VehicleBrandModel> vehicleBrandModel = VehicleBrandModel.GetVehicleBrandModelTypeByGroupId(groupId);
            return JsonConvert.SerializeObject(vehicleBrandModel);
        }

        [HttpPost]
        public string GetProductsGroup1List()
        {
            List<ProductGroup> productGroup = ProductGroup.GetProductGroup1List();
            productGroup.Insert(0, new ProductGroup() { Name = "Hepsi" });
            return JsonConvert.SerializeObject(productGroup);
        }

        [HttpPost]
        public string GetProductsGroup2List(string productGroup1)
        {
            List<ProductGroup> productGroup2 = ProductGroup.GetProductGroup2List(-1, productGroup1);
            return JsonConvert.SerializeObject(productGroup2);
        }

        [HttpPost]
        public string GetProductsGroup3List(string productGroup1, string productGroup2)
        {
            List<ProductGroup> productlistGroup3 = ProductGroup.GetProductGroup3List(-1, productGroup1, productGroup2);
            return JsonConvert.SerializeObject(productlistGroup3);
        }

        [HttpPost]
        public string GetProductsManufacturerList()
        {
            List<Manufacturer> manufacturer = Manufacturer.GetList(-1);
            manufacturer.Insert(0, new Manufacturer() { Name = "Hepsi" });
            return JsonConvert.SerializeObject(manufacturer);
        }

        [HttpPost]
        public string GetProductsRuleList()
        {
            List<Rule> rule = Rule.GetRuleProductList();
            return JsonConvert.SerializeObject(rule);
        }
        [HttpPost]
        public string GetNextProduct(string code, int type)
        {
            List<Product> product = Product.AdminGetNextProduct(code, type);
            return JsonConvert.SerializeObject(product);
        }

        [HttpPost]
        public string UpdateAlternativeProducts(int newProductGroupId, int mainProductGroupId)
        {
            bool result = Product.UpdateGroupId(newProductGroupId, mainProductGroupId, AdminCurrentSalesman.Id);
            MessageBox messageBox = result ? new MessageBox(MessageBoxType.Success, "İşleminiz Tamamlandı") : new MessageBox(MessageBoxType.Error, "İşlem Sırasında Hata Oluştu.");
            return JsonConvert.SerializeObject(messageBox);
        }
        [HttpPost]
        public string GetProductById(int prodcutId)
        {
            return JsonConvert.SerializeObject(Product.AdminGetProductById(prodcutId));
        }
        [HttpPost]
        public string DeleteAlternativeProducts(int productId)
        {
            bool result = Product.UpdateGroupIdByProductId(productId, productId, AdminCurrentSalesman.Id);
            MessageBox messageBox = result ? new MessageBox(MessageBoxType.Success, "İşleminiz Tamamlandı") : new MessageBox(MessageBoxType.Error, "İşlem Sırasında Hata Oluştu.");
            return JsonConvert.SerializeObject(messageBox);
        }
        [HttpPost]
        public string DeleteLinkedProducts(int pId)
        {
            bool result = Product.DeleteLinkedProduct(pId, AdminCurrentSalesman.Id);
            MessageBox messageBox = result ? new MessageBox(MessageBoxType.Success, "İşleminiz Tamamlandı") : new MessageBox(MessageBoxType.Error, "İşlem Sırasında Hata Oluştu.");
            return JsonConvert.SerializeObject(messageBox);
        }
        [HttpPost]
        public string InertLinkedProduct(int linkedProductId, int mainProductId)
        {
            bool result = Product.InsertLinkedProduct(linkedProductId, mainProductId, AdminCurrentSalesman.Id);
            MessageBox messageBox = result ? new MessageBox(MessageBoxType.Success, "İşleminiz Tamamlandı") : new MessageBox(MessageBoxType.Error, "İşlem Sırasında Hata Oluştu.");
            return JsonConvert.SerializeObject(messageBox);
        }
        [HttpPost]
        public string SaveProduct(Product product)
        {
            if (product.Id != 0)
            {
                bool result = Product.UpdateProduct(product, AdminCurrentSalesman.Id);
                MessageBox messageBox = result
                    ? new MessageBox(MessageBoxType.Success, "İşleminiz Tamamlandı")
                    : new MessageBox(MessageBoxType.Error, "İşlem Sırasında Hata Oluştu.");
                return JsonConvert.SerializeObject(messageBox);
            }
            else
            {
                bool result = false;// Product.Save(product, AdminCurrentSalesman.Id);
                MessageBox messageBox = result
                     ? new MessageBox(MessageBoxType.Success, "İşleminiz Tamamlandı")
                     : new MessageBox(MessageBoxType.Error, "İşlem Sırasında Hata Oluştu.");
                return JsonConvert.SerializeObject(messageBox);

            }

        }
        [HttpPost]
        public string SaveOem(Product product, Oem row, int type)
        {
            bool result = false;
            result = row.Id == 0 ? Oem.Insert(product, row, (OemType)type, AdminCurrentSalesman.Id, 1) : Oem.Update(row.Id, row.Brand, row.OemNo, AdminCurrentSalesman.Id);

            MessageBox messageBox = result ? new MessageBox(MessageBoxType.Success, "İşleminiz Tamamlandı") : new MessageBox(MessageBoxType.Error, "İşlem Sırasında Hata Oluştu.");
            return JsonConvert.SerializeObject(messageBox);
        }
        [HttpPost]
        public string DeleteOem(int pId)
        {
            Oem item = new Oem()
            {
                Id = pId,
                EditId = AdminCurrentSalesman.Id
            };

            bool result = item.Delete();
            MessageBox messageBox = result ? new MessageBox(MessageBoxType.Success, "İşleminiz Tamamlandı") : new MessageBox(MessageBoxType.Error, "İşlem Sırasında Hata Oluştu.");
            return JsonConvert.SerializeObject(messageBox);
        }
        [HttpPost]
        public string DeleteVehicleProducts(int vehicleId)
        {
            Vehicle vehicle = new Vehicle { Id = vehicleId, EditId = AdminCurrentSalesman.Id };
            bool result = vehicle.Delete();
            MessageBox messageBox = result ? new MessageBox(MessageBoxType.Success, "İşleminiz Tamamlandı") : new MessageBox(MessageBoxType.Error, "İşlem Sırasında Hata Oluştu.");
            return JsonConvert.SerializeObject(messageBox);
        }
        [HttpPost]
        public string GetVehicleList()//Oem
        {
            return JsonConvert.SerializeObject(Oem.GetVehicleBrandList());
        }
        [HttpPost]
        public string GetVehicleBrandList()
        {
            return JsonConvert.SerializeObject(VehicleBrandModel.GetVehicleBrandList());
        }
        [HttpPost]
        public string GetVehicleModelList(string brand)
        {
            return JsonConvert.SerializeObject(VehicleBrandModel.GetVehicleBrandModelList(brand));
        }
        [HttpPost]
        public string GetVehicleTypeList(string brand, string model)
        {
            return JsonConvert.SerializeObject(VehicleBrandModel.GetVehicleTypeList(brand, model));
        }
        [HttpPost]
        public string GetCurrencyTypeList()
        {
            return JsonConvert.SerializeObject(Currency.GetList());
        }
        [HttpPost]
        public string InsertPrice(Product product, ProductPrice price)
        {
            bool result = false;
            result = price.Id == 0 ? ProductPrice.Insert(product.Id, price.PriceNumber, product.Code, price.Type, price.Price, price.Currency, AdminCurrentSalesman.Id) : ProductPrice.Update(price.Id, price.PriceNumber, price.Type, price.Price, price.Currency, AdminCurrentSalesman.Id);

            MessageBox messageBox = result ? new MessageBox(MessageBoxType.Success, "İşleminiz Tamamlandı") : new MessageBox(MessageBoxType.Error, "İşlem Sırasında Hata Oluştu.");
            return JsonConvert.SerializeObject(messageBox);
        }
        [HttpPost]
        public string DeletePrice(int pId)
        {
            bool result = ProductPrice.Delete(pId, AdminCurrentSalesman.Id);
            MessageBox messageBox = result ? new MessageBox(MessageBoxType.Success, "İşleminiz Tamamlandı") : new MessageBox(MessageBoxType.Error, "İşlem Sırasında Hata Oluştu.");
            return JsonConvert.SerializeObject(messageBox);
        }
        [HttpPost]
        public string UpdatePicture(Picture picture)
        {
            picture.EditId = AdminCurrentSalesman.Id;
            bool result = picture.Update();
            MessageBox messageBox = result ? new MessageBox(MessageBoxType.Success, "İşleminiz Tamamlandı") : new MessageBox(MessageBoxType.Error, "İşlem Sırasında Hata Oluştu.");
            return JsonConvert.SerializeObject(messageBox);
        }

        [HttpPost]
        public string GetPictureList(int productId)
        {


            return JsonConvert.SerializeObject(Picture.GetPicturePathByProductId(productId));
        }


        [HttpPost]
        public string InsertVehicle(Product product, int selectedVehicleId)
        {
            Vehicle vehicle = new Vehicle
            {
                ProductId = product.Id,
                GroupId = product.GroupId,
                OldGroupId = product.OldGruopId,
                VehicleId = selectedVehicleId,
                CreateId = AdminCurrentSalesman.Id
            };
            bool result = vehicle.Insert();
            MessageBox messageBox = result ? new MessageBox(MessageBoxType.Success, "İşleminiz Tamamlandı") : new MessageBox(MessageBoxType.Error, "İşlem Sırasında Hata Oluştu.");
            return JsonConvert.SerializeObject(messageBox);
        }
        #endregion
    }
}